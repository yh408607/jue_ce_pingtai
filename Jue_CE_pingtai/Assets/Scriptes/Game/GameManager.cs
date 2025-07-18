using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using EasyRoads3Dv3;
using LitJson;

public class GameManager : MonoBehaviourInstanceExample<GameManager>
{
    private BaseLineData linedata;
    private TrainData trainData;
    private int train_count;//列车编组信息
    private TrainType trainType;//列车的类型
    private lineParameterData lineParameterData;


    public TrainType TrainType { get => trainType; }
    public int Train_count { get => train_count; }
    public string Substructural_Type { get; private set; }
    public GameObject Selected_gameobject { get => selected_gameobject;
        set {
         
            selected_gameobject = value;
            if (selected_gameobject == null)
            {
                //DataPanel ctr_panel = UIManager.Instance.ShowUIPanel("Data_Panel") as DataPanel;
                UIManager.Instance.HideUIPanel("Data_Panel");

            }
            else
            {
                CheXiang chexiang;
                if (selected_gameobject.TryGetComponent(out chexiang))
                {
                    DataPanel datapane = UIManager.Instance.ShowUIPanel("Data_Panel") as DataPanel;
                    datapane.ShowData(chexiang.ChexiangName, chexiang);
                }
                else
                {
                    UIManager.Instance.HideUIPanel("Data_Panel");
                }

            }
        } 
    }
    public fangdaxishu fangdaxishu = new fangdaxishu();//放大系数
    public string data_type = "列车位移响应.txt";//这个地方是代表读取是加速度响应还是位移响应


    //鼠标选择的物体
    private GameObject selected_gameobject;

    protected override void Awake()
    {
        //获取token
        NetWork.Instance.Init();
        NetWork.Instance.GetWebTokenMsg();

        //初始化UI
        UIManager.Instance.Init();
        UIManager.Instance.ShowUIPanel("Mask_Panel");

        StartCoroutine(StartGetData());
    }


    IEnumerator StartGetData()
    {
        yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR && UNITY_2017_3
        yield return NetWork.Instance.Get("平纵断面参数.txt", GetLindData, "Param");
        yield return NetWork.Instance.Get(data_type, GetTrainData, "result");
#elif UNITY_WEBGL
        yield return NetWork.Instance.Get("平纵断面参数", GetLindData, "Param");
        yield return NetWork.Instance.Get(data_type, GetTrainData, "result");
#endif
        //创建道路
        //var path = linedata.paths.ToArray();
        //CreatorRoad(path);
        //CreatorRoad(linedata.linePath);
        CreatorRoad();

        //创建列车数据.需要确定列车的类型与编组长度信息
        //TrainController.Instance.GenerateTarrin(TrainType.CHR, Train_count);
        TrainController.Instance.GenerateTarrin(TrainType.CHR);

        //隐藏UI
        UIManager.Instance.HideUIPanel("Mask_Panel");
        ContrPanel ctr_panel = UIManager.Instance.ShowUIPanel("Contr_Panel") as ContrPanel;
        var step = trainData.data_Count;
        Vector2 min_max = new Vector2(0, step-1);
        ctr_panel.SetSilderValueMaxAndMin(min_max);
    }


