#include "servidor.h"
#include "conectados.h"
#include "main.h"

pthread_mutex_t mutexSocket = PTHREAD_MUTEX_INITIALIZER;
int socketCount = 0;

/* Empieza a ejecutar el servidor, entrar￡ en un bucle infinito escuchando */
void EjecutarServidor()
{
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creando el socket\n");
	
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(PUERTO);
	
	setsockopt(sock_listen, SOL_SOCKET, SO_REUSEADDR, &(int){1}, sizeof(int));
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("No se pudo hacer el bind en el puerto especificado\n");
	
	if (listen(sock_listen, 4) < 0)
		printf("No se pudo empezar a escuchar en el puerto especificado\n");
	
	pthread_t thread;
	
	for (;;)
	{
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Recibida conexion\n");
		
		pthread_mutex_lock(&mutexSocket);
		
		// Maximo de conectados simultaneamente alcanzado
		if (socketCount >= MAX_CONECTADOS)
		{
			pthread_mutex_unlock(&mutexSocket);
			close(sock_conn);
			
			printf("maximos clientes alcanzados!\n");
			continue;
		}
		
		socketCount++;
		pthread_mutex_unlock(&mutexSocket);
		
		// crear thread y decirle que tiene que hacer
		// sock_conn es el socket que usamos para este cliente
		pthread_create(&thread, NULL, AtenderCliente, &sock_conn);
	}
}

size_t _write(int fd, const void* buf, size_t nb)
{
	if (fd == -1)
	{
		return 0;
	}
	
	char dst[NETWORK_BUFFER_SIZE];
	sprintf(dst, "%s$", (const char*)buf);
	//printf("_write (%d):%s\n", fd, dst);
	return write(fd, dst, nb + 1);
}

