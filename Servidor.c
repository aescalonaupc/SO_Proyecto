#include "servidor.h"
#include "conectados.h"
#include "main.h"

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
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("No se pudo hacer el bind en el puerto especificado\n");
	
	if (listen(sock_listen, 4) < 0)
		printf("No se pudo empezar a escuchar en el puerto especificado\n");
	
	int i = 0;
	int sockets[MAX_CONECTADOS];
	pthread_t thread;
	
	for (;;)
	{
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Recibida conexion\n");
		
		// sock_conn es el socket que usamos para este cliente
		sockets[i] = sock_conn;
		
		// crear thread y decirle qu￩ tiene que hacer
		pthread_create(&thread, NULL, AtenderCliente, &sockets[i]);
		i++;
	}
}

/* Crea un bucle infinito que atender￡ las peticiones de un cliente */
void* AtenderCliente(void* socket)
{
	int sock_conn = *((int*) socket);
	
	char peticion[BUFFER_SIZE];
	char respuesta[BUFFER_SIZE];
	
	strcpy(peticion, "");
	strcpy(respuesta, "");
	
	int ret;
	char usuario[STR_SIZE];
	strcpy(usuario, "");
	
	int acabado = 0;
	int notificarConectados = 0;
	
	while (acabado == 0)
	{
		ret = read(sock_conn, peticion, sizeof(peticion));
		printf("Recibida una peticion de: %s\n", usuario);
		
		peticion[ret] = '\0';
		printf("La peticion es: %s\n", peticion);
		
		char* p = strtok(peticion, "/");
		int codigo = atoi(p);
		
		/*
		Codigo 0: Desconexion
		C￳digo 1: Loguear
		C￳digo 2: Registrarse
		C￳digo 3: Ejecutar consulta
		C￳digo 4: Invitacion
		C￳digo 5: Aceptacion
		*/
		
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
			acabado = 1;
		}
		else if (codigo == 1)
		{
			char password[STR_SIZE];
			
			p = strtok(NULL, "/");
			strcpy(usuario, p);
			
			p = strtok(NULL, "/");
			strcpy(password, p);
			
			int Esta_en_la_lista = DamePos(&listaConectados,usuario);
			if(Esta_en_la_lista==-1)
			{
				printf("Login: usuario %s, contrase￱a %s\n", usuario, password);
				int resultado = Login(usuario, password);
				
				if (resultado <= 0)
				{
					strcpy(respuesta, "1/NO");
				} else
				{
					strcpy(respuesta, "1/SI");
					
					int solucion = IntroduceConectado(&listaConectados, usuario, sock_conn);
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
				strcpy(respuesta, "1/YA_ESTA");
			}
			
			
		}
		else if (codigo == 2)
		{
			char usuario[STR_SIZE];
			char password[STR_SIZE];
			int edad;
			
			p = strtok(NULL, "/");
			strcpy(usuario, p);
			
			p = strtok(NULL, "/");
			strcpy(password, p);
			
			p = strtok(NULL, "/");
			edad = atoi(p);
			int Existe_usuario = ComprobarSiYaEstaRegistrado(usuario);
			if(Existe_usuario==0)
			{
				printf("Registro: usuario %s, contrasena %s, edad %d\n", usuario, password, edad);
				int resultado = Registrarse(usuario, password, edad);
				
				if (resultado <= 0)
				{
					strcpy(respuesta, "2/NO");
				} else
				{
					strcpy(respuesta, "2/SI");
				}
			}
			else
			{
				printf(	"Ese nombre ya lo usa otro usuario");
				strcpy(respuesta, "2/YA_ESTA");
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
			printf("Recibida peticion de invitar jugador\n");
			int slot = IntroducePartida(&tablaPartidas);
			
			if (slot == -1)
			{
				printf("No se pudo crear la partida\n");
			}
			else
			{
				printf("Asignado slot de partida %d\n", slot);
				int exito = IntroduceJugadorEnPartida(&tablaPartidas, slot, sock_conn);
				
				if (exito == -1)
				{
					printf("No se pudo agregar a la partida al creador\n");
				}
				else
				{
					printf("Se ha conseguido agregar al creador\n");
					// El creador de la partida ya ha confirmado que quiere jugarla
					int a = MarcarJugadorConfirmado(&tablaPartidas, slot, sock_conn);
					
					printf("Se ha marcado como confirmado al creador %d\n", a);
					
					char amigo[STR_SIZE];
					int socketAmigo;
					
					for (p = strtok(NULL, "/"); p != NULL; p = strtok(NULL, "/"))
					{
						strcpy(amigo, p);
						socketAmigo = DameSocketDeNombre(&listaConectados, amigo);
						
						printf("amigo %s, socket %d\n", amigo, socketAmigo);
						
						if (socketAmigo == -1)
						{
							continue;
						}
						
						exito = IntroduceJugadorEnPartida(&tablaPartidas, slot, socketAmigo);
						
						printf("Agregado amigo a la partida %d\n", exito);
						
						if (exito == -1)
						{
							printf("No se pudo agregar al jugador a la partida\n");
						}
						else
						{
							sprintf(respuesta, "5/%d/%s", slot, usuario);
							printf("Enviando respuesta: %s\n", respuesta);
							
							write(socketAmigo, respuesta, strlen(respuesta));
						}
					}
					
					int buffer[MAX_JUGADORES_PARTIDA];
					ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
					printf("Jugadores que estaban dentro:\n");
					for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
					{
						printf("%d\n", buffer[i]);
					}
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
			
			if (strcmp(acepta, "NO") == 0)
			{
				int buffer[MAX_JUGADORES_PARTIDA];
				ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
				
				printf("El jugador ha rechazado la solicitud, jugadores que estaban dentro:\n");
				for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
				{
					printf("%d\n", buffer[i]);
				}
				
				sprintf(respuesta, "6/%d/0", slot);
				
				for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
				{
					// No necesito contestarme a mi mismo
					if (buffer[i] == sock_conn)
					{
						continue;
					}
					
					if (buffer[i] == -1)
					{
						continue;
					}
					
					printf("Enviando el mensaje a %d\n", buffer[i]);
					write(buffer[i], respuesta, strlen(respuesta));
				}
				
				EliminaPartida(&tablaPartidas, slot);
			}
			else if (strcmp(acepta, "SI") == 0)
			{
				int exito = MarcarJugadorConfirmado(&tablaPartidas, slot, sock_conn);
				
				if (exito == -1)
				{
					printf("No se pudo marcar el jugador como confirmado\n");
				}
				else
				{
					int estado = EstanTodosJugadoresConfirmados(&tablaPartidas, slot);
					if (estado == 1)
					{
						sprintf(respuesta, "6/%d/1", slot);
						
						int buffer[MAX_JUGADORES_PARTIDA];
						ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
						
						printf("El jugador ha aceptado la solicitud, jugadores que estaban dentro:\n");
						for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
						{
							printf("%d\n", buffer[i]);
						}
						
						for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
						{
							if (buffer[i] == -1)
							{
								continue;
							}
							
							write(buffer[i], respuesta, strlen(respuesta));
						}
					}
				}
			}
		}
		else if (codigo == 6)
		{
			char mensaje[STR_SIZE];
			int slot;
			
			p = strtok(NULL, "/");
			slot = atoi(p);
			
			p = strtok(NULL, "/");
			strcpy(mensaje, p);
			for (p = strtok(NULL, "/"); p != NULL; p = strtok(NULL, "/"))
			{
				strcat(mensaje, "/");
				strcat(mensaje, p);
			}
			
			int buffer[MAX_JUGADORES_PARTIDA];
			ObtenerSocketsJugadoresPartida(&tablaPartidas, slot, buffer);
			
			sprintf(respuesta, "7/%s/%s", usuario, mensaje);
			
			for (int i = 0; i < MAX_JUGADORES_PARTIDA; i++)
			{
				if (buffer[i] == -1)
				{
					continue;
				}
				
				write(buffer[i], respuesta, strlen(respuesta));
			}
			
		}
		
		if (codigo != 0 && codigo != 4 && codigo != 5 && codigo != 6)
		{
			if (strlen(respuesta) > 0)
			{
				printf("Respuesta: %s\n", respuesta);
				write(sock_conn, respuesta, strlen(respuesta));
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
			for (int i = 0; i < listaConectados.num; i++)
			{
				write(listaConectados.conectados[i].socket, notificacion, strlen(notificacion));
			}
			pthread_mutex_unlock(&mutexListaConectados);
			
			printf("Notificacion:%s \n",notificacion);
			notificarConectados = 0;
		}
		
	}
	
	close(sock_conn);
}

/*
	Intenta loguear a un usuario,
	Devuelve -1 en caso de error, 0 en caso de no existir el usuario, 1 si exito
*/
int Login(char usuario[STR_SIZE], char password[STR_SIZE])
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;

	char consulta[STR_SIZE];
	strcpy(consulta, "select jugador.id from jugador where jugador.username='");
	strcat(consulta,usuario);
	strcat(consulta,"'and jugador.password='");
	strcat(consulta, password);
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
	Devuelve -1 en caso de error, 1 en caso de ￩xito
*/
int Registrarse(char usuario[STR_SIZE], char password[STR_SIZE], int edad)
{
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[STR_SIZE];
	int siguiente_id;
	char siguiente_id_str[3];
	char edad_str[3];
	
	strcpy(consulta, "select max(id) from jugador;");
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	resultado = mysql_store_result(mysqlConn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL)
	{
		siguiente_id = 1;
	} else
	{
		siguiente_id = atoi(row[0]) + 1;
	}
	
	sprintf(siguiente_id_str, "%d", siguiente_id);
	sprintf(edad_str, "%d", edad);
	
	strcpy(consulta, "INSERT INTO jugador VALUES (");
	strcat(consulta, siguiente_id_str);
	strcat(consulta,",'");
	strcat(consulta, usuario);
	strcat(consulta,"','");
	strcat(consulta, password);
	strcat(consulta,"',");
	strcat(consulta, edad_str);
	strcat(consulta, ");");
	
	err = mysql_query(mysqlConn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	return 1;
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

