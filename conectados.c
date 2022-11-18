#include "conectados.h"
#include "main.h"

pthread_mutex_t mutex= PTHREAD_MUTEX_INITIALIZER;

int DamePos(TListaConectados* lista, char nombre[STR_SIZE])
{
	int i = 0;
	
	pthread_mutex_lock(&mutex);
	while (i < lista->num)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			pthread_mutex_unlock(&mutex);
			return i;
		}
	}
	
	pthread_mutex_unlock(&mutex);
	return -1;
}

int IntroduceConectado(TListaConectados* lista, char nombre[STR_SIZE], int socket)
{
	int posicion = lista->num;
	
	if (posicion < MAX_CONECTADOS)
	{
		pthread_mutex_lock(&mutex);
		strcpy(lista->conectados[posicion].nombre, nombre);
		lista->conectados[posicion].socket = socket;
		lista->num = posicion + 1;
		pthread_mutex_unlock(&mutex);
		
		return 0;
	}
	
	return 1;
}

int EliminaConectado(TListaConectados* lista, char nombre[STR_SIZE])
{
	int pos = DamePos(lista, nombre);
	
	if (pos == -1)
	{
		return -1;
	}
	
	int i = pos;
	pthread_mutex_lock(&mutex);
	for (; i < lista->num; i++)
	{
		strcpy(lista->conectados[i].nombre, lista->conectados[i + 1].nombre);
		lista->conectados[i].socket = lista->conectados[i + 1].socket;
	}
	
	lista->num = lista->num - 1;
	pthread_mutex_unlock(&mutex);
	return 0;
}

void DameConectados(TListaConectados* lista, char respuesta[BUFFER_SIZE])
{
	pthread_mutex_lock(&mutex);
	strcpy(respuesta, "");
	sprintf(respuesta, "%d/", lista->num);
	
	for (int i = 0; i < lista->num; i++)
	{
		strcat(respuesta, lista->conectados[i].nombre);
		strcat(respuesta, ",");
	}
	
	pthread_mutex_unlock(&mutex);
}
