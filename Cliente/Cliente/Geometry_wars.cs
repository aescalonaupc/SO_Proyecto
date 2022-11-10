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

namespace Cliente
{
    public partial class Geometry_wars : Form
    {
        Socket server;
        int conectado = 0;
        public Geometry_wars()
        {
            InitializeComponent();
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

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                if (mensaje == "SI")
                {
                    MessageBox.Show("Bienvenido");
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
                    ListaConectados.Visible = true;
                }
                else
                    MessageBox.Show("No se ha encontrado el usuario,revisa tu contraseña o registrate");

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

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                if (mensaje == "SI")
                {
                    MessageBox.Show("Te has inscrito correctamente");
                }
            }

        }

        private void Geometry_wars_Load(object sender, EventArgs e)
        {
            ListaConectadosView.Visible = false;
            try {
                IPAddress direc = IPAddress.Parse("192.168.56.101");
                IPEndPoint ipep = new IPEndPoint(direc, 9080);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);
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
            if (conectado == 0)
            {
                if (consulta1.Checked)
                {
                    //Intentamos conectar el socket
                    // Quiere saber la longitud
                    string mensaje = "3/1";
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                    MessageBox.Show("Resultado consulta: " + mensaje);
                }
                else if (consulta2.Checked)
                {
                    //Intentamos conectar el socket
                    // Quiere saber la longitud
                    string mensaje = "3/2";
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                    MessageBox.Show("Resultado consulta: " + mensaje);
                }
                else if (consulta3.Checked)
                {
                    //Intentamos conectar el socket
                    // Quiere saber la longitud
                    string mensaje = "3/3";
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                    MessageBox.Show("Resultado consulta: " + mensaje);
                }
                else
                {
                    MessageBox.Show("Por favor elige una consulta");
                }
            }
            else
                MessageBox.Show("No estas conectado, por favor conectate");
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
            ListaConectados.Visible = false;
            ListaConectadosView.Visible = false;
            conectado = -1;


        }

        private void conexion_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress direc = IPAddress.Parse("192.168.56.101");
                IPEndPoint ipep = new IPEndPoint(direc, 9080);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);
                MessageBox.Show("Te has vuelto a conectar");
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

            conectado = 0;

        }

        private void ListaConectados_Click(object sender, EventArgs e)
        {
            // Quiere saber la longitud
            string mensaje = "4/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //Recibimos la respuesta del servidor
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
            string[] trozos = mensaje.Split('/');
            int numConectados = Convert.ToInt32(trozos[0]);


            ListaConectadosView.Visible= true;
            ListaConectadosView.ColumnCount = 2;
            ListaConectadosView.RowCount = numConectados;
            ListaConectadosView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            ListaConectadosView.GridColor = Color.Black;
            ListaConectadosView.RowHeadersVisible = false;

            ListaConectadosView.Columns[0].Name = "Número";
            ListaConectadosView.Columns[1].Name = "Nombre";
            string[] conectados = trozos[1].Split(',');
            int i = 1;
            while (i <= numConectados)
            {
                ListaConectadosView[0, i-1].Value = i;
                ListaConectadosView[1, i-1].Value = conectados[i-1];
                i = i + 1;

            }           

        }
    }
}
