DROP DATABASE IF EXISTS geometry_wars;
CREATE DATABASE geometry_wars;

USE geometry_wars;

CREATE TABLE jugador (
	id INT NOT NULL,
	username VARCHAR(60) NOT NULL,
	password VARCHAR(60) NOT NULL,
	edad INT NOT NULL,
	
	PRIMARY KEY (id)
) ENGINE=InnoDB;

CREATE TABLE partida (
	id INT NOT NULL,
	nombre VARCHAR(60) NOT NULL,
	modo INT NOT NULL,
	
	fecha_fin TIMESTAMP, -- Este campo se actualizará al acabar la partida
	duracion INT, -- Este campo se actualizará al acabar la partida
	ganador INT, -- Este campo se actualizará al acabar la partida
	
	PRIMARY KEY (id)
) ENGINE=InnoDB;

CREATE TABLE partidas_jugadas (
	partida_id INT NOT NULL,
	jugador_id INT NOT NULL,
	puntos INT NOT NULL,
	
	FOREIGN KEY (partida_id) REFERENCES partida (id),
	FOREIGN KEY (jugador_id) REFERENCES jugador (id)
) ENGINE=InnoDB;

INSERT INTO jugador VALUES (1,'Juan','1234',15);
INSERT INTO jugador VALUES (2,'Albert','1234',32);
INSERT INTO jugador VALUES (3,'Eva','1234',45);
INSERT INTO jugador VALUES (4,'Julia','1234',19);
INSERT INTO jugador VALUES (5,'Marcos','1234',19);
INSERT INTO jugador VALUES (6,'Maria','1234',23);

INSERT INTO partida VALUES (1,'Partida 1',1,'2022-10-06 22:10:30',20,1);
INSERT INTO partida VALUES (2,'Partida 2',0,'2022-10-07 23:15:35',10,5);
INSERT INTO partida VALUES (3,'Partida 3',1,'2022-10-08 01:20:40',5,3);

INSERT INTO partidas_jugadas VALUES (1,1,150);
INSERT INTO partidas_jugadas VALUES (1,2,20);
INSERT INTO partidas_jugadas VALUES (1,3,45);
INSERT INTO partidas_jugadas VALUES (1,4,19);
INSERT INTO partidas_jugadas VALUES (2,5,500);
INSERT INTO partidas_jugadas VALUES (3,2,300);
INSERT INTO partidas_jugadas VALUES (3,3,34);
INSERT INTO partidas_jugadas VALUES (3,4,20);
INSERT INTO partidas_jugadas VALUES (3,6,21);
INSERT INTO partidas_jugadas VALUES (3,1,23);
