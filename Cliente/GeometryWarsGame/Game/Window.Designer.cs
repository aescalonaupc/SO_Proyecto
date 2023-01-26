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
            this.introPb = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.introPb)).BeginInit();
            this.SuspendLayout();
            // 
            // introPb
            // 
            this.introPb.Image = global::GeometryWarsGame.Properties.Resources.intro;
            this.introPb.Location = new System.Drawing.Point(589, 412);
            this.introPb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.introPb.Name = "introPb";
            this.introPb.Size = new System.Drawing.Size(114, 67);
            this.introPb.TabIndex = 0;
            this.introPb.TabStop = false;
            this.introPb.Visible = false;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 600);
            this.Controls.Add(this.introPb);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Window";
            this.Text = "Window";
            ((System.ComponentModel.ISupportInitialize)(this.introPb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox introPb;
    }
}