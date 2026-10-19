using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Siemens;
using Newtonsoft.Json;
using SmartVisionGroup;
using Timer = System.Windows.Forms.Timer;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;

namespace ErrorProofing
{
    public partial class MainForm : Form
    {
        public static MainForm MainForm_P;


        public MainForm()
        {
            InitializeComponent();  
            MainForm_P = this;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Process[] _process = Process.GetProcessesByName("ErrorProofing");
            if (_process.Length > 1)
            {
                MessageBox.Show("软件已经运行，请勿重复打开！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //杀掉软件进程
                Process.GetCurrentProcess().Kill();
            }
            string value = ConfigurationManager.AppSettings["MaxBatchCount"];

            if (!int.TryParse(value, out int size) || size <= 0)
            {
                size = 3; // 默认值
            }

            QueueConfig.MaxBatchCount = size;
            b_ls_tx = false;
            b_QueueFull_tx = false;
            b_WorkMode = true;
            SystemConfig.MaxQueueSize = SystemConfig.LoadMaxQueueSize();
            ConnectPLC();
            datagridviewbutton();
            datagridviewbutton3();
            CreateTimer_RefreshGrid();
            ConnectCheckIDReader();
            RecoverQueueFromDB();
            CreateTimer_GetMESMaterial();
            OpenCheckCodeReaderSerialPort();
            this.ActiveControl = textBox3;
            this.textBox3.Focus();
            //RefreshQueueGrid_Async();
            //RunLog($"【配置加载】QueueCount={materialQueue.Count}，MaxQueueSize={SystemConfig.MaxQueueSize}");
            TimerSend();
        }
        public static string _SN1 = "";
        public static int _Num = 0;

        #region 运行日志

        public delegate void dgshowcolor();
        private void showcolor( )
        {
            Invoke(new MethodInvoker(delegate ()
            {
                if (isTaking)
                {
                    textBox2.ForeColor = Color.Green;
                }
                else
                {
                    textBox2.ForeColor = Color.Red;
                }
                
            }));
        }

        //日志显示
        public delegate void dgSetText(string message);

        public void UpdateText(object msg)
        {
            //lock (this)
            //{
            Invoke(new MethodInvoker(delegate ()
            {
                textBox_Log.AppendText(msg + "\r\n");
                textBox_Log.ScrollToCaret();
                if (textBox_Log.Lines.Length > 1000)
                {
                    textBox_Log.Clear();
                }
            }));
            //}
        }

        //运行日志
        public void RunLog(string Logtxt)
        {
            string Logtime = DateTime.Now.ToString() + "   ";
            try
            {
                dgSetText dg = new dgSetText(UpdateText);
                dg.Invoke(System.DateTime.Now.ToString() + "--->" + "\r" + Logtxt + "\r");
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "RunLog\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "RunLog\\");
                }
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "RunLog\\" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "Log.txt", FileMode.OpenOrCreate);
                fs.Position = fs.Length;
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Logtime + Logtxt + "\r");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //
            }
        }
        public void ErrLog(string Logtxt)
        {
            string Logtime = DateTime.Now.ToString() + "   ";
            try
            {
                dgSetText dg = new dgSetText(UpdateText);
                dg.Invoke(System.DateTime.Now.ToString() + "--->" + "\r" + Logtxt + "\r");
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "ErrLog\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "ErrLog\\");
                }
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ErrLog\\" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "Log.txt", FileMode.OpenOrCreate);
                fs.Position = fs.Length;
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Logtime + Logtxt + "\r");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //
            }
        }
        public void Log(string Logtxt)
        {
            string Logtime = DateTime.Now.ToString() + "   ";
            try
            {
                dgSetText dg = new dgSetText(UpdateText);
                dg.Invoke(System.DateTime.Now.ToString() + "--->" + "\r" + Logtxt + "\r");
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Log\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Log\\");
                }
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Log\\" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "Log.txt", FileMode.OpenOrCreate);
                fs.Position = fs.Length;
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Logtime + Logtxt + "\r");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //
            }
        }
        public void MESLog(string Logtxt)
        {
            string Logtime = DateTime.Now.ToString() + "   ";
            try
            {
                dgSetText dg = new dgSetText(UpdateText);
                dg.Invoke(System.DateTime.Now.ToString() + "--->" + "\r" + Logtxt + "\r");
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "MESLog\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "MESLog\\");
                }
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "MESLog\\" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "Log.txt", FileMode.OpenOrCreate);
                fs.Position = fs.Length;
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Logtime + Logtxt + "\r");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //
            }
        }
        #endregion 运行日志

        #region sql

        public static string Str_SqlCon = @"Server=.;database=ZZSJ_DATA;Integrated Security=SSPI;Connect Timeout=3";
        public static string MaterialID = "";
        public static string PLCAddress = "";

        
        //从队列库里查询型号与物料号


        private void GetMaterialPose(int _x, int _y, out string _materialID, out int _n)
        {
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);
            //int x = 0;
            //int y = 0;
            //sql指令语句参数化           
            string materialID = "";
            int n = 0;
            SqlDataReader _sqldr = null;
            string _sqlstr = "SELECT CellX,CellY,MaterialID,Number  FROM  CellIDandMaterial  WHERE CellX = @CellX AND CellY = @CellY ";
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcmd.Parameters.AddWithValue("@CellX", _x);
                _sqlcmd.Parameters.AddWithValue("@CellY", _y);

                _sqlcon.Open();
                _sqldr = _sqlcmd.ExecuteReader();
                if (_sqldr.HasRows)
                {
                    while (_sqldr.Read())
                    {
                        try
                        {
                            //_x = int.Parse(_sqldr["CellX"].ToString());
                            //_y = int.Parse(_sqldr["CellY"].ToString());
                            materialID = _sqldr["MaterialID"].ToString();
                            n = int.TryParse(_sqldr["Number"]?.ToString(), out int tmp) ? tmp : 0;
                        }
                        catch (Exception ex)
                        {
                            RunLog(ex.ToString());
                        }
                    }
                }
                _sqldr.Close();
                
                _materialID = materialID;
                _n = n;
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
                _materialID = "";
                _n = 0;
            }
            finally
            {

                if (_sqldr != null && !_sqldr.IsClosed)
                    _sqldr.Close();

                if (_sqlcon != null && _sqlcon.State != ConnectionState.Closed)
                    _sqlcon.Close();

                _sqlcon?.Dispose();
            }
        }

        private string _mo = "";
        //查询型号和对应物料号
        private List<string> GetMaterialIDandModel(string keycode)
        {
            List<string> materialList = new List<string>();

            check12or17(keycode, out string model);
            _mo = model;

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = "SELECT MaterialID FROM ModelList WHERE Model = @Model";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Model", model);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string mat = reader["MaterialID"].ToString();
                        materialList.Add(mat);
                        RunLog($"型号:{keycode} 物料:{mat}");
                    }
                }
            }
            return materialList;
        }


        //入队数据库
        private void InsertQueueToDB(string _batchid,string _model,string materialid,int _cellID)
        {
            
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
                            INSERT INTO PickQueue
                            (BatchID, ModelID,MaterialID, EnqueueTime, Status,CellID)
                            VALUES
                            (@BatchID, @ModelID, @MaterialID,GETDATE(), 0,@CellID)";
                  
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BatchID", _batchid);
                cmd.Parameters.AddWithValue("@ModelID", _model);
                cmd.Parameters.AddWithValue("@MaterialID", materialid);
                cmd.Parameters.AddWithValue("@CellID", _cellID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        //获取数据库队列中有几个数据
        private int GetQueueCountFromDB()
        {
            SqlConnection conn = new SqlConnection(Str_SqlCon);
            try
            {
                string sql = "SELECT COUNT(*) FROM PickQueue WHERE Status IN (0,1)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();


                return count;
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
            }
            finally
            {  
                conn.Close();
            }
            return -1;
        }
        //根据id查询最早入队且状态为0的那一条
        private string GetNextWaitingBatchID()
        {
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
                            SELECT TOP 1 BatchID
                            FROM PickQueue
                            WHERE Status = 0
                            ORDER BY EnqueueTime";
                  
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }
        //查询当前应该出队的那一条
        private string GetCurrentBatchID()
        {
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
            SELECT TOP 1 BatchID
            FROM PickQueue
            WHERE Status = 1
            ORDER BY EnqueueTime";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }
        private List<MyStruct> GetBatchItems(string batchID)
        {
            List<MyStruct> list = new List<MyStruct>();

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
            SELECT ModelID, MaterialID, CellID
            FROM PickQueue
            WHERE BatchID = @BatchID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BatchID", batchID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new MyStruct
                    {
                        Key_code = reader["ModelID"].ToString(),
                        MaterialID = reader["MaterialID"].ToString(),
                        CellID = reader["CellID"].ToString()
                    });
                }
            }
            return list;
        }

        //更改数据库状态
        private void UpdateQueueStatusByBatch(string batchID, int status)
        {
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
            UPDATE PickQueue
            SET Status = @Status
            WHERE BatchID = @BatchID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@BatchID", batchID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
 
        //删除状态为2的队列数据
        private int DeleteFinishedQueueFromDB()
        {
            SqlConnection conn = new SqlConnection(Str_SqlCon);
            try
            {
                string sql = @"
                            DELETE FROM PickQueue
                            WHERE Status = 2";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                int count = cmd.ExecuteNonQuery();

                // 返回删除的行数
                return count;


            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return -1;
        }
        //通过物料获取其对应位置
        private List<(int x, int y)> PickAllCellsForMaterial(string materialId)
        {
            var list = new List<(int x, int y)>();

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
            SELECT CellX, CellY
            FROM CellIDandMaterial
            WHERE MaterialID = @MaterialID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaterialID", materialId);

                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add((
                        Convert.ToInt32(reader["CellX"]),
                        Convert.ToInt32(reader["CellY"])
                    ));
                }
            }

            return list;
        }
        //查询PLC地址和对应pose
        public string PLCAddressPose(int _x, int _y, out int _cellID)
        {
            string _plcaddress = "";
            int cellID = 0;
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);
            //sql指令语句参数化           
            string _sqlstr = "SELECT CellX,CellY,PLCAddress,CellID  FROM  CellPose  WHERE CellX = @CellX AND CellY = @CellY";
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcmd.Parameters.AddWithValue("@CellX", _x);
                _sqlcmd.Parameters.AddWithValue("@CellY", _y);
                _sqlcon.Open();
                SqlDataReader _sqldr = _sqlcmd.ExecuteReader();

                if (_sqldr.HasRows)
                {
                    while (_sqldr.Read())
                    {
                        try
                        {

                            _plcaddress = _sqldr["PLCAddress"].ToString();
                            //cellID = int.Parse(_sqldr["CellID"].ToString());
                            string cellIDStr = _sqldr["CellID"]?.ToString();

                            if (!string.IsNullOrWhiteSpace(cellIDStr) &&
                                int.TryParse(cellIDStr.Trim(), out int tmpCellID))
                            {
                                cellID = tmpCellID;
                            }

                        }
                        catch (Exception ex)
                        {
                            RunLog(ex.ToString());
                        }
                    }
                }
                _sqldr.Close();
                _cellID = cellID;
                return _plcaddress;
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
                _cellID = 0;
                return "";
            }
        }

        public DataTable GetPickQueue_ModelAndMaterial()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("冰箱条码");
            dt.Columns.Add("物料号");
            dt.Columns.Add("批次号");     // 这里显示的是“业务批次号”
            dt.Columns.Add("物料位置");
            dt.Columns.Add("取料状态");

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                string sql = @"
                            SELECT
                                BatchID,
                                ModelID,
                                MaterialID,
                                CellID,
                                Status
                            FROM PickQueue
                            WHERE Status IN (0,1)
                            ORDER BY EnqueueTime";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string modelID = reader["ModelID"]?.ToString();
                        string materialID = reader["MaterialID"]?.ToString();
                        string cellID = reader["CellID"]?.ToString();
                        int status = Convert.ToInt32(reader["Status"]);
                        string stateText = status == 1 ? "准备取料" : "排队中";

                        // 业务批次号规则
                        string batchText = "";
                        if (!string.IsNullOrEmpty(modelID) && modelID.Length >= 17)
                        {
                            batchText = modelID.Substring(13, 4);
                        }

                        DataRow row = dt.NewRow();
                        row["冰箱条码"] = modelID;
                        row["物料号"] = materialID;
                        row["批次号"] = batchText;
                        row["物料位置"] = cellID;
                        row["取料状态"] = stateText;

                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }
        //获取数据库里不同BatchID的数量
        public int GetDB_BatchID_Count()
        {
            int count = 0;
            SqlConnection _sqlcon = new SqlConnection(Str_SqlCon);        
            string _sqlstr = @"
            SELECT COUNT(DISTINCT BatchID)
            FROM PickQueue
            WHERE Status IN (0,1)";
            try
            {
                SqlCommand _sqlcmd = new SqlCommand(_sqlstr, _sqlcon);
                _sqlcon.Open();
                count = Convert.ToInt32(_sqlcmd.ExecuteScalar());
                _sqlcon.Close();
                
                return count;
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());  
                
                return 0;
            }
        }
        public void UpdateHistory(int position)
        {
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                conn.Open();

                string sql = @"
                            IF EXISTS (SELECT 1 FROM HistorySelect WHERE Position = @Position)
                            BEGIN
                                UPDATE HistorySelect
                                SET Count = Count + 1
                                WHERE Position = @Position
                            END
                            ELSE
                            BEGIN
                                INSERT INTO HistorySelect (Position, Count)
                                VALUES (@Position, 1)
                            END";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Position", position);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Dictionary<int, int> GetAllHistoryCount()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();

            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                conn.Open();

                string sql = "SELECT Position, Count FROM HistorySelect";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int pos = reader.GetInt32(0);
                        int count = reader.GetInt32(1);
                        dict[pos] = count;
                    }
                }
            }

            return dict;
        }
        public void ClearHistoryCount()
        {
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                conn.Open();
                string sql = "UPDATE HistorySelect SET Count = 0";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region 权限管理

        //权限
        public static string tUserLevel = " - 1";
        private static string UserName = "";
        int maxBatch = QueueConfig.MaxBatchCount;
        #region 用户登录验证

        public bool LoginCheck(string _name, string _paw)
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {
                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "select * from ResetList where UserName ='" + _name + "'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                //判断检索结果
                if (reader.Read())
                {
                    string _isPaw = reader["UserPaw"].ToString();
                    if (_isPaw == _paw)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;   //用户不存在
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                return false;
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }

        #endregion

        #region 获取当前用户信息

        public static int NGStopLine = 0; //停线信号 0不停 1停
        public static int UpStartMES = 0;

        public void GetUser(string _name)
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {
                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "select * from ResetList where UserName ='" + _name + "'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                //判断检索结果
                if (reader.Read())
                {
                    UserName = reader["UserName"].ToString();
                    tUserLevel = reader["UserLevel"].ToString();
                }
                else
                {
                    RunLog("未找到该用户数据！");
                }

                if (tUserLevel == "2")
                {
                    //checkBox1.Enabled = true;
                    //textBox2.Enabled = true;
                }
                else if (tUserLevel == "1")
                {
                    //checkBox1.Enabled = false;
                    //textBox2.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }

        #endregion

        public void EnbReset()
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {

                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "select * from EnbReset where IndexCode = '0'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                //判断检索结果
                if (reader.Read())
                {
                    NGStopLine = Convert.ToInt32(reader["Enb"].ToString());
                    UpStartMES = Convert.ToInt32(reader["MESEnd"].ToString());
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }

        public bool OpenNCEnb()
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {
                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "update EnbReset set Enb = '1' where IndexCode = '0'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                //SqlDataReader reader = cmd.ExecuteReader();

                if (cmd.ExecuteNonQuery() == 1)//执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
                {
                    NGStopLine = 1;
                    RunLog("启动不合格停线成功！");
                    return true;
                }
                else
                {
                    RunLog("启动不合格停线失败！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                return false;
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }


        public bool CloseNCEnb()
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {
                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "update EnbReset set Enb = '0' where IndexCode = '0'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                //SqlDataReader reader = cmd.ExecuteReader();

                if (cmd.ExecuteNonQuery() == 1)//执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
                {
                    NGStopLine = 0;
                    RunLog("禁用不合格停线成功！");
                    return true;
                }
                else
                {
                    RunLog("禁用不合格停线失败！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                return false;
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }


        public bool MESStart()
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {
                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "update EnbReset set MESEnd = '1' where IndexCode = '0'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                //SqlDataReader reader = cmd.ExecuteReader();

                if (cmd.ExecuteNonQuery() == 1)//执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
                {
                    UpStartMES = 1;
                    RunLog("MES开启成功！");
                    return true;
                }
                else
                {
                    RunLog("MES开启失败！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                return false;
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }

        public bool MESStop()
        {
            SqlConnection SqlCon;
            SqlCon = new SqlConnection(Str_SqlCon);
            try
            {
                SqlCon = new SqlConnection(Str_SqlCon);
                SqlCon.Open();
                string str_sql = "update EnbReset set MESEnd = '0' where IndexCode = '0'";
                SqlCommand cmd = new SqlCommand(str_sql, SqlCon);
                //SqlDataReader reader = cmd.ExecuteReader();

                if (cmd.ExecuteNonQuery() == 1)//执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
                {
                    UpStartMES = 0;
                    RunLog("MES禁用成功！");
                    return true;
                }
                else
                {
                    RunLog("MES禁用失败！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //SqlCon.Close();
                RunLog(ex.ToString());
                return false;
                //throw;
            }
            finally
            {
                SqlCon.Close();
            }
        }


        #endregion

        #region 模式切换
        public enum WorkMode
        {
            Normal,   // 普通模式
            Queue     // 队列模式
        }

        private WorkMode currentMode = WorkMode.Normal;
        public bool b_WorkMode = false;
        public bool b_ls_tx = false;
        public bool b_QueueFull_tx = false;
        public bool is_ls_tx=false;
        public bool is_QueueFull_tx=false;
        
        #endregion

        #region 队列
        public struct MyStruct
        {
            public string Key_code;
            public string Address;
            public string MaterialID;
            public string CellID;
            public string BatchID;
        }
        private int _batchCount = 0;
        //private Queue<MyStruct> materialQueue = new Queue<MyStruct>();
        private readonly object queueLock = new object();
        private Dictionary<string, bool> materialTakenStatus = new Dictionary<string, bool>();
        // 当前批次是否已完成取料
        //volatile bool _currentBatchFinished = false;

        //// 是否已经扫到下一条有效条码
        //volatile bool _nextBarcodeArrived = false;

        List<string> preList = new List<string>();
        List<string> preList_sd = new List<string>();

        private void ProcessQueueLogic(MyStruct newModel)
        {
            int currentBatchSize = Volatile.Read(ref _batchCount); ;

            if (currentBatchSize >= maxBatch)
            {
                
                if (is_QueueFull_tx)
                {
                    TriggerStopLine();
                }
                RunLog($"队列达到最大容量 {maxBatch}，拒绝入队");
                return;
            }

            // 1️⃣ 生成批次
            string batchId = Guid.NewGuid().ToString();
            newModel.BatchID = batchId;

            // 2️⃣ 查型号对应物料
            List<string> materials = GetMaterialIDandModel(newModel.Key_code);
            if (materials.Count == 0)
            {
                _currentBarcodeNotGetMes = true;
                RunLog("型号未配置物料");
                return;
            }

            int validMaterialCount = 0;

            // 3️⃣ DB 入队（物料级）
            foreach (var material in materials)
            {
                List<(int x, int y)> positionList = PickAllCellsForMaterial(material);

                foreach (var (x, y) in positionList)
                {
                    string plcAddr = PLCAddressPose(x, y, out int cellID);

                    if (string.IsNullOrEmpty(plcAddr) || cellID <= 0)
                    {
                        RunLog($"物料 {material} Pose X{x} Y{y} 未找到 PLC / CellID");
                        continue;
                    }

                    InsertQueueToDB(
                        batchId,
                        newModel.Key_code,
                        material,
                        cellID
                    );

                    validMaterialCount++;
                    RunLog($"入队：Batch={batchId} 物料={material} CellID={cellID}");
                }
            }

            if (validMaterialCount == 0)
            {
                RunLog("该批次未配置任何有效物料");
                return;
            }

            // 批次+
            Interlocked.Increment(ref _batchCount);
            RunLog($"批次入队：Batch={batchId}，物料数={validMaterialCount}，当前批次数={_batchCount}");
        }

        //private void ProcessQueueLogic(MyStruct newModel)
        //{

        //    int currentQueueSize = GetQueueCountFromDB();
        //    int currentBatchSzie = GetDB_BatchID_Count();
        //    if (currentQueueSize >= SystemConfig.MaxQueueSize)
        //    {
        //        RunLog("队列达到最大容量，拒绝入队");
        //        return;
        //    }

        //    if (currentBatchSzie >= 4)
        //    {
        //        TriggerStopLine();
        //    }

        //    // 1️⃣ 生成批次
        //    string batchId = Guid.NewGuid().ToString();
        //    newModel.BatchID = batchId;

        //    // 2️⃣ 查该型号对应的所有物料
        //    List<string> materials = GetMaterialIDandModel(newModel.Key_code);
        //    if (materials.Count == 0)
        //    {
        //        _currentBarcodeNotGetMes = true;
        //        RunLog("型号未配置物料");
        //        return;
        //    }

        //    // 3️⃣ 每个物料入一条队列数据  
        //    foreach (var material in materials)
        //    {
        //        List<(int x, int y)> positionList = PickAllCellsForMaterial(material);
        //        foreach (var (x, y) in positionList)
        //        {
        //            string plcAddr = PLCAddressPose(x, y, out int cellID);

        //            if (string.IsNullOrEmpty(plcAddr) || cellID <= 0)
        //            {
        //                RunLog($"物料 {material} Pose X{x} Y{y} 未找到 PLC / CellID");
        //                continue;
        //            }

        //            InsertQueueToDB(
        //                batchId,
        //                newModel.Key_code,
        //                material,
        //                cellID      
        //            );

        //            RunLog($"入队：Batch={batchId} 物料={material} CellID={cellID}");
        //        }
        //    }

        //    // 4️⃣ 内存队列只入「批次」
        //    materialQueue.Enqueue(newModel);

        //    RunLog($"型号 {newModel.Key_code} 入队，物料数={materials.Count}，Batch={batchId},内存队列：{materialQueue.Count}");

        //}


        private string _currentPickingKeyCode = null;


        public void MaterialTaken()
        {
            
            string batchId = GetCurrentBatchID(); // Status = 1

            if (string.IsNullOrEmpty(batchId))
            {
                RunLog("【异常】取料完成但未找到当前批次");
                return;
            }

            RunLog($"开始执行取料 BatchID={batchId}");

            // 1️⃣ 执行原有的 PLC 弹箱逻辑
            List<MyStruct> batchItems = GetBatchItems(batchId);
            if (batchItems.Count == 0)
            {
                RunLog($"【异常】批次 {batchId} 无物料");

            }
            string modelCode = batchItems[0].Key_code;
            List<string> materialList = GetMaterialIDandModel(modelCode);
            if (materialList.Count == 0)
            {
                RunLog($"【异常】型号 {modelCode} 未配置物料");
            }
            //弹箱播报：弹出哪个型号播报哪个
            SpeakPopup(modelCode);
            //允许部分成功
            bool hasAnySuccess = false;
            if (preList.Count > 0)
            {
                for (int i = 0; i < preList.Count; i++)
                {
                    WritePLC(preList[i], false);
                    RunLog(preList[i] + "已复位");
                    
                }
                preList.Clear();
            }
            foreach (var materialID in materialList)
            {
                List<(int x, int y)> positionList = PickAllCellsForMaterial(materialID);

                if (positionList.Count == 0)
                {
                    RunLog($"物料 {materialID} 未配置箱位，跳过");
                    continue;
                }

                foreach (var (x, y) in positionList)
                {
                    string plcAddr = PLCAddressPose(x, y, out int cellId);

                    if (string.IsNullOrEmpty(plcAddr))
                    {
                        RunLog($"物料 {materialID} Pose({x},{y}) 无 PLC 地址");
                        continue;
                    }
                    Task.Run(() =>
                    {
                        WritePLC(plcAddr, true);
                        Thread.Sleep(150);
                        UpdateHistory(cellId);
                        preList.Add(plcAddr);
                    });
                    hasAnySuccess = true;
                    RunLog($"弹箱成功：物料={materialID} CellID={cellId}");
                }
            }

            bool needDequeue = false;

            // 业务逻辑
            if (!hasAnySuccess)
            {
                RunLog("该批次下所有物料均未维护，批次异常结束");
                _currentBarcodeNotMaintained = true;
                needDequeue = true;
            }
            else
            {
                RunLog($"批次 {batchId} 取料完成");
                needDequeue = true;
            }
            // 2️⃣ DB 永远只处理一次
            UpdateQueueStatusByBatch(batchId, 2);
            DeleteFinishedQueueFromDB();

            // 3️⃣ 内存队列出批次
            Interlocked.Decrement(ref _batchCount);
            if (_batchCount < 0)
            {
                _batchCount = 0; // 兜底
            }

            RunLog($"批次出队完成，当前批次数={_batchCount}");

            // 4️⃣ 停线 / 复线判断
            if (_batchCount < 4)
            {
                ResumeProductionLine();
            }
            //_currentBatchFinished = true;
            //TrySendPlcRetractSignal();
            RunLog($"批次 {batchId} 取料完成，剩余批次={_batchCount}");
        }

        
        //系统重启，恢复队列
        private void RecoverQueueFromDB()
        {
            using (SqlConnection conn = new SqlConnection(Str_SqlCon))
            {
                conn.Open();

                // 1️⃣ 回滚异常中断的“取料中”批次
                new SqlCommand(
                    "UPDATE PickQueue SET Status = 0 WHERE Status = 1",
                    conn
                ).ExecuteNonQuery();

                // 3️⃣ 按 BatchID 恢复批次 + 物料数
                string sql = @"
                            SELECT COUNT(DISTINCT BatchID)
                            FROM PickQueue
                            WHERE Status = 0";

                int batchCount = (int)new SqlCommand(sql, conn).ExecuteScalar();

                // 3️⃣ 同步到内存计数器
                Interlocked.Exchange(ref _batchCount, batchCount);
            }

            RunLog($"系统重启完成，恢复待处理批次数：{_batchCount}");
        }

        //private void TrySendPlcRetractSignal()
        //{
        //    // 两个条件必须同时满足
        //    if (_currentBatchFinished && _nextBarcodeArrived)
        //    {
        //        RunLog("满足条件：取料完成 + 新条码到达，发送 PLC 收回信号");

        //        WritePLC("", true);
        //        Thread.Sleep(100);
        //        WritePLC("X点地址", false);

        //        // 复位状态
        //        _currentBatchFinished = false;
        //        _nextBarcodeArrived = false;
        //    }
        //}


        private void ResumeProductionLine()
        {

        }
        private void TriggerStopLine()
        {
            WritePLC("M155",true);
            RunLog("停线成功");
        }
        #endregion
 
        #region 光栅触发出队
        //等待出队的物料
        private MyStruct WaitingTakeItem;
        private bool rasterThreadRunning = false;
        private bool isTaking = false;
        //bool PreState = false;
        #endregion

        #region 读码控制
        // 当前“已弹出但未确认  
        private MyStruct? _physicalPendingItem = null;

        private int _physicalPendingQueueId = -1;
        public static Socket IDReader_Client = null;
        public static string AllCode = "";
        public static string LastCode = "";
        //public static string KeyCode = "";

        // 延迟一条数据开关：true=开启（先缓存当前码，处理上一个码）
        public bool b_DelayOneCode = false;
        private string _pendingDelayCode = null;   // 缓存的上一条码
        private readonly object delayLock = new object();

        private delegate void dgShowSNCode(string _str);
        private void ShowSNCode(string _str)
        {
            //lock (this)
            //{
            BeginInvoke(new MethodInvoker(delegate ()
            {

                if (_str != "NG")
                {
                    textBox1.Text = _str;
                    textBox1.ForeColor = Color.Green;
                }
                else
                {
                    textBox1.Text = _str;
                    textBox1.ForeColor = Color.Red;
                }
            }));
            //}
        }

        /// <summary>
        /// 创建客户端连接
        /// </summary>
        public void ConnectCheckIDReader()
        {
            try
            {
                IDReader_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse("192.168.2.60");
                IPEndPoint point = new IPEndPoint(ip, 51236);
                IDReader_Client.Connect(point);
                ThreadStart DM_Threaddelegate = new ThreadStart(CheckIDReaderServerData);//子线程绑定方法
                Thread DM_Thread = new Thread(DM_Threaddelegate);
                DM_Thread.IsBackground = true;
                DM_Thread.Start(); //子线程开启
                RunLog("检测端读码器连接成功");
            }
            catch (Exception ex)
            {
                RunLog("检测端读码器连接失败：" + ex.ToString());
                //throw;
            }
        }

        public string testcode = "";
        public void check12or17(string productcode, out string _md)
        {
            if (productcode != "NG")
            {
                if (productcode.Length < 7)
                {
                    _md = "NG";
                    return;
                }
                if (productcode.Substring(6, 1) == "Z")
                {
                    _md = productcode.Substring(0, 17);
                }
                else
                {
                    _md = productcode.Substring(0, 12);
                }
            }
            else
            {
                _md = "NG";
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        private volatile bool _currentBarcodeNotMaintained = false;
        private volatile bool _currentBarcodeNotGetMes = false;

        //private SpeechSynthesizer speak = new SpeechSynthesizer();

        /// <summary>
        /// 异步语音播报：在后台线程执行阻塞的 Speak，不阻塞主流程，
        /// 这样语音可以和弹箱等逻辑同时进行。
        /// </summary>
        private void SpeakAsync(string text)
        {
            Task.Run(() =>
            {
                try
                {
                    using (SpeechSynthesizer tempSpeak = new SpeechSynthesizer())
                    {
                        tempSpeak.Speak(text);
                    }
                }
                catch (Exception ex)
                {
                    RunLog("语音播报失败：" + ex.Message);
                }
            });
        }

        /// <summary>
        /// 顺序语音播报：在后台线程里依次播报多个文本，报完一个再报下一个，
        /// 避免多线程同时播报导致重叠。
        /// </summary>
        private void SpeakAsync1(IEnumerable<string> texts)
        {
            List<string> list = new List<string>(texts);
            Task.Run(() =>
            {
                try
                {
                    using (SpeechSynthesizer tempSpeak = new SpeechSynthesizer())
                    {
                        foreach (string text in list)
                        {
                            tempSpeak.Speak(text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    RunLog("语音播报失败：" + ex.Message);
                }
            });
        }

        /// <summary>
        /// 弹箱播报：保持原播报内容（17位码播报批号，12位码播报物料号），
        /// 改为在弹箱时触发，而不是扫码时触发。
        /// </summary>
        private void SpeakPopup(string keyCode)
        {
            if (string.IsNullOrEmpty(keyCode) || keyCode == "NG")
            {
                return;
            }
            if (keyCode.Length >= 7 && keyCode.Substring(6, 1) == "Z")
            {
                if (keyCode.Length >= 17)
                {
                    SpeakAsync(keyCode.Substring(13, 4));
                }
            }
            else
            {
                SpeakAsync1(GetMaterialIDandModel(keyCode));
            }
        }

        bool allMaterialsUnmaintained = true;
        List<string> unmaintainedMaterialList = new List<string>();

        ConcurrentQueue<(string address, bool value)> plcWriteQueue =new ConcurrentQueue<(string, bool)>();
        public void WritePLC1(string address, bool value)
        {
            plcWriteQueue.Enqueue((address, value));
        }
        public void CheckIDReaderServerData()  //数据接收
        {
            
            try
            {
                
                while (true)
                {
                    
                    int bufferSize = 1024;
                    string _str = string.Empty;
                    
                    byte[] receiveByteData = new byte[bufferSize];
                    string stringReceivedata = null;
                    int receiveServerDataLengh = IDReader_Client.Receive(receiveByteData);
                    if (receiveServerDataLengh <= 0)
                    {
                        RunLog("  ");
                        Thread.Sleep(200);
                        continue;
                    }
                    stringReceivedata = Encoding.UTF8.GetString(receiveByteData, 0, receiveServerDataLengh);
                    _str = stringReceivedata.Trim();

                    // ===== 延迟一条数据：开启时先缓存当前码，处理上一个码 =====
                    string rawCode = _str;   // 原始扫到的码，用于“正在读码”显示
                    if (b_DelayOneCode && _str.Length == 23)
                    {
                        bool skip = false;
                        lock (delayLock)
                        {
                            if (_str == _pendingDelayCode)
                            {
                                skip = true;          // 重复读码，忽略
                            }
                            else
                            {
                                string prev = _pendingDelayCode;
                                _pendingDelayCode = _str;   // 缓存当前码
                                if (string.IsNullOrEmpty(prev))
                                {
                                    skip = true;      // 第一条码，只缓存
                                }
                                else
                                {
                                    _str = prev;      // 换成上一个码，后续语音/弹箱全部跟随
                                    RunLog($"延迟一条：弹出上一个码 {prev}");
                                }
                            }
                        }
                        if (skip)
                        {
                            RunLog($"延迟一条：缓存 {rawCode}，等待下一条码");
                            dgShowSNCode dgShowSN0 = new dgShowSNCode(ShowSNCode);
                            BeginInvoke(dgShowSN0, rawCode);
                            continue;
                        }
                    }

                    if (_str.Length != 23)
                    {
                        
                        _str = "NG";

                        SpeakAsync("扫码失败");
                    }
                    else
                    {
                        _SN1 = _str;
                    }

                    //check12or17(_str,out string _md);
                   
                    // 光栅前状态

                    if (_str != "NG")
                    {
                        if (_str != LastCode)
                        {
                            
                            _notMaintainedMaterials.Clear();
                            allMaterialsUnmaintained = true;
                            unmaintainedMaterialList.Clear();
                            WritePLC1("M22",true);
                            LastCode = _str;
                            AllCode = _str;
                            RunLog("读码器读码：" + _str);
                            List<string> _materialID = GetMaterialIDandModel(AllCode);
                            if (_materialID.Count != 0)
                            {
                                _currentBarcodeNotGetMes = false;
                                //查询结果不为空，执行物料号查询Pose，
                                if (currentMode == WorkMode.Normal)
                                {
                                    MyStruct modelItem = new MyStruct
                                    {
                                        Key_code = AllCode
                                    };

                                    ProcessQueueLogic(modelItem);   // 生成 Batch + 入队
                                    RunLog($"Normal模式：型号 {AllCode} 已生成取料批次");
                                }
                                else if (currentMode == WorkMode.Queue)
                                {
                                    bool isQueueEmpty = false;
                                    MyStruct modelItem = new MyStruct
                                    {
                                        Key_code = AllCode
                                    };
                                    if (GetQueueCountFromDB() == 0)
                                    {
                                        isQueueEmpty = true;
                                        RunLog("队列为空");
                                    }
                                    else
                                    {
                                        RunLog("队列不为空");  
                                    }

                                    if (b_M125)
                                    {
                                        RunLog("待取料中");
                                    }
                                    else
                                    {
                                        RunLog("已取料");
                                    }

                                    if (isQueueEmpty && !b_M125&&b_X10)
                                    {
                                        RunLog("isQueueEmpty");
                                        bool hasAnyValidMaterial=false;
                                        // 队列为空 + PLC已取料，直接弹出，不入队
                                        if (preList.Count > 0)
                                        {
                                            for (int i = 0; i < preList.Count; i++)
                                            {
                                                WritePLC1(preList[i], false);
                                                RunLog(preList[i] + "已复位");
                                                
                                            }
                                            preList.Clear();
                                        }
                                        if (preList_sd.Count > 0)
                                        {
                                            for (int i = 0; i < preList_sd.Count; i++)
                                            {
                                                WritePLC1(preList_sd[i], false);
                                                RunLog(preList_sd[i] + "已复位");

                                            }
                                            preList_sd.Clear();
                                        }

                                        foreach (string wuliao in _materialID)
                                        {
                                            List<(int x, int y)> positionList = PickAllCellsForMaterial(wuliao);

                                            if (positionList.Count == 0)
                                            {
                                                unmaintainedMaterialList.Add(wuliao);

                                                if (!_notMaintainedMaterials.Contains(wuliao))
                                                {
                                                    _notMaintainedMaterials.Add(wuliao.Trim());
                                                }

                                                //SpeakAsync(wuliao+"未维护");
                                                
                                                RunLog($"物料 {wuliao} 未配置箱位");
                                            }
                                            else
                                            {
                                                // 只要有一个物料维护了，就不是全部未维护
                                                allMaterialsUnmaintained = false;
                                                hasAnyValidMaterial = true;
                                            }
                                        }

                                        // 根据检查结果处理
                                        if (allMaterialsUnmaintained)
                                        {
                                            // 所有物料都未维护，播报所有物料号
                                            string allMaterials = string.Join("、", unmaintainedMaterialList);
                                            //SpeakAsync(allMaterials + "未维护");
                                        }
                                        else
                                        {
                                            // 有物料维护了，只处理有维护的物料
                                            SpeakPopup(modelItem.Key_code);
                                            foreach (string wuliao in _materialID)
                                            {
                                                List<(int x, int y)> positionList = PickAllCellsForMaterial(wuliao);

                                                if (positionList.Count > 0) // 只处理有维护的物料
                                                {
                                                    foreach (var (x, y) in positionList)
                                                    {
                                                        string _plcaddress = PLCAddressPose(x, y, out int _cellID1);
                                                        if (_plcaddress != "")
                                                        {
                                                            WritePLC1(_plcaddress, true); // 打开对应仓位
                                                            
                                                            preList.Add(_plcaddress);
                                                            UpdateHistory(_cellID1);
                                                            BeginInvoke(new Action(() =>
                                                            {
                                                                textBox2.Text = (modelItem.Key_code + "位置" + _cellID1).Trim();
                                                            }));

                                                            RunLog("写入PLC地址：" + _plcaddress + "，物料号：" + wuliao + "，Pose：X" + x.ToString() + "Y" + y.ToString());
                                                        }
                                                        else
                                                        {
                                                            RunLog("根据Pose查询PLC地址失败，Pose：X" + x.ToString() + "Y" + y.ToString());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!hasAnyValidMaterial)
                                        {
                                            _currentBarcodeNotMaintained = true;
                                            RunLog("该型号下所有物料均未维护箱位，停线");
                                        }
                                        else
                                        {
                                            _currentBarcodeNotMaintained = false;
                                        }
                                        RunLog($"队列空 & 已取料，直接弹出：{modelItem.Key_code}");
                                    }
                                    else
                                    { 
                                        ProcessQueueLogic(modelItem);
                                        WaitingTakeItem = modelItem;

                                        RunLog($"Queue模式：型号 {AllCode} 已入队");
                                    }
                                    //_nextBarcodeArrived = true;
                                    //TrySendPlcRetractSignal();
                                }
                            }
                            else
                            {
                                _currentBarcodeNotGetMes = true;
                                //查询结果为空，1，从MES中获取物料号 2，写入数据库 3，根据物料号查询Pose，根据Pose查询PLC地址，写入PLC
                                RunLog("该型号未从Mes中获取");
                                //AddModelSQL(AllCode.Substring(0,12),null);
                            }

                        }
                        else
                        {
                            //RunLog("重复读码，忽略：" + _str);
                        }
                        dgShowSNCode dgShowSN = new dgShowSNCode(ShowSNCode);
                        BeginInvoke(dgShowSN, rawCode);
                    }
                    else
                    {
                        dgShowSNCode dgShowSN = new dgShowSNCode(ShowSNCode);
                        BeginInvoke(dgShowSN, _str);
                        if (is_ls_tx)
                        {
                            TriggerStopLine();
                        }
                        RunLog("读码器读码失败：" + _str);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
                //throw;
            }
            finally
            {
                AllCode = "";
               
            }
        }

        #endregion 读码控制

        #region PLC
        private bool _plcConnected = false;
        private readonly object plcLock = new object();
        public int ColumnIndex = 0;
        public int RowIndex = 0;
        DateTime lastPlcCommTime = DateTime.Now;

        public static string X4 = "X04";//手动与自动切换
        public static string X0 = "X0";
        public static string M27 = "M27";//防误触15仓光栅
        public static string M28 = "M28";//防误触6仓光栅
        public static string Y45 = "Y045";
        public static string M125 = "M125";//判断取没取料的点
        public static string X10 = "X010";//15仓光栅
        public static string X101 = "X0101";//6仓光栅
        public static string M90 = "M90";//挡光栅15秒
        public static string M155 = "M155";//停线

        public static bool b_X0 = false;
        public static bool b_M27 = false;
        public static bool b_M28 = false;
        public static bool b_X4 = false;
        public static bool b_M125 = false;
        public static bool b_X10 = false;
        public static bool b_X101 = false;
        public static bool b_M90 = false;
        public static bool b_M155 = false;

        public static bool _b_M27 = false;
        public static bool _b_M28 = false;
        public static bool _b_M125 = false;
        public static bool b_Y45 = false;

        public static MelsecMcNet ZZSJ_PLC = new MelsecMcNet();


        DateTime lastHeartbeatCheck = DateTime.Now;

        int heartbeatFailCount = 0;
        int readFailCount = 0;

        volatile bool _needClearCells;

        bool plcThreadStarted = false;
        public void ConnectPLC()
        {
            try
            {
                bool LoginHslSN = HslCommunication.Authorization.SetAuthorizationCode("891c8f18-d6de-409f-81f4-6de405431905");

                ZZSJ_PLC.IpAddress = "192.168.2.10";
                ZZSJ_PLC.Port = 6688;
                ZZSJ_PLC.ConnectTimeOut = 1000;
                ZZSJ_PLC.ReceiveTimeOut = 2000;  // 接收超时2s，防止假死时线程永久阻塞

                //ping主PLC
                Ping ping = new Ping();
                PingReply pingReply = ping.Send("192.168.2.10");
                if (pingReply.Status != IPStatus.Success)
                {
                    RunLog("PLC_1 Ping失败");
                    return;
                }

                OperateResult connect = ZZSJ_PLC.ConnectServer();
                if (connect.IsSuccess)
                {
                    //ZZSJ_PLC = true;
                    //Thread thread = new Thread(ScanPLC_1);
                    //thread.IsBackground = true;
                    //thread.Start();
                    _plcConnected = true;
                    RunLog("PLC_1连接成功");
                }
                else
                {
                    _plcConnected = false;
                    //ZZSJ_PLC = false;
                    RunLog("PLC_1连接失败");
                }
                

                if (!plcThreadStarted)
                {
                    plcThreadStarted = true;

                    Thread plcThread = new Thread(ReadPLC);
                    plcThread.IsBackground = true;
                    plcThread.Start();

                    Thread watchThread = new Thread(PLCWatchDog);
                    watchThread.IsBackground = true;
                    watchThread.Start();
                }

            }
            catch (Exception ex)
            {
                ErrLog(ex.ToString());
            }
        }
        private void ReconnectPLC()
        {
            lock (plcLock)
            {
                try
                {
                    RunLog("尝试重连PLC...");

                    try
                    {
                        ZZSJ_PLC?.ConnectClose();
                    }
                    catch { }

                    Thread.Sleep(300);
                    ZZSJ_PLC = new MelsecMcNet();
                    ZZSJ_PLC.IpAddress = "192.168.2.10";
                    ZZSJ_PLC.Port = 6688;
                    ZZSJ_PLC.ConnectTimeOut = 2000;
                    ZZSJ_PLC.ReceiveTimeOut = 2000;

                    OperateResult connect = ZZSJ_PLC.ConnectServer();

                    if (connect.IsSuccess)
                    {
                        RunLog("PLC重连成功");

                        _plcConnected = true;
                        heartbeatFailCount = 0;
                        readFailCount = 0;
                        lastHeartbeatCheck = DateTime.Now;
                        lastPlcCommTime = DateTime.Now;
                    }
                    else
                    {
                        RunLog($"PLC重连失败：{connect.Message}");
                        _plcConnected = false;
                    }
                }
                catch (Exception ex)
                {
                    RunLog($"PLC重连异常：{ex.Message}");
                    _plcConnected = false;
                }
            }
        }
        //private void ReconnectPLC()
        //{
        //    try
        //    {
        //        RunLog("尝试重连PLC...");

        //        // 先关闭旧连接
        //        ZZSJ_PLC?.ConnectClose();
        //        ZZSJ_PLC.IpAddress = "192.168.2.10";
        //        ZZSJ_PLC.Port = 6688;
        //        ZZSJ_PLC.ConnectTimeOut = 1000;
        //        ZZSJ_PLC.ReceiveTimeOut = 2000;  // 接收超时2s，防止假死时线程永久阻塞

        //        //ping主PLC
        //        Ping ping = new Ping();
        //        PingReply pingReply = ping.Send("192.168.2.10");
        //        if (pingReply.Status != IPStatus.Success)
        //        {
        //            RunLog("PLC_1 Ping失败");
        //            return;
        //        }
        //        // 重新连接
        //        OperateResult connect = ZZSJ_PLC.ConnectServer();
        //        if (connect.IsSuccess)
        //        {
        //            RunLog("PLC重连成功");
        //            _plcConnected = true;
        //            lastHeartbeatCheck = DateTime.Now;
        //        }
        //        else
        //        {
        //            RunLog($"PLC重连失败：{connect.Message}");
        //            _plcConnected = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RunLog($"重连异常：{ex.Message}");
        //    }
        //}



        public void ReadPLC()
        {

            while (true)
            {

                try
                {
                    while (plcWriteQueue.TryDequeue(out var cmd))
                    {
                        try
                        {
                            if (!_plcConnected)
                                continue;
                            var result = ZZSJ_PLC.Write(cmd.address, cmd.value);

                            if (result.IsSuccess)
                            {
                                RunLog($"PLC写入 {cmd.address}={cmd.value}");
                                lastPlcCommTime = DateTime.Now;
                            }
                            else
                            {
                                RunLog($"PLC写入失败: {result.Message}");
                                _plcConnected = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            RunLog("PLC写入异常：" + ex.Message);
                            _plcConnected = false;
                        }
                    }
                    if (!_plcConnected)
                    {
                        RunLog("检测到PLC断开，准备重连...");
                        ReconnectPLC();
                        Thread.Sleep(3000);
                        continue;
                    }

                    // 心跳检测
                    if ((DateTime.Now - lastHeartbeatCheck).TotalSeconds >= 10)
                    {
                        var heartbeat = ZZSJ_PLC.ReadBool("X04");

                        if (!heartbeat.IsSuccess)
                        {
                            heartbeatFailCount++;

                            RunLog($"PLC心跳失败 {heartbeatFailCount}/3");

                            if (heartbeatFailCount >= 3)
                            {
                                RunLog("PLC心跳连续失败，判定断线");

                                ZZSJ_PLC?.ConnectClose();
                                _plcConnected = false;
                            }
                        }
                        else
                        {
                            heartbeatFailCount = 0;
                            lastPlcCommTime = DateTime.Now;
                        }

                        lastHeartbeatCheck = DateTime.Now;
                    }

                    // 逐个读取PLC
                    b_X0 = ZZSJ_PLC.ReadBool(X0).Content;
                    b_M27 = ZZSJ_PLC.ReadBool(M27).Content;
                    b_M28 = ZZSJ_PLC.ReadBool(M28).Content;
                    b_X4 = ZZSJ_PLC.ReadBool(X4).Content;
                    b_Y45 = ZZSJ_PLC.ReadBool(Y45).Content;
                    b_M125 = ZZSJ_PLC.ReadBool(M125).Content;
                    b_X10 = ZZSJ_PLC.ReadBool(X10).Content;
                    b_X101 = ZZSJ_PLC.ReadBool(X101).Content;
                    b_M90 = ZZSJ_PLC.ReadBool(M90).Content;
                    b_M155 = ZZSJ_PLC.ReadBool(M155).Content;
                    //var rX = ZZSJ_PLC.ReadBool("X3", 120);
                    //var rM = ZZSJ_PLC.ReadBool("M0", 160);
                    //var rY = ZZSJ_PLC.ReadBool("Y0", 50);

                    //if (!rX.IsSuccess || !rM.IsSuccess || !rY.IsSuccess)
                    //{
                    //    _plcConnected = false;
                    //    continue;
                    //}

                    //bool[] x = rX.Content;
                    //bool[] m = rM.Content;
                    //bool[] y = rY.Content;

                    //b_X0 = x[0];
                    //b_X4 = x[4];
                    //b_X10 = x[8];
                    //b_X101 = x[65];

                    //b_M27 = m[27];
                    //b_M28 = m[28];
                    //b_M125 = m[125];
                    //b_M90 = m[90];
                    //b_M155 = m[155];

                    //b_Y45 = y[45];

                    lastPlcCommTime = DateTime.Now;
                    _plcConnected = true;

                    // 箱子缩回检测
                    if (_b_M125 == false && b_M125 == true)
                    {
                        _needClearCells = true;
                        RunLog("PLC反馈：全部箱子已缩回");
                    }

                    _b_M125 = b_M125;
                    if (b_X4)
                    {
                        if (currentMode == WorkMode.Queue)
                        {
                            if ((_b_M27 != b_M27) || (_b_M28 != b_M28))
                            {
                                if ((!_b_M27 && b_M27) || (!_b_M28 && b_M28))
                                {
                                    // 上升沿：完成
                                    if (isTaking)
                                    {
                                        //BeginInvoke(new dgshowcolor(showcolor));
                                        MaterialTaken();
                                        isTaking = false;
                                    }
                                }
                                else if ((_b_M27 && !b_M27) || (_b_M28 && !b_M28))
                                {
                                    // 下降沿：开始
                                    if (!isTaking)
                                    {
                                        MyStruct model = new MyStruct
                                        {
                                            Key_code = AllCode
                                        };
                                        //BeginInvoke(new dgshowcolor(showcolor));
                                        string batchId = GetNextWaitingBatchID();
                                        if (!string.IsNullOrEmpty(batchId))
                                        {
                                            UpdateQueueStatusByBatch(batchId, 1);
                                            WaitingTakeItem = model;
                                            isTaking = true;

                                            RunLog($"开始取料：{model.Key_code}");
                                        }
                                    }
                                }

                                _b_M27 = b_M27;
                                _b_M28 = b_M28;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    RunLog($"PLC读取异常：{ex.Message}");

                    try
                    {
                        ZZSJ_PLC?.ConnectClose();
                    }
                    catch { }

                    _plcConnected = false;

                    Thread.Sleep(2000);
                }
                
                Thread.Sleep(200);

            }
        }
        //public void ReadPLC()
        //{

        //    while (true)
        //    {

        //        try
        //        {
        //            if (!_plcConnected)
        //            {
        //                RunLog("检测到PLC连接断开，尝试重连...");
        //                ReconnectPLC();
        //                Thread.Sleep(3000);
        //                continue;
        //            }
        //            if ((DateTime.Now - lastHeartbeatCheck).TotalSeconds >= 30)
        //            {
        //                var heartbeatTest = ZZSJ_PLC.ReadBool("X04"); // 读取一个不常变的点
        //                if (!heartbeatTest.IsSuccess)
        //                {
        //                    RunLog("心跳检测失败，连接可能已断开");
        //                    ZZSJ_PLC?.ConnectClose();
        //                    _plcConnected = false;
        //                    Thread.Sleep(1000);
        //                    continue;
        //                }
        //                lastHeartbeatCheck = DateTime.Now;
        //            }
        //            b_X0 = ZZSJ_PLC.ReadBool(X0).Content;
        //            b_M27 = ZZSJ_PLC.ReadBool(M27).Content;
        //            b_M28 = ZZSJ_PLC.ReadBool(M28).Content;
        //            b_X4 = ZZSJ_PLC.ReadBool(X4).Content;
        //            b_Y45 = ZZSJ_PLC.ReadBool(Y45).Content;
        //            b_M30 = ZZSJ_PLC.ReadBool(M30).Content;
        //            b_X10 = ZZSJ_PLC.ReadBool(X10).Content;
        //            b_X101 = ZZSJ_PLC.ReadBool(X101).Content;
        //            b_M90 = ZZSJ_PLC.ReadBool(M90).Content;
        //            b_M155= ZZSJ_PLC.ReadBool(M155).Content;
        //            if (_b_M30 == false && b_M30 == true)
        //            {
        //                _needClearCells = true;
        //                RunLog("PLC反馈：全部箱子已缩回");

        //            }
        //            _b_M30 = b_M30;
        //            if (b_X4)
        //            {
        //                if (currentMode == WorkMode.Queue)
        //                {
        //                    if ((_b_M27 != b_M27) || (_b_M28 != b_M28))
        //                    {
        //                        if ((!_b_M27 && b_M27) || (!_b_M28 && b_M28))
        //                        {
        //                            // 上升沿：完成
        //                            if (isTaking)
        //                            {
        //                                BeginInvoke(new dgshowcolor(showcolor));
        //                                MaterialTaken();
        //                                isTaking = false;
        //                            }
        //                        }
        //                        else if ((_b_M27 && !b_M27) || (_b_M28 && !b_M28))
        //                        {
        //                            // 下降沿：开始
        //                            if (!isTaking)
        //                            {
        //                                MyStruct model = new MyStruct
        //                                {
        //                                    Key_code = AllCode
        //                                };
        //                                BeginInvoke(new dgshowcolor(showcolor));
        //                                string batchId = GetNextWaitingBatchID();
        //                                if (!string.IsNullOrEmpty(batchId))
        //                                {
        //                                    UpdateQueueStatusByBatch(batchId, 1);
        //                                    WaitingTakeItem = model;
        //                                    isTaking = true;

        //                                    RunLog($"开始取料：{model.Key_code}");
        //                                }
        //                            }
        //                        }

        //                        _b_M27 = b_M27;
        //                        _b_M28 = b_M28;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            RunLog($"PLC读取异常：{ex.Message}");
        //            // 标记连接断开，触发重连
        //            ZZSJ_PLC?.ConnectClose();
        //            _plcConnected = false;
        //            Thread.Sleep(2000);
        //        }
        //        Thread.Sleep(100);
        //    }

        //}
        public void PLCWatchDog()
        {
            while (true)
            {
                try
                {
                    double seconds = (DateTime.Now - lastPlcCommTime).TotalSeconds;

                    if (seconds > 15)
                    {
                        RunLog("PLC通讯超时，可能线程卡死，强制断开连接");

                        try
                        {
                            ZZSJ_PLC?.ConnectClose();
                        }
                        catch { }

                        _plcConnected = false;

                        lastPlcCommTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    RunLog("PLC WatchDog异常：" + ex.Message);
                }
                
                Thread.Sleep(5000);
            }
        }
        public void WritePLC(string _address, bool _value)
        {
            
            try
            {
                lock (plcLock)
                {
                    OperateResult result = ZZSJ_PLC.Write(_address, _value);
                    if (result.IsSuccess)
                    {
                        RunLog("写入PLC：" + _address);
                        lastPlcCommTime = DateTime.Now;
                        _plcConnected = true;
                    }
                    else
                    {
                        _plcConnected = false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                ErrLog(ex.ToString());
                _plcConnected = false;
                //throw;
            }
        }

        #endregion

        #region MES获取生产编码

        /////*
        //// * 获取
        ////    {
        ////    "plant_code": "6372",
        ////    "AREA_TIME": "6372-PA",
        ////    "START_TIME": "2025-07-01 08:00:00",
        ////    "END_TIME": "2025-09-05 18:00:00 "
        ////    }
        //// * 
        //// * 
        //// */


        /////*
        //// * 回复
        //// * {
        ////     "Result": [
        ////                 {
        ////                  "SERIAL_NUMBER": "1L02020040100084JP10231",
        ////                  "Final_Product_Code": "1L0202004010",
        ////                  "SALES_TYPE": "N",
        ////                  "STATUS": "10"，
        ////                  "COUNTRY":"印度"，
        ////             "packing_method":"塑封"
        ////    },
        ////                 {
        ////                "SERIAL_NUMBER": "1B0260Z0064JB6QH1P10126",
        ////                "Final_Product_Code": "1B0260Z0064JB6QH1",
        ////                "SALES_TYPE": "W",
        ////                 "STATUS": "10"，
        ////                 "COUNTRY":"印度"，
        ////           "packing_method":"塑封"
        ////                  }
        ////                ],
        ////    "code": 0,
        ////    "EVENT_TYPE": "S"
        ////    "alert": 0,
        ////    "msg": "查询成功!"
        ////    }
        //// * 
        //// */



        //string MES_GetSerialNumber_URL = "http://pddimessage-pdmes.pdmesprd.hisense.com/api/EqProduct/GetWorkOrderReq";

        ////MES获取生产编码接口
        //public class MesGetSNRequest
        //{
        //    public string plant_code { get; set; }
        //    public string AREA_TIME { get; set; }
        //    public string START_TIME { get; set; }
        //    public string END_TIME { get; set; }
        //}

        ////MES获取生产编码回复
        //public class MesGetSNResponse
        //{
        //    public string Result { get; set; }
        //    public int code { get; set; }
        //    public string EVENT_TYPE { get; set; }
        //    public int alert { get; set; }
        //    public string msg { get; set; }

        //    public List<MesSNResult> data { get; set; }
        //}

        ////MES获取生产编码结果
        //public class MesSNResult
        //{
        //    public string SERIAL_NUMBER { get; set; }
        //    public string Final_Product_Code { get; set; }
        //    public string SALES_TYPE { get; set; }
        //    public string STATUS { get; set; }
        //    public string COUNTRY { get; set; }
        //    public string packing_method { get; set; }
        //    public string MaterialID { get; set; }
        //}

        ////MES获取生产编码并更新到数据库
        //public bool GetMESSerialNumber()
        //{
        //    try
        //    {
        //        MesGetSNRequest _mesRequest = new MesGetSNRequest();
        //        _mesRequest.plant_code = "6370";
        //        _mesRequest.AREA_TIME = "6370-BZ-P5";
        //        _mesRequest.START_TIME = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss");
        //        _mesRequest.END_TIME = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
        //        string jsonData = JsonConvert.SerializeObject(_mesRequest);
        //        MESLog("MES获取生产编码请求数据：" + jsonData);
        //        using (HttpClient client = new HttpClient())
        //        {
        //            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        //            HttpResponseMessage response = client.PostAsync(MES_GetSerialNumber_URL, content).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                string responseContent = response.Content.ReadAsStringAsync().Result;
        //                MESLog("MES获取生产编码返回数据：" + responseContent);
        //                MesGetSNResponse _mesResponse = JsonConvert.DeserializeObject<MesGetSNResponse>(responseContent);
        //                if (_mesResponse.code == 0 && _mesResponse.data != null)
        //                {
        //                    foreach (var item in _mesResponse.data)
        //                    {
        //                        //更新数据库操作
        //                        if (UpdateSerialNumberToDatabase(
        //                            item.SERIAL_NUMBER,
        //                            item.Final_Product_Code,
        //                            item.SALES_TYPE,
        //                            item.STATUS,
        //                            item.COUNTRY,
        //                            item.packing_method

        //                            ))
        //                        {
        //                            MESLog("MES生产编码更新数据库成功，生产编码：" + item.SERIAL_NUMBER);
        //                        }
        //                        else
        //                        {
        //                            MESLog("MES生产编码更新数据库失败，生产编码：" + item.SERIAL_NUMBER);
        //                        }

        //                    }
        //                    return true;
        //                }
        //                else
        //                {
        //                    Log("MES获取生产编码失败，返回码：" + _mesResponse.code.ToString());
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                Log("MES获取生产编码请求失败，状态码：" + response.StatusCode.ToString());
        //                return false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("MES获取生产编码异常：" + ex.Message);
        //        return false;
        //    }
        //}


        ////Sqlserver数据库更新生产编码，库里有就更新，没有就插入
        //public bool UpdateSerialNumberToDatabase(string _serialNumber, string _model, string _salesType, string _status, string _country, string _packingMethod)
        //{
        //    SqlConnection conn = new SqlConnection(Str_SqlCon);
        //    try
        //    {

        //        conn.Open();
        //        string query = "IF EXISTS (SELECT 1 FROM ProductionCodes WHERE SERIAL_NUMBER = @SERIAL_NUMBER) " +
        //                        "BEGIN " +
        //                        "UPDATE ProductionCodes SET Final_Product_Code = @Final_Product_Code, SALES_TYPE = @SALES_TYPE, STATUS = @STATUS " +
        //                        "END " +
        //                        "ELSE " +
        //                        "BEGIN " +
        //                        "INSERT INTO ProductionCodes (SERIAL_NUMBER, Final_Product_Code, SALES_TYPE, STATUS) VALUES (@SERIAL_NUMBER, @Final_Product_Code, @SALES_TYPE, @STATUS) " +
        //                        "END";
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@SERIAL_NUMBER", _serialNumber);
        //            cmd.Parameters.AddWithValue("@Final_Product_Code", _model);
        //            cmd.Parameters.AddWithValue("@SALES_TYPE", _salesType);
        //            cmd.Parameters.AddWithValue("@STATUS", _status);
        //            //cmd.Parameters.AddWithValue("@MaterialID", _materialID);
        //            //cmd.Parameters.AddWithValue("@COUNTRY", _country);
        //            //cmd.Parameters.AddWithValue("@packing_method", _packingMethod);
        //            cmd.ExecuteNonQuery();
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("更新生产编码到数据库失败：" + ex.ToString());
        //        return false;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        ////创建MES获取生产编码定时器
        //public Timer timer_GetMESSN;
        //public void CreateTimer_GetMESSN()
        //{
        //    timer_GetMESSN = new Timer();
        //    timer_GetMESSN.Interval = 60 * 60 * 1000; //5分钟获取一次
        //    timer_GetMESSN.Tick += new EventHandler(timer_GetMESSN_Tick);
        //    timer_GetMESSN.Start();
        //}

        ////定时获取MES生产编码
        //private void timer_GetMESSN_Tick(object sender, EventArgs e)
        //{
        //    GetMESSerialNumber();
        //}

        #endregion

        #region MES获取物料

        /*
         * 获取物料号
         * {
                   "StartTime": "2023-02-28",
       	    "EndTime": "2023-02-28",
        	    "LineBody": "ZZ-P2",
        	    "MaterialDescripts": [
        	        "制冷剂",
        	        "工艺管"
	                ]

            }
         * 
         * 
         */

        /*
         * 回复
         * {
                "Result": [
                    {
                        "FinalDefID": "VB0265Z0304JBBZFR",
                        "ProductType": "BCD-265DJ1NA1A/HC4(EAD2)",
                        "JiaJuZu": "BBZFR",
                        "MaterialID": "1092068",
                        "MaterialDescript": "制冷剂\\R600a",
                        "RequiredQty": "78.000"
                    },
                    {
                        "FinalDefID": "VB0265Z0304JBBZFR",
                        "ProductType": "BCD-265DJ1NA1A/HC4(EAD2)",
                        "JiaJuZu": "BBZFR",
                        "MaterialID": "1639201",
                        "MaterialDescript": "工艺管-B02071005-铜管!4.75mm!115mm",
                        "RequiredQty": "1.000"
                    }
                ],
                "Success": true,
                "ReturnMsg": ""
            }

         * 
         */

        public static string MES_GetMaterialCode_URL = "http://172.16.153.41:8080/api/FCC/GetWorkOrderBOMInfo";

        //MES获取物料号接口
        public class MesGetMaterialRequest
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string LineBody { get; set; }
            public List<string> MaterialDescripts { get; set; }
        }

        //MES获取物料号回复
        public class MesGetMaterialResponse
        {
            public List<MesMaterialResult> Result { get; set; }
            public bool Success { get; set; }
            public string ReturnMsg { get; set; }
        }

        //MES获取物料号结果
        public class MesMaterialResult
        {
            public string FinalDefID { get; set; }
            public string ProductType { get; set; }
            public string JiaJuZu { get; set; }
            public string MaterialID { get; set; }
            public string MaterialDescript { get; set; }
            public string RequiredQty { get; set; }
        }

        //MES获取物料号并存入数据库，有就更新，没有就插入
        public bool GetMESMaterialCode()
        {
            try
            {
                int _docount = 0;
                MesGetMaterialRequest _mesRequest = new MesGetMaterialRequest();
                DateTime bt = DateTime.Now.AddDays(-3);
                for (int j = 0; j < 6;j++)
                {
                    DateTime st = bt.AddDays(j);
                    DateTime et = bt.AddDays(j+1);//DateTime.Now.AddDays(3);
                   
                    MESLog(st.ToString()+","+et.ToString());
                    _mesRequest.StartTime = st.ToString("yyyy-MM-dd");
                    _mesRequest.EndTime = et.ToString("yyyy-MM-dd");
                    _mesRequest.LineBody = "6370-BZ-P5";
                    _mesRequest.MaterialDescripts = new List<string> { "说明书", "电子说明书", "订单说明书", "使用说明书" };
                    string jsonData = JsonConvert.SerializeObject(_mesRequest);
                    MESLog("MES获取物料号请求数据：" + jsonData);
                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = client.PostAsync(MES_GetMaterialCode_URL, content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = response.Content.ReadAsStringAsync().Result;
                            MESLog("MES获取物料号返回数据：" + responseContent);

                            MesGetMaterialResponse _mesResponse = JsonConvert.DeserializeObject<MesGetMaterialResponse>(responseContent);
                            if (_mesResponse.Success && _mesResponse.Result != null)
                            {
                                int i = 0;
                                foreach (var item in _mesResponse.Result)
                                {

                                    if (UpdateMaterialCodeToDatabase(
                                    item.ProductType,
                                    item.FinalDefID,
                                    item.MaterialID,
                                    item.MaterialDescript,
                                    item.RequiredQty
                                    ))
                                    {
                                        i++;
                                        MESLog("MES物料号更新数据库成功，物料号：" + item.MaterialID + "," + "型号：" + item.FinalDefID + ",物料号数量：" + i.ToString());
                                        
                                    }
                                    else
                                    {
                                        MESLog("MES物料号更新数据库失败，物料号：" + item.MaterialID);
                                        return false;
                                    }
                                    
                                    //Thread.Sleep(10);
                                    //更新数据库操作

                                }
                                

                            }
                            else
                            {
                                Log("MES获取物料号失败，返回信息：" + _mesResponse.ReturnMsg);
                                return false;
                            }
                             
                        }
                        else
                        {
                            Log("MES获取物料号请求失败，状态码：" + response.StatusCode.ToString());
                            return false;
                        }
                    }
                }
                
               // _mesRequest.EndTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
               return true ;
               
            }
            catch (Exception ex)
            {
                Log("MES获取物料号异常：" + ex.ToString());
                return false;
            }
        }

        //Sqlserver数据库更新物料号，库里有就更新，没有就插入
        public bool UpdateMaterialCodeToDatabase(string _productID, string _model, string _materialID, string _materialDescript, string _requiredQty)
        {
            SqlConnection conn = new SqlConnection(Str_SqlCon);
            try
            {
                conn.Open();
                string query = "IF EXISTS (SELECT * FROM ModelList WHERE Model=@Model AND MaterialID = @MaterialID) " +
                               "BEGIN " +
                               "UPDATE ModelList SET  Model=@Model,MaterialID=@MaterialID,MaterialDescript = @MaterialDescript,RequiredQty = @RequiredQty WHERE Model=@Model AND MaterialID = @MaterialID " +
                               "END " +
                               "ELSE " +
                               "BEGIN " +
                               "INSERT INTO ModelList (ProductID,Model,MaterialID, MaterialDescript, RequiredQty) VALUES (@ProductID,@Model,@MaterialID, @MaterialDescript, @RequiredQty) " +
                               "END";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", _productID);
                    cmd.Parameters.AddWithValue("@Model", _model);
                    cmd.Parameters.AddWithValue("@MaterialID", _materialID);
                    cmd.Parameters.AddWithValue("@MaterialDescript", _materialDescript);
                    cmd.Parameters.AddWithValue("@RequiredQty", _requiredQty);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log("更新物料号到数据库失败：" + ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        //public bool GetMaterialCodeFromLocalJson(string jsonFilePath)
        //{
        //    try
        //    {
        //        if (!File.Exists(jsonFilePath))
        //        {
        //            Log("本地JSON文件不存在：" + jsonFilePath);
        //            return false;
        //        }

        //        string jsonContent = File.ReadAllText(jsonFilePath, Encoding.UTF8);
        //        MESLog("本地JSON读取内容：" + jsonContent);

        //        MesGetMaterialResponse _mesResponse =JsonConvert.DeserializeObject<MesGetMaterialResponse>(jsonContent);

        //        if (_mesResponse == null || !_mesResponse.Success || _mesResponse.Result == null)
        //        {
        //            Log("本地JSON解析失败或数据无效");
        //            return false;
        //        }

        //        int successCount = 0;

        //        foreach (var item in _mesResponse.Result)
        //        {
        //            bool result = UpdateMaterialCodeToDatabase(
        //                item.ProductType,
        //                item.FinalDefID,
        //                item.MaterialID,
        //                item.MaterialDescript,
        //                item.RequiredQty
        //            );

        //            if (result)
        //            {
        //                successCount++;
        //            }
        //            else
        //            {
        //                Log("写入数据库失败，物料号：" + item.MaterialID);
        //            }
        //        }

        //        Log($"本地JSON写入完成，成功 {successCount}/{_mesResponse.Result.Count}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("从本地JSON写数据库异常：" + ex);
        //        return false;
        //    }
        //}

        //跨天时获取MES物料号
        public Timer timer_GetMESMaterial;
        public void CreateTimer_GetMESMaterial()
        {
            timer_GetMESMaterial = new Timer();
            timer_GetMESMaterial.Interval = 12*60 * 60 * 1000; //每12小时检测一次是否跨天
            timer_GetMESMaterial.Tick += new EventHandler(timer_GetMESMaterial_Tick);
            timer_GetMESMaterial.Start();

        }
        private DateTime lastMESMaterialDate = DateTime.Now.Date;
        //定时检测是否跨天获取MES物料号
        private void timer_GetMESMaterial_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                GetMESMaterialCode();
            });
        }

        #endregion

        #region 设备状态上传

        public static string STATUS_URL = "http://172.16.153.73:8011/api/EqProduct/ReceiveEquipmentStatueAndAnalysis?user_key=lbv6wxnjr4n7h9kpejkehsb2gznvt5ke";

        public class StatusMessageHeader
        {
            public string MESSAGE_ID { get; set; }
            public string SOURCE_SYSTEM { get; set; }
            public string MESSAGE_TYPE { get; set; }
            public string MESSAGE_TIME { get; set; }
            public RunningStatusContent CONTENT { get; set; }
        }


        public class RunningStatusContent
        {
            public string STATION_CODE { get; set; }
            public string EQUIPMENT_CODE { get; set; }
            public string EQUIPMENT_NAME { get; set; }
            public string EQUIPMENT_IP { get; set; }
            public string MAC_INFO { get; set; }
            public string RUNNING_STATUS { get; set; }
        }

        public StatusMessageHeader CreateStatusMessage()
        {
            StatusMessageHeader _statusheader = new StatusMessageHeader();
            _statusheader.MESSAGE_ID = Guid.NewGuid().ToString();
            _statusheader.SOURCE_SYSTEM = "EQUIPMENT";
            _statusheader.MESSAGE_TYPE = "SAVE_RUNNING_STATUS";
            _statusheader.MESSAGE_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            RunningStatusContent _runningstatus = new RunningStatusContent();
            _runningstatus.STATION_CODE = "P5-ShuoMingShuLiangDengQuLiao";
            _runningstatus.EQUIPMENT_CODE = "ShuoMingShuLiangDengQuLiao";
            _runningstatus.EQUIPMENT_NAME = "说明书亮灯取料";
            _runningstatus.EQUIPMENT_IP = "10.35.174.120";
            _runningstatus.MAC_INFO = "";
            _runningstatus.RUNNING_STATUS = "R";

            _statusheader.CONTENT = _runningstatus;

            return _statusheader;
        }

        public void TimerSend()
        {
            try
            {
                Thread _thread = new Thread(UploadMESStatus);
                _thread.IsBackground = true;
                _thread.Start();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                //throw;
            }
        }

        public void UploadMESStatus()
        {
            while (true)
            {
                try
                {
                    StatusMessageHeader _statusheader = CreateStatusMessage();

                    string jsonData = JsonConvert.SerializeObject(_statusheader);
                    MESLog("MES上传数据：" + jsonData);
                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = client.PostAsync(STATUS_URL, content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = response.Content.ReadAsStringAsync().Result;
                            Log("MES上传成功，返回数据：" + responseContent);

                        }
                        else
                        {
                            Log("MES上传失败，状态码：" + response.StatusCode.ToString());

                        }
                    }
                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    MESLog("MES上传异常：" + ex.Message);

                }

            }
        }



        #endregion

        #region 手持扫码枪

        private static SerialPort CheckCodeReaderSerialPort = new SerialPort();
        public static string CheckCodeReaderSerialPort_PortName = "COM1";
        public static string CheckCodeReaderSerialPort_BaudRate = "9600";
        public static string CheckCodeReaderSerialPort_DataBits = "8";
        public static string CheckCodeReaderSerialPort_Parity = "None";
        public static string CheckCodeReaderSerialPort_StopBits = "One";

        public static string CheckCodeReaderSerialPort_AllCodeStr = "";
        public static string CheckCodeReaderSerialPort_LastCodeStr = "";

        public static string CheckCode_12_Str = "";
        public static string CheckCode_17_Str = "";

        private bool OpenCheckCodeReaderSerialPort()
        {
            try
            {
                if (CheckCodeReaderSerialPort.IsOpen)
                {
                    CheckCodeReaderSerialPort.Close();
                }
                CheckCodeReaderSerialPort.PortName = CheckCodeReaderSerialPort_PortName;
                CheckCodeReaderSerialPort.BaudRate = int.Parse(CheckCodeReaderSerialPort_BaudRate);
                CheckCodeReaderSerialPort.DataBits = int.Parse(CheckCodeReaderSerialPort_DataBits);
                CheckCodeReaderSerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), CheckCodeReaderSerialPort_Parity);
                CheckCodeReaderSerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), CheckCodeReaderSerialPort_StopBits);
                CheckCodeReaderSerialPort.DataReceived += CheckCodeReaderSerialPort_DataReceived;
                CheckCodeReaderSerialPort.Open();
                RunLog("手持扫码枪串口连接成功 --->" + CheckCodeReaderSerialPort_PortName);
                return true;
            }
            catch (Exception ex)
            {
                RunLog("手持扫码枪串口连接失败：" + ex.ToString());
                return false;
                //throw;
            }
        }

        private void CheckCodeReaderSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(100);
                string _str = CheckCodeReaderSerialPort.ReadExisting().Trim();
                if (_str != "")
                {
                   
                    if (_str.Length != 23)
                    {

                        _str = "NG";
                        SpeakAsync("扫码失败");
                    }
                    else
                    {
                        _SN1 = _str;
                    }
                    if (_str != "NG")
                    {
                        if (_str != LastCode)
                        {

                            _notMaintainedMaterials.Clear();
                            allMaterialsUnmaintained = true;
                            unmaintainedMaterialList.Clear();
                            WritePLC1("M22", true);
                            LastCode = _str;
                            AllCode = _str;
                            RunLog("读码器读码：" + _str);
                            List<string> _materialID = GetMaterialIDandModel(AllCode);
                            if (_materialID.Count != 0)
                            {
                                _currentBarcodeNotGetMes = false;
                                //查询结果不为空，执行物料号查询Pose，
                                if (currentMode == WorkMode.Normal)
                                {
                                    MyStruct modelItem = new MyStruct
                                    {
                                        Key_code = AllCode
                                    };

                                    ProcessQueueLogic(modelItem);   // 生成 Batch + 入队
                                    RunLog($"Normal模式：型号 {AllCode} 已生成取料批次");
                                }
                                else if (currentMode == WorkMode.Queue)
                                {
                                    bool isQueueEmpty = false;
                                    MyStruct modelItem = new MyStruct
                                    {
                                        Key_code = AllCode
                                    };
                                    if (GetQueueCountFromDB() == 0)
                                    {
                                        isQueueEmpty = true;
                                        RunLog("队列为空");
                                    }
                                    else
                                    {
                                        RunLog("队列不为空");
                                    }

                                    if (b_M125)
                                    {
                                        RunLog("待取料中");
                                    }
                                    else
                                    {
                                        RunLog("已取料");
                                    }

                                    if (isQueueEmpty && !b_M125 && b_X10)
                                    {
                                        RunLog("isQueueEmpty");
                                        bool hasAnyValidMaterial = false;
                                        // 队列为空 + PLC已取料，直接弹出，不入队
                                        if (preList.Count > 0)
                                        {
                                            for (int i = 0; i < preList.Count; i++)
                                            {
                                                WritePLC1(preList[i], false);
                                                RunLog(preList[i] + "已复位");

                                            }
                                            preList.Clear();
                                        }
                                        if (preList_sd.Count > 0)
                                        {
                                            for (int i = 0; i < preList_sd.Count; i++)
                                            {
                                                WritePLC1(preList_sd[i], false);
                                                RunLog(preList_sd[i] + "已复位");

                                            }
                                            preList_sd.Clear();
                                        }

                                        foreach (string wuliao in _materialID)
                                        {
                                            List<(int x, int y)> positionList = PickAllCellsForMaterial(wuliao);

                                            if (positionList.Count == 0)
                                            {
                                                unmaintainedMaterialList.Add(wuliao);

                                                if (!_notMaintainedMaterials.Contains(wuliao))
                                                {
                                                    _notMaintainedMaterials.Add(wuliao.Trim());
                                                }
                                                SpeakAsync(wuliao + "未维护");
                                                
                                                RunLog($"物料 {wuliao} 未配置箱位");
                                            }
                                            else
                                            {
                                                // 只要有一个物料维护了，就不是全部未维护
                                                allMaterialsUnmaintained = false;
                                                hasAnyValidMaterial = true;
                                            }
                                        }

                                        // 根据检查结果处理
                                        if (allMaterialsUnmaintained)
                                        {
                                            // 所有物料都未维护，播报所有物料号
                                            string allMaterials = string.Join("、", unmaintainedMaterialList);

                                            SpeakAsync(allMaterials + "未维护");

                                        }
                                        else
                                        {
                                            // 有物料维护了，只处理有维护的物料
                                            SpeakPopup(modelItem.Key_code);
                                            foreach (string wuliao in _materialID)
                                            {
                                                List<(int x, int y)> positionList = PickAllCellsForMaterial(wuliao);

                                                if (positionList.Count > 0) // 只处理有维护的物料
                                                {
                                                    foreach (var (x, y) in positionList)
                                                    {
                                                        string _plcaddress = PLCAddressPose(x, y, out int _cellID1);
                                                        if (_plcaddress != "")
                                                        {
                                                            WritePLC1(_plcaddress, true); // 打开对应仓位

                                                            preList.Add(_plcaddress);
                                                            UpdateHistory(_cellID1);
                                                            BeginInvoke(new Action(() =>
                                                            {
                                                                textBox2.Text = (modelItem.Key_code + "位置" + _cellID1).Trim();
                                                            }));

                                                            RunLog("写入PLC地址：" + _plcaddress + "，物料号：" + wuliao + "，Pose：X" + x.ToString() + "Y" + y.ToString());
                                                        }
                                                        else
                                                        {
                                                            RunLog("根据Pose查询PLC地址失败，Pose：X" + x.ToString() + "Y" + y.ToString());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!hasAnyValidMaterial)
                                        {
                                            _currentBarcodeNotMaintained = true;
                                            RunLog("该型号下所有物料均未维护箱位，停线");
                                        }
                                        else
                                        {
                                            _currentBarcodeNotMaintained = false;
                                        }
                                        RunLog($"队列空 & 已取料，直接弹出：{modelItem.Key_code}");
                                    }
                                    else
                                    {
                                        ProcessQueueLogic(modelItem);
                                        WaitingTakeItem = modelItem;

                                        RunLog($"Queue模式：型号 {AllCode} 已入队");
                                    }
                                    //_nextBarcodeArrived = true;
                                    //TrySendPlcRetractSignal();
                                }
                            }
                            else
                            {
                                _currentBarcodeNotGetMes = true;
                                //查询结果为空，1，从MES中获取物料号 2，写入数据库 3，根据物料号查询Pose，根据Pose查询PLC地址，写入PLC
                                RunLog("该型号未从Mes中获取");
                                //AddModelSQL(AllCode.Substring(0,12),null);
                            }

                        }
                        else
                        {
                            //RunLog("重复读码，忽略：" + _str);
                        }
                        dgShowSNCode dgShowSN = new dgShowSNCode(ShowSNCode);
                        BeginInvoke(dgShowSN, _str);
                    }
                    else
                    {
                        dgShowSNCode dgShowSN = new dgShowSNCode(ShowSNCode);
                        BeginInvoke(dgShowSN, _str);
                        if (is_ls_tx)
                        {
                            TriggerStopLine();
                        }
                        RunLog("手持扫码枪读码失败：" + _str);
                    }
                }
                else
                {
                    CheckCodeReaderSerialPort_AllCodeStr = "";
                    RunLog("手持扫码枪读码失败：空字符串");
                }
            }
            catch (Exception ex)
            {
                RunLog("手持扫码枪数据接收异常：" + ex.ToString());
                //throw;
            }
        }



        #endregion

        public Timer timer_RefreshGrid;
        public void CreateTimer_RefreshGrid()
        {
            timer_RefreshGrid = new Timer();
            timer_RefreshGrid.Interval = 3000;
            timer_RefreshGrid.Tick += new EventHandler(timer_RefreshGrid_Tick);
            timer_RefreshGrid.Start();

        }
        private bool _refreshing = false;
        private DateTime _lastClearDate = DateTime.MinValue;

        private void CheckShiftClear()
        {
            DateTime now = DateTime.Now;

            // 白班 7:45 清零
            if (now.Hour == 7 && now.Minute == 45)
            {
                if (_lastClearDate.Date != now.Date || _lastClearDate.Hour != 8)
                {
                    ClearHistoryCount();
                    _lastClearDate = now;
                }
            }

            // 夜班 19:45 清零
            if (now.Hour == 19 && now.Minute == 45)
            {
                if (_lastClearDate.Date != now.Date || _lastClearDate.Hour != 20)
                {
                    ClearHistoryCount();
                    _lastClearDate = now;
                }
            }
        }
        private void timer_RefreshGrid_Tick(object sender, EventArgs e)
        {
            if (_refreshing) return;

            _refreshing = true;
            
            Task.Run(() =>
            {
                try
                {
                    
                    // 后台取数据
                    var cache = new (string material, int qty)[3, 7];
                    var cache3 = new (string material, int qty)[10];
                    var cache3CellId = new int[10];
                    var historyDict = GetAllHistoryCount();
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            GetMaterialPose(i, j, out string materialID, out int qty);
                            cache[i, j] = (materialID, qty);
                        }
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        GetMaterialPose(Grid3CellX, j, out string materialID, out int qty);
                        cache3[j] = (materialID, qty);
                        PLCAddressPose(Grid3CellX, j, out int cellId);
                        cache3CellId[j] = cellId;
                    }

                    // 回到 UI 线程更新界面
                    this.BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 7; j++)
                            {
                                var item = cache[i, j]; 
                                int _id = (j * 3 + i + 1);
                                int count = historyDict.ContainsKey(_id)
                                ? historyDict[_id]
                                : 0;
                                if (!string.IsNullOrEmpty(item.material))
                                {
                                    dataGridView1.Rows[i].Cells[j].Value =
                                        $"{_id}\r\n{item.material}\r\n{count}";
                                }
                                else
                                {
                                    dataGridView1.Rows[i].Cells[j].Value = "";
                                }
                            }
                        }

                        // dataGridView3：10列×1行（CellX = Grid3CellX）
                        if (dataGridView3.Rows.Count == 0)
                        {
                            datagridviewbutton3();
                        }
                        for (int j = 0; j < 10; j++)
                        {
                            var item = cache3[j];
                            int cellId = cache3CellId[j];
                            int count = (cellId > 0 && historyDict.ContainsKey(cellId)) ? historyDict[cellId] : 0;
                            if (!string.IsNullOrEmpty(item.material))
                            {
                                dataGridView3.Rows[0].Cells[j].Value =
                                    $"{cellId}\r\n{item.material}\r\n{count}";
                            }
                            else
                            {
                                dataGridView3.Rows[0].Cells[j].Value = "";
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    RunLog(ex.ToString());
                }
                finally
                {
                    _refreshing = false;
                }
            });
            RefreshQueueGrid_Async();
            CheckShiftClear();
        }
        private bool _refreshingQueue = false;
        private void RefreshQueueGrid_Async()
        {
            if (_refreshingQueue) return;
            _refreshingQueue = true;

            Task.Run(() =>
            {
                try
                {
                    // 后台取数据
                    DataTable dt = GetPickQueue_ModelAndMaterial();

                    // 回 UI 线程更新
                    this.BeginInvoke(new Action(() =>
                    {
                        dataGridView2.AutoGenerateColumns = false;
                        dataGridView2.DataSource = dt;

                        dataGridView2.AllowUserToAddRows = false;
                        dataGridView2.RowHeadersVisible = false;
                        dataGridView2.ReadOnly = true;
                        dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }));
                }
                catch (Exception ex)
                {
                    RunLog(ex.ToString());
                }
                finally
                {
                    _refreshingQueue = false;
                }
            });
        }
        //21个控件设计
        public bool datagridviewbutton()
        {
            try
            {
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();


                for (int i = 0; i < 7; i++)
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.Name = "col" + i.ToString();
                    col.ReadOnly = true;               // 不可编辑
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    col.Resizable = DataGridViewTriState.False;
                    dataGridView1.Columns.Add(col);
                    //dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[i].Width = dataGridView1.Width / 7;
                }


                for (int i = 0; i < 3; i++)
                {
                    dataGridView1.Rows.Add();

                    dataGridView1.Rows[i].Height = dataGridView1.Height / 3 - 1;
                    dataGridView1.Rows[i].Resizable = DataGridViewTriState.False;

                }


                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.ColumnHeadersVisible = false;
                dataGridView1.RowHeadersVisible = false;

                dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.MultiSelect = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
                dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;

                //dataGridView1.DefaultCellStyle.Font = new Font("微软雅黑", 11, FontStyle.Bold);
                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        GetMaterialPose(i, j, out string materialID, out int qty);

                        if (!string.IsNullOrEmpty(materialID))
                        {
                            int _id = (j * 3 + i + 1);
                            var historyDict = GetAllHistoryCount();

                            int position = j * 3 + i + 1;
                            int count = historyDict.ContainsKey(position)
                                        ? historyDict[position]
                                        : 0;

                            dataGridView1.Rows[i].Cells[j].Value =
                                $"{position}\r\n{materialID}\r\n{count}";


                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[j].Value = "";
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
                return false;
            }
        }

        // dataGridView3：10列×1行（与 dataGridView1 同交互/显示风格）
        private const int Grid3CellX = 3;
        public bool datagridviewbutton3()
        {
            try
            {
                dataGridView3.Columns.Clear();
                dataGridView3.Rows.Clear();

                for (int i = 0; i < 10; i++)
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.Name = "col3_" + i.ToString();
                    col.ReadOnly = true;
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    col.Resizable = DataGridViewTriState.False;
                    dataGridView3.Columns.Add(col);
                    dataGridView3.Columns[i].Width = dataGridView3.Width / 10;
                }

                dataGridView3.Rows.Add();
                dataGridView3.Rows[0].Height = dataGridView3.Height - 1;
                dataGridView3.Rows[0].Resizable = DataGridViewTriState.False;

                dataGridView3.AllowUserToAddRows = false;
                dataGridView3.AllowUserToResizeColumns = false;
                dataGridView3.AllowUserToResizeRows = false;
                dataGridView3.ColumnHeadersVisible = false;
                dataGridView3.RowHeadersVisible = false;

                dataGridView3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView3.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView3.MultiSelect = false;
                dataGridView3.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dataGridView3.DefaultCellStyle.SelectionBackColor = Color.White;
                dataGridView3.DefaultCellStyle.SelectionForeColor = dataGridView3.DefaultCellStyle.ForeColor;

                var historyDict = GetAllHistoryCount();
                for (int j = 0; j < 10; j++)
                {
                    GetMaterialPose(Grid3CellX, j, out string materialID, out int qty);
                    PLCAddressPose(Grid3CellX, j, out int cellId);
                    int count = (cellId > 0 && historyDict.ContainsKey(cellId)) ? historyDict[cellId] : 0;

                    if (!string.IsNullOrEmpty(materialID))
                    {
                        dataGridView3.Rows[0].Cells[j].Value =
                            $"{cellId}\r\n{materialID}\r\n{count}";
                    }
                    else
                    {
                        dataGridView3.Rows[0].Cells[j].Value = "";
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
                return false;
            }
        }



        //21个控件鼠标点击事件
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //int _cellID = 0;
            try
            {
                if (!b_X4)
                {
                    ColumnIndex = e.ColumnIndex;
                    RowIndex = e.RowIndex;
                    string cellText = dataGridView1.Rows[RowIndex].Cells[ColumnIndex].Value?.ToString();
                    if (e.Button == MouseButtons.Left)
                    {
                        
                        Task.Run(() =>
                          {
                              WritePLC(PLCAddressPose(RowIndex, ColumnIndex, out int _cellID1), true);
                              Thread.Sleep(300);
                              WritePLC(PLCAddressPose(RowIndex, ColumnIndex, out int _cellID2), false);
                          });
                    }
                    else
                    {
                        PLCAddressPose(RowIndex, ColumnIndex, out int _cellID1);
                        AddMaterialID addMaterialID = new AddMaterialID(_cellID1, RowIndex, ColumnIndex);
                        addMaterialID.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
                //throw;
            }
        }

        private void dataGridView3_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (!b_X4)
                {
                    int columnIndex = e.ColumnIndex;
                    int rowIndex = e.RowIndex;
                    if (rowIndex < 0 || columnIndex < 0) return;

                    if (e.Button == MouseButtons.Left)
                    {
                        Task.Run(() =>
                        {
                            WritePLC(PLCAddressPose(Grid3CellX, columnIndex, out int _cellID1), true);
                            Thread.Sleep(300);
                            WritePLC(PLCAddressPose(Grid3CellX, columnIndex, out int _cellID2), false);
                        });
                    }
                    else
                    {
                        PLCAddressPose(Grid3CellX, columnIndex, out int _cellID1);
                        AddMaterialID addMaterialID = new AddMaterialID(_cellID1, Grid3CellX, columnIndex);
                        addMaterialID.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                RunLog(ex.ToString());
            }
        }
        

        private void bt_login_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.ShowDialog();
        }



        private void button4_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                GetMESMaterialCode();
                //GetMaterialCodeFromLocalJson(@"C:\Users\12717\Desktop\response2.json");
            });

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MesShow._Mes_P == null || MesShow._Mes_P.IsDisposed)
            {
                MesShow mes = new MesShow();
                mes.Show();
            }
            else
            {
                MesShow._Mes_P.Activate();       // 拉到最前
                MesShow._Mes_P.WindowState = FormWindowState.Normal;
            }
            MesShow._Mes_P.RefreshMedGrid_Async();
            //MesShow mesShow = new MesShow();
            //mesShow.ShowDialog();
            // GetMESSerialNumber();
        }

        //private void listBox1_MouseClick(object sender, MouseEventArgs e)
        //{
        //    string _str = listBox1.SelectedItem.ToString();
        //    //if (e.KeyCode == Keys.Enter)
        //    //{
        //        RunLog("收到扫码枪条码：" + _str);
        //        if (!b_X4)
        //        {//手动

        //            AllCode = _str;
        //            GetMaterialIDandModel(AllCode, out string _materialID);
        //            if (_materialID != "")
        //            {
        //                //查询结果不为空，执行物料号查询Pose，
        //                GetMaterialIDandPLCAddress(_materialID, out int _x, out int _y);
        //                if (_x >= 0 && _y >= 0)
        //                {
        //                    //根据Pose查询PLC地址，写入PLC
        //                    string _plcaddress = PLCAddressPose(_x, _y);
        //                    if (_plcaddress != "")
        //                    {
        //                        WritePLC(_plcaddress, true);//打开对应仓位
        //                        Thread.Sleep(300);
        //                        WritePLC(_plcaddress, false);
        //                        RunLog("写入PLC地址：" + _plcaddress + "，物料号：" + _materialID);
        //                        textBox2.Text = _materialID;
        //                    }
        //                    else
        //                    {
        //                        RunLog("根据Pose查询PLC地址失败，Pose：X" + _x.ToString() + "Y" + _y.ToString());
        //                    }
        //                    textBox1.Text = null;
        //                }
        //            }
        //            else
        //            {
        //                //查询结果为空，1，从MES中获取物料号 2，写入数据库 3，根据物料号查询Pose，根据Pose查询PLC地址，写入PLC
        //                //AddModelSQL(AllCode.Substring(0, 12), null);
        //            }

        //        }
        //        else
        //        {
        //            //自动
        //            MyStruct model = new MyStruct();
        //            model.Key_code = _str.Substring(0, 12);
        //            GetMaterialIDandModel(model.Key_code, out string _materialID);
        //            if (_materialID != "")
        //            {
        //                //查询结果不为空，执行物料号查询Pose，
        //                GetMaterialIDandPLCAddress(_materialID, out int _x, out int _y);
        //                model.Address = $"{_x}_{_y}";
        //                if (_x >= 0 && _y >= 0)
        //                {
        //                    //根据Pose查询PLC地址，写入PLC
        //                    string _plcaddress = PLCAddressPose(_x, _y);
        //                    if (_plcaddress != "")
        //                    {
        //                        ProcessQueueLogic(model);
        //                        RunLog("写入PLC地址：" + _plcaddress + "，物料号：" + _materialID);
        //                        textBox2.Text = _materialID;
        //                    }
        //                    else
        //                    {
        //                        RunLog("根据Pose查询PLC地址失败，Pose：X" + _x.ToString() + "Y" + _y.ToString());
        //                    }
        //                    textBox1.Text = null;
        //                }
        //            }
        //            else
        //            {
        //                //查询结果为空，1，从MES中获取物料号 2，写入数据库 3，写入队列
        //                //AddModelSQL(AllCode.Substring(0, 12), null);
        //                ProcessQueueLogic(model);
        //            }
        //        //}
        //    }
        //}

        private List<string> _notMaintainedMaterials = new List<string>();
        private void timer_UI_Tick(object sender, EventArgs e)
        {
            try
            {
                //显示扫到的条码
                //if (_SN1=="NG")
                //{
                //    textBox1.Text = "NG";
                //    textBox1.ForeColor= Color.Red;
                //}
                //else 
                //{
                //    textBox1.Text=_SN1;
                //    textBox1.ForeColor= Color.Green;
                //}
                if (!isTaking)
                {
                    textBox2.ForeColor = Color.Green;
                }
                else
                {
                    textBox2.ForeColor= Color.Red;
                }

                if (b_WorkMode)
                {
                    currentMode = WorkMode.Queue;
                    //RestoreQueueState();
                }
                else
                {
                    currentMode = WorkMode.Normal;

                }
                if (b_ls_tx)
                {
                    is_ls_tx = true;
                }
                else
                {
                    is_ls_tx = false;
                }
                if (b_QueueFull_tx)
                {
                    is_QueueFull_tx=true;
                }
                else
                {
                    is_QueueFull_tx = false;
                }
                //GetMaterialIDandPLCAddress(_materialID, out int _x, out int _y, out int _cellID);
                //警告
                if (currentMode != WorkMode.Queue)
                {
                    lb_Alarm.Text = "当前未使用队列";
                    lb_Alarm.ForeColor = Color.OrangeRed;
                }
                // 队列已满
                else if (_batchCount >= SystemConfig.MaxQueueSize)
                {
                    RunLog($"【配置加载】QueueCount={_batchCount}，MaxQueueSize={SystemConfig.MaxQueueSize}");
                    lb_Alarm.Text = "队列已满";
                    lb_Alarm.ForeColor = Color.Red;
                }
                else if (_SN1 == "NG")
                {
                    lb_Alarm.Text = "扫码失败";
                    lb_Alarm.ForeColor = Color.Red;
                }
                else if (currentMode == WorkMode.Queue && _notMaintainedMaterials.Count>0)
                {
                    
                    lb_Alarm.Text ="物料" + _notMaintainedMaterials[0] + "未维护";
                    
                    lb_Alarm.ForeColor = Color.Red;
                }
                else if (currentMode == WorkMode.Queue && _currentBarcodeNotGetMes)
                {
                    lb_Alarm.Text = "型号:" + _mo + "未从Mes获取";
                    lb_Alarm.ForeColor = Color.Red;
                }
                else if (!b_X10)
                {
                    lb_Alarm.Text = "光栅遮挡";
                    lb_Alarm.ForeColor = Color.Red;
                }
                //else if (b_M90)
                //{
                //    lb_Alarm.Text = "光栅长时间遮挡";
                //    lb_Alarm.ForeColor = Color.Red;
                //}
                else
                {
                    lb_Alarm.Text = "";
                    // 正常状态
                    //lb_Alarm.Text = "正常";
                    //lb_Alarm.ForeColor = Color.Green;
                }

                if (b_X4)
                {
                    label4.Text = "自动";
                    label4.ForeColor = Color.Green;
                }
                else
                {
                    label4.Text = "手动";
                    label4.ForeColor = Color.Red;
                }

                if (this.lb_Alarm.Text == "")
                {
                    //ZZSJ_PLC_1.Write("M905", false);
                }
                else if (this.lb_Alarm.Text == "队列已满")
                {
                    //ZZSJ_PLC_1.Write("M905", true);
                }



            }
            catch (Exception ex)
            {
                ErrLog(ex.ToString());
                //throw;
            }  
        }

        

        private void MainForm_Resize(object sender, EventArgs e)
        {
            datagridviewbutton();
            datagridviewbutton3();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tUserLevel == "2")
            {
                ControlForm form = new ControlForm();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("权限不足，请登录!");
            }

        }

        private void bt_logout_Click(object sender, EventArgs e)
        {
            tUserLevel = "-1";
        }

        private void bt_delayOne_Click(object sender, EventArgs e)
        {
            b_DelayOneCode = !b_DelayOneCode;
            lock (delayLock)
            {
                _pendingDelayCode = null;   // 切换时清空缓存，避免残留
            }
            if (b_DelayOneCode)
            {
                bt_delayOne.Text = "延迟一条：开";
                bt_delayOne.BackColor = Color.LimeGreen;
                RunLog("延迟一条：已开启");
            }
            else
            {
                bt_delayOne.Text = "延迟一条：关";
                bt_delayOne.BackColor = Color.Gray;
                RunLog("延迟一条：已关闭");
            }
        }


        //private void button2_Click(object sender, EventArgs e)
        //{
        //    if (_physicalPendingItem == null || _physicalPendingQueueId < 0)
        //    {
        //        RunLog("无可回溯物料");
        //        return;
        //    }

        //    // 回溯数据库状态
        //    UpdateQueueStatus(_physicalPendingQueueId, 0);

        //    // 把物料插回队首（内存）
        //    var list = materialQueue.ToList();
        //    materialQueue.Clear();
        //    materialQueue.Enqueue(_physicalPendingItem.Value);
        //    foreach (var it in list)
        //        materialQueue.Enqueue(it);

        //    RunLog($"误触回溯：{_physicalPendingItem.Value.Key_code}");

        //    _physicalPendingItem = null;
        //    _physicalPendingQueueId = -1;
        //}
        

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_KeyDown_1(object sender, KeyEventArgs e)
        {
            string _str = textBox3.Text.Trim();

            if (e.KeyCode == Keys.Enter)
            {
                SpeechSynthesizer speak = new SpeechSynthesizer();
                speak.Rate = 0;
                WritePLC("M22", true);
                RunLog("收到扫码枪条码：" + _str);
                _notMaintainedMaterials.Clear();
                allMaterialsUnmaintained = true;
                unmaintainedMaterialList.Clear();
                List<string> wuliaoDate = GetMaterialIDandModel(_str);
                if (_str == "NG")
                {

                    speak.SpeakAsync("扫码失败");
                }
                if (wuliaoDate.Count != 0)
                {
                    _currentBarcodeNotGetMes = false;
                    //查询结果不为空，执行物料号查询Pose，
                    if (currentMode == WorkMode.Normal)
                    {
                        MyStruct modelItem = new MyStruct
                        {
                            Key_code = _str
                        };

                        ProcessQueueLogic(modelItem);   // 生成 Batch + 入队
                        RunLog($"Normal模式：型号 {_str} 已生成取料批次");
                    }
                    else if (currentMode == WorkMode.Queue)
                    {
                        bool isQueueEmpty = false;
                        MyStruct modelItem = new MyStruct
                        {
                            Key_code = _str
                        };
                        if (GetQueueCountFromDB() == 0)
                        {
                            isQueueEmpty = true;
                            RunLog("队列为空");
                        }
                        else
                        {
                            RunLog("队列不为空");
                        }

                        if (b_M125)
                        {
                            RunLog("待取料中");
                        }
                        else
                        {
                            RunLog("已取料");
                        }

                        if (isQueueEmpty && !b_M125)
                        {
                            bool hasAnyValidMaterial = false;
                            if (preList.Count > 0)
                            {
                                for (int i = 0; i < preList.Count; i++)
                                {
                                    WritePLC(preList[i], false);
                                    RunLog(preList[i] + "已复位");

                                }
                                preList.Clear();
                            }
                            if (preList_sd.Count > 0)
                            {
                                for (int i = 0; i < preList_sd.Count; i++)
                                {
                                    WritePLC(preList_sd[i], false);
                                    RunLog(preList_sd[i] + "已复位");
                                }
                                preList_sd.Clear();
                            }
                            // 队列为空 + PLC已取料，直接弹出，不入队
                            foreach (string wuliao in wuliaoDate)
                            {
                                List<(int x, int y)> positionList = PickAllCellsForMaterial(wuliao);

                                if (positionList.Count == 0)
                                {
                                    unmaintainedMaterialList.Add(wuliao);

                                    if (!_notMaintainedMaterials.Contains(wuliao))
                                    {
                                        _notMaintainedMaterials.Add(wuliao.Trim());
                                    }
                                    RunLog($"物料 {wuliao} 未配置箱位");
                                }
                                else
                                {
                                    // 只要有一个物料维护了，就不是全部未维护
                                    allMaterialsUnmaintained = false;
                                    hasAnyValidMaterial = true;
                                }
                            }

                            // 根据检查结果处理
                            if (allMaterialsUnmaintained)
                            {
                                // 所有物料都未维护，播报所有物料号
                                string allMaterials = string.Join("、", unmaintainedMaterialList);
                                speak.SpeakAsync(allMaterials + "未维护");
                            }
                            else
                            {
                                // 有物料维护了，只处理有维护的物料
                                SpeakPopup(modelItem.Key_code);
                                foreach (string wuliao in wuliaoDate)
                                {
                                    List<(int x, int y)> positionList = PickAllCellsForMaterial(wuliao);

                                    if (positionList.Count > 0) // 只处理有维护的物料
                                    {
                                        foreach (var (x, y) in positionList)
                                        {
                                            string _plcaddress = PLCAddressPose(x, y, out int _cellID1);
                                            if (_plcaddress != "")
                                            {
                                                WritePLC(_plcaddress, true); // 打开对应仓位
                                                Thread.Sleep(300);
                                                UpdateHistory(_cellID1);
                                                preList.Add(_plcaddress);

                                                BeginInvoke(new Action(() =>
                                                {
                                                    textBox2.Text = (modelItem.Key_code + "位置" + _cellID1).Trim();
                                                }));

                                                RunLog("写入PLC地址：" + _plcaddress + "，物料号：" + wuliao + "，Pose：X" + x.ToString() + "Y" + y.ToString());
                                            }
                                            else
                                            {
                                                RunLog("根据Pose查询PLC地址失败，Pose：X" + x.ToString() + "Y" + y.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            if (!hasAnyValidMaterial)
                            {
                                _currentBarcodeNotMaintained = true;
                                RunLog("该型号下所有物料均未维护箱位，停线");
                            }
                            else
                            {
                                _currentBarcodeNotMaintained = false;
                            }
                            RunLog($"队列空 & 已取料，直接弹出：{modelItem.Key_code}");
                        }
                        else
                        {
                            ProcessQueueLogic(modelItem);
                            WaitingTakeItem = modelItem;

                            RunLog($"Queue模式：型号 {AllCode} 已入队");
                        }
                    }
                }
                else
                {
                    _currentBarcodeNotGetMes = true;
                    RunLog("该型号未从Mes中获取");
                }
                textBox3.Clear();
                //textBox3.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HistorySelect form = new HistorySelect();
            form.ShowDialog();
        }

    }

}