    private void GetLindData(string str)
    {
#if UNITY_EDITOR && UNITY_2017_3
        string[] strs = str.Split('\n');
        Dictionary<string, float> msgMap = new Dictionary<string, float>();
        foreach (string item in strs)
        {
            if (string.IsNullOrEmpty(item)) continue;
            string[] temps = item.Split('%');
            float value = float.Parse(temps[0]);
            string key = temps[1];
            key = Regex.Replace(key, @"\s", "");
            msgMap.Add(key, value);
        }

        float huanqu = msgMap["缓和曲线长度"];
        float zuozhixian = msgMap["左侧直线长度"];
        float youzhixian = msgMap["右侧直线长度"];
        float quxian = msgMap["是否考虑曲线0-不考虑；1-考虑"];
        bool isquxian = quxian == 1 ? true : false;
        float yuanquxian = msgMap["曲线长度"];
        float yuan_R = msgMap["曲线半径"];

        //造假数据
        Severlinedata_paragm data = new Severlinedata_paragm();
        float[] len_bridge = new float[]
        {
            32,32,32
        };
        float[] X_B = new float[]
        {
            0,0,0,0
        };

        float[] Y_B = new float[]
        {
            0,0,0,0
        };

        float[] Z_B = new float[]
        {
            0,0,0f,0
        };

        data.Len_Bri = len_bridge;
        data.XB1 = X_B;
        data.YB1 = Y_B;
        data.ZB1 = Z_B;

        //计算路径点
        calculateLine(isquxian, zuozhixian, huanqu, yuanquxian, yuan_R, youzhixian,data);

        //计算沉降，沉降需要考虑桥墩的位置
#elif UNITY_WEBGL

        //将string转为Json
        var data = JsonUtility.FromJson<Severlinedata<Severlinedata_paragm>>(str);
        //Debug.LogFormat("开始解析获取到的平纵断面的数据{0}", data.data.TrainF);
        if (data.code == 200)
        {
            float huanqu = data.data.HH_Len;
            float zuozhixian = data.data.ST_LenL1;
            float youzhixian = data.data.ST_LenR;
            bool isquxian = data.data.CurveT == 0 ? false : true;
            float yuan_R = data.data.CR;
            float yuanquxian = data.data.CR_Len;

            string VehEffectType = data.data.Substructural_Type;
            switch (VehEffectType)
            {
                case "1"://车-线-隧-土
                    Substructural_Type = "che_xian_sui_tui";
                    break;
                case "2"://列车-轨道-隧道系统模块
                    Substructural_Type = "che_xian_sui";
                    break;
                case "3":
                    //列车-轨道-路基系统模块
                    Substructural_Type = "che_xian_luji";
                    break;
                case "4":
                    //列车-轨道-桥梁系统模块
                    Substructural_Type = "che_xian_qiao";
                    break;
                default:
                    break;
            }

            // calculateLine(isquxian, zuozhixian, huanqu, yuanquxian, yuan_R, youzhixian,data.data);
            //生成路线
            calculateLine(data.data);

            float trian_type = data.data.Train_type;
            float[] TrainF = data.data.TrainF;
            TrainController.Instance.SetTrainTypeAndNumber(trian_type, TrainF);
        }


        // 处理完成后强制GC
        System.GC.Collect();
        Resources.UnloadUnusedAssets();

#endif
    }

    /// <summary>
    /// 解析列车的数据
    /// </summary>
    /// <param name="str"></param>
    private void GetTrainData(string str)
    {
        string temp_str = "";

#if UNITY_EDITOR && UNITY_2017_3
        temp_str = str;
        string[] strs = temp_str.Split('\n');
        trainData = new TrainData();

        foreach (string item in strs)
        {
            if (string.IsNullOrEmpty(item)) continue;
            string[] temp = item.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

            LichengDta licheng = new LichengDta(temp);

            trainData.GenerLichengData(licheng);
        }

#elif UNITY_WEBGL
        //var data = JsonMapper.ToObject<ServerLiChengData_Param>(str);
        //var str_s = data.data;
        var json = JsonUtility.FromJson<ServerLiChengData_Param>(str);
        //Debug.LogFormat("开始解析{0}的参数，获取到的参数长度为{1}", data_type, json.data.Length);
        if (json.code == 200)
        {
            var datas = json.data;
            trainData = new TrainData();

            foreach (string item in datas)
            {
                if (string.IsNullOrEmpty(item)) continue;
                string[] temp = item.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

                LichengDta licheng = new LichengDta(temp);

                trainData.GenerLichengData(licheng);
            }
        }

        //测试看看

#endif
        // 处理完成后强制GC
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 生成线路信息
    /// </summary>
    /// <param name="paragm"></param>
    private void calculateLine(Severlinedata_paragm paragm)
    {
        var type = paragm.Substructural_Type;
        switch (type)
        {
            case "1":// 列车 - 线-隧-土 -系统模块
                //Debug.LogError("");
                linedata = new Che_xian_sui_tu_lineData(paragm);
                break;
            case "4"://列车-轨道-桥梁系统模块
                linedata = new Che_xian_qiao_lineData(paragm);
                break;
            case "2"://列车-轨道-隧道系统模块
                linedata = new Che_xian_sui_lineData(paragm);
                break;
            case "3"://列车-轨道-路基系统模块
                break;

            default:
                //没有配数据暂时车-线-隧
                linedata = new Che_xian_sui_lineData(paragm);
                break;
        }
    }


    /// <summary>
    /// 计算并且生成对应的路径
    /// </summary>
    /// <param name="isquxian"></param>
    /// <param name="zuozhixian_length"></param>
    /// <param name="huanqu_length"></param>
    /// <param name="yuan_length"></param>
    /// <param name="yuan_R"></param>
    /// <param name="youzhixian_length"></param>
    /// <param name="data"></param>
    private void calculateLine(bool isquxian,float zuozhixian_length, float huanqu_length, float yuan_length, float yuan_R,float youzhixian_length, Severlinedata_paragm data)
    {
        linedata = new Che_xian_qiao_lineData();
        var  _linedata = linedata as Che_xian_qiao_lineData;
        _linedata.isquxian = isquxian;
        _linedata.zuozhixian_length = zuozhixian_length;
        _linedata.huanqu_length = huanqu_length;
        _linedata.yuan_length = yuan_length;
        _linedata.yuan_R = yuan_R;
        _linedata.youzhixian_length = youzhixian_length;

        //计算沉降，沉降需要考虑桥墩的位置
        float[] bride_len = data.Len_Bri;
        float[] x_bs = data.XB1;
        float[] y_bs = data.YB1;
        float[] z_bs = data.ZB1;
        //设置桥墩的沉降点
        _linedata.SetBridgeData(x_bs, y_bs, z_bs, bride_len);

        //隧道设置

        _linedata.calculatePath("");
    }

    //动态生成道路
    private void CreatorRoad(Vector3[] path)
    {

        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-500),
            new Vector3(0, 0,0),
        };

        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_luji", "第一段");

