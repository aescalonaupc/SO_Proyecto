#include "conectados.h"
#include "main.h"

pthread_mutex_t mutexListaConectados = PTHREAD_MUTEX_INITIALIZER;
pthread_mutex_t mutexTablaPartidas = PTHREAD_MUTEX_INITIALIZER;

/*
	Devuelve la posicion en la lista del conectado con el nombre especificado,
	-1 si no lo encuentra
*/
int DamePos(TListaConectados* lista, char nombre[STR_SIZE])
{
	int i = 0;
	pthread_mutex_lock(&mutexListaConectados);
	while (i < lista->num)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			pthread_mutex_unlock(&mutexListaConectados);
			return i;
		}
		i++;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return -1;
}

/*
Devuelve la posicion en la lista del conectado con el socket especificado,
-1 si no lo encuentra
*/
int DamePosSocket(TListaConectados* lista, int socket)
{
	int i = 0;
	pthread_mutex_lock(&mutexListaConectados);
	while (i < lista->num)
	{
		if (lista->conectados[i].socket == socket)
		{
			pthread_mutex_unlock(&mutexListaConectados);
			return i;
		}
		i++;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return -1;
}

/*
	Devuelve el nombre del usuario con el socket indicado,
	devuelve 1 si lo encuentra, 0 si no
*/
int ObtenerNombreDeSocket(TListaConectados* lista, int socket, char nombre[STR_SIZE])
{
	pthread_mutex_lock(&mutexListaConectados);
	
	int i = 0;
	while (i < lista->num)
	{
		if (lista->conectados[i].socket == socket)
		{
			strcpy(nombre, lista->conectados[i].nombre);
			pthread_mutex_unlock(&mutexListaConectados);
			return 1;
		}
		
		i++;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return 0;
}

/*
	Introduce un usuario y socket en la lista de conectados, devuelve 0 si
	no es posible, 1 en caso de exito
*/
int IntroduceConectado(TListaConectados* lista, char nombre[STR_SIZE], int socket, int id)
{
	pthread_mutex_lock(&mutexListaConectados);
	int posicion = lista->num;
	
	if (posicion < MAX_CONECTADOS)
	{
		lista->conectados[posicion].id = id;
		strcpy(lista->conectados[posicion].nombre, nombre);
		lista->conectados[posicion].socket = socket;
		lista->conectados[posicion].estado = 0;
		
		lista->num = posicion + 1;
		pthread_mutex_unlock(&mutexListaConectados);
		return 0;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return 1;
}

/*
	Elimina el conectado con el nombre especificado, devuelve -1 en caso de no
	existir, 0 en caso de exito
	Desplaza los elementos a la derecha de la lista 1 posicion hacia la izquierda
*/
int EliminaConectado(TListaConectados* lista, char nombre[STR_SIZE])
{
	int pos = DamePos(lista, nombre);
	
	if (pos == -1)
	{
		return -1;
	}
	else{
		pthread_mutex_lock(&mutexListaConectados);
		int i;
		for (i=pos; i < lista->num-1; i++)
		{
			lista->conectados[i].id = lista->conectados[i + 1].id;
			strcpy(lista->conectados[i].nombre, lista->conectados[i + 1].nombre);
			lista->conectados[i].socket = lista->conectados[i + 1].socket;
			lista->conectados[i].estado = lista->conectados[i + 1].estado;
		}
		lista->num = lista->num-1;
		pthread_mutex_unlock(&mutexListaConectados);
		
		return 0;
	}
}

/*
	Modifica el indicador de estado del jugador conectado especificado
*/
void EstablecerEstadoConectado(TListaConectados* lista, int socket, int estado)
{
	int pos = DamePosSocket(lista, socket);
	
	if (pos == -1)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexListaConectados);
	lista->conectados[pos].estado = estado;
	pthread_mutex_unlock(&mutexListaConectados);
}

/*
	Devuelve el estado del usuario conectado con el socket dado, -1 si no existe
*/
int ObtenerEstadoConectado(TListaConectados* lista, int socket)
{
	int pos = DamePosSocket(lista, socket);
	
	if (pos == -1)
	{
		return -1;
	}
	
	int estado;
	
	pthread_mutex_lock(&mutexListaConectados);
	estado = lista->conectados[pos].estado;
	pthread_mutex_unlock(&mutexListaConectados);
	
	return estado;
}

/*
	Crea una nueva partida en la tabla de partidas y devuelve su slot en la
	tabla, -1 en caso de error o la tabla esta llena
*/
int IntroducePartida(TTablaPartidas* tabla)
{
	int partidaSlot = -1;
	pthread_mutex_lock(&mutexTablaPartidas);
	
	for (int i = 0; i < MAX_PARTIDAS; i++)
	{
		// Buscamos una partida que no haya sido creada, cuando
		// encontremos una usaremos ese hueco
		if (tabla->partidas[i].creada != 1)
		{
			partidaSlot = i;
			break;
		}
	}
	
	// La marcamos como creada
	if (partidaSlot != -1)
	{
		tabla->partidas[partidaSlot].creada = 1;
		tabla->partidas[partidaSlot].empezada = 0;
		tabla->partidas[partidaSlot].finalizada = 0;
		
		tabla->partidas[partidaSlot].id = -1;
		tabla->partidas[partidaSlot].lider = -1;
		tabla->partidas[partidaSlot].tipo = -1;
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return partidaSlot;
}

/*
	Establece la Id de la base de datos de la partida con el slot indicado
*/
void EstablecerPartidaId(TTablaPartidas* tabla, int slot, int id)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);

	if (tabla->partidas[slot].creada != 1)
	{
		return;
	}

	tabla->partidas[slot].id = id;
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Establece el socket del lider que ha creado la partida indicada
*/
void EstablecerPartidaLider(TTablaPartidas* tabla, int slot, int socket)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);

	if (tabla->partidas[slot].creada != 1)
	{
		return;
	}

	tabla->partidas[slot].lider = socket;
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Establece el tipo de juego de la partida indicada
*/
void EstablecerPartidaTipo(TTablaPartidas* tabla, int slot, int tipo)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);

	if (tabla->partidas[slot].creada != 1)
	{
		return;
	}

	tabla->partidas[slot].tipo = tipo;
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Devuelve el socket del lider de la partida indicada
*/
int ObtenerLiderPartida(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return -1;
	}
	
	int socket;
	pthread_mutex_lock(&mutexTablaPartidas);
	socket = tabla->partidas[slot].lider;
	pthread_mutex_unlock(&mutexTablaPartidas);
	return socket;
}

