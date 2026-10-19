using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace ErrorProofing
{
    public partial class AddMaterialID : Form
    {
        private int _cellID = 0;


        private int _row = 0;
        private int _column = 0;
        public static AddMaterialID _AdMaterialID ;
        public AddMaterialID(int cellID, int row, int column)
        {
            _column = column;
            _row = row;
            _cellID = cellID;
            InitializeComponent();
            _AdMaterialID = this;
            _row = row;
            _column = column;
        }

        public static string Str_SqlCon = @"Server=.;database=ZZSJ_DATA;Integrated Security=SSPI;Connect Timeout=3";
        public void UpdateMaterialIDandPLCAddress(string _materialID, int _celliD, int _qty)
        {
            
            
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);
            //sql指令语句参数化           
            string _sqlstr = "UPDATE CellIDandMaterial  SET Number = @Number, MaterialID = @MaterialID  WHERE CellID = @CellID ";
            //string _sqlUpdateQty = "UPDATE CellIDandMaterial SET Number = @Number WHERE CellX = @CellX AND CellY = @CellY";
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcmd.Parameters.AddWithValue("@MaterialID", _materialID);
                //_sqlcmd.Parameters.AddWithValue("@CellX", _x);
                //_sqlcmd.Parameters.AddWithValue("@CellY", _y);
                _sqlcmd.Parameters.AddWithValue("@Number", _qty);
                _sqlcmd.Parameters.AddWithValue("@CellID", _celliD);
                _sqlcon.Open();
                SqlDataReader _sqldr = _sqlcmd.ExecuteReader();

                if (_sqldr.HasRows)
                {
                    while (_sqldr.Read())
                    {
                        try
                        {
                            //_x = int.Parse(_sqldr["CellX"].ToString());
                            //_y = int.Parse(_sqldr["CellY"].ToString());
                            _materialID = _sqldr["MaterialID"].ToString();
                            _qty = int.Parse( _sqldr["@Number"].ToString());
                            _celliD = int.Parse(_sqldr["@CellID"].ToString());
                        }
                        catch (Exception ex)
                        {
                            //RunLog(ex.ToString());
                        }
                    }
                }
                _sqldr.Close();
               
            }
            catch (Exception ex)
            {
                //RunLog(ex.ToString());

            }
        }

        private void AddMaterialID_Load(object sender, EventArgs e)
        {
            //tb_column.Text = _column.ToString();
            //tb_Row.Text = _row.ToString();
            this.tb_Material.Focus();
            tb_Material.AutoCompleteMode = AutoCompleteMode.None;
            tb_Material.AutoCompleteSource = AutoCompleteSource.None;
            tb_Material.AutoCompleteCustomSource = null;
            //MainForm.MainForm_P.PLCAddressPose(_column, _row, out int _cellID);
            MainForm.MainForm_P.WritePLC(MainForm.MainForm_P.PLCAddressPose( _row, _column,out int _cellID1), true  );

            tb_CellID.Text = _cellID.ToString();
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string _materialID= tb_Material.Text;
            
            //int _x = int.Parse(tb_Row.Text);
            //int _y = int.Parse(tb_column.Text);
            int cellID = int.Parse(tb_CellID.Text);
            int number = 50;
            if (list.Contains(_materialID)) 
            {
                UpdateMaterialIDandPLCAddress(_materialID, cellID, number);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if(_materialID == "")
            {
                UpdateMaterialIDandPLCAddress(_materialID, cellID, number);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("物料不存在");
            }
            //textBox1.Clear();
            
        }

        private void AddMaterialID_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MainForm.MainForm_P.datagridviewbutton();
           // MainForm.MainForm_P.PLCAddressPose(_column,_row,out int _cellID);
            MainForm.MainForm_P.WritePLC(MainForm.MainForm_P.PLCAddressPose(_row, _column, out int _cellID1), false);

        }


        private List<string> GetMaterialIDList(string prefix)
        {
            List<string> list = new List<string>();

            if (string.IsNullOrWhiteSpace(prefix))
                return list;

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
            SELECT MaterialID
            FROM ModelList
            WHERE MaterialID IS NOT NULL
              AND MaterialID LIKE '%' + @prefix + '%'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@prefix", SqlDbType.VarChar, 50).Value = prefix;

                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            list.Add(dr.GetString(0));
                    }
                }
            }

            return list;
        }

        private void PositionListBox()
        {
            Point p = tb_Material.Parent.PointToScreen(tb_Material.Location);
            p = this.PointToClient(p);

            lbSuggest.Left = p.X;
            lbSuggest.Top = p.Y + tb_Material.Height;
            lbSuggest.Width = tb_Material.Width;
            lbSuggest.Height = Math.Max(
                30,
                Math.Min(150, lbSuggest.ItemHeight * lbSuggest.Items.Count + 2)
                );

            lbSuggest.BringToFront();
        }
       

        private void lbSuggest_MouseClick(object sender, MouseEventArgs e)
        {
            if (lbSuggest.SelectedItem == null) return;

            tb_Material.Text = lbSuggest.SelectedItem.ToString();
            tb_Material.SelectionStart = tb_Material.Text.Length;

            lbSuggest.Visible = false;
        }
        public List< string> list = new List<string>();

        private void tb_Material_TextChanged(object sender, EventArgs e)
        {
            string raw = tb_Material.Text;
            string keyword = Regex.Replace(raw, @"\D", "");
            if (raw != keyword)
            {
                int pos = tb_Material.SelectionStart;
                tb_Material.Text = keyword;
                tb_Material.SelectionStart = Math.Min(pos, keyword.Length);
                return;
            }

            // 没输入内容就隐藏提示
            if (keyword.Length == 0)
            {
                lbSuggest.Visible = false;
                return;
            }

            // 查询
             list = GetMaterialIDList(keyword);

            // 更新 ListBox
            lbSuggest.BeginUpdate();
            lbSuggest.Items.Clear();
            foreach (var item in list)
                lbSuggest.Items.Add(item);
            lbSuggest.EndUpdate();

            if (lbSuggest.Items.Count > 0)
            {
                PositionListBox();
                lbSuggest.Visible = true;
            }
            else
            {
                lbSuggest.Visible = false;
            }
        }
    }
}
