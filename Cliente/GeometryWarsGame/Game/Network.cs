using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using GeometryWarsGame.Game.Entities;
using GeometryWarsGame.Game.Utils;

namespace GeometryWarsGame.Game
{
    public static class Network
    {
        private static Socket? server;
        private static Thread? networkThread;

        private static bool running = true;

        /// <summary>
        /// Initialize networking thread
        /// </summary>
        public static void Initialize()
        {
            networkThread = new Thread(NetworkingLoop);
            networkThread.Priority = ThreadPriority.AboveNormal;
            networkThread.Start();
        }

        /// <summary>
        /// Set the socket instance with server
        /// </summary>
        /// <param name="s"></param>
        public static void SetSocket(Socket s)
        {
            server = s;
            server.ReceiveTimeout = 0;
        }

        /// <summary>
        /// Stop networking thread
        /// </summary>
        public static void Stop()
        {
            running = false;
        }

        /// <summary>
        /// Convert the given string in ASCII and send to the server
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task Send(string data)
        {
            await Send(Encoding.ASCII.GetBytes(data + "$"));
        }

        /// <summary>
        /// Send the given data to the server
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task Send(byte[] data)
        {
            if (server == null)
            {
                return;
            }

            _ = await server.SendAsync(data, SocketFlags.None);
        }

        private static void NetworkingLoop()
        {
            if (server == null)
            {
                return;
            }

            Logs.PrintDebug("Started networking");

            // Create buffer once!
            byte[] buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(8192);

            try
            {
                while (running)
                {                    
                    int n;

                    try
                    {
                        n = server.Receive(buffer);
                    }
                    catch (SocketException)
                    {
                        continue;
                    }

                    if (n == 0)
                    {
                        continue;
                    }

                    string[] messages = Encoding.ASCII.GetString(buffer, 0, n).Split('\0')[0].Split('$');

                    foreach (string m in messages)
                    {
                        if (m.Length <= 0)
                        {
                            continue;
                        }

                        ProcessMessage(m);
                    }
                }
            }
            catch (Exception) { }
        }

