using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Renci.SshNet;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
//using ZZSJ_SmartVisionForHisense;
///using static ZZSJ_SmartVisionForHisense.ReEditResultForm;
using System.IO;
using Newtonsoft.Json.Linq;

//接口说明
//## 3. 检测记录上传接口

//采用Http接口传输，信智平台提供Restful接口，传输内容为json格式。**如果判断NG，则可以考虑人工确认后再汇总数据统一上传，就无需调用第二个复判接口。
//**
//### 3.1 接口地址
//https://inner-apisix.hisense.com/hiaiApi/aiinterface/aiPredict/saveAiData?user_key=7wudcczim7gtktlaj0tlvw8mdu9c3bra
//### 3.2 接口输入字段

//| 字段名称 | 字段类型 | 是否必填 | 字段说明 |
//| -------------------| ------------| ------| ---------------------------------------------------------------------------------------------------------------|
//| baseName | String | Y | 园区：PD(平度)， HD(黄岛)，GY(贵阳)，MXG(墨西哥)，JM(江门)，等等 |
//| companyNo | String | Y | 产品公司编号，6370(冰箱平度), 2000(海信视像科技)，6210(空调平度) |
//| lineNo | String | Y | 产线，E, BW(B外)，FW(F外)，AN(A内)，CW(C外)                                                                             |
//| sceneNo           | String     | Y    | 场景，**传固定编码**，BX_ZKB(冰箱主控板)，KT_WJWGJ(空调外机外观检)，SX_HMJ(视像画面检)，SX_JNJ(视像机内检)， RL_DBD(日立打包带)，RL_QMX日立气密性，KT_PM空调喷码 |
//| dataType          | String     | Y    | 数据格式,  01图片，02 视频，03 语音                                                                                       |
//| productType       | String     | N    | 机型(产品型号)                                                                                                      |
//| productNo         | String     | Y    | 产品编码，唯一确定一个产品                                                                                                 |
//| bom               | String     | N    | bom                                                                                                           |
//| checkDate         | String     | Y    | 检测时间 yyyy-MM-dd hh:mm: ss |
//| totalResult | String | Y | 检测结果，OK， NG，ERROR                                                                                             |
//| **confirmDate**   | **String** | N    | **复判时间（可在后面接口单独传）, yyyy-MM-dd hh:mm: ss * *                                                                      |
//| **confirmResult * * | **String * * | N | **复判结果（可在后面接口单独传），OK， NG，或者根据业务扩展(过检：GJ，返流：FL，无配方：WPF等等)**                                                    |
//| **modifiedBy**    | **String** |      | **复判人员（可在后面接口单独传）**                                                                                           |
//| **data**          | **[] * *     |      | **明细数据，具体图片视频信息 * *                                                                                             |
//| fileName | String |      | 文件名称, 按规则命名：线体+相机（区分哪个相机拍的）+时间戳+产品编码.（jpg, mp4）                                                                |
//| fileType          | String     |      | 文件类型, 左图，右图，前图，后图，整图，子图，图1，图2，...等 （相机（区分哪个相机拍的））                                                             |
//| filePath          | String     |      | 文件地址，不同场景按规划写；                                                                                                |
//| **TemplateName**  | **String** |      | **若有，对应模板名称，按规则命名：产品型号 + 模板名字.（jpg, mp4）**                                                                    |
//| **TemplatePath**  | **String** |      | **若有：对应模板地址，不同场景按规划写；**                                                                                       |
//| checkResult       | String     |      | 检测结果, OK， NG，ERROR                                                                                            |
//| **confirmDate**   | **String** |      | **复判时间（可在后面接口单独传）, yyyy-MM-dd hh:mm: ss * *                                                                      |
//| **confirmResult * * | **String * * |      | **复判结果（可在后面接口单独传），OK， NG**                                                                                    |
//| items             | String     |      | **检测项坐标，具体如下json格式**                                                                                          |


