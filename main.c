#include "main.h"

void PrintDebug()
{
	printf("STR_SIZE: %d\n", STR_SIZE);
	printf("BUFFER_SIZE: %d\n", BUFFER_SIZE);
	printf("MAX_CONECTADOS: %d\n", MAX_CONECTADOS);
}

int main (int argc, char** argv)
{
	listaConectados.num = 0;
	
	printf("Ejecutando servidor...\n");
	EjecutarServidor();
	return 0;
}
