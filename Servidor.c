#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>

#define PUERTO 9050
#define STR_SIZE 256

int Login(char usuario[STR_SIZE], char password[STR_SIZE]);
int Registrarse(char usuario[STR_SIZE], char password[STR_SIZE], int edad);
void Consulta(int consulta_id, char resultadoBuff[STR_SIZE]);

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	char peticion[512];
	char respuesta[512];
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creando el socket\n");
	
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(PUERTO);
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("No se pudo hacer el bind en el puerto especificado\n");
	
	if (listen(sock_listen, 4) < 0)
		printf("No se pudo empezar a escuchar en el puerto especificado\n");
	
	for(;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("Recibida conexion\n");
		
		int acabado=0;
		while (acabado==0)
		{
			ret=read(sock_conn,peticion, sizeof(peticion));
			printf ("Recibida una peticion\n");
			
			// Tenemos que añadirle la marca de fin de string 
			// para que no escriba lo que hay despues en el buffer
			peticion[ret]='\0';
			
			printf ("La peticion es: %s\n", peticion);
			char *p = strtok(peticion, "/");
			int codigo =  atoi (p);
			
			/*
				Codigo 1: Loguear
				Codigo 2: Registrarse
				Codigo 3: Ejecutar consulta
			*/
			
			if (codigo == 1)
			{
				char usuario[STR_SIZE];
				char password[STR_SIZE];
				
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				
				p = strtok(NULL, "/");
				strcpy(password, p);
				
				printf("Login: usuario %s, contrasena %s\n", usuario, password);
				int resultado = Login(usuario, password);
				
				if (resultado <= 0)
				{
					strcpy(respuesta, "NO");
				} else
				{
					strcpy(respuesta, "SI");
				}
			} else if (codigo == 2)
			{
				char usuario[STR_SIZE];
				char password[STR_SIZE];
				int edad;
				
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				
				p = strtok(NULL, "/");
				strcpy(password, p);
				
				p = strtok(NULL, "/");
				edad = atoi(p);
				
				printf("Registro: usuario %s, contrasena %s, edad %d\n", usuario, password, edad);
				int resultado = Registrarse(usuario, password, edad);
				
				if (resultado <= 0)
				{
					strcpy(respuesta, "NO");
				} else
				{
					strcpy(respuesta, "SI");
				}
			} else if (codigo == 3)
			{
				int consulta;
				
				p = strtok(NULL, "/");
				consulta = atoi(p);
				
				printf("Consulta: %d\n", consulta);
				char resultado[STR_SIZE];
				
				Consulta(consulta, resultado);
				strcpy(respuesta, resultado);
			}
			
			if (codigo != 0)
			{
				printf ("Respuesta: %s\n", respuesta);
				write (sock_conn,respuesta, strlen(respuesta));
			}
		}
		close(sock_conn);
	}
}

int Login(char usuario[STR_SIZE], char password[STR_SIZE])
{
	MYSQL* conn;
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	conn = mysql_init(NULL);
	if (conn == NULL)
	{
		printf("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	
	conn = mysql_real_connect(conn, "localhost","root", "mysql", "geometry_wars",0, NULL, 0);
	if (conn == NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	
	char consulta[STR_SIZE];
	strcpy(consulta, "select jugador.id from jugador where jugador.username='");
	strcat(consulta,usuario);
	strcat(consulta,"'and jugador.password='");
	strcat(consulta, password);
	strcat(consulta, "';");
	
	err = mysql_query(conn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(conn), mysql_error(conn));
		mysql_close(conn);
		return -1;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	int resultado_login;
	if (row == NULL)
	{
		resultado_login = 0;
	} else
	{
		resultado_login = 1;
	}

	mysql_close(conn);
	return resultado_login;
}

int Registrarse(char usuario[STR_SIZE], char password[STR_SIZE], int edad)
{
	MYSQL* conn;
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	conn = mysql_init(NULL);
	if (conn == NULL)
	{
		printf("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	
	conn = mysql_real_connect(conn, "localhost","root", "mysql", "geometry_wars",0, NULL, 0);
	if (conn == NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	
	char consulta[STR_SIZE];
	int siguiente_id;
	char siguiente_id_str[3];
	char edad_str[3];
	
	strcpy(consulta, "select max(id) from jugador;");
	
	err = mysql_query(conn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(conn), mysql_error(conn));
		mysql_close(conn);
		return -1;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL)
	{
		siguiente_id = 1;
	} else
	{
		siguiente_id = atoi(row[0]) + 1;
	}
	
	sprintf(siguiente_id_str, "%d", siguiente_id);
	sprintf(edad_str, "%d", edad);
	
	strcpy(consulta, "INSERT INTO jugador VALUES (");
	strcat(consulta, siguiente_id_str);
	strcat(consulta,",'");
	strcat(consulta, usuario);
	strcat(consulta,"','");
	strcat(consulta, password);
	strcat(consulta,"',");
	strcat(consulta, edad_str);
	strcat(consulta, ");");
	
	err = mysql_query(conn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(conn), mysql_error(conn));
		mysql_close(conn);
		return -1;
	}

	mysql_close(conn);
	return 1;
}

void Consulta(int consulta_id, char resultadoBuff[STR_SIZE])
{
	MYSQL* conn;
	MYSQL_RES* resultado;
	MYSQL_ROW row;
	int err;
	
	conn = mysql_init(NULL);
	if (conn == NULL)
	{
		printf("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		return;
	}
	
	conn = mysql_real_connect(conn, "localhost","root", "mysql", "geometry_wars",0, NULL, 0);
	if (conn == NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		return;
	}
	
	char consulta[STR_SIZE];
	if (consulta_id == 1)
	{
		strcpy (consulta,"SELECT jugador.username FROM (jugador,partidas_jugadas) WHERE partidas_jugadas.puntos = (SELECT MAX(partidas_jugadas.puntos) FROM partidas_jugadas) AND jugador.id = partidas_jugadas.jugador_id");
	} else if (consulta_id == 2)
	{
		strcpy (consulta,"select partidas_jugadas.puntos from partidas_jugadas,jugador,partida where jugador.username='Juan' and jugador.id=partidas_jugadas.jugador_id and partidas_jugadas.partida_id=partida.id and partida.ganador=jugador.id");
	} else if (consulta_id == 3)
	{
		strcpy (consulta,"select jugador.username from (jugador, partida) where jugador.edad < 18 and partida.ganador = jugador.id");
	} else
	{
		printf("Consulta %d desconocida\n", consulta_id);
		mysql_close(conn);
		return;
	}
	
	err = mysql_query(conn, consulta);
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s\n",mysql_errno(conn), mysql_error(conn));
		mysql_close(conn);
		return;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL)
	{
		strcpy(resultadoBuff, row[0]);
	} else
	{
		strcpy(resultadoBuff, row[0]);
	}
	
	mysql_close(conn);
}



