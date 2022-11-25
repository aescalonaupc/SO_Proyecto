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
using System.Media;
using System.CodeDom;

namespace Cliente
{
    public partial class Geometry_wars : Form
    {
        Socket server;
        Thread atender;

        int estadoVerPassword;
        string miUsuario;
        int partidaId;

        List<string> usuariosInvitar = new List<string>();

        public const string ServidorIp = "147.83.117.22";
        public const int ServidorPuerto = 50059;

        public Geometry_wars()
        {
            InitializeComponent();
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
                    switch (peticion)
                    {
                        case 1:
                            //MessageBox.Show(respuesta);

                            if (trozos[1] == "SI")
                            {
                                MessageBox.Show("Bienvenido");
                                miUsuario = usuario.Text;

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
                                    verContra.Visible = false;

                                    label1.Visible = true;
                                    consulta1.Visible = true;
                                    consulta2.Visible = true;
                                    consulta3.Visible = true;
                                    desconexion.Visible = true;
                                    ejecutarBtn.Visible = true;
                                    btnInvitar.Visible = true;
                                    
                                }));
                            }
                            else
                                MessageBox.Show("No se ha encontrado el usuario,revisa tu contraseña o registrate");
                            break;
                        case 2:
                            if (trozos[1] == "SI")
                                MessageBox.Show("Te has inscrito correctamente");
                            else
                                MessageBox.Show("No te has podido inscribir vuelve a probar");
                            break;
                        case 3:
                            MessageBox.Show("Resultado consulta: " + trozos[2]);
                            break;
                        case 4:
                            int numConectados = Convert.ToInt32(trozos[1]);

                            ListaConectadosView.Invoke(new Action(() =>
                            {
                                if (!ListaConectadosView.Visible)
                                {
                                    ListaConectadosView.Visible = true;
                                    ListaConectadosView.ColumnCount = 1;
                                    ListaConectadosView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                                    ListaConectadosView.GridColor = Color.Black;
                                    ListaConectadosView.RowHeadersVisible = false;
                                    //ListaConectadosView.Columns[0].Name = "Número";
                                    //ListaConectadosView.Columns[0].ReadOnly = true;
                                    ListaConectadosView.Columns[0].Name = "Jugador";
                                }

                                ListaConectadosView.Rows.Clear();

                                string[] conectados = trozos[2].Split(',');
                                //MessageBox.Show("Han llegado " + numConectados + " conectados");
                                //int i = 1;

                                ListaConectadosView.RowCount = numConectados;

                                for (int i = 0; i < numConectados; i++)
                                {
                                    ListaConectadosView[0, i].Value = conectados[i];
                                }

                                //while (i <= numConectados)
                                //{
                                //    //MessageBox.Show(conectados[i - 1]);
                                //    //ListaConectadosView[0, i - 1].Value = i;
                                //    ListaConectadosView[0, i - 1].Value = conectados[i - 1];
                                //    i++;
                                //}
                                ListaConectadosView.ClearSelection();
                            }));

                            break;
                        case 5:
                            partidaId = Convert.ToInt32(trozos[1]);
                            DialogResult r= MessageBox.Show("Quieres jugar con: " + trozos[2] + "?", "Aceptar Partida", MessageBoxButtons.YesNo);
                            if (r==DialogResult.Yes)
                            {
                                string mensaje2 = "5/" + partidaId + "/SI";
                                byte[] msg2 = Encoding.ASCII.GetBytes(mensaje2);
                                server.Send(msg2);
                                //MessageBox.Show("Mensaje enviado");

                                Invoke(new Action(() =>
                                {
                                    progressBar1.Visible = true;
                                    cargandoLabel.Visible = true;
                                }));
                            }
                            else if (r == DialogResult.No)
                            {
                                string mensaje2 = "5/" + partidaId + "/NO";
                                byte[] msg2 = Encoding.ASCII.GetBytes(mensaje2);
                                server.Send(msg2);
                                //MessageBox.Show("Mensaje enviado");

                                partidaId = -1;
                            }
                            break;
                        case 6:
                            int estado  = Convert.ToInt32(trozos[2]);

                            if (estado == 0)
                            {
                                partidaId = -1;
                                MessageBox.Show("Un jugador ha rechazado la invitación");
                            } else if (estado == 1)
                            {
                                MessageBox.Show("La partida empieza, todos han aceptado");
                            }

                            Invoke(new Action(() =>
                            {
                                progressBar1.Visible = false;
                                cargandoLabel.Visible = false;
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
                estadoVerPassword = 0;
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
                // Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "3/1";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else if (consulta2.Checked)
            {
                //Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "3/2";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);


            }
            else if (consulta3.Checked)
            {
                //Intentamos conectar el socket
                // Quiere saber la longitud
                string mensaje = "3/3";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
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
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
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
            verContra.Visible= true;

        }

        private void verContra_Click(object sender, EventArgs e)
        {
            if (estadoVerPassword==0)
            {
                contraseña.PasswordChar = default;
                verContra.Text = "Dejar de ver";
                estadoVerPassword = 1;
            }
            else
            {
                contraseña.PasswordChar = '*';
                verContra.Text = "Ver contraseña";
                estadoVerPassword = 0;
            }
        }

        private void btnInvitar_Click(object sender, EventArgs e)
        {
            usuariosInvitar.Clear();

            for (int i = 0; i < ListaConectadosView.RowCount; i++)
            {
                if (ListaConectadosView[0, i].Selected)
                {
                    string usuario = ((string)ListaConectadosView[0, i].Value).Trim();

                    if (usuario.Length <= 0 || usuario == miUsuario)
                    {
                        continue;
                    }

                    usuariosInvitar.Add(usuario);
                }
            }

            if (usuariosInvitar.Count == 0)
            {
                MessageBox.Show("Elige algun usuario valido (no puedes ser tu mismo)");
                return;
            }

            if (usuariosInvitar.Count <= 5)
            {
                string mensaje = "4/" + string.Join("/", usuariosInvitar);
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                progressBar1.Visible = true;
                cargandoLabel.Visible = true;
            }
            else
                MessageBox.Show("Escoge a menos personas, max 5");
              
        }

        //        try {
        //                if (e.ColumnIndex == 0)
        //                    MessageBox.Show("Por favor pulsa encima del nombre");
        //                else
        //                {
        //                    string amigo = Convert.ToString(ListaConectadosView[e.ColumnIndex, e.RowIndex].Value);
        //                    if (amigo == usuario.Text)
        //                        MessageBox.Show("Escoge a un amigo, no a ti mismo");
        //                    else
        //                    {
        //                        string mensaje = "4/" + amigo;
        //        // Enviamos al servidor el nombre tecleado
        //        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        //        server.Send(msg);
        //                    }
        //}
        //            }
        //            catch (System.ArgumentOutOfRangeException)
        //{
        //    MessageBox.Show("No pulse ahi");
        //    return;
        //}
    }
}
