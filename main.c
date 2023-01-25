#include "main.h"

int main (int argc, char** argv)
{
	// Inicializamos la lista de conectados
	listaConectados.num = 0;
	
	// Inicializamos la conexion con la base de datos
	mysqlConn = mysql_init(NULL);
	if (mysqlConn == NULL)
	{
		printf("Error al crear la conexion: %u %s\n", mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	mysqlConn = mysql_real_connect(mysqlConn, MYSQL_HOST, MYSQL_USER, MYSQL_PASS, MYSQL_DB, 0, NULL, 0);
	if (mysqlConn == NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", mysql_errno(mysqlConn), mysql_error(mysqlConn));
		return -1;
	}
	
	printf("Ejecutando servidor...\n");
	printf("Hora: %d\n", (unsigned)time(NULL));
	EjecutarServidor();
	return 0;
}
