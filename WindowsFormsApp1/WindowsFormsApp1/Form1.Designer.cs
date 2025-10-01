namespace WindowsFormsApp1
{
    partial class Form1
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
            this.altLbl = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBoxPC = new System.Windows.Forms.PictureBox();
            this.pictureBoxDron = new System.Windows.Forms.PictureBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDron)).BeginInit();
            this.SuspendLayout();
            // 
            // altLbl
            // 
            this.altLbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.altLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.altLbl.ForeColor = System.Drawing.Color.Red;
            this.altLbl.Location = new System.Drawing.Point(22, 309);
            this.altLbl.Name = "altLbl";
            this.altLbl.Size = new System.Drawing.Size(178, 45);
            this.altLbl.TabIndex = 1;
            this.altLbl.Text = "0";
            this.altLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(22, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 50);
            this.button1.TabIndex = 2;
            this.button1.Text = "Conectar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(22, 94);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 50);
            this.button2.TabIndex = 3;
            this.button2.Text = "Despegar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(22, 150);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 50);
            this.button3.TabIndex = 4;
            this.button3.Text = "Aterrizar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pictureBoxPC
            // 
            this.pictureBoxPC.Location = new System.Drawing.Point(275, 38);
            this.pictureBoxPC.Name = "pictureBoxPC";
            this.pictureBoxPC.Size = new System.Drawing.Size(264, 128);
            this.pictureBoxPC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPC.TabIndex = 5;
            this.pictureBoxPC.TabStop = false;
//            this.pictureBoxPC.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBoxDron
            // 
            this.pictureBoxDron.Location = new System.Drawing.Point(275, 195);
            this.pictureBoxDron.Name = "pictureBoxDron";
            this.pictureBoxDron.Size = new System.Drawing.Size(264, 128);
            this.pictureBoxDron.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDron.TabIndex = 6;
            this.pictureBoxDron.TabStop = false;
//            this.pictureBoxDron.Click += new System.EventHandler(this.pictureBoxDron_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.SlateGray;
            this.button4.Location = new System.Drawing.Point(275, 331);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(264, 48);
            this.button4.TabIndex = 7;
            this.button4.Text = "Streaming video";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(438, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Video PC";
//            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(438, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Video Dron";
//            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.pictureBoxDron);
            this.Controls.Add(this.pictureBoxPC);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.altLbl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDron)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label altLbl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBoxPC;
        private System.Windows.Forms.PictureBox pictureBoxDron;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