/* Crea un bucle infinito que atender￡ las peticiones de un cliente */
void* AtenderCliente(void* socket)
{
	int sock_conn = *((int*) socket);

	/* Peticion entrante sin procesar, pueden ser varias juntas */
	char peticionEntrante[NETWORK_BUFFER_SIZE];
	
	/* Peticion individual que estamos procesando */
	char peticion[BUFFER_SIZE];
	
	/* Usado para separar varias peticiones de peticionEntrante */
	char* peticionPtr;
	
	/* Peticion original una vez la empezamos a procesar */
	char peticionOriginal[BUFFER_SIZE];
	
	/* Respuesta a enviar al cliente */
	char respuesta[NETWORK_BUFFER_SIZE];
	
	// Solo he llegado a ver 3, pero por si acaso...
	const int MAX_MSG_PET = 20;
	char mensajes[MAX_MSG_PET][BUFFER_SIZE];
	
	strcpy(peticionEntrante, "");
	strcpy(peticion, "");
	strcpy(peticionOriginal, "");
	strcpy(respuesta, "");
	
	int ret;
	
	/* Usuario conectado */
	char usuario[STR_SIZE];
	strcpy(usuario, "");
	
	/* Usuario Id para la BD */
	int usuarioId = -1;
	
	int acabado = 0;
	
	// Indica si notificar a todos los conectados el estado de la lista
	// al acabar la iteracion del bucle
	int notificarConectados = 0;
	
	// Indica si notificar a los jugadores dentro de la partida 'slotPartidaNotificar'
	// el estado de la lista al acabar la iteracion del bucle
	int notificarEnPartida = 0;
	int slotPartidaNotificar = -1;
	
	while (acabado == 0)
	{
		ret = read(sock_conn, peticionEntrante, sizeof(peticionEntrante));
	
		printf("ret %d from %s (%d)\n", ret, usuario, sock_conn);
		
		if (ret == -1)
		{
			continue;
		}
		
		if (ret == 0)
		{
			EliminaConectado(&listaConectados, usuario);
			
			// Si el jugador que se desconecta estaba en una partida
			// tenemos que realizar una serie de comprobaciones:
			//
			// 1. Si esta en una sala y el es el lider: la sala se elimina
			// 2. Si esta en sala y el no es el lider:
			//   2.1. Si hay +2 jugadores en ella, abandona la sala y ya
			//   2.2. Si hay 2 jugadores (o menos?), la sala se elimina
			// 3. Si esta en partida y el es el lider: la partida acaba y nadie gana
			// 4. Si esta en partida y el no es el lider: Se elimina de la partida
			//    y se notifica al lider de la partida el abandono, el decidira
			//    en base al estado del juego (p. ej. jugadores vivos) que le
			//    pasa a la partida
			
			int slot = ObtenerPartidaJugador(&tablaPartidas, sock_conn);
			if (slot != -1)
			{
				int lider = ObtenerLiderPartida(&tablaPartidas, slot);
				
				int buffer[MAX_JUGADORES_PARTIDA];
				int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
				
				int empezada = EstaPartidaEmpezada(&tablaPartidas, slot);
				
				if (empezada == 1)
				{
					printf("la partida estaba empezada\n");
				}
				
				// Casos 1. y 3.
				//
				// La partida/sala se elimina. Nadie gana. Se notifica al resto de jugadores
				
				if (lider == sock_conn)
				{
					printf("Se ha desconectado el lider\n");
					
					/* EstaPartidaFinalizada == 0 para no actualizarla dos veces en bd */
					if (empezada == 1 && 0 == EstaPartidaFinalizada(&tablaPartidas, slot))
					{
						// registrar partida en la base de datos...	
						int id, duracion, ganador;
						pthread_mutex_lock(&mutexTablaPartidas);
						
						id = tablaPartidas.partidas[slot].id;
						duracion = ((unsigned)time(NULL)) - tablaPartidas.partidas[slot].tiempoInicio;
						
						/* se desconecta el lider, nadie gana */
						ganador = -1;
						
						pthread_mutex_unlock(&mutexTablaPartidas);
						RegistrarFinPartida(id, duracion, ganador);
					}
					
					EliminaPartida(&tablaPartidas, slot);
					
					// Si la partida estaba empezada, notificamos al resto de jugadores
					if (empezada == 1)
					{
						// `gop` 100 = partida cancelada, lider abandona
						sprintf(respuesta, "100/100");
						printf("se envia 100/100 a %d jugadores\n", n);
						for (int i = 0; i < n; i++)
						{
							if (buffer[i] == sock_conn)
							{
								continue;
							}
							
							printf("Enviando cierre del juego a %d\n", buffer[i]);
							_write(buffer[i], respuesta, strlen(respuesta));
						}
						
					} else
					// Si no estaba empezada, podemos simplemente mandar la lista
					// de los jugadores vacia
					{
						// Establecemos su estado al default "disponible"
						for (int i = 0; i < n; i++)
						{
							if (buffer[i] == sock_conn)
							{
								continue;
							}
							
							EstablecerEstadoConectado(&listaConectados, buffer[i], 0);
						}
						
						// Indicamos la sala vacia
						sprintf(respuesta, "44/0");
						for (int i = 0; i < n; i++)
						{							
							if (buffer[i] == sock_conn)
							{
								continue;
							}
							
							_write(buffer[i], respuesta, strlen(respuesta));
						}
					}
				}
				
				// Casos 2. y 4.
				//
				// Si el jugador no es el lider, le eliminamos de la partida
				// Si no estaba empezada, le sacamos de la sala, si son 2 o menos
				// la sala se elimina
				//
				// Si ya estaba empezada, la logica de acabar la partida recae
				// en el lider de la partida
				
				else
				{
					// Si la partida esta empezada, notificamos al lider
					// el abandono y el procesara la logica para determinar
					// si la partida se acaba, notificar al resto visualmente, etc
					if (empezada == 1)
					{
						// `gop` 101 = jugador abandona notificar lider
						sprintf(respuesta, "100/101/%s", usuario);
						
						for (int i = 0; i < n; i++)
						{
							_write(buffer[i], respuesta, strlen(respuesta));	
						}
					}
					
					// Si no esta empezada, deberemos actualizar estados,
					// listas, eliminar la partida, etc
					else
					{
						// Hay mas de 2 jugadores, por tanto podemos simplemente
						// eliminarle de la sala/partida y ya
						if (n > 2)
						{
							// Lo eliminamos de la partida
							EliminaJugadorEnPartida(&tablaPartidas, slot, sock_conn);
							
							// Notificamos a la gente de la partida que se ha salido
							char jugadores[BUFFER_SIZE];
							strcpy(jugadores, "");
							
							ObtenerNombresJugadoresPartida(&listaConectados, &tablaPartidas, slot, jugadores);
							sprintf(respuesta, "44/%s", jugadores);
							
							for (int i = 0; i < n; i++)
							{
								if (buffer[i] == sock_conn)
								{
									continue;
								}
								
								_write(buffer[i], respuesta, strlen(respuesta));
							}
						}
						
						// Hay 2 (o menos?) jugadores en la partida
						else
						{
							EliminaPartida(&tablaPartidas, slot);
							
							// Establecemos su estado al default "disponible"
							for (int i = 0; i < n; i++)
							{
								if (buffer[i] == sock_conn)
								{
									continue;
								}
								
								EstablecerEstadoConectado(&listaConectados, buffer[i], 0);
							}
							
							// Indicamos la sala vacia
							sprintf(respuesta, "44/0");
							for (int i = 0; i < n; i++)
							{							
								if (buffer[i] == sock_conn)
								{
									continue;
								}
								
								_write(buffer[i], respuesta, strlen(respuesta));
							}
						}
					}
				} 
			}
			
			// Debemos manejarlo aqui porque no llegara al final del bucle
			// dado el continue;
			{
				char conectados[BUFFER_SIZE];
				strcpy(conectados, "");
				
				DameConectados(&listaConectados,conectados);
				sprintf(respuesta,"4/%s",conectados);
				
				printf("Se envia la nueva lista de conectados: %s\n", respuesta);
				
				pthread_mutex_lock(&mutexListaConectados);
				for (int i = 0, socket = 0; i < listaConectados.num; i++)
				{
					socket = listaConectados.conectados[i].socket;
					_write(socket, respuesta, strlen(respuesta));
				}
				pthread_mutex_unlock(&mutexListaConectados);
			};
			
			acabado = 1;
			
			// Abandonamos el bucle global con un continue;
			continue;
		}
		
		// printf("Recibida una peticion de: %s\n", usuario);
		
		peticionEntrante[ret] = '\0';
		//printf("%s\n", peticionEntrante);
		
		int i = 0;
		int j = 0;
		
		// La peticion acaba en $, p.ej. 1/Prueba/1234$
		// Cuando enviamos muchos mensajes seguidos o leemos mas bytes de los que enviamos podemos leer dos mensajes a la vez
		// Si no tenemos separador, los mensajes se mezclarian y se perderian, de esta manera podemos
		// separarlos por $ y recuperarlos totalmente
		for (peticionPtr = strtok(peticionEntrante, "$"); peticionPtr != NULL; peticionPtr = strtok(NULL, "$"), i++)
		{
			strcpy(mensajes[i], peticionPtr);
		}
		
		while (j < i)
		{
			strcpy(peticion, mensajes[j++]);
			strcpy(peticionOriginal, peticion);
			
			//printf("peticion %s\n", peticion);
			
			char* p = strtok(peticion, "/");
			int codigo = atoi(p);
			
			// Codigo 0: Desconexion
			// Codigo 1: Loguear
			// Codigo 2: Registrarse
			// Codigo 3: Ejecutar consulta
			// Codigo 4: Invitacion
			// Codigo 5: Aceptacion
			// Codigo 6: Mensaje chat
			// Codigo 7: Pedir lista de conectados
			// Codigo 8: Salir de la sala actual
			
			// Codigo 100: Mensajes juego
			
			if (codigo == 0)
			{
				int eliminado = EliminaConectado(&listaConectados, usuario);
				
				if (eliminado == 0)
				{
					printf("Eliminado con exito: %s\n", usuario);
					notificarConectados = 1;
				}
				else
				{
					printf("No se ha eliminado con exito\n");
				}
				
				notificarConectados = 1;
				acabado = 1;
			}
			else if (codigo == 1)
			{
				char password[STR_SIZE];
				
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				
				p = strtok(NULL, "/");
				strcpy(password, p);
				
				int estaUsuarioConectado = DamePos(&listaConectados, usuario);
				if (estaUsuarioConectado == -1)
				{
					printf("Login: usuario %s, contrasena %s\n", usuario, password);
					int resultado = Login(usuario, password);
					
					if (resultado <= 0)
					{
						strcpy(respuesta, "1/NOK/1");
					} else
					{
						sprintf(respuesta, "1/OK/%s/%d", usuario, resultado);
						printf("%s\n", respuesta);
						
						/* Guardamos el usuario Id */
						usuarioId = resultado;
						
						int solucion = IntroduceConectado(&listaConectados, usuario, sock_conn, usuarioId);
						if (solucion == 0)
						{
							printf("Se ha introducido correctamente\n");
							notificarConectados = 1;
						} else
						{
							printf("No se ha guardado nada\n");
						}
					}
				}
				else
				{
					printf("Ya esta contectado\n");
					strcpy(respuesta, "1/NOK/2");
				}
			}
			else if (codigo == 2)
			{
				char password[STR_SIZE];
				
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				
				p = strtok(NULL, "/");
				strcpy(password, p);
				
				int existeUsuario = ComprobarSiYaEstaRegistrado(usuario);
				if (existeUsuario == 0)
				{
					printf("Registro: usuario %s, contrasena %s\n", usuario, password);
					int resultado = Registrarse(usuario, password);
					
					if (resultado == -1)
					{
						strcpy(respuesta, "2/NOK/2");
					} else
					{
						/* id en la bd del usuario */
						resultado = Login(usuario, password);
						
						if (resultado < 0)
						{
							strcpy(respuesta, "2/NOK/2");
						} else
						{
							/* Guardamos el usuario Id */
							usuarioId = resultado;
							
							int solucion = IntroduceConectado(&listaConectados, usuario, sock_conn, resultado);
							if (solucion == 0)
							{
								notificarConectados = 1;
								sprintf(respuesta, "2/OK/%s/%d", usuario, resultado);
							} else
							{
								printf("No se ha guardado nada\n");
								sprintf(respuesta, "2/NOK/2");
							}
						}
					}
				}
				else
				{
					printf(	"Ese nombre ya lo usa otro usuario");
					strcpy(respuesta, "2/NOK/1");
				}
			}
			else if (codigo == 3)
			{
				int consulta;
				
				p = strtok(NULL, "/");
				consulta = atoi(p);
				
				printf("Consulta: %d\n", consulta);
				char resultado[STR_SIZE];
				Consulta(consulta, resultado);
				
				sprintf(respuesta,"3/%d/%s",consulta,resultado);
			}
			else if (codigo == 4)
			{
				// Obtenemos el slot de la partida del jugador
				// o sea, si ya ha invitado a m�s gente y ha formado una sala
				// Si slot = -1, entonces no tiene ninguna partida y creamos una
				// Si slot != -1, invitamos al nuevo jugador a la sala ya existente
				int slot = ObtenerPartidaJugador(&tablaPartidas, sock_conn);
				
				if (slot == -1)
				{
					slot = IntroducePartida(&tablaPartidas);
					
					if (slot == -1)
					{
						printf("no se pudo crear la partida de cero\n");
					} else
					{
						// La partida es nueva, por tanto debemos añadir al creador primeramente
						printf("nuevo slot de partida %d\n", slot);
						
						int exito = IntroduceJugadorEnPartida(&tablaPartidas, slot, sock_conn);
						
						if (exito == -1)
						{
							printf("No se pudo agregar a la partida al creador\n");
						}
						else
						{
							// Lo marcamos como lider de la partida
							EstablecerPartidaLider(&tablaPartidas, slot, sock_conn);

							// El creador de la partida ya ha confirmado que quiere jugarla
							MarcarJugadorConfirmado(&tablaPartidas, slot, sock_conn);
							
							// Modificamos el estado del jugador
							// Ahora estara "en sala"
							EstablecerEstadoConectado(&listaConectados, sock_conn, 2);
							
							notificarConectados = 1;
						}
					}
				}
				
				char invitado[STR_SIZE];
				int socketInvitado;
				
				p = strtok(NULL, "/");
				
				strcpy(invitado, p);
				socketInvitado = DameSocketDeNombre(&listaConectados, invitado);
				
				printf("invitado %s, socket %d\n", invitado, socketInvitado);
				
				if (socketInvitado != -1)
				{
					int exito = IntroduceJugadorEnPartida(&tablaPartidas, slot, socketInvitado);
					
					if (exito == -1)
					{
						printf("no se pudo agregar al invitado a la partida\n");
					} else
					{
						printf("agregado invitado a la partida\n");
						
						// Modificamos el estado del jugador
						// Ahora estara "en invitacion"
						EstablecerEstadoConectado(&listaConectados, socketInvitado, 1);
						
						notificarConectados = 1;
						
						// Le preguntamos al invitado si quiere entrar en la partida
						sprintf(respuesta, "5/%d/%s", slot, usuario);
						printf("Le preguntamos al invitado si quiere entrar en la partida: %s\n", respuesta);
						
						_write(socketInvitado, respuesta, strlen(respuesta));
					}
				}
			}
			else if (codigo == 5)
			{
				printf("Respuesta de una solicitud de %s\n", usuario);
				int slot;
				
				p = strtok(NULL, "/");
				slot = atoi(p);
				
				char acepta[STR_SIZE];
				p = strtok(NULL, "/");
				strcpy(acepta, p);
				
				if (strcmp(acepta, "0") == 0)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					printf("El jugador ha rechazado la solicitud, jugadores que estaban dentro:\n");
					for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
					{
						printf("%d\n", buffer[i]);
					}
					
					// Modificamos el estado del jugador
					// Ahora estara "default"
					EstablecerEstadoConectado(&listaConectados, sock_conn, 0);
					
					notificarConectados = 1;
					
					// Lo eliminamos de la partida en que estaba
					EliminaJugadorEnPartida(&tablaPartidas, slot, sock_conn);
					
					// Si solo queda el creador, vamos a eliminar la partida
					if (1 == ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer))
					{
						// El unico conectado (deberia ser el lider) es buffer[0]
						EstablecerEstadoConectado(&listaConectados, buffer[0], 0);
						EliminaPartida(&tablaPartidas, slot);
						notificarConectados = 1;
						
						// Le notificamos al cliente manualmente
						// Que su sala ahora esta vacia
						sprintf(respuesta, "44/0/");
						_write(buffer[0], respuesta, strlen(respuesta));
					}
				}
				else if (strcmp(acepta, "1") == 0)
				{
					int exito = MarcarJugadorConfirmado(&tablaPartidas, slot, sock_conn);
					
					if (exito == -1)
					{
						printf("No se pudo marcar el jugador como confirmado\n");
					}
					else
					{
						// Modificamos el estado del jugador
						// Ahora estara "en sala"
						EstablecerEstadoConectado(&listaConectados, sock_conn, 2);
						printf("El estado del jugador es en sala ahora\n");
						
						notificarConectados = 1;
						notificarEnPartida = 1;
						slotPartidaNotificar = slot;
					}
				}
			}
			else if (codigo == 6)
			{
				char mensaje[STR_SIZE];
				int slot;
				
				p = strtok(NULL, "/");
				
				if (p != NULL)
				{
					strcpy(mensaje, p);
					for (p = strtok(NULL, "/"); p != NULL; p = strtok(NULL, "/"))
					{
						strcat(mensaje, "/");
						strcat(mensaje, p);
					}
					
					sprintf(respuesta, "8/%s/%s", usuario, mensaje);
					slot = ObtenerPartidaJugador(&tablaPartidas, sock_conn);
					
					// Chat global a todos los que no estan en sala
					if (slot == -1)
					{
						int buffer[MAX_CONECTADOS];
						int n = DameSocketsConectados(&listaConectados, buffer);
						
						for (int i = 0; i < n; i++)
						{
							if (-1 != ObtenerPartidaJugador(&tablaPartidas, buffer[i]))
							{
								continue;
							}
							
							_write(buffer[i], respuesta, strlen(respuesta));
						}
					} else
						// Chat privado a los miembros de la partida
					{
						int buffer[MAX_JUGADORES_PARTIDA];
						int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
						
						for (int i = 0; i < n; i++)
						{
							_write(buffer[i], respuesta, strlen(respuesta));
						}
					}
				}
				
			}
			else if (codigo == 7)
			{
				char conectados[BUFFER_SIZE];
				strcpy(conectados, "");
				
				DameConectados(&listaConectados,conectados);
				sprintf(respuesta,"4/%s",conectados);
				
				/* tambien damos las estadisticas del jugador */
				{
					int jugadorId = ObtenerIdDeUsuarioConectado(&listaConectados, usuario);
					
					int partidas = ObtenerPartidasJugador(jugadorId);
					int ganadas = ObtenerPartidasGanadasJugador(jugadorId);
					int minutos = ObtenerMinutosJugador(jugadorId);
					
					char respuesta2[BUFFER_SIZE];
					sprintf(respuesta2, "77/%d/%d/%d", partidas, ganadas, minutos);
					_write(sock_conn, respuesta2, strlen(respuesta2));
				};
			}
			else if (codigo == 8)
			{
				int slot = ObtenerPartidaJugador(&tablaPartidas, sock_conn);
				
				if (slot != -1)
				{
					int lider = ObtenerLiderPartida(&tablaPartidas, slot);

					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);

					// Si el jugador no es el lider de la partida
					// simplemente le eliminamos y listo
					// Pero si la sala se quedara vacia (solo 1 jugador), 
					// tambien eliminamos la sala
					if (lider != sock_conn && n > 2)
					{
						EliminaJugadorEnPartida(&tablaPartidas, slot, sock_conn);
						EstablecerEstadoConectado(&listaConectados, sock_conn, 0);

						notificarConectados = 1;

						notificarEnPartida = 1;
						slotPartidaNotificar = slot;
					} else
					// Si es el lider, tenemos que eliminar la partida
					{
						for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
						{
							if (buffer[i] == -1)
							{
								continue;
							}

							EstablecerEstadoConectado(&listaConectados, buffer[i], 0);
						}

						EliminaPartida(&tablaPartidas, slot);

						notificarConectados = 1;

						// Notificamos que la partida ahora esta vacia
						// No podemos enviarlo como notificacion al final porque
						// La lista de los jugadores ahora esta vacia
						{
							sprintf(respuesta, "44/0");
							
							for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
							{
								if (buffer[i] == -1)
								{
									continue;
								}
								
								if (buffer[i] == sock_conn)
								{
									continue;
								}
								
								_write(buffer[i], respuesta, strlen(respuesta));
							}
						};
					}
				}
			}
			else if (codigo == 9)
			{
				if (1 == EliminarUsuario(usuarioId))
				{
					/* Desconexion como si el cliente se fuera */
					
					EliminaConectado(&listaConectados, usuario);
					
					int slot = ObtenerPartidaJugador(&tablaPartidas, sock_conn);
					if (slot != -1)
					{
						int lider = ObtenerLiderPartida(&tablaPartidas, slot);
						
						int buffer[MAX_JUGADORES_PARTIDA];
						int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
						
						if (lider == sock_conn)
						{
							EliminaPartida(&tablaPartidas, slot);
							
							// Establecemos su estado al default "disponible"
							for (int i = 0; i < n; i++)
							{
								if (buffer[i] == sock_conn)
								{
									continue;
								}
								
								EstablecerEstadoConectado(&listaConectados, buffer[i], 0);
							}
							
							// Indicamos la sala vacia
							sprintf(respuesta, "44/0");
							for (int i = 0; i < n; i++)
							{							
								if (buffer[i] == sock_conn)
								{
									continue;
								}
								
								_write(buffer[i], respuesta, strlen(respuesta));
							}
						}
						else
						{
							// Hay mas de 2 jugadores, por tanto podemos simplemente
							// eliminarle de la sala/partida y ya
							if (n > 2)
							{
								// Lo eliminamos de la partida
								EliminaJugadorEnPartida(&tablaPartidas, slot, sock_conn);
								
								// Notificamos a la gente de la partida que se ha salido
								char jugadores[BUFFER_SIZE];
								strcpy(jugadores, "");
								
								ObtenerNombresJugadoresPartida(&listaConectados, &tablaPartidas, slot, jugadores);
								sprintf(respuesta, "44/%s", jugadores);
								
								for (int i = 0; i < n; i++)
								{
									if (buffer[i] == sock_conn)
									{
										continue;
									}
									
									_write(buffer[i], respuesta, strlen(respuesta));
								}
							}
							
							// Hay 2 (o menos?) jugadores en la partida
							else
							{
								EliminaPartida(&tablaPartidas, slot);
								
								// Establecemos su estado al default "disponible"
								for (int i = 0; i < n; i++)
								{
									if (buffer[i] == sock_conn)
									{
										continue;
									}
									
									EstablecerEstadoConectado(&listaConectados, buffer[i], 0);
								}
								
								// Indicamos la sala vacia
								sprintf(respuesta, "44/0");
								for (int i = 0; i < n; i++)
								{							
									if (buffer[i] == sock_conn)
									{
										continue;
									}
									
									_write(buffer[i], respuesta, strlen(respuesta));
								}
							}
						}
					}
					
					// Debemos manejarlo aqui porque no llegara al final del bucle
					// dado el continue;
					{
						char conectados[BUFFER_SIZE];
						strcpy(conectados, "");
						
						DameConectados(&listaConectados,conectados);
						sprintf(respuesta,"4/%s",conectados);
						
						printf("Se envia la nueva lista de conectados: %s\n", respuesta);
						
						pthread_mutex_lock(&mutexListaConectados);
						for (int i = 0, socket = 0; i < listaConectados.num; i++)
						{
							socket = listaConectados.conectados[i].socket;
							_write(socket, respuesta, strlen(respuesta));
						}
						pthread_mutex_unlock(&mutexListaConectados);
					};
						
					acabado = 1;
					continue;
				}
				
			}
			else if (codigo == 100)
			{
				int gop;
				p = strtok(NULL, "/");
				gop = atoi(p);
				
				int slot = ObtenerPartidaJugador(&tablaPartidas, sock_conn);
				
				if (gop != 0 && slot == -1)
				{
					printf("partida no existente!\n");
					continue;
				}
				
				/*
				Indica que el lider de la sala ha ordenado empezar la partida
				*/
				if (gop == 0)
				{
					int tipoJuego;
					p = strtok(NULL, "/");
					tipoJuego = atoi(p);
					
					// Sucede cuando un solo jugador elige 'Sandbox'
					// Creamos una partida para ese jugador
					if (tipoJuego == 0 && slot == -1)
					{
						slot = IntroducePartida(&tablaPartidas);
						IntroduceJugadorEnPartida(&tablaPartidas, slot, sock_conn);
						MarcarJugadorConfirmado(&tablaPartidas, slot, sock_conn);
					}
					
					EstablecerPartidaTipo(&tablaPartidas, slot, tipoJuego);
					MarcarPartidaEmpezada(&tablaPartidas, slot);
					
					/* Creamos la partida en la base de datos */
					int partidaId;
					{
						partidaId = CrearPartidaBd();
						
						if (partidaId >= 0)
						{
							pthread_mutex_lock(&mutexTablaPartidas);
							
							tablaPartidas.partidas[slot].id = partidaId;
							tablaPartidas.partidas[slot].tiempoInicio = (unsigned)time(NULL);
							
							pthread_mutex_unlock(&mutexTablaPartidas);	
						}
					};

					printf("Empezada partida %d, id %d, tipo de juego %d\n", slot, partidaId, tipoJuego);
					
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					/* registramos la partida para los jugadores */
					/* se podria hacer mas eficiente... pero llevo 2 semanas de fqs */
					{
						for (int i = 0; i < n; i++)
						{
							if (buffer[i] == -1)
							{
								continue;
							}
							
							char tmp_usuario[STR_SIZE];
							
							if (1 == ObtenerNombreDeSocket(&listaConectados, buffer[i], tmp_usuario))
							{
								int tmp_uId = ObtenerIdDeUsuarioConectado(&listaConectados, tmp_usuario);
								RegistrarPartidaParaJugador(tmp_uId, partidaId);	
							}
						}
					};
					
					sprintf(respuesta, "7/%d", tipoJuego);
					
					for (int i = 0; i < n; i++)
					{
						EstablecerEstadoConectado(&listaConectados, buffer[i], 3);
						
						if (buffer[i] == sock_conn)
						{
							continue;
						}
						
						_write(buffer[i], respuesta, strlen(respuesta));
					}
					
					notificarConectados = 1;
					
					sprintf(respuesta, "100/0");
					_write(sock_conn, respuesta, strlen(respuesta));
					
				/*
				Indica la creacion de un jugador, la reenviamos a los jugadores
				*/
				} else if (gop == 1)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Indica empezar la partida para los jugadores
				*/
				else if (gop == 3)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Paquete de sincronizar heading/posicion/vida jugador
				*/
				else if (gop == 5)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						if (buffer[i] == sock_conn)
						{
							continue;
						}
						
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Paquete de sincronizar disparos
				*/
				else if (gop == 6)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Sincronizar una actualizacion de vida
				*/
				else if (gop == 7)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						if (buffer[i] == sock_conn)
						{
							continue;
						}
						
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Sincronizar una eliminar entidad
				*/
				else if (gop == 8)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Muerte permanente de un jugador (9),
				Muerte de un jugador que le quedan vidas (10)
				*/
				else if (gop == 9 || gop == 10)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Respawn de un jugador
				*/
				else if (gop == 11)
				{
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
				
				/*
				Fin de la partida
				*/
				else if (gop == 200)
				{
					char ganador[STR_SIZE];
					
					p = strtok(NULL, "");
					strcpy(ganador, p);
					
					int buffer[MAX_JUGADORES_PARTIDA];
					int n = ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					
					// guardar bd...
					{
						int id, duracion, ganadorId;
						pthread_mutex_lock(&mutexTablaPartidas);
						
						id = tablaPartidas.partidas[slot].id;
						duracion = ((unsigned)time(NULL)) - tablaPartidas.partidas[slot].tiempoInicio;
						ganadorId = ObtenerIdDeUsuarioConectado(&listaConectados, ganador);
						
						pthread_mutex_unlock(&mutexTablaPartidas);
						RegistrarFinPartida(id, duracion, ganadorId);
						
						MarcarPartidaFinalizada(&tablaPartidas, slot);
					};
					
					for (int i = 0; i < n; i++)
					{
						_write(buffer[i], peticionOriginal, strlen(peticionOriginal));
					}
				}
			}
			
			if (codigo != 0 && codigo != 4 && codigo != 5 && codigo != 6 && codigo != 100)
			{
				if (strlen(respuesta) > 0)
				{
					printf("Respuesta: %s\n", respuesta);
					_write(sock_conn, respuesta, strlen(respuesta));
				}
			}
			
			if (notificarConectados == 1)
			{
				char notificacion[BUFFER_SIZE];
				char conectados[BUFFER_SIZE];
				
				strcpy(notificacion, "");
				strcpy(conectados, "");
				
				DameConectados(&listaConectados,conectados);
				sprintf(notificacion,"4/%s",conectados);
				
				pthread_mutex_lock(&mutexListaConectados);
				for (int i = 0, socket = 0; i < listaConectados.num; i++)
				{
					socket = listaConectados.conectados[i].socket;					  
					_write(socket, notificacion, strlen(notificacion));
				}
				pthread_mutex_unlock(&mutexListaConectados);
				
				printf("Notificacion:%s \n",notificacion);
				notificarConectados = 0;
			}
			
			if (notificarEnPartida == 1 && slotPartidaNotificar != -1)
			{
				char notificacionPartida[BUFFER_SIZE];
				char jugadores[BUFFER_SIZE];
				
				strcpy(notificacionPartida, "");
				strcpy(jugadores, "");
				
				ObtenerNombresJugadoresPartida(&listaConectados, &tablaPartidas, slotPartidaNotificar, jugadores);
				sprintf(notificacionPartida, "44/%s", jugadores);
				printf("Notificacion 2: %s\n", notificacionPartida);
				
				int buffer[MAX_JUGADORES_PARTIDA];
				ObtenerSocketsJugadoresPartida(&tablaPartidas, slotPartidaNotificar, buffer);
				
				for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
				{
					if (buffer[i] == -1)
					{
						continue;
					}
					
					_write(buffer[i], notificacionPartida, strlen(notificacionPartida));
				}
				
				notificarEnPartida = 0;
				slotPartidaNotificar = -1;
			}
		}
	}
	
	pthread_mutex_lock(&mutexSocket);
	socketCount--;
	pthread_mutex_unlock(&mutexSocket);
	close(sock_conn);
}

