namespace C_Sharp_Onscreenkeys
{
    partial class Form_Config
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
            this.label1 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bt_TC = new System.Windows.Forms.Button();
            this.bt_TP = new System.Windows.Forms.Button();
            this.bt_FT = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // colorDialog1
            // 
            this.colorDialog1.Color = System.Drawing.Color.MediumOrchid;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Fundo da tela:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Teclas padrão:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Teclas do cursor:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "Definição de cores:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bt_TC);
            this.panel1.Controls.Add(this.bt_TP);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.bt_FT);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 112);
            this.panel1.TabIndex = 6;
            // 
            // bt_TC
            // 
            this.bt_TC.Location = new System.Drawing.Point(98, 84);
            this.bt_TC.Name = "bt_TC";
            this.bt_TC.Size = new System.Drawing.Size(75, 23);
            this.bt_TC.TabIndex = 7;
            this.bt_TC.Text = "Cores";
            this.bt_TC.UseVisualStyleBackColor = true;
            // 
            // bt_TP
            // 
            this.bt_TP.Location = new System.Drawing.Point(98, 54);
            this.bt_TP.Name = "bt_TP";
            this.bt_TP.Size = new System.Drawing.Size(75, 23);
            this.bt_TP.TabIndex = 6;
            this.bt_TP.Text = "Cores";
            this.bt_TP.UseVisualStyleBackColor = true;
            // 
            // bt_FT
            // 
            this.bt_FT.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::C_Sharp_Onscreenkeys.Properties.Settings.Default, "bt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.bt_FT.Location = new System.Drawing.Point(98, 25);
            this.bt_FT.Name = "bt_FT";
            this.bt_FT.Size = new System.Drawing.Size(75, 23);
            this.bt_FT.TabIndex = 1;
            this.bt_FT.Text = global::C_Sharp_Onscreenkeys.Properties.Settings.Default.bt;
            this.bt_FT.UseVisualStyleBackColor = true;
            this.bt_FT.Click += new System.EventHandler(this.bt_FT_Click);
            // 
            // Form_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 160);
            this.Controls.Add(this.panel1);
            this.Name = "Form_Config";
            this.Text = "Configurações";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button bt_FT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bt_TC;
        private System.Windows.Forms.Button bt_TP;
    }
}