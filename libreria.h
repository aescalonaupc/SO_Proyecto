#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#define STR_SIZE 256


int Login(char usuario[STR_SIZE], char password[STR_SIZE]);
int Registrarse(char usuario[STR_SIZE], char password[STR_SIZE], int edad);
void Consulta(int consulta_id, char resultadoBuff[STR_SIZE]);
void *AtenderCliente (void *socket);