/*
	Intenta loguear a un usuario,
	Devuelve -2 en caso de error, -1 en caso de no existir el usuario, o el
	sqlid id del usuario si existe
*/
int Login(char usuario[STR_SIZE], char password[STR_SIZE])
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;

	char consulta[STR_SIZE];
	sprintf(
			consulta,
			"select jugador.id from jugador where jugador.username='%s' and jugador.password='%s';",
			usuario, password
	);
	/*strcpy(consulta, "select jugador.id from jugador where jugador.username='");
	strcat(consulta,usuario);
	strcat(consulta,"'and jugador.password='");
	strcat(consulta, password);
	strcat(consulta, "';");*/
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -2;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	int resultado_login;
	if (row == NULL)
	{
		resultado_login = -1;
	} else
	{
		resultado_login = atoi(row[0]);
	}
	
	return resultado_login;
}
/*
Busca si el usuario ya esta en la base de datos,
Devuelve -1 en caso de error, 0 en caso de no existir el usuario, 1 si exito
*/
int ComprobarSiYaEstaRegistrado(char usuario[STR_SIZE])
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	strcpy(consulta, "select jugador.id from jugador where jugador.username='");
	strcat(consulta,usuario);
	strcat(consulta, "';");
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	int resultado_login;
	if (row == NULL)
	{
		resultado_login = 0;
	} else
	{
		resultado_login = 1;
	}

	return resultado_login;
}
/*
	Intenta registrar a un usuario en la base de datos,
	Devuelve -1 en caso de error, 1 en caso de exito
*/
int Registrarse(char usuario[STR_SIZE], char password[STR_SIZE])
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	int siguiente_id;
	
	strcpy(consulta, "select max(id) from jugador;");
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL || row[0] == NULL)
	{
		siguiente_id = 1;
	} else
	{
		siguiente_id = atoi(row[0]) + 1;
	}
	
	sprintf(consulta, "insert into jugador values(%d, '%s', '%s');", siguiente_id, usuario, password);
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	return 1;
}

