using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cliente
{
    public partial class Geometry_wars : Form
    {
        Socket server;
        Thread atender;

        public const string ServidorIp = "147.83.117.22";

        public const int ServidorPuerto = 50059;

        public Geometry_wars()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }


        private void AtenderServidor()
        {
            try
            {
                while (true)
                {
                    byte[] msg = new byte[80];
                    server.Receive(msg);
                    string mensaje = Encoding.ASCII.GetString(msg).Split('\0')[0];

                    //MessageBox.Show(mensaje);
                    string[] trozos = mensaje.Split('/');
                    int peticion = Convert.ToInt32(trozos[0]);
                    string respuesta;
                    switch (peticion)
                    {
                        case 1:
                            respuesta = trozos[1].Split('\0')[0];
                            //MessageBox.Show(respuesta);

                            if (respuesta == "SI")
                            {
                                MessageBox.Show("Bienvenido");

                                Invoke(new Action(() =>
                                {
                                    usuario.Visible = false;
                                    contraseña.Visible = false;
                                    label2.Visible = false;
                                    label3.Visible = false;
                                    edad.Visible = false;
                                    label4.Visible = false;
                                    login.Visible = false;
                                    registration.Visible = false;

                                    label1.Visible = true;

                                    consulta1.Visible = true;
                                    consulta2.Visible = true;
                                    consulta3.Visible = true;
                                    desconexion.Visible = true;
                                    ejecutarBtn.Visible = true;
                                }));
                            }
                            else
                                MessageBox.Show("No se ha encontrado el usuario,revisa tu contraseña o registrate");
                            break;
                        case 2:
                            respuesta = trozos[1].Split('\0')[0];
                            MessageBox.Show(respuesta);
                            if (respuesta == "SI")
                                MessageBox.Show("Te has inscrito correctamente");
                            else
                                MessageBox.Show("No te has podido inscribir vuelve a probar");
                            break;
                        case 3:
                            respuesta = trozos[2].Split('\0')[0];
                            MessageBox.Show("Resultado consulta: " + respuesta);
                            break;
                        case 4:
                            respuesta = trozos[2].Split('\0')[0];
                            int numConectados = Convert.ToInt32(trozos[1]);

                            ListaConectadosView.Invoke(new Action(() =>
                            {
                                if (!ListaConectadosView.Visible)
                                {
                                    ListaConectadosView.Visible = true;
                                    ListaConectadosView.ColumnCount = 2;
                                    ListaConectadosView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                                    ListaConectadosView.GridColor = Color.Black;
                                    ListaConectadosView.RowHeadersVisible = false;
                                    ListaConectadosView.Columns[0].Name = "Número";
                                    ListaConectadosView.Columns[1].Name = "Nombre";
                                }

                                ListaConectadosView.Rows.Clear();

                                string[] conectados = respuesta.Split(',');
                                //MessageBox.Show("Han llegado " + numConectados + " conectados");
                                int i = 1;
                                while (i <= numConectados)
                                {
                                    //MessageBox.Show(conectados[i - 1]);
                                    ListaConectadosView.Rows.Add(i, conectados[i - 1]);
                                    i++;
                                }
                            }));
                            break;
                    }
                }
            } catch (Exception) { }
        }
        private void login_Click(object sender, EventArgs e)
        {
            if (usuario.TextLength == 0)
                MessageBox.Show("Introduce tu usuario");
            else if (contraseña.TextLength == 0)
                MessageBox.Show("Introduce tu contraseña");
            else 
            {
                string mensaje = "1/" + usuario.Text + "/" + contraseña.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            
        }

        private void registration_Click(object sender, EventArgs e)
        {

            //Creamos el socket 
            if (usuario.TextLength == 0)
                MessageBox.Show("Introduce el usuario que quieres crear");
            else if(usuario.TextLength >256)
                MessageBox.Show("Escribe un nombre más corto");
            else if (contraseña.TextLength == 0)
                MessageBox.Show("Introduce tu contraseña");
            else if (edad.TextLength==0)
                MessageBox.Show("Introduce tu edad");
            else
            {
                //Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "2/" + usuario.Text + "/" + contraseña.Text + "/" + edad.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }

        }

        private void Geometry_wars_Load(object sender, EventArgs e)
        {
            ListaConectadosView.Visible = false;
            try {
                IPAddress direc = IPAddress.Parse(ServidorIp);
                IPEndPoint ipep = new IPEndPoint(direc, ServidorPuerto);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);
                //Creamos el thread
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (consulta1.Checked)
            {
                //Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "3/1";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else if (consulta2.Checked)
            {
                //Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "3/2";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);


            }
            else if (consulta3.Checked)
            {
                //Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "3/3";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                MessageBox.Show("Por favor elige una consulta");
            }
        }

        private void desconexion_Click(object sender, EventArgs e)
        {
            string mensaje = "0/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            MessageBox.Show("Te has desconectado");
            conexion.Visible = true;
            desconexion.Visible = false;
            label1.Visible = false;
            consulta1.Visible = false;
            consulta2.Visible = false;
            consulta3.Visible = false;
            desconexion.Visible = false;
            ejecutarBtn.Visible = false;
            ListaConectadosView.Visible = false;
            atender.Abort();
        }

        private void conexion_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress direc = IPAddress.Parse(ServidorIp);
                IPEndPoint ipep = new IPEndPoint(direc, ServidorPuerto);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);
                MessageBox.Show("Te has vuelto a conectar");
                //Creamos el thread
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
            conexion.Visible = false;
            usuario.Visible = true;
            contraseña.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            edad.Visible = true;
            label4.Visible = true;
            login.Visible = true;
            registration.Visible = true;

        }
    }
}
