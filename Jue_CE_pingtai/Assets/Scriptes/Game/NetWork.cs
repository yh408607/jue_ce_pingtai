using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class NetWork :NoramlInstanceExample<NetWork>
{
    //private string Url = "http://192.168.1.106:9080";
    private string Url = "http://122.207.82.23:2526/api";
    //private string child_url = "/hello/test/{msg}";
    //private string url = "http://h6458294j5.yicp.fun/hello/test/yyy0";
    private string token = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJhZG1pbiIsImxvZ2luX3VzZXJfa2V5IjoiYmMyYjEzOGItMzI3My00YzBmLWEwMjQtZDdmZmQ1MTRmZjdkIn0.kCBO5HYFH6dPJ_tsVbGTddNUA4mLyuwv4Wehqb2gkFZXSsFQJiTmEhSyAwDMvHqWo5RPjK4Y6q4JjK_x8hk_nA";
#if UNITY_EDITOR && UNITY_2017_3
    private string child_result = "/business/unity/getFileLinesByName?resultId=59&fileName=";
#else
    private string child_result = "/business/unity/getFileLinesByName?";
#endif

    private string child_agrs = "/business/unity/getNeedParams";

    //private long resultId = 74;
    //private long resultId = 90;
    private long resultId = 74;

    /// <summary>
    /// 用户是选择查看位移响应还是加速度响应，对应的结果
    /// </summary>
    private string data_type
    {
        set
        {
            GameManager.Instance.data_type = value;
        }

        get
        {
            return GameManager.Instance.data_type;
        }
    }

    public override void Init()
    {

#if UNITY_EDITOR && UNITY_2017_3
        data_type = "列车位移响应.txt";


#elif UNITY_WEBGL || UNITY_EDITOR

        data_type = "列车位移响应";
#endif
    }

    /// <summary>
    /// 获取数据并数据接收完成后，需要对数据进行处理
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="uil"></param>
    /// <param name="action"></param>
    public void Get(MonoBehaviour mono,string uRl,Action<string> action,string dataType)
    {
        mono.StartCoroutine(Get(uRl, action,dataType));
    }

    public IEnumerator Get(string FlieName,Action<string> action,string dataType )
    {
        UnityWebRequest request = null;

#if UNITY_EDITOR && UNITY_2017_3
       var path = Application.dataPath + "/Resources/" + FlieName;
        request = UnityWebRequest.Get(path);

#elif UNITY_WEBGL || UNITY_EDITOR

        //Debug.LogFormat("开始发送请求,请求类型为{0}", dataType);
        switch (dataType)
        {
            case "Param":
                string path_parga = Url + child_agrs;

                lineParameterData data = new lineParameterData();
                data.resultId = resultId;

                data.paramNames.Add("MulTrain");
                data.paramNames.Add("HH_Len");
                data.paramNames.Add("HH_Len_1");

                data.paramNames.Add("ST_LenL1");
                data.paramNames.Add("ST_LenL1_1");

                data.paramNames.Add("ST_LenR");
                data.paramNames.Add("ST_LenR_1");

                data.paramNames.Add("CurveT");

                data.paramNames.Add("CR");
                data.paramNames.Add("CR_1");

                data.paramNames.Add("CR_Len");
                data.paramNames.Add("CR_Len_1");

                data.paramNames.Add("Train_type");
                data.paramNames.Add("TrainF");
                data.paramNames.Add("TrainF_1");

                data.paramNames.Add("Len_Bri");

                data.paramNames.Add("XB1");
                data.paramNames.Add("YB1");
                data.paramNames.Add("ZB1");

                data.paramNames.Add("VehEffectType");
                data.paramNames.Add("L_tunl");

                data.paramNames.Add("Substructural_Type");


                //设置data为json，并且转为utf-8格式
                string str_json = JsonUtility.ToJson(data);
                
                request = new UnityWebRequest(path_parga, "POST");
                byte[] jsonBytes = Encoding.UTF8.GetBytes(str_json);
                request.uploadHandler = new UploadHandlerRaw(jsonBytes);
                request.downloadHandler = new DownloadHandlerBuffer();

                // 设置请求头
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + token);

                break;

            case "result":

#if UNITY_EDITOR && UNITY_2017_3
                var path = Url + child_result + FlieName;
#elif UNITY_WEBGL
                var path = Url + child_result + "resultId=" + resultId + "&" + "fileName=" + FlieName;
                Debug.LogFormat("发送的URL为{0},", path);
#endif
                request = UnityWebRequest.Get(path);
                request.SetRequestHeader("Authorization", "Bearer " + token);

                break;
            default:
                //Debug.LogFormat("发送的请求，选择了default，请找原因，发送的地址为{0}", Url);
                break;
        }
#endif
        //Debug.LogFormat("1111111发送的请求，请找原因，发送的地址为{0}", request.url);

        DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
        request.downloadHandler = handler;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            //Debug.Log("接收到信息Received: " + request.downloadHandler.text);
            string str = request.downloadHandler.text;
            action(str);


            //using (MemoryStream stream = new MemoryStream(request.downloadHandler.data))
            //{
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        using (JsonTextReader jsonReader = new JsonTextReader(reader))
            //        {
            //            while (jsonReader.Read())
            //            {
            //                Debug.Log("循环读取");
            //                if (jsonReader.TokenType == JsonToken.StartObject)
            //                {
            //                    Debug.Log("开始处理对象");
            //                    // 处理单个JSON对象
            //                    JObject obj = JObject.Load(jsonReader);
            //                    //ProcessJSONObject(obj);
            //                    action(obj.ToString());
            //                    yield return null; // 处理每个对象后让出一帧
            //                }
            //            }
            //        }
            //    }
            //}

            // 立即释放内存
            request.Dispose();
            handler.Dispose();
            System.GC.Collect();
        }


    }

    public void GetWebTokenMsg()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // 获取URL参数
        string url = Application.absoluteURL;
        Uri uri = new Uri(url);
        var parameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
        
        string initParam = parameters.Get("initParam");
        if (!string.IsNullOrEmpty(initParam))
        {
            Debug.Log("获取到的参数Initial parameter from URL: " + initParam);
            // 处理参数
            string[] temp = initParam.Split('@');
            resultId = long.Parse(temp[0]);
            token = temp[1];
            //var  data_type = temp[2];
            jiexiDataType(temp[2]);
        }
        else
        {
            Debug.Log("没有接收到参数");
        }