/*
	Elimina un usuario (id) de la base de datos, devuelve 1 si fue un exito,
	-1 si hubo un error
*/
int EliminarUsuario(int usuarioId)
{
	if (usuarioId < 0)
	{
		return -1;
	}
	
	char consulta[STR_SIZE];
	int err;
	
	/* Eliminamos el usuario y ya, la tabla de partidas_jugador tiene una
	   relacion "ON DELETE CASCADE" con jugador_id y se eliminaran automaticamente
	*/
	sprintf(consulta, "delete from jugador where id = %d;", usuarioId);
	err = mysql_query(mysqlConn, consulta);
	
	if (err != 0)
	{
		return -1;
	}
	
	return 1;
}

/*
	Crea una partida en la base de datos y devuelve su Id,
	devuelve -1 en caso de error
*/
int CrearPartidaBd()
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	int siguiente_id;
	
	/* Obtenemos la id que se utilizara */
	strcpy(consulta, "select max(id) from partida;");
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL || row[0] == NULL)
	{
		siguiente_id = 1;
	} else
	{
		siguiente_id = atoi(row[0]) + 1;
	}
	
	sprintf(consulta, "insert into partida(id, nombre) values(%d, 'Partida %d');", siguiente_id, siguiente_id);
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		return -1;
	}
	
	return siguiente_id;
}