        private static void ProcessMessage(string _message)
        {
            string[] message = _message.Split('/');
            int op = Convert.ToInt32(message[0]);

            // op = 100 utilizado para paquetes del juego
            // Formato: 100/<gop>/..., `gop` = game op
            if (op == 100)
            {
                int gop = Convert.ToInt32(message[1]);

                switch (gop)
                {
                    // Indica que el servidor ya ha marcado la partida como empezada
                    // y el resto de jugadores estan en pantalla de carga,
                    // podemos empezar a ordenar crear entidades (como jugadores)
                    case 0:
                        Window.CallThreaded(() =>
                        {
                            foreach (var item in Program.GameWindow.PlayerSpawnData)
                            {
                                // Somos nosotros, actualizamos nuestra posicion
                                if (item.Name == Program.GameWindow.MyUsername)
                                {
                                    Program.GameWindow.MyPlayer!.Position = item.Position;
                                    continue;
                                }

                                // Es otro jugador, lo creamos
                                Player p = new Player(item.Name, item.Position.X, item.Position.Y);
                                p.Create();

                                Utils.Task.RunAndForget(Send("100/1/" + p.Id + "/" + p.Name + "/" + p.Position.X + "/" + p.Position.Y));
                            }

                            Utils.Task.RunAndForget(Send("100/1/" + Program.GameWindow.MyPlayer!.Id + "/" + Program.GameWindow.MyPlayer.Name + "/" + Program.GameWindow.MyPlayer.Position.X + "/" + Program.GameWindow.MyPlayer.Position.Y));

                            new TimerManager.Timer(5000, () =>
                            {
                                Utils.Task.RunAndForget(Send("100/3"));
                            });
                        });
                        break;

                    // Indica la creacion de un jugador
                    // Formato: 100/1/<entidad id>/<usuario>/<x>/<y>
                    case 1:
                        int id = Convert.ToInt32(message[2]);
                        string username = message[3];
                        float x = Convert.ToSingle(message[4]);
                        float y = Convert.ToSingle(message[5]);

                        // Lo ejecutamos en el hilo principal
                        Window.CallThreaded(() =>
                        {
                            // Si somos nosotros mismos, creamos un LocalPlayer
                            if (username == Program.GameWindow.MyUsername && !Program.GameWindow.Master)
                            {
                                Program.GameWindow.MyPlayer = new LocalPlayer(id, username);
                                Program.GameWindow.MyPlayer.Position.X = x;
                                Program.GameWindow.MyPlayer.Position.Y = y;

                                Program.GameWindow.MyPlayer.Create();
                            }

                            // Si la entidad ya existe, no la creamos
                            Player? e = (Player?)EntityManager.GetById(id);

                            if (e != null)
                            {
                                e.Spawn();
                                return;
                            }

                            // Si no existe, la creamos y spawneamos
                            Player pp = new Player(id, username, x, y);
                            pp.Create();
                            pp.Spawn();
                        });
                        break;

                    // Empezar el gameplay
                    // A jugarrrr
                    case 3:
                        Window.CallThreaded(() =>
                        {
                            Program.GameWindow.StartGameplay();
                        });
                        break;

                    // Sincronizar posicion y heading jugadores
                    // Formato: 100/5/<id>/<x>/<y>/<heading>
                    case 5:
                        id = Convert.ToInt32(message[2]);
                        x = Convert.ToSingle(message[3]);
                        y = Convert.ToSingle(message[4]);
                        float heading = Convert.ToSingle(message[5]);

                        Window.CallThreaded(() =>
                        {
                            Player? p = (Player?)EntityManager.GetById(id);

                            if (p == null)
                            {
                                return;
                            }

                            p.Heading = heading;
                            p.Position.X = x;
                            p.Position.Y = y;
                        });
                        break;

                    // Sincronizar disparos
                    // Formato: 100/6/<from entity id>/<heading>/<x>/<y>
                    case 6:
                        id = Convert.ToInt32(message[2]);
                        heading = Convert.ToSingle(message[3]);
                        x = Convert.ToSingle(message[4]);
                        y = Convert.ToSingle(message[5]);

                        Window.CallThreaded(() =>
                        {
                            Bullet b = new Bullet(heading, new Vector2D(x, y));
                            b.Creator = EntityManager.GetById(id);
                            b.Create();
                        });
                        break;

                    // Sincronizar nueva vida entidad
                    // Formato: 100/7/<id>/<health>
                    case 7:
                        id = Convert.ToInt32(message[2]);
                        int health = Convert.ToInt32(message[3]);

                        Window.CallThreaded(() =>
                        {
                            Entity? e = EntityManager.GetById(id);

                            if (e == null)
                            {
                                return;
                            }

                            if (e is not AliveEntity)
                            {
                                return;
                            }

                            (e as AliveEntity)!.Health = health;
                        });
                        break;

                    // Eliminar una entidad
                    // Formato: 100/8/<id>
                    case 8:
                        id = Convert.ToInt32(message[2]);
                        Window.CallThreaded(() =>
                        {
                            Entity? e = EntityManager.GetById(id);

                            if (e == null)
                            {
                                return;
                            }

                            if (e.State == EntityState.Destroyable)
                            {
                                return;
                            }

                            e.MarkForDestroy();
                        });
                        break;

                    // Muerte permamente de un jugador
                    // Formato: 100/9/<id>
                    case 9:
                        id = Convert.ToInt32(message[2]);
                        Window.CallThreaded(() =>
                        {
                            Player? p = (Player?)EntityManager.GetById(id);

                            if (p == null)
                            {
                                return;
                            }

                            if (p.Id == Program.GameWindow.MyPlayer!.Id)
                            {
                                NotificationManager.Notify("You died with no more lifes :(", 4);
                                _ = new TimerManager.Timer(4000, () =>
                                {
                                    LocalPlayer.StartSpecMode();
                                });
                            } else
                            {
                                NotificationManager.Notify(p.Name + " died >:)", 4);
                            }

                            p.Health = 0;
                            p.MarkForDestroy();
                        });
                        break;

                    // Muerte de un jugador (le quedan vidas)
                    // Formato: 100/10/<id>/<vidas>
                    case 10:
                        id = Convert.ToInt32(message[2]);
                        int lifes = Convert.ToInt32(message[3]);
                        Window.CallThreaded(() =>
                        {
                            Logs.PrintDebug("Sync death of player " + id);
                            Player? p = (Player?)EntityManager.GetById(id);

                            if (p == null)
                            {
                                return;
                            }

                            // if its me, show respawn timer
                            if (p.Id == Program.GameWindow.MyPlayer!.Id)
                            {
                                //NotificationManager.Notify("3", 1);
                                //NotificationManager.Notify("2", 1);
                                //NotificationManager.Notify("1", 1);
                            }

                            // If we are not `Master`
                            // we just update player lifes and mark as dead
                            if (!Program.GameWindow.Master)
                            {
                                p.Health = 0;
                                p.Lifes = lifes;
                                return;
                            }

                            // Process respawn
                            Random r = new Random();
                            float x = r.NextSingle() * World.Width;
                            float y = r.NextSingle() * World.Height;

                            // After timeout, player respawns
                            _ = new TimerManager.Timer(3000, () =>
                            {
                                Utils.Task.RunAndForget(Send("100/11/" + p.Id + "/" + x + "/" + y));
                            });
                        });
                        break;

                    // Respawn de un jugador que ha muerto
                    // Formato: 100/11/<id>/<x>/<y>
                    case 11:
                        id = Convert.ToInt32(message[2]);
                        x = Convert.ToSingle(message[3]);
                        y = Convert.ToSingle(message[4]);

                        Window.CallThreaded(() =>
                        {
                            Logs.PrintDebug("Sync player respawn " + id);

                            Player? p = (Player?)EntityManager.GetById(id);

                            if (p == null)
                            {
                                return;
                            }

                            p.Position.X = x;
                            p.Position.Y = y;
                            p.MarkedAsDead = false;
                            p.Health = 100;

                            if (p.Id == Program.GameWindow.MyPlayer!.Id)
                            {
                                NotificationManager.Notify("Remaining lifes " + p.Lifes, 3);
                            }
                        });
                        break;
                }
            }
        }
    }
}