        CreaterRoad.CreatRoad_new(path, "Guidao_qiao", "第二段");
        //Vector3[] yansheng = new Vector3[2] {
        //    new Vector3(0,10, 97.8f),
        //    new Vector3(0, 10, 320),
        //};


        //CreaterRoad.CreatRoad_new(yansheng, "Guidao_qiao2");
    }

    private void CreatorRoad(List<Baseline> linepath)
    {
        ////这是生成初始端
        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-500),
            new Vector3(0, 0,0),
        };

        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_luji", "初始段");

        foreach (var item in linepath)
        {
            switch (item.Name)
            {
                case "qiao_2":
                    var path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;

                case "luji":
                    path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;
                case "qiao":
                    path = item.path.ToArray();
                    GameObject road = CreaterRoad.CreatRoad_new(path, "Guidao_qiao", item.Name);
                    //根据位置生成桥墩
                    var temp = item as qiao_line;
                    var chengjiang_pos = temp.qiaodun_pos;
                    foreach (var pos in chengjiang_pos)
                    {
                        var prefab = Loader.LoadPrefab("Prefab/qiao_dun");
                        var qiao_dun = Instantiate(prefab, pos, Quaternion.identity);
                        qiao_dun.transform.SetParent(road.transform);
                    }
                    break;
                default:
                    break;
            }
        }


    }

    private void CreatorRoad()
    {
        linedata.CreatorRoad();
    }

    //通过index获取对应的列车数据
    public void GetChexiangData(int index)
    {
        LichengDta licheng = trainData.GetLicheng_Data(index);

        TrainController.Instance.SetTrainPosAndRotation(licheng,linedata);
    }

    /// <summary>
    /// 设置镜头的跟随方式
    /// </summary>
    /// <param name="isplay"></param>
    public void SetCammeraFlowType(bool isplay)
    {
        GameObject camera = Camera.main.gameObject;
        var temp = camera.GetComponent<CameraAroundWithDragMove>();
        
        var temp_1 = camera.GetComponent<CameraFlowTraget>();

        if (isplay)
        {
            //镜头要跟随车
            temp.enabled = false;
            temp_1.enabled = true;
        }
        else
        {
            //镜头要自由
            temp.enabled = true;
            temp_1.enabled = false;
        }
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var sui = linedata as Che_xian_sui_lineData;
        //    sui.TestRay();
        //}
    }
}