//### 3.3 输入参数示例

//[
//  {
//    "baseName": "PD",
//    "companyNo": "6370",
//    "lineNo": "E",
//    "sceneNo": "BX_ZKB",
//    "dataType": "01",
//    "productType": "50A53FUA(0011)",
//    "productNo": "1KK035137W00D0",
//    "bom": "1",
//    "checkDate": "2023-11-29 14:29:28",
//    "totalResult": "NG",
//    "confirmResult": "NG",
//    "confirmDate": "2023-11-29 14:29:28",
//    "data":
//    [
//      {
//        "fileName": "E_Img1_20231129141924_1KK035137W00D0A68PF7462.jpg.jpg",
//        "fileType": "图1",
//        "filePath": "/mnt/aps/prd/predict/customdata/image/fridge/BX_ZKB/20231129/",
//        "checkResult": "NG",
//        "confirmResult": "NG",
//        "items":
//        [
//          {
//            "elementType": "螺钉与电容",
//            "localNumber": "正面检测",
//            "algorithmResult": "OK",
//            "rejudgeResult": "OK",
//          }
//        ],
//        [
//          {
//            "elementType": "螺钉与电容",
//            "localNumber": "正面检测",
//            "algorithmResult": "OK",
//            "rejudgeResult": "OK",
//          }
//        ]
//      }
//    ]
//  }
//]

//### 3.4 输出参数示例

//{
//  "ts": 1700629645738,
//  "data": 1,
//  //本次插入的主表条数
//  "code": "0",
//  "msg": "操作成功",
//  "alert": "0"
//}


namespace ErrorProofing
{
    class HisenseAI_Server
    {

        #region 日志

        public void MESLog(string Logtxt)
        {
            string Logtime = DateTime.Now.ToString() + "   ";
            try
            {
                //dgSetText dg = new dgSetText(UpdateText);
                //dg.Invoke(System.DateTime.Now.ToString() + ":" + Logtxt + "\r");
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
            finally
            {
                //
            }
        }

        public void MESERRLog(string Logtxt)
        {
            string Logtime = DateTime.Now.ToString() + "   ";
            try
            {
                //dgSetText dg = new dgSetText(UpdateText);
                //dg.Invoke(System.DateTime.Now.ToString() + ":" + Logtxt + "\r");
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "MESERRLog\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "MESERRLog\\");
                }
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "MESERRLog\\" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + "Log.txt", FileMode.OpenOrCreate);
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
            finally
            {
                //
            }
        }

        #endregion


        #region 检测记录上传

        //地址
        string HisenseAI_Url = "http://172.16.101.36:8098/api/Ats/AtsTestRun";

        string HisenseAI_Check_Url = "http://172.16.101.36:8098/api/Ats/AtsCheckRun";

        //上传检测记录
        public bool UploadCheckRecord(string _json)
        {
            try
            {
                HttpClient client = new HttpClient();
                StringContent content = new StringContent(_json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(HisenseAI_Url, content).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                MESLog("上传检测记录：" + response.Content.ReadAsStringAsync().Result);
                JObject jsonObject = JObject.Parse(result);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (jsonObject["code"].ToString() == "OK")
                    {
                        MESLog(jsonObject["code"].ToString() + "." + jsonObject["result"].ToString());
                        return true;
                    }
                    else
                    {
                        MESERRLog(jsonObject["code"].ToString()+","+ jsonObject["result"].ToString());
                        MESERRLog(response.Content.ReadAsStringAsync().Result);
                        return false;
                    }
                }
                else
                {
                    MESERRLog(response.Content.ReadAsStringAsync().Result);
                    return false;
                }

            }
            catch (Exception ex)
            {
                MainForm.MainForm_P.RunLog("上传检测记录报错：" + ex.ToString());
                return false;
            }
        }