/*
	Devuelve 1 si la partida ya ha finalizado, 0 si no
*/
int EstaPartidaFinalizada(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return 0;
	}
	
	int finalizada = 0;
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada == 1 && tabla->partidas[slot].finalizada == 1)
	{
		finalizada = 1;
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return finalizada;
}

/*
	Marca una partida como finalizada
*/
void MarcarPartidaFinalizada(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	tabla->partidas[slot].finalizada = 1;
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Introduce un jugador (su socket) en la partida con el slot especificado,
	devuelve -1 en caso de error, 1 en caso de exito
	El jugador introducido figurara como que no ha confirmado todavia la partida
*/
int IntroduceJugadorEnPartida(TTablaPartidas* tabla, int slot, int socket)
{
	if (slot >= MAX_PARTIDAS)
	{
		return -1;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	// La partida debe estar creada
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return -1;
	}
	
	// La partida no debe haber empezado
	if (tabla->partidas[slot].empezada == 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return -1;
	}
	
	// Debe haber hueco en la partida
	if (tabla->partidas[slot].jugadores.num >= MAX_JUGADORES_PARTIDA)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return -1;
	}
	
	int pos = tabla->partidas[slot].jugadores.num;
	
	tabla->partidas[slot].jugadores.lista[pos].socket = socket;
	
	// Inicialmente asignaremos el jugador como no confirmado,
	// quiere decir que aun no ha confirmado pertenecer a la partida (invitacion)
	tabla->partidas[slot].jugadores.lista[pos].confirmado = 0;
	tabla->partidas[slot].jugadores.num++;
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return 1;
}

