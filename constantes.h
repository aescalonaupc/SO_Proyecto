#ifndef CONSTANTS_H
#define CONSTANTS_H

//#define DEV 1

// Tamaño en caracteres de un string
#define STR_SIZE 256

// Tamaño en caracteres de un buffer, es decir,
// cualquier string que codifique un mensaje
#define BUFFER_SIZE 512

// Puerto en que escuchara conexiones el servidor
#define PUERTO 50059

// Maximo numero de jugadores conectados
#define MAX_CONECTADOS 100

// Configuracion de la base de datos
#ifndef DEV
#define MYSQL_HOST "shiva2.upc.es"
#else
#define MYSQL_HOST "localhost"
#endif

#define MYSQL_USER "root"
#define MYSQL_PASS "mysql"
#define MYSQL_DB "TG4_Geometry_Wars"

#endif
