using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Che_xian_luji_lineData : BaseLineData
{


    public Che_xian_luji_lineData(Severlinedata_paragm paragm)
    {
        lineType = paragm.Substructural_Type;
        isquxian = paragm.CurveT == 0 ? false : true;
        zuozhixian_length = paragm.ST_LenL1;
        huanqu_length = paragm.HH_Len;
        yuan_length = paragm.CR_Len;
        yuan_R = paragm.CR;
        youzhixian_length = paragm.ST_LenR;

        //计算坐标点
        calculatePath();
    }

    public override void calculatePath(string data)
    {
        
    }

    public override void CreatorRoad()
    {
        //这是生成初始端
        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-500),
            new Vector3(0, 0,0),
        };
        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_luji", "初始段");

        foreach (var item in linePath)
        {
            switch (item.lineType)
            {
                case "Guidao_luji":
                    var path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;
                default:
                    break;
            }
        }
    }

    protected override void calculatePath()
    {
        linePath = new List<Baseline>();
        if (isquxian)
        {
            var star_Pos = Vector3.zero;


            //直线段
            var end_pos = new Vector3(0, 0, zuozhixian_length);
            ludi_line luji = new ludi_line("zuozhixian");
            luji.path.Add(star_Pos);
            luji.path.Add(end_pos);
            luji.lineType = "Guidao_luji";
            linePath.Add(luji);

            star_Pos = end_pos;

            //左弧线段
            var temp_tutul_length = huanqu_length + yuan_length + huanqu_length;//弧线总长度
            List<Vector3> luji_2_path = calculateQuxianPath(temp_tutul_length, star_Pos, (int)temp_tutul_length);
            luji = new ludi_line("quxian");
            luji.path = luji_2_path;
            luji.lineType = "Guidao_luji";
            var temp_index = luji_2_path.Count - 1;
            star_Pos = luji_2_path[temp_index];
            linePath.Add(luji);

            //右直线段
            Vector3 startPoint = new Vector3(0, 0, zuozhixian_length);
            float thetaStart = Mathf.Atan2(startPoint.z - center.z, startPoint.x - center.x);
            float totalAngleRad = temp_tutul_length / yuan_R;
            float thetaEnd = thetaStart - totalAngleRad;
            Vector3 tangentDir = new Vector3(Mathf.Sin(thetaEnd), 0, -Mathf.Cos(thetaEnd));
            end_pos = star_Pos + tangentDir * 500;
            ludi_line ludi_4 = new ludi_line("youzhixian");
            ludi_4.lineType = "Guidao_luji";
            ludi_4.path.Add(star_Pos);
            ludi_4.path.Add(end_pos);
            linePath.Add(ludi_4);
        }
        else
        {
            //这里全是路基
            huanqu_length = 0;
            var totulLenth = zuozhixian_length + huanqu_length + yuan_length + huanqu_length + youzhixian_length;
            var star_Pos = Vector3.zero;
            var end_pos = new Vector3(0, 0, totulLenth);
            ludi_line luji = new ludi_line("luji_1");
            luji.path.Add(star_Pos);
            luji.path.Add(end_pos);
            luji.lineType = "Guidao_luji";

            linePath.Add(luji);
        }
    }
}
