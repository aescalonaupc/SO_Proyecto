namespace GeometryWarsGame.Game
{
    partial class Window
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
            this.components = new System.ComponentModel.Container();
            this.introPb = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.introPb)).BeginInit();
            this.SuspendLayout();
            // 
            // introPb
            // 
            this.introPb.Image = global::GeometryWarsGame.Properties.Resources.intro;
            this.introPb.Location = new System.Drawing.Point(515, 309);
            this.introPb.Name = "introPb";
            this.introPb.Size = new System.Drawing.Size(100, 50);
            this.introPb.TabIndex = 0;
            this.introPb.TabStop = false;
            this.introPb.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 3000;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.introPb);
            this.Name = "Window";
            this.Text = "Window";
            ((System.ComponentModel.ISupportInitialize)(this.introPb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox introPb;
        private System.Windows.Forms.Timer timer1;
    }
}