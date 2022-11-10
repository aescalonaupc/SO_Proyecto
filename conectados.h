#ifndef CONECTADOS_H
#define CONECTADOS_H

#include "constantes.h"
#include <pthread.h>

typedef struct {
	char nombre[STR_SIZE];
	int socket;
} TConectado;

typedef struct {
	TConectado conectados[MAX_CONECTADOS];
	int num;
} TListaConectados;

int IntroduceConectado(TListaConectados* lista, char nombre[STR_SIZE], int socket);
int DamePos(TListaConectados* lista, char nombre[STR_SIZE]);
int EliminaConectado(TListaConectados* lista, char nombre[STR_SIZE]);

#endif