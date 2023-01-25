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
using System.Threading;
using GeometryWarsGame.Game;

namespace GeometryWarsGame.Launcher
{
    public partial class Lobby : Form
    {
        string myUsername = "";

        int myId = -1;

        int partidaId = -1;

        bool soyLider = true;

        List<string> miSala = new List<string>();

        Color EnPartida = Color.FromArgb(255, 242, 75, 95);

        Color EnSala = Color.FromArgb(255, 71, 148, 141);

        Color Disponible = Color.FromArgb(255, 99, 255, 131);

        Color Pendiente = Color.FromArgb(255, 255, 250, 110);

        public void SetUser(string username, int id)
        {
            myUsername = username;
            myId = id;
        }

        public Lobby()
        {
            InitializeComponent();

            SoundManager.PlayLobby();

            if (SoundManager.IsPlaying())
            {
                nowPlayingLabel.Text = "Title: " + SoundManager.CurrentTrack!.Name;
            }
        }

        private void OnNetworkMessage(string _message)
        {
            string[] message = _message.Split('/');
            int op = Convert.ToInt32(message[0]);

            switch (op)
            {
                // Notificacion de jugadores conectados
                // Formato: 4/<num conectados>/<nombre 1>,<nombre 2>,...
                case 4:
                    int numConectados = Convert.ToInt32(message[1]);

                    Invoke(() =>
                    {
                        if (!conectadosGrid.Enabled)
                        {
                            conectadosGrid.Enabled = true;

                            conectadosGrid.ColumnCount = 2;
                            conectadosGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                            conectadosGrid.RowHeadersVisible = false;
                            conectadosGrid.ForeColor = Color.Black;
                            conectadosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            conectadosGrid.Columns[0].Name = "Jugador";
                            conectadosGrid.Columns[1].Name = "Estado";

                            DataGridViewButtonColumn inviteButtonColumn = new DataGridViewButtonColumn();
                            inviteButtonColumn.Text = "Invitar";
                            inviteButtonColumn.UseColumnTextForButtonValue = true;
                            conectadosGrid.Columns.Add(inviteButtonColumn);
                        }

                        if (!soyLider)
                        {
                            conectadosGrid.Columns[2].Visible = false;
                        }

                        conectadosGrid.Rows.Clear();
                        conectadosGrid.RowCount = numConectados;

                        string[] conectados = message[2].Split(',');
                        for (int i = 0, rowIndex = 0; i < numConectados * 2; i += 2)
                        {
                            conectadosGrid[0, rowIndex].Value = conectados[i];

                            if (conectados[i] == myUsername)
                            {
                                conectadosGrid[2, rowIndex] = new DataGridViewTextBoxCell();
                                rowIndex++;
                                continue;
                            }

                            if (conectados[i + 1] == "1")
                            {
                                conectadosGrid[1, rowIndex].Value = "Pendiente";
                                conectadosGrid[2, rowIndex] = new DataGridViewTextBoxCell();
                                conectadosGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Pendiente;
                            }
                            else if (conectados[i + 1] == "3")
                            {
                                conectadosGrid[1, rowIndex].Value = "Jugando";
                                conectadosGrid[2, rowIndex] = new DataGridViewTextBoxCell();
                                conectadosGrid.Rows[rowIndex].DefaultCellStyle.BackColor = EnPartida;
                            }
                            else if (conectados[i + 1] == "2")
                            {
                                conectadosGrid[1, rowIndex].Value = "En sala";
                                conectadosGrid[2, rowIndex] = new DataGridViewTextBoxCell();
                                conectadosGrid.Rows[rowIndex].DefaultCellStyle.BackColor = EnSala;
                            }
                            else
                            {
                                conectadosGrid[1, rowIndex].Value = "Disponible";
                                conectadosGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Disponible;
                            }

                            rowIndex++;
                        }

                        conectadosGrid.ClearSelection();
                    });
                    break;

                // Notificacion de los jugadores en tu sala
                // Formato: 44/<num jugadores>/<nombre 1>,<nombre 2>,...
                case 44:
                    int numJugadores = Convert.ToInt32(message[1]);

                    Invoke((Delegate)(() =>
                    {
                        if (!salaGrid.Enabled)
                        {
                            salaGrid.Enabled = true;

                            salaGrid.ColumnCount = 1;
                            salaGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                            salaGrid.RowHeadersVisible = false;
                            salaGrid.ForeColor = Color.Black;
                            salaGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            salaGrid.Columns[0].Name = "Jugador";
                        }

                        if (!salirSalaBtn.Visible)
                        {
                            salirSalaBtn.Visible = true;
                        }

                        int before = salaGrid.Rows.Count;
                        salaGrid.Rows.Clear();
                        salaGrid.RowCount = numJugadores > 0 ? numJugadores - 1 : 0; // - 1 ya que nosotros no salimos en la sala

                        if (numJugadores <= 0)
                        {
                            labelSalaGrid.Text = "No tienes amigos :(";
                            helpLabel.Text = "Ahora mismo no estás en ninguna sala, espera que alguno de los jugadores te invite a la suya o invita tú a alguien.";

                            salaGrid.Enabled = false;
                            salaGrid.Columns.Clear();
                            salirSalaBtn.Visible = false;

                            conectadosGrid.Columns[2].Visible = true;

                            soyLider = true;
                            allBtn.Enabled = false;

                            if (before > 0)
                            {
                                chatBox.Items.Add("** ya no estás en la sala **");
                                chatBox.Items.Add("[ chat global activado ]");
                            }
                        }
                        else
                        {
                            labelSalaGrid.Text = "Jugadores en tu sala";

                            if (before <= 0 && soyLider)
                            {
                                chatBox.Items.Add("** has creado la sala **");
                                chatBox.Items.Add("[ chat global desactivado ]");
                            }

                            if (soyLider)
                            {
                                allBtn.Enabled = true;
                            }
                        }

                        miSala.Clear();

                        if (numJugadores > 0)
                        {
                            string[] jugadores = message[2].Split(',');

                            for (int i = 0, row = 0; i < numJugadores; i++)
                            {
                                if (jugadores[i] == myUsername || jugadores[i].Length <= 0)
                                {
                                    continue;
                                }

                                miSala.Add(jugadores[i]);

                                salaGrid[0, row].Value = jugadores[i];
                                row++;
                            }

                            salaGrid.ClearSelection();

                            if (soyLider)
                            {
                                helpLabel.Text = "Eres el líder de la sala. Puedes invitar a más jugadores hasta un máximo de 5. Cuando estés listo, puedes empezar la partida haciendo click sobre cualquier de los tres modos de juego.\n\nSi abandonas la sala, todos los jugadores saldrán de la sala y la partida se eliminará.";
                            }
                        }
                    }));
                    break;

                // Peticion de unirte a una sala
                // Formato: 5/<slot partida>/<usuario invita>
                // P.ej: 5/0/Adrian
                case 5:
                    int slot = Convert.ToInt32(message[1]);
                    string usuarioInvita = message[2];

                    partidaId = slot;

                    DialogResult r = MessageBox.Show("¿Quieres unirte a la sala de " + usuarioInvita + "?", "Invitación", MessageBoxButtons.YesNo);

                    if (r == DialogResult.Yes)
                    {
                        Shared.NetworkHandler.Send("5/" + partidaId + "/1");
                        soyLider = false;

                        Invoke((Delegate)(() =>
                        {
                            allBtn.Enabled = false;
                            conectadosGrid.Columns[2].Visible = false;
                            helpLabel.Text = "Te has unido a la sala de " + usuarioInvita + ". Cuando esté listo, " + usuarioInvita + " empezará la partida eligiendo uno de los 3 modos de juego. Puedes abandonar la sala haciendo click en 'Abandonar Sala'. Si no, simplemente espera a que empiece la partida.";

                            chatBox.Items.Add("** te has unido a la sala de " + usuarioInvita + " **");
                            chatBox.Items.Add("[ chat global desactivado ]");
                        }));
                    }
                    else if (r == DialogResult.No)
                    {
                        Shared.NetworkHandler.Send("5/" + partidaId + "/0");
                        partidaId = -1;
                    }
                    break;

                // Abrir el juego (siendo un invitado)
                // Simplemente debe abrir el juego y esperar mas informacion
                // Formato: 7/<tipo juego>
                case 7:
                    int gameType = Convert.ToInt32(message[1]);
                    BeginInvoke(() =>
                    {
                        StartGame(gameType);
                    });
                    break;

                // Recibimos las estadisticas del jugador
                // Formato: 77/<partidas jugadas>/<partidas ganadas>/<minutos jugados>
                case 77:
                    int jugadas = Convert.ToInt32(message[1]);
                    int ganadas = Convert.ToInt32(message[2]);
                    int minutos = Convert.ToInt32(message[3]);

                    Invoke(() =>
                    {
                        jugadasLabel.Text = jugadasLabel.Text.Replace("XX", jugadas.ToString());
                        ganadasLabel.Text = ganadasLabel.Text.Replace("XX", ganadas.ToString());
                        minutosLabel.Text = minutosLabel.Text.Replace("XXXX", minutos.ToString());
                    });
                    break;

                // Mensaje de chat
                // Formato: 8/<autor>/<mensaje>
                case 8:
                    string author = message[1];
                    string chatMessage = message[2];

                    // quizas tenia "/"
                    if (message.Length > 3)
                    {
                        for (int i = 3; i < message.Length; i++)
                        {
                            chatMessage += "/" + message[i];
                        }
                    }

                    if (author == myUsername)
                    {
                        author = "Tú";
                    }

                    chatBox.Invoke(() =>
                    {
                        chatBox.Items.Add(author + ": " + chatMessage);

                        // trick to scroll
                        chatBox.SelectedIndex = chatBox.Items.Count - 1;
                        chatBox.SelectedIndex = -1;
                    });
                    break;
            }
        }

