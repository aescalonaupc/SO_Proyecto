#ifndef CONECTADOS_H
#define CONECTADOS_H

#include "constantes.h"
#include <pthread.h>

/* Representa una persona conectada en el servidor */
typedef struct {
	/* Nombre del usuario */
	char nombre[STR_SIZE];
	
	/* Id en la base de datos */
	int id;
	
	/* Socket para comunicarnos */
	int socket;
	
	/* 0 = Default, 1 = Invitacion pendiente, 2 = En sala, 3 = Jugando */
	int estado;
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
	/* Id de la partida en la base de datos */
	int id;
	
	/* Referencia temporal cuando empezo la partida, para saber la duracion */
	int tiempoInicio;
	
	/* Lista de jugadores en la partida */
	TListaJugadores jugadores;
	
	/* 1 si la partida esta empezada (jugando), 0 si no (lobby) */
	int empezada;
	
	/* 1 si la partida ha sido creada, teoricamente es siempre 1 */
	int creada;
	
	/* Socket del creador de la partida */
	int lider;
	
	/* Modo de juego de la partida, 0 = sandbox, 1 = AvA, 2 = Coop */
	int tipo;
	
	/* 1 si la partida ya ha sido finalizada, 0 si aun no */
	int finalizada;
} TPartida;

/* Representa todas las partidas del servidor */
typedef struct {
	TPartida partidas[MAX_PARTIDAS];
} TTablaPartidas;

/* Mutex de la lista de conectados */
pthread_mutex_t mutexListaConectados;

/* Mutex de la tabla de partidas */
pthread_mutex_t mutexTablaPartidas;

int IntroduceConectado(TListaConectados* lista, char nombre[STR_SIZE], int socket, int id);
int EliminaConectado(TListaConectados* lista, char nombre[STR_SIZE]);
int DamePos(TListaConectados* lista, char nombre[STR_SIZE]);
int DamePosSocket(TListaConectados* lista, int socket);
void DameConectados(TListaConectados* lista, char respuesta[BUFFER_SIZE]);
int DameSocketDeNombre(TListaConectados* lista, char nombre[STR_SIZE]);
void EstablecerEstadoConectado(TListaConectados* lista, int socket, int estado);
int ObtenerEstadoConectado(TListaConectados* lista, int socket);
int ObtenerNombreDeSocket(TListaConectados* lista, int socket, char nombre[STR_SIZE]);
int DameSocketsConectados(TListaConectados* lista, int buffer[MAX_CONECTADOS]);
int ObtenerIdDeUsuarioConectado(TListaConectados* lista, char usuario[STR_SIZE]);

int IntroducePartida(TTablaPartidas* tabla);
int IntroduceJugadorEnPartida(TTablaPartidas* tabla, int slot, int socket);
void EliminaJugadorEnPartida(TTablaPartidas* tabla, int slot, int socket);
int MarcarJugadorConfirmado(TTablaPartidas* tabla, int slot, int socket);
int ObtenerSocketsJugadoresPartida(TTablaPartidas* tabla, int slot, int buffer[MAX_JUGADORES_PARTIDA]);
void ObtenerNombresJugadoresPartida(TListaConectados* lista, TTablaPartidas* tabla, int slot, char respuesta[BUFFER_SIZE]);
int EstanTodosJugadoresConfirmados(TTablaPartidas* tabla, int slot);
void EliminaPartida(TTablaPartidas* tabla, int slot);
int ObtenerPartidaJugador(TTablaPartidas* tabla, int socket);
void MarcarPartidaEmpezada(TTablaPartidas* tabla, int slot);
int EstaPartidaEmpezada(TTablaPartidas* tabla, int slot);
void EstablecerPartidaId(TTablaPartidas* tabla, int slot, int id);
void EstablecerPartidaLider(TTablaPartidas* tabla, int slot, int socket);
void EstablecerPartidaTipo(TTablaPartidas* tabla, int slot, int tipo);
int ObtenerLiderPartida(TTablaPartidas* tabla, int slot);
int EstaPartidaFinalizada(TTablaPartidas* tabla, int slot);
void MarcarPartidaFinalizada(TTablaPartidas* tabla, int slot);

#endif
