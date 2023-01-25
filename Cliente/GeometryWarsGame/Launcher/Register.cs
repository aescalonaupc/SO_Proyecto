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

        public Register()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = userTb.Text.Trim();
            string pass = passTb.Text.Trim();
            string pass2 = pass2Tb.Text.Trim();

            if (user.Length <= 0 || pass.Length <= 0 || pass2.Length <= 0)
            {
                MessageBox.Show("Debes introducir todos los datos.");
                return;
            }

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

            Shared.NetworkHandler.Send("2/" + user + "/" + pass);
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}
