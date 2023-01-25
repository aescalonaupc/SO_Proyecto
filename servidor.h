#ifndef SERVIDOR_H
#define SERVIDOR_H

#include <mysql.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <pthread.h>

#include "constantes.h"
#include "conectados.h"

void EjecutarServidor();
void* AtenderCliente(void* socket);

int Login(char usuario[STR_SIZE], char password[STR_SIZE]);
int Registrarse(char usuario[STR_SIZE], char password[STR_SIZE]);
void Consulta(int consulta_id, char resultadoBuff[STR_SIZE]);
void DameConectados(TListaConectados* lista, char respuesta[BUFFER_SIZE]);
int ComprobarSiYaEstaRegistrado(char usuario[STR_SIZE]);
int EliminarUsuario(int usuarioId);

int CrearPartidaBd();
void RegistrarFinPartida(int id, int duracion, int ganador);
void RegistrarPartidaParaJugador(int jugadorId, int partidaId);

int ObtenerPartidasJugador(int jugadorId);
int ObtenerPartidasGanadasJugador(int jugadorId);
int ObtenerMinutosJugador(int jugadorId);

#endif
