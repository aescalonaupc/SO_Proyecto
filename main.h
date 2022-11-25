#ifndef MAIN_H
#define MAIN_H

#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <stdio.h>

#include "constantes.h"

#ifndef DEV
//#include <my_global.h>
#endif

#include "servidor.h"
#include "conectados.h"

/* Lista de conectados en el servidor */
TListaConectados listaConectados;

/* Tabla de partidas en el servidor */
TTablaPartidas tablaPartidas;

/* Conexión con la base de dados */
MYSQL* mysqlConn;

#endif
