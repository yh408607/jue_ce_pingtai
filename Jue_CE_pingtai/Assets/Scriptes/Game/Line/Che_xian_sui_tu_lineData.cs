using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Che_xian_sui_tu_lineData : BaseLineData
{

    private float suidao_len;
    private suidao_line _suidao;
    private Transform _tuti;
    private Transform _jianzu;


    public Che_xian_sui_tu_lineData(Severlinedata_paragm paragm)
    {
        step_index = 1;

        lineType = paragm.Substructural_Type;
        isquxian = paragm.CurveT == 0 ? false : true;
        zuozhixian_length = paragm.ST_LenL1;
        huanqu_length = paragm.HH_Len;
        yuan_length = paragm.CR_Len;
        yuan_R = paragm.CR;
        youzhixian_length = paragm.ST_LenR;

        //todo隧道的长度值需要模拟
        suidao_len = paragm.L_tunl;
        //suidao_len = 600;
        //放大4倍
        suidao_len = suidao_len * 4;
        //suidao_len = 650;
        //计算坐标点
        calculatePath();
    }

    
    public override void calculatePath(string data)
    {

    }



    public override void CreatorRoad()
    {
        Debug.Log("开始创建车-线-隧-土 路线");

        if (linePath.Count > 0)
        {
            //这是生成初始端
            Vector3[] ludi_point = new Vector3[2] {

                new Vector3(0,0,-200),
                new Vector3(0, 0,0),
            };
            CreaterRoad.CreatRoad_new(ludi_point, "Guidao_ludi", "初始段");

            foreach (var item in linePath)
            {
                switch (item.Name)
                {
                    case "ludi_1":
                        var path = item.path.ToArray();
                        CreaterRoad.CreatRoad_new(path, "Guidao_ludi", item.Name);
                        break;
                    case "suidao":
                        path = item.path.ToArray();
                        CreaterRoad.CreatRoad_new(path, "DunGou_Suidao", item.Name);

                        //创建土体与建筑
                        creatorTuti(path[0]);
                        break;
                    case "ludi_2":
                        path = item.path.ToArray();
                        CreaterRoad.CreatRoad_new(path, "Guidao_ludi", item.Name);
                        break;
                    default:
                        break;
                }
            }

            //隐藏地面
            //隐藏地面
            //GameObject.Find("地面").gameObject.SetActive(false);

        }
    }

    protected override void calculatePath()
    {
        linePath = new List<Baseline>();
        if (isquxian)
        {

        }
        else
        {
            //直线生成
            //直线情况，地基到Z-H点，后面就开始是桥梁，计算桥梁的总长度，每个桥梁桥墩的位置与沉降关系，桥梁后面还是地基

            //缓和曲线长度不取值
            huanqu_length = 0;

            var star_Pos = Vector3.zero;
            var end_pos = new Vector3(0, 0, zuozhixian_length);
            ludi_line ludi_1 = new ludi_line("ludi_1");
            ludi_1.path.Add(star_Pos);
            ludi_1.path.Add(end_pos);
            linePath.Add(ludi_1);

            star_Pos = end_pos;

            //隧道部分
            suidao_line suidao = new suidao_line("suidao");
            end_pos = new Vector3(0, 0, zuozhixian_length + suidao_len);
            suidao.path.Add(star_Pos);
            suidao.path.Add(end_pos);
            suidao.suidao_len = suidao_len;
            linePath.Add(suidao);
            star_Pos = end_pos;
            _suidao = suidao;

            //后续延长线部分
            //检测剩余长度
            var totulLenth = zuozhixian_length + huanqu_length + yuan_length + huanqu_length + youzhixian_length;
            var temp = totulLenth - star_Pos.z;
            if (temp > 100)
            {
                end_pos = new Vector3(0, 0, totulLenth);
            }
            else
            {
                end_pos = new Vector3(0, 0, totulLenth + 300);
            }
            ludi_line ludi_2 = new ludi_line("ludi_2");
            ludi_2.path.Add(star_Pos);
            ludi_2.path.Add(end_pos);


            linePath.Add(ludi_2);
        }
    }

    /// <summary>
    /// 创建土体
    /// </summary>
    private void creatorTuti(Vector3 pos)
    {
        var path = "Prefab/tuti/tuti";
        pos = new Vector3(pos.x, 4, pos.z);
        _tuti = Loader.InstantilGameObject(path, null, pos).transform;

        var temp = suidao_len / 100;
        _tuti.transform.localScale = new Vector3(1, 1, temp);


        _jianzu = _tuti.Find("jianzu_pos");

        var jianzu_path = "Prefab/tuti/jianzu";
        pos = new Vector3(0, 22, 155);
        var jianzu = Loader.InstantilGameObject(jianzu_path, null, pos);
    }
}
