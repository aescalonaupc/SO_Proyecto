
namespace Cliente
{
    partial class Geometry_wars
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.usuario = new System.Windows.Forms.TextBox();
            this.contraseña = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.login = new System.Windows.Forms.Button();
            this.registration = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.consulta1 = new System.Windows.Forms.RadioButton();
            this.consulta2 = new System.Windows.Forms.RadioButton();
            this.consulta3 = new System.Windows.Forms.RadioButton();
            this.edad = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ejecutarBtn = new System.Windows.Forms.Button();
            this.desconexion = new System.Windows.Forms.Button();
            this.conexion = new System.Windows.Forms.Button();
            this.ListaConectadosView = new System.Windows.Forms.DataGridView();
            this.verContra = new System.Windows.Forms.Button();
            this.btnInvitar = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cargandoLabel = new System.Windows.Forms.Label();
            this.chatBox = new System.Windows.Forms.ListBox();
            this.chatTb = new System.Windows.Forms.TextBox();
            this.chatSend = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ListaConectadosView)).BeginInit();
            this.SuspendLayout();
            // 
            // usuario
            // 
            this.usuario.Location = new System.Drawing.Point(191, 77);
            this.usuario.Margin = new System.Windows.Forms.Padding(2);
            this.usuario.Name = "usuario";
            this.usuario.Size = new System.Drawing.Size(133, 20);
            this.usuario.TabIndex = 0;
            // 
            // contraseña
            // 
            this.contraseña.Location = new System.Drawing.Point(191, 133);
            this.contraseña.Margin = new System.Windows.Forms.Padding(2);
            this.contraseña.Name = "contraseña";
            this.contraseña.PasswordChar = '*';
            this.contraseña.Size = new System.Drawing.Size(133, 20);
            this.contraseña.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(204, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Usuario";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, 107);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Contraseña";
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(200, 161);
            this.login.Margin = new System.Windows.Forms.Padding(2);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(100, 27);
            this.login.TabIndex = 5;
            this.login.Text = "Iniciar session";
            this.login.UseVisualStyleBackColor = true;
            this.login.Click += new System.EventHandler(this.login_Click);
            // 
            // registration
            // 
            this.registration.Location = new System.Drawing.Point(202, 201);
            this.registration.Margin = new System.Windows.Forms.Padding(2);
            this.registration.Name = "registration";
            this.registration.Size = new System.Drawing.Size(98, 27);
            this.registration.TabIndex = 6;
            this.registration.Text = "Registrarse";
            this.registration.UseVisualStyleBackColor = true;
            this.registration.Click += new System.EventHandler(this.registration_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Consultas";
            this.label1.Visible = false;
            // 
            // consulta1
            // 
            this.consulta1.AutoSize = true;
            this.consulta1.Location = new System.Drawing.Point(51, 77);
            this.consulta1.Margin = new System.Windows.Forms.Padding(2);
            this.consulta1.Name = "consulta1";
            this.consulta1.Size = new System.Drawing.Size(72, 17);
            this.consulta1.TabIndex = 8;
            this.consulta1.TabStop = true;
            this.consulta1.Text = "Consulta1";
            this.consulta1.UseVisualStyleBackColor = true;
            this.consulta1.Visible = false;
            // 
            // consulta2
            // 
            this.consulta2.AutoSize = true;
            this.consulta2.Location = new System.Drawing.Point(51, 104);
            this.consulta2.Margin = new System.Windows.Forms.Padding(2);
            this.consulta2.Name = "consulta2";
            this.consulta2.Size = new System.Drawing.Size(72, 17);
            this.consulta2.TabIndex = 9;
            this.consulta2.TabStop = true;
            this.consulta2.Text = "Consulta2";
            this.consulta2.UseVisualStyleBackColor = true;
            this.consulta2.Visible = false;
            // 
            // consulta3
            // 
            this.consulta3.AutoSize = true;
            this.consulta3.Location = new System.Drawing.Point(51, 133);
            this.consulta3.Margin = new System.Windows.Forms.Padding(2);
            this.consulta3.Name = "consulta3";
            this.consulta3.Size = new System.Drawing.Size(72, 17);
            this.consulta3.TabIndex = 10;
            this.consulta3.TabStop = true;
            this.consulta3.Text = "Consulta3";
            this.consulta3.UseVisualStyleBackColor = true;
            this.consulta3.Visible = false;
            // 
            // edad
            // 
            this.edad.Location = new System.Drawing.Point(345, 77);
            this.edad.Margin = new System.Windows.Forms.Padding(2);
            this.edad.Name = "edad";
            this.edad.Size = new System.Drawing.Size(133, 20);
            this.edad.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(355, 53);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Edad";
            // 
            // ejecutarBtn
            // 
            this.ejecutarBtn.Location = new System.Drawing.Point(51, 165);
            this.ejecutarBtn.Name = "ejecutarBtn";
            this.ejecutarBtn.Size = new System.Drawing.Size(84, 23);
            this.ejecutarBtn.TabIndex = 13;
            this.ejecutarBtn.Text = "Ejecutar";
            this.ejecutarBtn.UseVisualStyleBackColor = true;
            this.ejecutarBtn.Visible = false;
            this.ejecutarBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // desconexion
            // 
            this.desconexion.Location = new System.Drawing.Point(53, 205);
            this.desconexion.Margin = new System.Windows.Forms.Padding(2);
            this.desconexion.Name = "desconexion";
            this.desconexion.Size = new System.Drawing.Size(82, 23);
            this.desconexion.TabIndex = 14;
            this.desconexion.Text = "Desconexión";
            this.desconexion.UseVisualStyleBackColor = true;
            this.desconexion.Visible = false;
            this.desconexion.Click += new System.EventHandler(this.desconexion_Click);
            // 
            // conexion
            // 
            this.conexion.Location = new System.Drawing.Point(191, 66);
            this.conexion.Margin = new System.Windows.Forms.Padding(2);
            this.conexion.Name = "conexion";
            this.conexion.Size = new System.Drawing.Size(82, 38);
            this.conexion.TabIndex = 15;
            this.conexion.Text = "Volver a conectarte";
            this.conexion.UseVisualStyleBackColor = true;
            this.conexion.Visible = false;
            this.conexion.Click += new System.EventHandler(this.conexion_Click);
            // 
            // ListaConectadosView
            // 
            this.ListaConectadosView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ListaConectadosView.Location = new System.Drawing.Point(191, 21);
            this.ListaConectadosView.Margin = new System.Windows.Forms.Padding(2);
            this.ListaConectadosView.Name = "ListaConectadosView";
            this.ListaConectadosView.RowHeadersWidth = 62;
            this.ListaConectadosView.RowTemplate.Height = 28;
            this.ListaConectadosView.Size = new System.Drawing.Size(160, 98);
            this.ListaConectadosView.TabIndex = 17;
            // 
            // verContra
            // 
            this.verContra.Location = new System.Drawing.Point(357, 133);
            this.verContra.Margin = new System.Windows.Forms.Padding(2);
            this.verContra.Name = "verContra";
            this.verContra.Size = new System.Drawing.Size(105, 27);
            this.verContra.TabIndex = 18;
            this.verContra.Text = "Ver contraseña";
            this.verContra.UseVisualStyleBackColor = true;
            this.verContra.Click += new System.EventHandler(this.verContra_Click);
            // 
            // btnInvitar
            // 
            this.btnInvitar.Location = new System.Drawing.Point(358, 21);
            this.btnInvitar.Name = "btnInvitar";
            this.btnInvitar.Size = new System.Drawing.Size(75, 23);
            this.btnInvitar.TabIndex = 19;
            this.btnInvitar.Text = "Invitar";
            this.btnInvitar.UseVisualStyleBackColor = true;
            this.btnInvitar.Visible = false;
            this.btnInvitar.Click += new System.EventHandler(this.btnInvitar_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(207, 250);
            this.progressBar1.MarqueeAnimationSpeed = 20;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 20;
            this.progressBar1.Value = 50;
            this.progressBar1.Visible = false;
            // 
            // cargandoLabel
            // 
            this.cargandoLabel.AutoSize = true;
            this.cargandoLabel.Location = new System.Drawing.Point(313, 260);
            this.cargandoLabel.Name = "cargandoLabel";
            this.cargandoLabel.Size = new System.Drawing.Size(109, 13);
            this.cargandoLabel.TabIndex = 21;
            this.cargandoLabel.Text = "Invitando jugadores...";
            this.cargandoLabel.Visible = false;
            // 
            // chatBox
            // 
            this.chatBox.FormattingEnabled = true;
            this.chatBox.Location = new System.Drawing.Point(508, 21);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(225, 212);
            this.chatBox.TabIndex = 22;
            this.chatBox.Visible = false;
            // 
            // chatTb
            // 
            this.chatTb.Location = new System.Drawing.Point(508, 250);
            this.chatTb.Name = "chatTb";
            this.chatTb.Size = new System.Drawing.Size(169, 20);
            this.chatTb.TabIndex = 23;
            this.chatTb.Visible = false;
            // 
            // chatSend
            // 
            this.chatSend.Location = new System.Drawing.Point(683, 247);
            this.chatSend.Name = "chatSend";
            this.chatSend.Size = new System.Drawing.Size(50, 23);
            this.chatSend.TabIndex = 24;
            this.chatSend.Text = "Enviar";
            this.chatSend.UseVisualStyleBackColor = true;
            this.chatSend.Visible = false;
            this.chatSend.Click += new System.EventHandler(this.chatSend_Click);
            // 
            // Geometry_wars
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 292);
            this.Controls.Add(this.chatSend);
            this.Controls.Add(this.chatTb);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.cargandoLabel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnInvitar);
            this.Controls.Add(this.verContra);
            this.Controls.Add(this.ListaConectadosView);
            this.Controls.Add(this.conexion);
            this.Controls.Add(this.desconexion);
            this.Controls.Add(this.ejecutarBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.edad);
            this.Controls.Add(this.consulta3);
            this.Controls.Add(this.consulta2);
            this.Controls.Add(this.consulta1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.registration);
            this.Controls.Add(this.login);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.contraseña);
            this.Controls.Add(this.usuario);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Geometry_wars";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Geometry_wars_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ListaConectadosView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usuario;
        private System.Windows.Forms.TextBox contraseña;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button login;
        private System.Windows.Forms.Button registration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton consulta1;
        private System.Windows.Forms.RadioButton consulta2;
        private System.Windows.Forms.RadioButton consulta3;
        private System.Windows.Forms.TextBox edad;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ejecutarBtn;
        private System.Windows.Forms.Button desconexion;
        private System.Windows.Forms.Button conexion;
        private System.Windows.Forms.DataGridView ListaConectadosView;
        private System.Windows.Forms.Button verContra;
        private System.Windows.Forms.Button btnInvitar;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label cargandoLabel;
        private System.Windows.Forms.ListBox chatBox;
        private System.Windows.Forms.TextBox chatTb;
        private System.Windows.Forms.Button chatSend;
    }
}

