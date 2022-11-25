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
		i=i+1;
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
	return -1;
}

/*
	Introduce un usuario y socket en la lista de conectados, devuelve 0 si
	no es posible, 1 en caso de exito
*/
int IntroduceConectado(TListaConectados* lista, char nombre[STR_SIZE], int socket)
{
	pthread_mutex_lock(&mutexListaConectados);
	int posicion = lista->num;
	
	if (posicion < MAX_CONECTADOS)
	{
		strcpy(lista->conectados[posicion].nombre, nombre);
		lista->conectados[posicion].socket = socket;
		
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
			strcpy(lista->conectados[i].nombre, lista->conectados[i + 1].nombre);
			lista->conectados[i].socket = lista->conectados[i + 1].socket;
		}
		lista->num = lista->num-1;
		pthread_mutex_unlock(&mutexListaConectados);
		
		return 0;
	}
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
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
	return partidaSlot;
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
void ObtenerSocketsJugadoresPartida(TTablaPartidas* tabla, int slot, int buffer[MAX_JUGADORES_PARTIDA])
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
	
	if (tabla->partidas[slot].jugadores.num <= 0)
	{
		pthread_mutex_unlock(&mutexTablaPartidas);
		return;
	}
	
	for (int i = 0; i < tabla->partidas[slot].jugadores.num; i++)
	{
		buffer[i] = tabla->partidas[slot].jugadores.lista[i].socket;
	}
	
	// Los huecos libres los dejaremos como -1 para identificarlos facilmente
	for (int i = tabla->partidas[slot].jugadores.num; i < MAX_JUGADORES_PARTIDA; i++)
	{
		buffer[i] = -1;
	}
	
	pthread_mutex_unlock(&mutexTablaPartidas);
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
	
	for (int i = 0; i < lista->num; i++)
	{
		strcat(respuesta, lista->conectados[i].nombre);
		strcat(respuesta, ",");
	}
	
	pthread_mutex_unlock(&mutexListaConectados);
}