/*
	Elimina un jugador (su socket) de la partida con el slot especificado,
	devuelve -1 en caso de error, 1 en caso de exito
*/
void EliminaJugadorEnPartida(TTablaPartidas* tabla, int slot, int socket)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	// La partida debe estar creada
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	for (int i = 0; i < tabla->partidas[slot].jugadores.num; i++)
	{
		if (tabla->partidas[slot].jugadores.lista[i].socket == socket)
		{
			for (int j = i; j < tabla->partidas[slot].jugadores.num - 1; j++)
			{
				tabla->partidas[slot].jugadores.lista[j].socket = tabla->partidas[slot].jugadores.lista[j + 1].socket;
				tabla->partidas[slot].jugadores.lista[j].confirmado = tabla->partidas[slot].jugadores.lista[j + 1].confirmado;
			}
			
			tabla->partidas[slot].jugadores.num--;
			break;
		}
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Marca como confirmado el jugador con el socket especificado en la partida
	especificada, devuelve -1 en caso de error, 1 en caso de exito
*/
int MarcarJugadorConfirmado(TTablaPartidas* tabla, int slot, int socket)
{
	if (slot >= MAX_PARTIDAS)
	{
		return -1;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return -1;
	}
	
	if (tabla->partidas[slot].empezada == 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return -1;
	}
	
	for (int i = 0; i < tabla->partidas[slot].jugadores.num; i++)
	{
		if (tabla->partidas[slot].jugadores.lista[i].socket == socket)
		{
			tabla->partidas[slot].jugadores.lista[i].confirmado = 1;
			
			pthread_mutex_unlock(&mutexTablaPartidas);
			return 1;
		}
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return -1;
}

/*
	Elimina la partida especificada de la tabla
*/
void EliminaPartida(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	// Al marcar creada = 0, usaremos este hueco nuevamente y quedara libre
	// tambien debemos reiniciar la lista de jugadores
	tabla->partidas[slot].creada = 0;
	tabla->partidas[slot].jugadores.num = 0;
	
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Devuelve (llena el buffer especificado) la lista de sockets de los jugadores
	de la partida especificada. Si la partida tiene menos jugadores que el maximo,
	los huecos vacios seran -1
*/
int ObtenerSocketsJugadoresPartida(TTablaPartidas* tabla, int slot, int buffer[MAX_JUGADORES_PARTIDA])
{
	if (slot >= MAX_PARTIDAS)
	{
		return 0;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return 0;
	}
	
	if (tabla->partidas[slot].jugadores.num <= 0)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return 0;
	}
	
	for (int i = 0; i < tabla->partidas[slot].jugadores.num; i++)
	{
		buffer[i] = tabla->partidas[slot].jugadores.lista[i].socket;
	}
	
	// Los huecos libres los dejaremos como -1 para identificarlos facilmente
	for (int j = tabla->partidas[slot].jugadores.num; j < MAX_JUGADORES_PARTIDA; j++)
	{
		buffer[j] = -1;
	}
	
	int n = tabla->partidas[slot].jugadores.num;
	pthread_mutex_unlock(&mutexTablaPartidas);
	
	return n;
}

/*
	Devuelve 1 si la partida especificada tiene todos los jugadores confirmados,
	0 en caso contrario o de error
*/
int EstanTodosJugadoresConfirmados(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return 0;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return 0;
	}
	
	if (tabla->partidas[slot].jugadores.num <= 0)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return 0;
	}
	
	for (int i = 0; i < tabla->partidas[slot].jugadores.num; i++)
	{
		if (tabla->partidas[slot].jugadores.lista[i].confirmado == 0)
		{
			pthread_mutex_unlock(&mutexTablaPartidas);
			return 0;
		}
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return 1;
}

/*
	Devuelve el slot de la partida en que esta el jugador, -1 si no esta en ninguna
*/
int ObtenerPartidaJugador(TTablaPartidas* tabla, int socket)
{
	pthread_mutex_lock(&mutexTablaPartidas);
	
	for (int i = 0; i < MAX_PARTIDAS; i++)
	{
		for (int j = 0, n = tabla->partidas[i].jugadores.num; j < n; j++)
		{
			if (tabla->partidas[i].jugadores.lista[j].socket == socket)
			{
				pthread_mutex_unlock(&mutexTablaPartidas);
				return i;
			}
		}
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return -1;
}

/*
	Devuelve el socket que corresponde al nombre especificado,
	-1 en caso de error
*/
int DameSocketDeNombre(TListaConectados* lista, char nombre[STR_SIZE])
{
	int i = 0;
	pthread_mutex_lock(&mutexListaConectados);
	while (i < lista->num)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			pthread_mutex_unlock(&mutexListaConectados);
			return lista->conectados[i].socket;
		}
		i=i+1;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return -1;
}
	
/*
	Devuelve (llena en el buffer) la lista de conectados
*/
void DameConectados(TListaConectados* lista, char respuesta[BUFFER_SIZE])
{
	pthread_mutex_lock(&mutexListaConectados);
	strcpy(respuesta, "");
	sprintf(respuesta, "%d/", lista->num);
	
	char buffer[BUFFER_SIZE];
	strcpy(buffer, "");
	
	for (int i = 0; i < lista->num; i++)
	{
		sprintf(buffer, "%s%s,%d,", buffer, lista->conectados[i].nombre, lista->conectados[i].estado);
		//strcat(respuesta, lista->conectados[i].nombre);
		//strcat(respuesta, ",");
	}
	
	sprintf(respuesta, "%s%s", respuesta, buffer);
	
	pthread_mutex_unlock(&mutexListaConectados);
}

/*
	Devuelve la lista de sockets conectados
*/
int DameSocketsConectados(TListaConectados* lista, int buffer[MAX_CONECTADOS])
{
	pthread_mutex_lock(&mutexListaConectados);
	
	for (int i = 0; i < lista->num; i++)
	{
		buffer[i] = lista->conectados[i].socket;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return lista->num;
}

/*
	Devuelve la Id de la base de datos de un usuario en base a su nombre de usuario,
	devuelve -1 en caso de no existir
*/
int ObtenerIdDeUsuarioConectado(TListaConectados* lista, char usuario[STR_SIZE])
{
	int i = 0;
	pthread_mutex_lock(&mutexListaConectados);
	while (i < lista->num)
	{
		if (strcmp(lista->conectados[i].nombre, usuario) == 0)
		{
			pthread_mutex_unlock(&mutexListaConectados);
			return lista->conectados[i].id;
		}
		i++;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return -1;
}

/*
	Devuelve (llena en el buffer) los jugadores que estan en la partida indicada
*/
void ObtenerNombresJugadoresPartida(TListaConectados* lista, TTablaPartidas* tabla, int slot, char respuesta[BUFFER_SIZE])
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return;
	}
	
	strcpy(respuesta, "");
	sprintf(respuesta, "%d/", tabla->partidas[slot].jugadores.num);
	
	if (tabla->partidas[slot].jugadores.num > 0)
	{
		char nombre[STR_SIZE];
		int socket;
		int i = 0;
		
		for (; i < tabla->partidas[slot].jugadores.num; i++)
		{
			socket = tabla->partidas[slot].jugadores.lista[i].socket;
			
			if (ObtenerNombreDeSocket(lista, socket, nombre) == 0)
			{
				continue;
			}
			
			// El jugador debe estar en la sala, no pendiente (o en partida?)
			if (2 != ObtenerEstadoConectado(lista, socket))
			{
				continue;
			}
			
			strcat(respuesta, nombre);
			strcat(respuesta, ",");
		}
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Marca la partida indicada como empezada
*/
void MarcarPartidaEmpezada(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return;
	}
	
	tabla->partidas[slot].empezada = 1;
	pthread_mutex_unlock(&mutexTablaPartidas);
}

/*
	Devuelve si una partida esta empezada o no
*/
int EstaPartidaEmpezada(TTablaPartidas* tabla, int slot)
{
	if (slot >= MAX_PARTIDAS)
	{
		return 0;
	}
	
	pthread_mutex_lock(&mutexTablaPartidas);
	
	if (tabla->partidas[slot].creada != 1)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return 0;
	}
	
	int estado = 0;
	
	if (tabla->partidas[slot].empezada == 1)
	{
		estado = 1;
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return estado;
}
