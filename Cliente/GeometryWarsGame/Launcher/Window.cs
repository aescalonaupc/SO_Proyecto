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
        /// Formulario de registro
        /// </summary>
        Register? registerForm = null;

        public Window()
        {
            InitializeComponent();
        }

        private void OnMessage(string message)
        {
            string username = "";
            int userId = -1;

            bool register = false;
            bool openLobby = false;

            string[] parts = message.Split('/');
            int op = Convert.ToInt32(parts[0]);

            switch (op)
            {
                // Login result
                // Format: 1/OK/<username>/<userId>, 1/NOK/<error code>
                // e.g. 1/OK/test1234/1, 1/NOK/1
                case 1:
                    if (parts[1] == "OK")
                    {
                        username = parts[2];
                        userId = Convert.ToInt32(message[3]);
                        openLobby = true;
                    }
                    else if (parts[1] == "NOK")
                    {
                        int errorCode = Convert.ToInt32(parts[2]);
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
                    if (parts[1] == "OK")
                    {
                        username = parts[2];
                        userId = Convert.ToInt32(parts[3]);

                        MessageBox.Show("Te has registrado correctamente.");
                        register = true;
                        openLobby = true;
                    }
                    else if (parts[1] == "NOK")
                    {
                        int errorCode = Convert.ToInt32(parts[2]);
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

            if (openLobby)
            {
                BeginInvoke(() =>
                {
                    if (register)
                    {
                        registerForm?.Dispose();
                        Show();
                    }

                    Login(userId, username);
                });
            }
        }

        private void Login(int userId, string username)
        {
            Shared.NetworkHandler.OnNetworkMessage -= OnMessage;

            Lobby form = new Lobby();
            form.SetUser(username, userId);

            Hide();
            form.ShowDialog();
            Close();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string user = userTb.Text.Trim();
            string password = passTb.Text.Trim();

            if (user.Length <= 0 || password.Length <= 0)
            {
                MessageBox.Show("Debes rellenar todos los campos.");
                return;
            }

            if (user.Length > 255 || password.Length > 255)
            {
                MessageBox.Show("Máxima longitud usuario/contraseña de 255 caracteres.");
                return;
            }

            Shared.NetworkHandler.Send("1/" + user + "/" + password);
        }

        private void Window_Load(object sender, EventArgs e)
        {
            Shared.NetworkHandler.OnNetworkMessage += OnMessage;
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            // Abrimos un formulario para el registro, pero recibiremos los mensajes del servicio en este formulario
            registerForm = new Register();
            Hide();
            registerForm.ShowDialog();

            try
            {
                Show();
            } catch (Exception) { }
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shared.NetworkHandler.OnNetworkMessage -= OnMessage;
        }
    }
}
