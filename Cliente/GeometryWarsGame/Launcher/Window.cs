using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GeometryWarsGame.Launcher
{
    public partial class Window : Form
    {
        /// <summary>
        /// Conexión con el servidor
        /// </summary>
        Socket serverSocket;

        /// <summary>
        /// Hilo para leer constantemente del servidor
        /// </summary>
        Thread atenderThread;

        /// <summary>
        /// Formulario de registro
        /// </summary>
        Register registerForm;

        /// <summary>
        /// Indica si se debe continuar escuchando del servidor
        /// Utilizado por si se cierra el formulario actual y se abre el lobby,
        /// puesto que se destruye este Thread y se crea uno nuevo en el formulario nuevo
        /// </summary>
        bool ejecutando = true;

        //const string servidorIp = "147.83.117.22";
        //const int servidorPuerto = 50059;

        const string servidorIp = "192.168.56.101";
        const int servidorPuerto = 5059;

        public Window()
        {
            InitializeComponent();
        }

        private bool ConectarServidor()
        {
            try
            {
                IPAddress direc = IPAddress.Parse(servidorIp);
                IPEndPoint ipep = new IPEndPoint(direc, servidorPuerto);

                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Si en 100ms después de ejecutar `.Receive()` no se ha recibido nada, se lanzará una excepción `SocketException`
                // Podremos manejar esta excepción y reevaluar `while (ejecutando)` para salir del Thread si es necesario
                serverSocket.ReceiveTimeout = 100;

                serverSocket.Connect(ipep);

                atenderThread = new Thread(AtenderServidor);
                atenderThread.Start();

                return true;
            } catch (SocketException)
            {
                MessageBox.Show("No se pudo conectar con el servidor, reinicia el programa.");
            }

            return false;
        }

        private void AtenderServidor()
        {
            string username = "";
            int userId = -1;
            bool register = false;

            try
            {
                while (Volatile.Read(ref ejecutando))
                {
                    //MessageBox.Show("dentro while");
                    byte[] buffer = new byte[512];
                    int n;

                    try
                    {
                        n = serverSocket.Receive(buffer);
                        //MessageBox.Show("Received window");
                    } catch (SocketException e)
                    {
                        //MessageBox.Show("Excepcion receive " + e);
                        continue;
                    }

                    if (n == 0)
                    {
                        Volatile.Write(ref ejecutando, false);
                        MessageBox.Show("La conexion con el servidor se ha cerrado");
                        Environment.Exit(0);
                        continue;
                    }

                    string[] message = Encoding.ASCII.GetString(buffer).Split('\0')[0].Split('$')[0].Split('/');
                    int op = Convert.ToInt32(message[0]);

                    switch (op)
                    {
                        // Login result
                        // Format: 1/OK/<username>/<userId>, 1/NOK/<error code>
                        // e.g. 1/OK/test1234/1, 1/NOK/1
                        case 1:
                            if (message[1] == "OK")
                            {
                                username = message[2];
                                userId = Convert.ToInt32(message[3]);

                                Volatile.Write(ref ejecutando, false);
                                //ejecutando = false;

                                //Invoke(() =>
                                //{
                                //    //Login(userId, username);
                                //});
                            } else if (message[1] == "NOK")
                            {
                                int errorCode = Convert.ToInt32(message[2]);
                                switch (errorCode)
                                {
                                    case 1:
                                        MessageBox.Show("El usuario y/o la contraseña no existen.");
                                        break;
                                    case 2:
                                        MessageBox.Show("El usuario ya está conectado.");
                                        break;
                                }
                            }
                            break;

                        // Register result
                        // Format: 2/OK/<username>/<userId>, 2/NOK/<error code>
                        // e.g. 2/OK/test1234/5, 2/NOK/1
                        case 2:
                            if (message[1] == "OK")
                            {
                                username = message[2];
                                userId = Convert.ToInt32(message[3]);

                                MessageBox.Show("Te has registrado correctamente.");

                                Volatile.Write(ref ejecutando, false);
                                //ejecutando = false;

                                register = true;
                                //Invoke(() =>
                                //{
                                //    registerForm.Dispose();
                                //    Show();

                                //    Login(userId, username);
                                //});
                            } else if (message[1] == "NOK")
                            {
                                int errorCode = Convert.ToInt32(message[2]);
                                switch (errorCode)
                                {
                                    case 1:
                                        MessageBox.Show("El usuario ya existe en el sistema.");
                                        break;
                                    case 2:
                                        MessageBox.Show("Ha ocurrido un error, vuelve a intentarlo.");
                                        break;
                                }
                            }
                            break;
                    }
                }
                //MessageBox.Show("fuera while");
            }
            catch (Exception e) { MessageBox.Show("Exception " + e); }

            //ejecutando = false;
            Volatile.Write(ref ejecutando, false);
            //MessageBox.Show("out 1");

            BeginInvoke(() =>
            {
                if (register)
                {
                    registerForm.Dispose();
                    Show();
                }

                Login(userId, username);
            });
        }

        private void Login(int userId, string username)
        {
            Lobby form = new Lobby();

            form.SetSocket(serverSocket);
            form.SetUser(username, userId);

            //ejecutando = false;
            Volatile.Write(ref ejecutando, false);
            atenderThread.Join();

            Hide();
            form.ShowDialog();
            Close();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (serverSocket == null)
            {
                return;
            }

            string user = userTb.Text.Trim();
            string password = passTb.Text.Trim();

            if (user.Length > 255 || password.Length > 255)
            {
                MessageBox.Show("Máxima longitud usuario/contraseña de 255 caracteres.");
                return;
            }

            serverSocket.Send(Encoding.ASCII.GetBytes("1/" + user + "/" + password));
        }

        private void Window_Load(object sender, EventArgs e)
        {
            if (serverSocket == null && ConectarServidor())
            {
                loginBtn.Enabled = true;
                registerBtn.Enabled = true;
            }
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            if (serverSocket == null)
            {
                return;
            }

            // Abrimos un formulario para el registro, pero recibiremos los mensajes del servicio en este formulario
            registerForm = new Register();
            registerForm.SetSocket(serverSocket);
            Hide();
            registerForm.ShowDialog();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            ejecutando = false;
            atenderThread.Join();

            if (serverSocket.Connected)
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
            }
        }
    }
}
