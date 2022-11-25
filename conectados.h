#ifndef CONECTADOS_H
#define CONECTADOS_H

#include "constantes.h"
#include <pthread.h>

/* Representa una persona conectada en el servidor */
typedef struct {
	char nombre[STR_SIZE];
	int socket;
} TConectado;

/* Representa la lista de personas conectadas en el servidor */
typedef struct {
	TConectado conectados[MAX_CONECTADOS];
	int num;
} TListaConectados;

/* Representa un jugador de una partida */
typedef struct {
	int socket;
	int confirmado;
} TJugador;

/* Representa la lista de jugadores de una partida */
typedef struct {
	TJugador lista[MAX_JUGADORES_PARTIDA];
	int num;
} TListaJugadores;

/* Representa una partida */
typedef struct {
	int id;
	TListaJugadores jugadores;
	int empezada;
	int creada;
} TPartida;

/* Representa todas las partidas del servidor */
typedef struct {
	TPartida partidas[MAX_PARTIDAS];
} TTablaPartidas;

/* Mutex de la lista de conectados */
pthread_mutex_t mutexListaConectados;

/* Mutex de la tabla de partidas */
pthread_mutex_t mutexTablaPartidas;

int IntroduceConectado(TListaConectados* lista, char nombre[STR_SIZE], int socket);
int EliminaConectado(TListaConectados* lista, char nombre[STR_SIZE]);

int IntroducePartida(TTablaPartidas* tabla);
int IntroduceJugadorEnPartida(TTablaPartidas* tabla, int slot, int socket);
int MarcarJugadorConfirmado(TTablaPartidas* tabla, int slot, int socket);
void ObtenerSocketsJugadoresPartida(TTablaPartidas* tabla, int slot, int buffer[MAX_JUGADORES_PARTIDA]);
int EstanTodosJugadoresConfirmados(TTablaPartidas* tabla, int slot);
void EliminaPartida(TTablaPartidas* tabla, int slot);

int DamePos(TListaConectados* lista, char nombre[STR_SIZE]);
void DameConectados(TListaConectados* lista, char respuesta[BUFFER_SIZE]);
int DameSocketDeNombre(TListaConectados* lista, char nombre[STR_SIZE]);

#endif