        private void Lobby_Load(object sender, EventArgs e)
        {
            Shared.NetworkHandler.OnNetworkMessage += OnNetworkMessage;

            welcomeLabel.Text = welcomeLabel.Text.Replace("{usuario}", myUsername);
            Shared.NetworkHandler.Send("7/");
        }

        private void sandboxBtn_Click(object sender, EventArgs e)
        {
            if (soyLider)
            {
                StartGame(0);
            }
        }

        private void StartGame(int gameType)
        {
            Shared.NetworkHandler.OnNetworkMessage -= OnNetworkMessage;

            List<string> datosJugadores = new List<string>();

            if (soyLider)
            {
                datosJugadores.Add(myUsername);
                datosJugadores.AddRange(miSala);
            }

            Network.Initialize();

            Program.GameWindow.SetLocalPlayer(myUsername, myId, soyLider);
            Program.GameWindow.SetInitialData(datosJugadores, gameType);

            Hide();
            Program.GameWindow.ShowDialog();
        }

        private void conectadosGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!soyLider)
            {
                return;
            }

            if (conectadosGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                string usuario = ((string)conectadosGrid[0, e.RowIndex].Value).Trim();

                if (usuario.Length <= 0 || usuario == myUsername)
                {
                    return;
                }

                Shared.NetworkHandler.Send("4/" + usuario);
            }
        }

        private void allBtn_Click(object sender, EventArgs e)
        {
            if (soyLider)
            {
                StartGame(1);
            }
        }

        private void salirSalaBtn_Click(object sender, EventArgs e)
        {
            Shared.NetworkHandler.Send("8/");

            Invoke((Delegate)(() =>
            {
                miSala.Clear();
                salaGrid.Rows.Clear();

                salirSalaBtn.Visible = false;
                helpLabel.Text = "Ahora mismo no estás en ninguna sala, espera que alguno de los jugadores te invite a la suya o invita tú a alguien.";

                chatBox.Items.Add("** has abandonado la sala **");
            }));
        }

        private void musicControlBtn_Click(object sender, EventArgs e)
        {
            if (SoundManager.IsPlaying())
            {
                SoundManager.StopQueue();

                nowPlayingLabel.Text = "Nothing";
                musicControlBtn.Text = "Play";
            } else
            {
                SoundManager.PlayLobby();

                nowPlayingLabel.Text = "Title: " + SoundManager.CurrentTrack?.Name;
                musicControlBtn.Text = "Stop";
            }
        }

        private void Lobby_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shared.NetworkHandler.OnNetworkMessage -= OnNetworkMessage;
        }

        private void SendChatMessage(string text)
        {
            if (text.Length > 256)
            {
                text = text.Substring(0, 256);
            }

            Shared.NetworkHandler.Send("6/" + text);
            chatTb.Clear();
        }

        private void chatSendBtn_Click(object sender, EventArgs e)
        {
            string t = chatTb.Text.Trim();

            if (t.Length > 0)
            {
                SendChatMessage(t);
            }
        }

        private void chatTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
            {
                return;
            }

            e.Handled = true;

            string t = chatTb.Text.Trim();

            if (t.Length > 0)
            {
                SendChatMessage(t);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Estás seguro de eliminar tu cuenta? Perderás todo tu progreso y es una acción irreversible. Además, tendrás que abrir el juego de nuevo.", "Eliminar cuenta", MessageBoxButtons.YesNo);

            if (r == DialogResult.Yes)
            {
                Shared.NetworkHandler.Send("9/");

                myUsername = "";
                myId = -1;
                soyLider = true;
                miSala.Clear();
                chatBox.Items.Clear();
                chatTb.Clear();
                conectadosGrid.Rows.Clear();
                salaGrid.Rows.Clear();

                Close();
            }
        }
    }
}
