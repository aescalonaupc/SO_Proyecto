using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace GeometryWarsGame.Launcher
{
    public partial class Register : Form
    {
        private Socket serverSocket;

        public void SetSocket(Socket socket)
        {
            serverSocket = socket;
        }

        public Register()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serverSocket == null)
            {
                return;
            }

            string user = userTb.Text.Trim();
            string pass = passTb.Text.Trim();
            string pass2 = pass2Tb.Text.Trim();

            if (user.Length > 255 || pass.Length > 255)
            {
                MessageBox.Show("Máxima longitud usuario/contraseña de 255 caracteres.");
                return;
            }

            if (pass != pass2)
            {
                MessageBox.Show("Las contraseñas no coinciden.");
                return;
            }

            serverSocket.Send(Encoding.ASCII.GetBytes("2/" + user + "/" + pass));
        }
    }
}
