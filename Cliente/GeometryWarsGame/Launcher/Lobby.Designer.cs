namespace GeometryWarsGame.Launcher
{
    partial class Lobby
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Lobby));
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.sandboxBtn = new System.Windows.Forms.Button();
            this.conectadosGrid = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.salirSalaBtn = new System.Windows.Forms.Button();
            this.labelSalaGrid = new System.Windows.Forms.Label();
            this.allBtn = new System.Windows.Forms.Button();
            this.salaGrid = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.helpLabel = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.musicControlBtn = new System.Windows.Forms.Button();
            this.nowPlayingLabel = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chatSendBtn = new System.Windows.Forms.Button();
            this.chatTb = new System.Windows.Forms.TextBox();
            this.chatBox = new System.Windows.Forms.ListBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.minutosLabel = new System.Windows.Forms.Label();
            this.ganadasLabel = new System.Windows.Forms.Label();
            this.jugadasLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.conectadosGrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.salaGrid)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.welcomeLabel.Location = new System.Drawing.Point(34, 18);
            this.welcomeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(221, 30);
            this.welcomeLabel.TabIndex = 0;
            this.welcomeLabel.Text = "Bienvenido, {usuario}.";
            // 
            // sandboxBtn
            // 
            this.sandboxBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.sandboxBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sandboxBtn.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.sandboxBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sandboxBtn.ForeColor = System.Drawing.Color.Transparent;
            this.sandboxBtn.Location = new System.Drawing.Point(215, 486);
            this.sandboxBtn.Margin = new System.Windows.Forms.Padding(2);
            this.sandboxBtn.Name = "sandboxBtn";
            this.sandboxBtn.Size = new System.Drawing.Size(201, 39);
            this.sandboxBtn.TabIndex = 4;
            this.sandboxBtn.Text = "Sandbox";
            this.sandboxBtn.UseVisualStyleBackColor = false;
            this.sandboxBtn.Click += new System.EventHandler(this.sandboxBtn_Click);
            // 
            // conectadosGrid
            // 
            this.conectadosGrid.AllowUserToAddRows = false;
            this.conectadosGrid.AllowUserToDeleteRows = false;
            this.conectadosGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conectadosGrid.Enabled = false;
            this.conectadosGrid.Location = new System.Drawing.Point(7, 28);
            this.conectadosGrid.MultiSelect = false;
            this.conectadosGrid.Name = "conectadosGrid";
            this.conectadosGrid.ReadOnly = true;
            this.conectadosGrid.RowTemplate.Height = 25;
            this.conectadosGrid.Size = new System.Drawing.Size(409, 199);
            this.conectadosGrid.TabIndex = 5;
            this.conectadosGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.conectadosGrid_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.salirSalaBtn);
            this.groupBox1.Controls.Add(this.labelSalaGrid);
            this.groupBox1.Controls.Add(this.allBtn);
            this.groupBox1.Controls.Add(this.salaGrid);
            this.groupBox1.Controls.Add(this.conectadosGrid);
            this.groupBox1.Controls.Add(this.sandboxBtn);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(34, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(421, 537);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Jugadores conectados";
            // 
            // salirSalaBtn
            // 
            this.salirSalaBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.salirSalaBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.salirSalaBtn.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.salirSalaBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.salirSalaBtn.ForeColor = System.Drawing.Color.Transparent;
            this.salirSalaBtn.Location = new System.Drawing.Point(215, 249);
            this.salirSalaBtn.Margin = new System.Windows.Forms.Padding(2);
            this.salirSalaBtn.Name = "salirSalaBtn";
            this.salirSalaBtn.Size = new System.Drawing.Size(201, 31);
            this.salirSalaBtn.TabIndex = 9;
            this.salirSalaBtn.Text = "Abandonar sala";
            this.salirSalaBtn.UseVisualStyleBackColor = false;
            this.salirSalaBtn.Visible = false;
            this.salirSalaBtn.Click += new System.EventHandler(this.salirSalaBtn_Click);
            // 
            // labelSalaGrid
            // 
            this.labelSalaGrid.AutoSize = true;
            this.labelSalaGrid.Location = new System.Drawing.Point(7, 259);
            this.labelSalaGrid.Name = "labelSalaGrid";
            this.labelSalaGrid.Size = new System.Drawing.Size(143, 21);
            this.labelSalaGrid.TabIndex = 7;
            this.labelSalaGrid.Text = "No tienes amigos :(";
            // 
            // allBtn
            // 
            this.allBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.allBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.allBtn.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.allBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.allBtn.ForeColor = System.Drawing.Color.Transparent;
            this.allBtn.Location = new System.Drawing.Point(7, 486);
            this.allBtn.Margin = new System.Windows.Forms.Padding(2);
            this.allBtn.Name = "allBtn";
            this.allBtn.Size = new System.Drawing.Size(204, 39);
            this.allBtn.TabIndex = 7;
            this.allBtn.Text = "All vs All";
            this.allBtn.UseVisualStyleBackColor = false;
            this.allBtn.Click += new System.EventHandler(this.allBtn_Click);
            // 
            // salaGrid
            // 
            this.salaGrid.AllowUserToAddRows = false;
            this.salaGrid.AllowUserToDeleteRows = false;
            this.salaGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.salaGrid.Enabled = false;
            this.salaGrid.Location = new System.Drawing.Point(7, 283);
            this.salaGrid.Name = "salaGrid";
            this.salaGrid.ReadOnly = true;
            this.salaGrid.RowTemplate.Height = 25;
            this.salaGrid.Size = new System.Drawing.Size(409, 196);
            this.salaGrid.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(473, 113);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(434, 253);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modos de juego";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(6, 28);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(422, 219);
            this.textBox1.TabIndex = 15;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.helpLabel);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(473, 372);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(434, 278);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Consejos y Ayuda";
            // 
            // helpLabel
            // 
            this.helpLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.helpLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.helpLabel.Enabled = false;
            this.helpLabel.ForeColor = System.Drawing.Color.White;
            this.helpLabel.Location = new System.Drawing.Point(6, 28);
            this.helpLabel.Multiline = true;
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.helpLabel.Size = new System.Drawing.Size(422, 244);
            this.helpLabel.TabIndex = 15;
            this.helpLabel.Text = "Ahora mismo no estás en ninguna sala, espera que alguno de los jugadores te invit" +
    "e a la suya o invita tú a alguien.";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.musicControlBtn);
            this.groupBox4.Controls.Add(this.nowPlayingLabel);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(913, 531);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(319, 119);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "🔊 Now Playing 🔊";
            // 
            // musicControlBtn
            // 
            this.musicControlBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.musicControlBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.musicControlBtn.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.musicControlBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.musicControlBtn.ForeColor = System.Drawing.Color.Transparent;
            this.musicControlBtn.Location = new System.Drawing.Point(11, 70);
            this.musicControlBtn.Margin = new System.Windows.Forms.Padding(2);
            this.musicControlBtn.Name = "musicControlBtn";
            this.musicControlBtn.Size = new System.Drawing.Size(297, 38);
            this.musicControlBtn.TabIndex = 14;
            this.musicControlBtn.Text = "Stop";
            this.musicControlBtn.UseVisualStyleBackColor = false;
            this.musicControlBtn.Click += new System.EventHandler(this.musicControlBtn_Click);
            // 
            // nowPlayingLabel
            // 
            this.nowPlayingLabel.Location = new System.Drawing.Point(6, 28);
            this.nowPlayingLabel.Name = "nowPlayingLabel";
            this.nowPlayingLabel.Size = new System.Drawing.Size(297, 29);
            this.nowPlayingLabel.TabIndex = 0;
            this.nowPlayingLabel.Text = "Title: {songName}";
            this.nowPlayingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chatSendBtn);
            this.groupBox5.Controls.Add(this.chatTb);
            this.groupBox5.Controls.Add(this.chatBox);
            this.groupBox5.ForeColor = System.Drawing.Color.White;
            this.groupBox5.Location = new System.Drawing.Point(913, 113);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(319, 412);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Chat";
            // 
            // chatSendBtn
            // 
            this.chatSendBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chatSendBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chatSendBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chatSendBtn.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.chatSendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chatSendBtn.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chatSendBtn.ForeColor = System.Drawing.Color.Transparent;
            this.chatSendBtn.Location = new System.Drawing.Point(247, 377);
            this.chatSendBtn.Margin = new System.Windows.Forms.Padding(2);
            this.chatSendBtn.Name = "chatSendBtn";
            this.chatSendBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chatSendBtn.Size = new System.Drawing.Size(61, 29);
            this.chatSendBtn.TabIndex = 15;
            this.chatSendBtn.Text = "📤";
            this.chatSendBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.chatSendBtn.UseVisualStyleBackColor = false;
            this.chatSendBtn.Click += new System.EventHandler(this.chatSendBtn_Click);
            // 
            // chatTb
            // 
            this.chatTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chatTb.ForeColor = System.Drawing.Color.Silver;
            this.chatTb.Location = new System.Drawing.Point(11, 377);
            this.chatTb.MaxLength = 256;
            this.chatTb.Name = "chatTb";
            this.chatTb.PlaceholderText = " Hello...";
            this.chatTb.Size = new System.Drawing.Size(231, 29);
            this.chatTb.TabIndex = 15;
            this.chatTb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chatTb_KeyPress);
            // 
            // chatBox
            // 
            this.chatBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chatBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatBox.ForeColor = System.Drawing.Color.Silver;
            this.chatBox.FormattingEnabled = true;
            this.chatBox.ItemHeight = 21;
            this.chatBox.Location = new System.Drawing.Point(11, 28);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(297, 336);
            this.chatBox.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.minutosLabel);
            this.groupBox6.Controls.Add(this.ganadasLabel);
            this.groupBox6.Controls.Add(this.jugadasLabel);
            this.groupBox6.ForeColor = System.Drawing.Color.White;
            this.groupBox6.Location = new System.Drawing.Point(473, 36);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(759, 65);
            this.groupBox6.TabIndex = 15;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Mis Estadísticas";
            // 
            // minutosLabel
            // 
            this.minutosLabel.AutoSize = true;
            this.minutosLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.minutosLabel.Location = new System.Drawing.Point(516, 25);
            this.minutosLabel.Name = "minutosLabel";
            this.minutosLabel.Size = new System.Drawing.Size(233, 25);
            this.minutosLabel.TabIndex = 17;
            this.minutosLabel.Text = "Has jugado XXXX minutos.";
            // 
            // ganadasLabel
            // 
            this.ganadasLabel.AutoSize = true;
            this.ganadasLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ganadasLabel.Location = new System.Drawing.Point(270, 25);
            this.ganadasLabel.Name = "ganadasLabel";
            this.ganadasLabel.Size = new System.Drawing.Size(216, 25);
            this.ganadasLabel.TabIndex = 1;
            this.ganadasLabel.Text = "Has ganado XX partidas.";
            // 
            // jugadasLabel
            // 
            this.jugadasLabel.AutoSize = true;
            this.jugadasLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.jugadasLabel.Location = new System.Drawing.Point(17, 25);
            this.jugadasLabel.Name = "jugadasLabel";
            this.jugadasLabel.Size = new System.Drawing.Size(211, 25);
            this.jugadasLabel.TabIndex = 0;
            this.jugadasLabel.Text = "Has jugado XX partidas.";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.Transparent;
            this.button2.Location = new System.Drawing.Point(41, 61);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(409, 40);
            this.button2.TabIndex = 17;
            this.button2.Text = "Eliminar mi cuenta";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Lobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(1269, 681);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.welcomeLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Lobby";
            this.Text = "Geometry Wars Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Lobby_FormClosing);
            this.Load += new System.EventHandler(this.Lobby_Load);
            ((System.ComponentModel.ISupportInitialize)(this.conectadosGrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.salaGrid)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label welcomeLabel;
        private Button sandboxBtn;
        private DataGridView conectadosGrid;
        private GroupBox groupBox1;
        private Label labelSalaGrid;
        private Button allBtn;
        private DataGridView salaGrid;
        private Button salirSalaBtn;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Label nowPlayingLabel;
        private Button musicControlBtn;
        private GroupBox groupBox5;
        private Button chatSendBtn;
        private TextBox chatTb;
        private ListBox chatBox;
        private TextBox textBox1;
        private TextBox helpLabel;
        private GroupBox groupBox6;
        private Label minutosLabel;
        private Label ganadasLabel;
        private Label jugadasLabel;
        private Button button2;
    }
}