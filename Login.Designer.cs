namespace SmartVisionGroup
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.panel_Logo = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_PWs = new System.Windows.Forms.Label();
            this.label_Name = new System.Windows.Forms.Label();
            this.label_EN = new System.Windows.Forms.Label();
            this.button_Close = new System.Windows.Forms.Button();
            this.label_Name_1 = new System.Windows.Forms.Label();
            this.button_Login = new System.Windows.Forms.Button();
            this.textBox_PassWord = new System.Windows.Forms.TextBox();
            this.panel_PassWord = new System.Windows.Forms.Panel();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.panel_UserName = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Logo
            // 
            this.panel_Logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_Logo.BackgroundImage")));
            this.panel_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel_Logo.Location = new System.Drawing.Point(23, 94);
            this.panel_Logo.Name = "panel_Logo";
            this.panel_Logo.Size = new System.Drawing.Size(272, 125);
            this.panel_Logo.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_PWs);
            this.panel1.Controls.Add(this.label_Name);
            this.panel1.Controls.Add(this.label_EN);
            this.panel1.Controls.Add(this.button_Close);
            this.panel1.Controls.Add(this.label_Name_1);
            this.panel1.Controls.Add(this.button_Login);
            this.panel1.Controls.Add(this.textBox_PassWord);
            this.panel1.Controls.Add(this.panel_PassWord);
            this.panel1.Controls.Add(this.textBox_UserName);
            this.panel1.Controls.Add(this.panel_UserName);
            this.panel1.Location = new System.Drawing.Point(321, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 309);
            this.panel1.TabIndex = 7;
            // 
            // label_PWs
            // 
            this.label_PWs.AutoSize = true;
            this.label_PWs.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_PWs.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label_PWs.Location = new System.Drawing.Point(86, 203);
            this.label_PWs.Name = "label_PWs";
            this.label_PWs.Size = new System.Drawing.Size(90, 16);
            this.label_PWs.TabIndex = 9;
            this.label_PWs.Text = "（默认为 admin）";
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Name.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label_Name.Location = new System.Drawing.Point(86, 134);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(187, 16);
            this.label_Name.TabIndex = 8;
            this.label_Name.Text = "（用户名为您的真实姓名，如：张某某）";
            // 
            // label_EN
            // 
            this.label_EN.AutoSize = true;
            this.label_EN.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_EN.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label_EN.Location = new System.Drawing.Point(118, 57);
            this.label_EN.Name = "label_EN";
            this.label_EN.Size = new System.Drawing.Size(158, 17);
            this.label_EN.TabIndex = 7;
            this.label_EN.Text = "Object Manager System ";
            // 
            // button_Close
            // 
            this.button_Close.BackColor = System.Drawing.Color.DodgerBlue;
            this.button_Close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Teal;
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Close.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button_Close.Location = new System.Drawing.Point(196, 244);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(139, 42);
            this.button_Close.TabIndex = 6;
            this.button_Close.Text = "关闭";
            this.button_Close.UseVisualStyleBackColor = false;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // label_Name_1
            // 
            this.label_Name_1.AutoSize = true;
            this.label_Name_1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Name_1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label_Name_1.Location = new System.Drawing.Point(115, 24);
            this.label_Name_1.Name = "label_Name_1";
            this.label_Name_1.Size = new System.Drawing.Size(158, 31);
            this.label_Name_1.TabIndex = 5;
            this.label_Name_1.Text = "权限管理系统";
            // 
            // button_Login
            // 
            this.button_Login.BackColor = System.Drawing.Color.DodgerBlue;
            this.button_Login.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Teal;
            this.button_Login.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Login.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Login.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button_Login.Location = new System.Drawing.Point(38, 244);
            this.button_Login.Name = "button_Login";
            this.button_Login.Size = new System.Drawing.Size(139, 42);
            this.button_Login.TabIndex = 4;
            this.button_Login.Text = "登录";
            this.button_Login.UseVisualStyleBackColor = false;
            this.button_Login.Click += new System.EventHandler(this.button_Login_Click);
            // 
            // textBox_PassWord
            // 
            this.textBox_PassWord.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_PassWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PassWord.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox_PassWord.Location = new System.Drawing.Point(89, 181);
            this.textBox_PassWord.Name = "textBox_PassWord";
            this.textBox_PassWord.PasswordChar = '*';
            this.textBox_PassWord.Size = new System.Drawing.Size(239, 19);
            this.textBox_PassWord.TabIndex = 3;
            // 
            // panel_PassWord
            // 
            this.panel_PassWord.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_PassWord.BackgroundImage")));
            this.panel_PassWord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel_PassWord.Location = new System.Drawing.Point(38, 172);
            this.panel_PassWord.Name = "panel_PassWord";
            this.panel_PassWord.Size = new System.Drawing.Size(35, 35);
            this.panel_PassWord.TabIndex = 2;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UserName.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox_UserName.Location = new System.Drawing.Point(89, 109);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(239, 16);
            this.textBox_UserName.TabIndex = 1;
            // 
            // panel_UserName
            // 
            this.panel_UserName.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_UserName.BackgroundImage")));
            this.panel_UserName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel_UserName.Location = new System.Drawing.Point(38, 102);
            this.panel_UserName.Name = "panel_UserName";
            this.panel_UserName.Size = new System.Drawing.Size(35, 35);
            this.panel_UserName.TabIndex = 0;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 334);
            this.Controls.Add(this.panel_Logo);
            this.Controls.Add(this.panel1);
            this.Name = "Login";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Logo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_PWs;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.Label label_EN;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Label label_Name_1;
        private System.Windows.Forms.Button button_Login;
        private System.Windows.Forms.TextBox textBox_PassWord;
        private System.Windows.Forms.Panel panel_PassWord;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Panel panel_UserName;
    }
}