#endif
    }


    private void jiexiDataType(string dataType)
    {
        //Debug.LogFormat("获取到的数据类型为{0}", dataType);

        switch (dataType)
        {
            case "w":
                data_type = "列车位移响应";
                GameManager.Instance.data_type = "列车位移响应";
                Debug.LogFormat("需要显示的是{0}", data_type);
                break;
            case "j":
                data_type = "列车加速度响应";
                GameManager.Instance.data_type = "列车加速度响应";
                Debug.LogFormat("需要显示的是{0}", data_type);
                break;
            default:
                break;
        }
    }
}
[Serializable]
public enum GetDataType
{
    Param,
    result
}

/// <summary>
/// 需要在服务器获取的参数
/// </summary>
[Serializable]
public class lineParameterData
{
    public long resultId;
    public List<string> paramNames;
    public lineParameterData()
    {
        paramNames = new List<string>();
    }
}


/// <summary>
/// f服务端解析用途
/// </summary>
[System.Serializable]
public class Severlinedata<T>
{
    public int code;
    public string msg;
    public T data;
}



/// <summary>
/// 服务端线路的数据
/// </summary>
[System.Serializable]
public struct Severlinedata_paragm
{
    /// <summary>
    /// 车轨类型
    /// 0：单线单向运行
   ///1： 双线同向运行
   ///2：双线异向运行
    ///3：双线单向运行
    /// </summary>
    public string MulTrain;


    /// <summary>
    /// 缓和曲线长度
    /// </summary>
    public float HH_Len;
    public float HH_Len_1;

    /// <summary>
    /// 左直线长度
    /// </summary>
    public float ST_LenL1;
    public float ST_LenL1_1;

    /// <summary>
    /// 右直线长度
    /// </summary>
    public float ST_LenR;
    public float ST_LenR_1;

    /// <summary>
    /// 是否考虑曲线 0-false,1-true
    /// </summary>
    public int CurveT;

    /// <summary>
    /// 圆曲线半径长度R
    /// </summary>
    public float CR;
    public float CR_1;

    /// <summary>
    /// 圆曲线长度
    /// </summary>
    public float CR_Len;
    public float CR_Len_1;

    /// <summary>
    /// 列车类型。1-高速列车，
    /// </summary>
    public float Train_type;

    /// <summary>
    /// 第二个列车的种类
    /// </summary>
    public float Train_type_1;

    /// <summary>
    /// 编组信息，为一个数组
    /// </summary>
    public float[] TrainF;
    public float[] TrainF_1;

    /// <summary>
    /// 桥墩信息为一个数组
    /// </summary>
    public float[] Len_Bri;

    /// <summary>
    /// X沉降信息
    /// </summary>
    public float[] XB1;

    /// <summary>
    /// Y沉降信息
    /// </summary>
    public float[] YB1;

    /// <summary>
    /// Z沉降信息
    /// </summary>
    public float[] ZB1;

    /// <summary>
    /// 模块类型
    /// 列车-轨道系统模块	6
    ///列车-轨道-隧道系统模块	2
    ///列车-轨道-桥梁系统模块	4
    ///列车-轨道-复杂桥梁模块	12
    ///列车-轨道-路基系统模块	3
    ///列车-轨道-隧道-土体系统模块	1
    ///列车-轨道-土体-管道系统模块	14
    ///列车-轨道-桥梁-土体系统模块	8
    ///列车-轨道-隧道-土体-高架桥梁系统模块	5
    ///列车-轨道-隧道-土体系统（地震激励）模块	7
    ///轨下板结构联合仿真模块	9
    ///轨下实体结构联合仿真模	13
    ///轨下基础联合仿真模块	10
    ///列车-轨道-隧道-土体(建筑)系统模块	11
    ///列车-轨道-路基-隧道系统模块	15
    ///列车-轨道-路基-桥梁系统模块	16
    ///列车-轨道-桥梁-隧道系统模块	17
    /// </summary>
    public string Substructural_Type;

    /// <summary>
    /// 隧道长度
    /// </summary>
    public float L_tunl;
}

/// <summary>
/// 解析服务端的数据
/// </summary>
[System.Serializable]
public class ServerLiChengData_Param
{
    public int code;
    public string msg;
    //public string[] data;
    public string data;
}
