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
    private int train_count;//�г�������Ϣ
    private TrainType trainType;//�г�������
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
    public fangdaxishu fangdaxishu = new fangdaxishu();//�Ŵ�ϵ��
    public string data_type = "�г�λ����Ӧ.txt";//����ط��Ǵ����ȡ�Ǽ��ٶ���Ӧ����λ����Ӧ


    //���ѡ�������
    private GameObject selected_gameobject;

    protected override void Awake()
    {
        //��ȡtoken
        NetWork.Instance.Init();
        NetWork.Instance.GetWebTokenMsg();

        //��ʼ��UI
        UIManager.Instance.Init();
        UIManager.Instance.ShowUIPanel("Mask_Panel");

        StartCoroutine(StartGetData());
    }


    IEnumerator StartGetData()
    {
        yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR && UNITY_2017_3
        yield return NetWork.Instance.Get("ƽ�ݶ������.txt", GetLindData, "Param");
        yield return NetWork.Instance.Get(data_type, GetTrainData, "result");
#elif UNITY_WEBGL
        yield return NetWork.Instance.Get("ƽ�ݶ������", GetLindData, "Param");
        yield return NetWork.Instance.Get(data_type, GetTrainData, "result");
#endif
        //������·
        //var path = linedata.paths.ToArray();
        //CreatorRoad(path);
        //CreatorRoad(linedata.linePath);
        CreatorRoad();

        //�����г�����.��Ҫȷ���г�����������鳤����Ϣ
        //TrainController.Instance.GenerateTarrin(TrainType.CHR, Train_count);
        TrainController.Instance.GenerateTarrin(TrainType.CHR);

        //����UI
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

        float huanqu = msgMap["�������߳���"];
        float zuozhixian = msgMap["���ֱ�߳���"];
        float youzhixian = msgMap["�Ҳ�ֱ�߳���"];
        float quxian = msgMap["�Ƿ�������0-�����ǣ�1-����"];
        bool isquxian = quxian == 1 ? true : false;
        float yuanquxian = msgMap["���߳���"];
        float yuan_R = msgMap["���߰뾶"];

        //�������
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

        //����·����
        calculateLine(isquxian, zuozhixian, huanqu, yuanquxian, yuan_R, youzhixian,data);

        //���������������Ҫ�����Ŷյ�λ��
#elif UNITY_WEBGL

        //��stringתΪJson
        var data = JsonUtility.FromJson<Severlinedata<Severlinedata_paragm>>(str);
        //Debug.LogFormat("��ʼ������ȡ����ƽ�ݶ��������{0}", data.data.TrainF);
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
                case "1"://��-��-��-��
                    Substructural_Type = "che_xian_sui_tui";
                    break;
                case "2"://�г�-���-���ϵͳģ��
                    Substructural_Type = "che_xian_sui";
                    break;
                case "3":
                    //�г�-���-·��ϵͳģ��
                    Substructural_Type = "che_xian_luji";
                    break;
                case "4":
                    //�г�-���-����ϵͳģ��
                    Substructural_Type = "che_xian_qiao";
                    break;
                default:
                    break;
            }

            // calculateLine(isquxian, zuozhixian, huanqu, yuanquxian, yuan_R, youzhixian,data.data);
            //����·��
            calculateLine(data.data);

            float trian_type = data.data.Train_type;
            float[] TrainF = data.data.TrainF;
            TrainController.Instance.SetTrainTypeAndNumber(trian_type, TrainF);
        }


        // ������ɺ�ǿ��GC
        System.GC.Collect();
        Resources.UnloadUnusedAssets();

#endif
    }

    /// <summary>
    /// �����г�������
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
        //Debug.LogFormat("��ʼ����{0}�Ĳ�������ȡ���Ĳ�������Ϊ{1}", data_type, json.data.Length);
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

        //���Կ���

#endif
        // ������ɺ�ǿ��GC
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// ������·��Ϣ
    /// </summary>
    /// <param name="paragm"></param>
    private void calculateLine(Severlinedata_paragm paragm)
    {
        var type = paragm.Substructural_Type;
        switch (type)
        {
            case "1":// �г� - ��-��-�� -ϵͳģ��
                //Debug.LogError("");
                linedata = new Che_xian_sui_tu_lineData(paragm);
                break;
            case "4"://�г�-���-����ϵͳģ��
                linedata = new Che_xian_qiao_lineData(paragm);
                break;
            case "2"://�г�-���-���ϵͳģ��
                linedata = new Che_xian_sui_lineData(paragm);
                break;
            case "3"://�г�-���-·��ϵͳģ��
                break;

            default:
                //û����������ʱ��-��-��
                linedata = new Che_xian_sui_lineData(paragm);
                break;
        }
    }


    /// <summary>
    /// ���㲢�����ɶ�Ӧ��·��
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

        //���������������Ҫ�����Ŷյ�λ��
        float[] bride_len = data.Len_Bri;
        float[] x_bs = data.XB1;
        float[] y_bs = data.YB1;
        float[] z_bs = data.ZB1;
        //�����Ŷյĳ�����
        _linedata.SetBridgeData(x_bs, y_bs, z_bs, bride_len);

        //�������

        _linedata.calculatePath("");
    }

    //��̬���ɵ�·
    private void CreatorRoad(Vector3[] path)
    {

        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-500),
            new Vector3(0, 0,0),
        };

        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_luji", "��һ��");

        CreaterRoad.CreatRoad_new(path, "Guidao_qiao", "�ڶ���");
        //Vector3[] yansheng = new Vector3[2] {
        //    new Vector3(0,10, 97.8f),
        //    new Vector3(0, 10, 320),
        //};


        //CreaterRoad.CreatRoad_new(yansheng, "Guidao_qiao2");
    }

    private void CreatorRoad(List<Baseline> linepath)
    {
        ////�������ɳ�ʼ��
        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-500),
            new Vector3(0, 0,0),
        };

        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_luji", "��ʼ��");

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
                    //����λ�������Ŷ�
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

    //ͨ��index��ȡ��Ӧ���г�����
    public void GetChexiangData(int index)
    {
        LichengDta licheng = trainData.GetLicheng_Data(index);

        TrainController.Instance.SetTrainPosAndRotation(licheng,linedata);
    }

    /// <summary>
    /// ���þ�ͷ�ĸ��淽ʽ
    /// </summary>
    /// <param name="isplay"></param>
    public void SetCammeraFlowType(bool isplay)
    {
        GameObject camera = Camera.main.gameObject;
        var temp = camera.GetComponent<CameraAroundWithDragMove>();
        
        var temp_1 = camera.GetComponent<CameraFlowTraget>();

        if (isplay)
        {
            //��ͷҪ���泵
            temp.enabled = false;
            temp_1.enabled = true;
        }
        else
        {
            //��ͷҪ����
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