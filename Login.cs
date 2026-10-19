using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ErrorProofing;

namespace SmartVisionGroup
{
    public partial class Login : Form
    {
        

        public Login()
        {
            InitializeComponent();
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm.tUserLevel = "-1";
                if (!String.IsNullOrEmpty(this.textBox_UserName.Text.Trim()))
                {
                    if (!String.IsNullOrEmpty(this.textBox_PassWord.Text.Trim()))
                    {
                        if (MainForm.MainForm_P.LoginCheck(this.textBox_UserName.Text.Trim(), this.textBox_PassWord.Text.Trim()))
                        {
                            //获取当前登录状态及等级
                            MainForm.MainForm_P.GetUser(this.textBox_UserName.Text.Trim());
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("验证失败，请检查用户名或密码！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("密码不能为空！");
                    }
                }
                else
                {
                    MessageBox.Show("用户名不能为空！");
                }
            }
            catch (Exception ex)
            {
                MainForm.MainForm_P.RunLog(ex.ToString());
                //throw;
            }
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