/*
	Registra el fin de una partida de la base de datos, id es la id de la partida
	en la BD, duracion es la duracion en segundos, ganador es la id del jugador
	que ha ganado
*/
void RegistrarFinPartida(int id, int duracion, int ganador)
{
	char consulta[STR_SIZE];
	sprintf(consulta, "update partida set duracion = %d, ganador = %d where id = %d", duracion, ganador, id);
	
	mysql_query(mysqlConn, consulta);
}

/*
	Registra en la Bd que el jugador indicado ha jugado a la partida indicada
*/
void RegistrarPartidaParaJugador(int jugadorId, int partidaId)
{
	char consulta[STR_SIZE];
	sprintf(consulta, "insert into partidas_jugadas(partida_id, jugador_id) values(%d, %d);", partidaId, jugadorId);
	
	mysql_query(mysqlConn, consulta);
}

/*
	Devuelve el numero de partidas jugadas por un jugador
*/
int ObtenerPartidasJugador(int jugadorId)
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	
	sprintf(consulta, "select count(partida_id) from partidas_jugadas where jugador_id = %d;", jugadorId);
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL || row[0] == NULL)
	{
		return 0;
	} else
	{
		return atoi(row[0]);
	}
}

/*
	Devuelve el numero de partidas ganadas de un jugador
*/
int ObtenerPartidasGanadasJugador(int jugadorId)
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	
	sprintf(consulta, "select count(id) from partida where ganador = %d;", jugadorId);
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL || row[0] == NULL)
	{
		return 0;
	} else
	{
		return atoi(row[0]);
	}
}

