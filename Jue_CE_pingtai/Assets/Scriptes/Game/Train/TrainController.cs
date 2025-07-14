using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviourInstanceExample<TrainController>
{

    public float Start_Hitgh
    {
        get
        {
            float hitgh = 0;
            switch (GameManager.Instance.TrainType)
            {
                case TrainType.CR:
                    hitgh = hitgh_CR;
                    break;
                case TrainType.CHR:
                    hitgh = hitgh_CRH;
                    break;
                case TrainType.PUTONGCHE:
                    hitgh = hitgh_Putong;
                    break;
                default:
                    break;
            }


            return hitgh;
        }
    }

    private float hitgh_CRH = 2.51f;//初始高度
    private float hitgh_CR = 2.51f;//初始高度
    private float hitgh_Putong=3.3f;//初始高度

    private float chetou_chexiang_length_CR = -28.15f;//与车头连接车厢长度
    private float chetou_chexiang_length_CHR = -26.38f;
    private float chetou_chexiang_length_PUTONG = -20.5f;

    private float chexiang_chewei_length_CR = -26.5f;//车厢与车尾连接长度
    private float chexiang_chewei_length_CHR = -26.65f;//车厢与车尾连接长度
    private float chexiang_chewei_length_PUTONG = -20.89f;//车厢与车尾连接长度

    private float step_lenth_CHR = -25.5f;//车厢之间的间隔
    private float step_lenth_CR = -29.5f;
    private float step_lenth__PUTONG = -20f;

    private float che_wei_length_CR = -27.95f;//车尾长度
    private float che_wei_length_CRH = -25.87f;
    private float che_wei_length_PUTONG = -20.5f;


    private List<CheXiang> chengxiang_list = new List<CheXiang>();

    private TrainType _trainType;
    private int _chexiang_number;


    /// <summary>
    /// 设置列车的类型与列车车厢的数量
    /// </summary>
    /// <param name="trainType"></param>
    /// <param name="numbers"></param>
    public void SetTrainTypeAndNumber(float trainType,float[] numbers)
    {
        switch (trainType)
        {
            case 1:
                _trainType = TrainType.CR;
                break;
            case 2:
                _trainType = TrainType.CHR;
                break;
            case 3:
                _trainType = TrainType.PUTONGCHE;
                break;
            case 4:
                _trainType = TrainType.CR;
                break;
            case 5:
                _trainType = TrainType.CHR;
                break;
            case 6:
                _trainType = TrainType.PUTONGCHE;
                break;

            default:
                break;
        }

        _chexiang_number = numbers.Length;
    }


    /// <summary>
    /// 根据列车的类型与编组信息进行生成
    /// </summary>
    /// <param name="type"></param>
    /// <param name="trainCount"></param>
    public void GenerateTarrin(TrainType type, int trainCount)
    {
        CheXiang _chengxiang;
        string path = "Prefab/";
        float hight = 0;
        float chetou_chengxiang_length = 0;
        float chengxiang_chewei_length = 0;
        float chewei_length = 0;
        float step_lengt = 0;

        var chexiang_count = trainCount - 2;
        switch (type)
        {
            case TrainType.CR:
                path += "CR/";
                chetou_chengxiang_length = chetou_chexiang_length_CR;
                chengxiang_chewei_length = chexiang_chewei_length_CR;
                step_lengt = step_lenth_CR;
                chewei_length = che_wei_length_CR;
                hight = hitgh_CR;
                break;
            case TrainType.CHR:
                path += "CRH/";
                hight = hitgh_CRH;
                chetou_chengxiang_length = chetou_chexiang_length_CHR;
                chengxiang_chewei_length = chexiang_chewei_length_CHR;
                step_lengt = step_lenth_CHR;
                chewei_length = che_wei_length_CRH;
                break;
            case TrainType.PUTONGCHE:
                path += "PUTONGCHE/";
                hight = hitgh_Putong;
                chetou_chengxiang_length = chetou_chexiang_length_PUTONG;
                chengxiang_chewei_length = chexiang_chewei_length_PUTONG;
                chewei_length = che_wei_length_PUTONG;
                step_lengt = step_lenth__PUTONG;

                //
                chexiang_count = chexiang_count + 2;
                break;
            default:
                break;
        }

        Vector3 star_point = new Vector3(2.5f, hight, 0);

        var prefab_chetou = Loader.LoadPrefab(path + "chetou");
        var chetou = Instantiate(prefab_chetou, star_point, Quaternion.identity, this.transform);

        if(type!= TrainType.PUTONGCHE)
        {
             _chengxiang = chetou.GetComponent<CheXiang>();
            _chengxiang.CheXiangID = 0;
            _chengxiang.Init();
            chengxiang_list.Add(_chengxiang);
        }

        for (int i = 0; i < chexiang_count; i++)
        {
            var chexiang_prefab = Loader.LoadPrefab(path + "chexiang");
            
            if (i == 0)
            {
                star_point = new Vector3(2.5f, hight, chetou_chengxiang_length);
            }
            else
            {
                star_point = new Vector3(star_point.x, star_point.y, star_point.z + step_lengt);
            }

            var chexiang = Instantiate(chexiang_prefab, star_point, Quaternion.identity, this.transform);

            if( !chexiang.TryGetComponent(out _chengxiang))
            {
                _chengxiang = chexiang.AddComponent<CheXiang>();
            }

            _chengxiang.Init();
            _chengxiang.CheXiangID = i + 1;
            chengxiang_list.Add(_chengxiang);        
        }

        var prefab_chewei = Loader.LoadPrefab(path + "chewei");

        if (chexiang_count >= 0)
        {

            star_point = new Vector3(star_point.x, star_point.y, star_point.z + chewei_length);

            var chewei = Instantiate(prefab_chewei, star_point, Quaternion.identity, this.transform);

            if (type != TrainType.PUTONGCHE)
            {
                int count = chengxiang_list.Count;
                _chengxiang = chewei.GetComponent<CheXiang>();
                _chengxiang.CheXiangID = count;
                _chengxiang.Init();
                chengxiang_list.Add(_chengxiang);
            }
        }
        else
        {
            star_point = new Vector3(star_point.x, star_point.y, star_point.z + chengxiang_chewei_length);
        }


        //var chewei = Instantiate(prefab_chewei, star_point, Quaternion.identity, this.transform);

        //if (type != TrainType.PUTONGCHE)
        //{
        //    int count = chengxiang_list.Count;
        //    _chengxiang = chewei.GetComponent<CheXiang>();
        //    _chengxiang.CheXiangID = count;
        //    _chengxiang.Init();          
        //    chengxiang_list.Add(_chengxiang);
        //}
    }


    /// <summary>
    /// 根据里程信息设置对应的坐标与车厢数据与车厢的朝向）
    /// </summary>
    /// <param name="lichengDta"></param>
    public void SetTrainPosAndRotation(LichengDta lichengDta,BaseLineData lineData)
    {
        float lc = lichengDta.licheng;

        bujian_data _che = lineData.CalculatePointAndRotation(lc);

        //计算路径
        this.transform.position = _che.positon;

        //TODO 计算旋转角度这里需要将每个车厢的旋转角度进行计算，由于是直线先不做考虑

        //应用车厢数据
        if (chengxiang_list.Count != lichengDta.chexiangdata_list.Count)
        {
            Debug.LogErrorFormat("车厢数量与解析的车厢数据不匹配，请检查，车厢数量为{0}，解析的车厢数据长度为{1}", chengxiang_list.Count, lichengDta.chexiangdata_list.Count);
        }

        foreach (CheXiang item in chengxiang_list)
        {
            var id = item.CheXiangID;
            var data = lichengDta.chexiangdata_list[id];
            item.SetChexiangData(data);
        }

        //chengxiang_list[0].SetChexiangData(lichengDta.chexiangdata_list[0]);

        //Debug.LogFormat("列车运动的里程为{0}", lc);

        //更新UI
        ContrPanel ctr_ui = UIManager.Instance.GetUIPanel("Contr_Panel") as ContrPanel;
        ctr_ui.ShowLiChengText(lc.ToString("0.00"));

       //Debug.Log("调用了几次");
    }

    /// <summary>
    /// 显示或者隐藏车体
    /// </summary>
    /// <param name="showRoHide"></param>
    public void ShowRoHideCheti(bool showRoHide)
    {
        foreach (var item in chengxiang_list)
        {
            item.HidCheti(showRoHide);
        }
    }

    /// <summary>
    /// 将所有车厢的数据进行归正
    /// </summary>
    public void RestChexiangPose()
    {
        foreach (var item in chengxiang_list)
        {
            item.RestLocalPose();
        }
    }


    public void GenerateTarrin(TrainType trainType)
    {
        //GenerateTarrin(_trainType, _chexiang_number);
        GenerateTarrin(trainType, _chexiang_number);
    }

}


public enum TrainType
{
    CR,
    CHR,
    PUTONGCHE
}
