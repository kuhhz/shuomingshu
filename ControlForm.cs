using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using static ErrorProofing.MainForm;

namespace ErrorProofing
{

    public partial class ControlForm : Form
    {
        
        public static ControlForm _control;
        public ControlForm()
        {
            InitializeComponent();
            _control = this;
        }

        

        private void ControlForm_Load(object sender, EventArgs e)
        {

 
            textBox1.Text = QueueConfig.MaxBatchCount.ToString();
            if (MainForm.MainForm_P.b_WorkMode)
            {
                checkBox1.Checked = true;
            }
            else
            { 
                checkBox1.Checked = false; 
            }
            if (MainForm.MainForm_P.b_ls_tx)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
            if (MainForm.MainForm_P.b_QueueFull_tx)
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                MainForm.MainForm_P.b_WorkMode = false;
            }
            else
            {
                MainForm.MainForm_P.b_WorkMode = true;
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int newSize) || newSize <= 0)
            {
                MessageBox.Show("请输入大于 0 的整数");
                return;
            }

            QueueConfig.MaxBatchCount = newSize;
            SaveMaxBatchCount(newSize);

            MessageBox.Show($"队列最大批次数已修改为 {newSize}");
        }
        private void SaveMaxBatchCount(int value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["MaxBatchCount"] == null)
                config.AppSettings.Settings.Add("MaxBatchCount", value.ToString());
            else
                config.AppSettings.Settings["MaxBatchCount"].Value = value.ToString();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true; 
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int newSize) || newSize <= 0)
            {
                textBox1.ForeColor = Color.Red;
                return;
            }

            textBox1.ForeColor = Color.Black;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                MainForm.MainForm_P.b_ls_tx = false;
            }
            else
            {
                MainForm.MainForm_P.b_ls_tx = true;

            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {
                MainForm.MainForm_P.b_QueueFull_tx = false;
            }
            else
            {
                MainForm.MainForm_P.b_QueueFull_tx = true;

            }
        }
    }
}
