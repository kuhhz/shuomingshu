using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ErrorProofing
{
    
    public partial class MesShow : Form
    {
        public static MesShow _Mes_P;
        public MesShow()
        {
            InitializeComponent();
            _Mes_P = this;
        }
        private void MesShow_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("型号");
            comboBox1.Items.Add("物料号");

            comboBox1.SelectedIndex = 0;
        }
        public static string Str_SqlCon = @"Server=.;database=ZZSJ_DATA;Integrated Security=SSPI;Connect Timeout=3";
        public DataTable GetMes_fromModelList()
        {
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);
            SqlDataReader _sqldr = null;

            DataTable dt = new DataTable();
            dt.Columns.Add("ProductID");
            dt.Columns.Add("Model");
            dt.Columns.Add("MaterialID");
            dt.Columns.Add("MaterialDescript");

            string _sqlstr = @"
                            SELECT ProductID,Model, MaterialID,MaterialDescript
                            FROM ModelList ";

            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcon.Open();
                _sqldr = _sqlcmd.ExecuteReader();

                if (_sqldr.HasRows)
                {
                    while (_sqldr.Read())
                    {
                        try
                        {
                            DataRow row = dt.NewRow();
                            row["ProductID"] = _sqldr["ProductID"]?.ToString();
                            row["Model"] = _sqldr["Model"]?.ToString();
                            row["MaterialID"] = _sqldr["MaterialID"]?.ToString();
                            row["MaterialDescript"] = _sqldr["MaterialDescript"]?.ToString();
                            dt.Rows.Add(row);
                        }
                        catch (Exception ex)
                        {

                            MainForm.MainForm_P.RunLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.MainForm_P.RunLog(ex.ToString());
            }
            finally
            {
                if (_sqldr != null && !_sqldr.IsClosed)
                    _sqldr.Close();

                if (_sqlcon.State != ConnectionState.Closed)
                    _sqlcon.Close();

                _sqlcon.Dispose();
            }

            return dt;
        }
        private bool _refreshing = false;
        public void RefreshMedGrid_Async()
        {
            if (this.IsDisposed) return;
            if (!this.IsHandleCreated) return;
            if (_refreshing) return;
            _refreshing = true;

            Task.Run(() =>
            {
                try
                {
                    // 后台取数据
                    DataTable dt = GetMes_fromModelList();

                    // 回 UI 线程更新
                    this.BeginInvoke(new Action(() =>
                    {
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = dt;

                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.RowHeadersVisible = false;
                        dataGridView1.ReadOnly = true;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }));
                }
                catch (Exception ex)
                {
                    MainForm.MainForm_P.RunLog(ex.ToString());
                }
                finally
                {
                    _refreshing = false;
                }
            });
        }

        private int AddModel_and_Material(string _Mod, string _Mat)
        {
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);
            string _sqlstr = "INSERT INTO ModelList (Model, MaterialID) VALUES (@Model, @MaterialID)";
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcmd.Parameters.AddWithValue("@Model", _Mod);
                _sqlcmd.Parameters.AddWithValue("@MaterialID", _Mat);
                _sqlcon.Open();

                int count = _sqlcmd.ExecuteNonQuery();

                return count;


            }
            catch (Exception ex)
            {
                MainForm.MainForm_P.RunLog(ex.ToString());
                return -1;

            }
            finally
            {
                _sqlcon.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = AddModel_and_Material(textBox1.Text, textBox2.Text);
            if (count > 0)
            {
                MessageBox.Show("插入成功");
            }
            else
            {
                MessageBox.Show("插入失败");
            }
        }
        public DataTable GetInformation(string modelValue, string materialValue)
        {
            DataTable dt = new DataTable();
            string _sq = "";

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                // ① 根据传进来的值决定 SQL
                if (!string.IsNullOrWhiteSpace(modelValue))
                {
                    _sq = @"
                SELECT ProductID, Model, MaterialID, MaterialDescript
                FROM ModelList
                WHERE Model LIKE @Model";
                }
                else if (!string.IsNullOrWhiteSpace(materialValue))
                {
                    _sq = @"
                SELECT ProductID, Model, MaterialID, MaterialDescript
                FROM ModelList
                WHERE MaterialID LIKE @MaterialID";
                }
                else
                {
                    return dt; // 两个都空，直接返回
                }

                SqlCommand cmd = new SqlCommand(_sq, conn);

                // ② 参数
                if (!string.IsNullOrWhiteSpace(modelValue))
                {
                    cmd.Parameters.Add("@Model", SqlDbType.NVarChar, 50)
                                  .Value = "%" + modelValue + "%";
                }
                else
                {
                    cmd.Parameters.Add("@MaterialID", SqlDbType.NVarChar, 50)
                                  .Value = "%" + materialValue + "%";
                }

                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    MainForm.MainForm_P.RunLog("查询失败：" + ex.Message);
                }
            }

            return dt;

        }
        public void RefreshMesGrid_Async()
        {
            string keyword = textBox3.Text.Trim();
            string modelValue = null;
            string materialValue = null;

            // 根据 ComboBox 决定查什么
            if (comboBox1.Text == "型号")
            {
                modelValue = keyword;
            }
            else if (comboBox1.Text == "物料号")
            {
                materialValue = keyword;
            }

            Task.Run(() =>
            {
                try
                {
                    DataTable dt = GetInformation(modelValue, materialValue);

                    this.BeginInvoke(new Action(() =>
                    {
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = dt;
                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.RowHeadersVisible = false;
                        dataGridView1.ReadOnly = true;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }));
                }
                catch (Exception ex)
                {
                    MainForm.MainForm_P.RunLog(ex.ToString());
                }
            });
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            RefreshMesGrid_Async();
        }

        private int DeleteModel_and_Material(string _Mod)
        {
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);
            string _sqlstr = "Delete from ModelList where Model=@Model";
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcmd.Parameters.AddWithValue("@Model", _Mod);
                _sqlcon.Open();
                int count = _sqlcmd.ExecuteNonQuery();
                return count;
            }
            catch (Exception ex)
            {
                MainForm.MainForm_P.RunLog(ex.ToString());
                return -1;

            }
            finally
            {
                _sqlcon.Close();
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            int count = DeleteModel_and_Material(textBox4.Text);
            if (count > 0)
            {
                MessageBox.Show("删除成功");
            }
            else
            {
                MessageBox.Show("删除失败");
            }
        }
    }
}