/*
	Devuelve los minutos jugados de un jugador
*/
int ObtenerMinutosJugador(int jugadorId)
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	
	sprintf(consulta, "select sum(partida.duracion) from (partida, partidas_jugadas) where partida.id = partidas_jugadas.partida_id and partidas_jugadas.jugador_id = %d;", jugadorId);
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL || row[0] == NULL)
	{
		return 0;
	} else
	{
		return atoi(row[0]) / 60;
	}
}

/*
	Ejecuta la consulta con el id indicado e inserta el resultado en "resultadoBuff"
	Id 1: Devuelve el jugador que tiene mas puntos
	Id 2: Deuvelve los puntos de las partidas en que Juan es ganador
	Id 3: Devuelve los ganadores menores de edad
*/
void Consulta(int consulta_id, char resultadoBuff[STR_SIZE])
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	if (consulta_id == 1)
	{
		strcpy (consulta,"SELECT jugador.username FROM (jugador,partidas_jugadas) WHERE partidas_jugadas.puntos = (SELECT MAX(partidas_jugadas.puntos) FROM partidas_jugadas) AND jugador.id = partidas_jugadas.jugador_id");
	} else if (consulta_id == 2)
	{
		strcpy (consulta,"select partidas_jugadas.puntos from partidas_jugadas,jugador,partida where jugador.username='Juan' and jugador.id=partidas_jugadas.jugador_id and partidas_jugadas.partida_id=partida.id and partida.ganador=jugador.id");
	} else if (consulta_id == 3)
	{
		strcpy (consulta,"select jugador.username from (jugador, partida) where jugador.edad < 18 and partida.ganador = jugador.id");
	} else
	{
		printf("Consulta %d desconocida\n", consulta_id);
		return;
	}
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL)
	{
		strcpy(resultadoBuff, row[0]);
	} else
	{
		strcpy(resultadoBuff, row[0]);
	}
}