        //请求核验
        public bool UploadCheck(string _json)
        {
            try
            {
                HttpClient client = new HttpClient();
                StringContent content = new StringContent(_json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(HisenseAI_Check_Url, content).Result;
                //string result = response.Content.ReadAsStringAsync().Result;

                MESLog("上传核验记录：" + response.Content.ReadAsStringAsync().Result);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    MESERRLog(response.Content.ReadAsStringAsync().Result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MainForm.MainForm_P.RunLog("上传核验记录报错：" + ex.ToString());
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iType">对接代码</param>
        /// <param name="_empNo">员工编号</param>
        /// <param name="_pcbCode">产品条形码</param>
        /// <param name="_workStationSn">工作中心SN</param>
        /// <param name="_tsRes">测试结果</param>
        /// <param name="_pcName">电脑名称</param>
        /// <param name="_pcIp">电脑IP地址</param>
        /// <param name="_devName">设备名称</param>
        /// <param name="_devModel">设备型号</param>
        /// <param name="_devNo">设备编号</param>
        /// <param name="_itemId">测试顺序号</param>
        /// <param name="_itemName">测试项目名称(位号)</param>
        /// <param name="_TestValue">测试项目的测试值</param>
        /// <param name="_itemLenght">测试的时长,单位秒(S)</param>
        /// <param name="_itemRes">测试项目结果</param>
        /// <returns></returns>
        public string GetAI_ResultJson(
            string _iType,
            string _empNo,
            string _pcbCode,
            string _workStationSn,
            string _tsRes,
            string _tsLength,
            string _pcName,
            string _pcIp,
            string _devName,
            string _devModel,
            string _devNo,
            string _itemId,
            string _itemName,
            string _TestValue,
            string _itemLenght,
            string _itemRes
            )
        {
            try
            {
                AI_ResultJson item = new AI_ResultJson();
                atsTsItems _atsTsItem = new atsTsItems();
                item.iType = _iType;
                item.empNo = _empNo;
                item.pcbCode = _pcbCode;
                item.workStationSn = _workStationSn;
                item.tsRes = _tsRes;
                item.tsLength = _tsLength;
                item.pcName = _pcName;
                item.pcIp = _pcIp;
                item.devName = _devName;
                item.devModel = _devModel;
                item.devNo = _devNo;

                _atsTsItem.itemId = _itemId;
                _atsTsItem.itemName = _itemName;
                _atsTsItem.TestValue = _TestValue;
                _atsTsItem.itemLenght = _itemLenght;
                _atsTsItem.itemRes = _itemRes;

                item.atsTsItemsModels = new List<atsTsItems> { _atsTsItem };

                return JsonConvert.SerializeObject(item);
            }
            catch (Exception ex)
            {
                return "";
                //throw;
            }

        }

        public string GetCheck(string _iType,string _empNo,string _pcbCode,string _workStationSn)
        {
            try
            {
                atsCheck _ats = new atsCheck();
                _ats.iType = _iType;
                _ats.empNo = _empNo;
                _ats.pcbCode = _pcbCode;
                _ats.workStationSn = _workStationSn;
                return JsonConvert.SerializeObject(_ats);
            }
            catch (Exception ex)
            {
                //MESERRLog(ex.ToString());
                return "";
                //throw;
            }
        }


    }



    public class atsCheck
    {
        /// <summary>
        /// 
        /// </summary>
        public string iType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string empNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pcbCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string workStationSn { get; set; }
    }

    public class atsTsItems
    {
        /// <summary>
        /// 
        /// </summary>
        public string itemId { get; set; }
        /// <summary>
        /// 图1
        /// </summary>
        public string itemName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TestValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string itemLenght { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string itemRes { get; set; }

    }

    public class AI_ResultJson
    {
        /// <summary>
        /// 
        /// </summary>
        public string iType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string empNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pcbCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string workStationSn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tsRes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tsLength {  get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pcIp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pcName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string devName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string devModel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string devNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<atsTsItems> atsTsItemsModels { get; set; }



    }


}


