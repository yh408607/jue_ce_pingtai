using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Che_xian_lineData : BaseLineData
{

    public Che_xian_lineData(Severlinedata_paragm paragm)
    {
        lineType = paragm.Substructural_Type;
        isquxian = paragm.CurveT == 0 ? false : true;
        zuozhixian_length = paragm.ST_LenL1;
        huanqu_length = paragm.HH_Len;
        yuan_length = paragm.CR_Len;
        yuan_R = paragm.CR;
        youzhixian_length = paragm.ST_LenR;
        //���������
        calculatePath();

        // ���õ���λ��
        setDimianPos();
    }



    public override void calculatePath(string data)
    {
       
    }

    public override void CreatorRoad()
    {
        //�������ɳ�ʼ��
        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-300),
            new Vector3(0, 0,0),
        };
        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_ludi", "��ʼ��");
        foreach (var item in linePath)
        {
            switch (item.lineType)
            {
                case "Guidao_ludi":
                    var path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_ludi", item.Name);
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
            //ֱ�߶�
            var end_pos = new Vector3(0, 0, zuozhixian_length);
            ludi_line luji = new ludi_line("zuozhixian");
            luji.path.Add(star_Pos);
            luji.path.Add(end_pos);
            luji.lineType = "Guidao_ludi";
            linePath.Add(luji);

            star_Pos = end_pos;

            //���߶�
            var temp_tutul_length = huanqu_length + yuan_length + huanqu_length;//�����ܳ���
            List<Vector3> luji_2_path = calculateQuxianPath(temp_tutul_length, star_Pos, (int)temp_tutul_length);
            luji = new ludi_line("quxian");
            luji.path = luji_2_path;
            luji.lineType = "Guidao_ludi";
            var temp_index = luji_2_path.Count - 1;
            star_Pos = luji_2_path[temp_index];
            linePath.Add(luji);

            //��ֱ�߶�
            Vector3 startPoint = new Vector3(0, 0, zuozhixian_length);
            float thetaStart = Mathf.Atan2(startPoint.z - center.z, startPoint.x - center.x);
            float totalAngleRad = temp_tutul_length / yuan_R;
            float thetaEnd = thetaStart - totalAngleRad;
            Vector3 tangentDir = new Vector3(Mathf.Sin(thetaEnd), 0, -Mathf.Cos(thetaEnd));
            end_pos = star_Pos + tangentDir * 500;
            ludi_line ludi_4 = new ludi_line("youzhixian");
            ludi_4.lineType = "Guidao_ludi";
            ludi_4.path.Add(star_Pos);
            ludi_4.path.Add(end_pos);
            linePath.Add(ludi_4);
        }
        else
        {
            //����ȫ��·��
            huanqu_length = 0;
            var totulLenth = zuozhixian_length + huanqu_length + yuan_length + huanqu_length + youzhixian_length;
            var star_Pos = Vector3.zero;
            var end_pos = new Vector3(0, 0, totulLenth);
            ludi_line luji = new ludi_line("zhixian");
            luji.path.Add(star_Pos);
            luji.path.Add(end_pos);
            luji.lineType = "Guidao_ludi";

            linePath.Add(luji);
        }
    }
    /// <summary>
    /// ��-�� ���ǵ������̹������Ҫ�ѵ���λ�����ú�
    /// </summary>
    private void setDimianPos()
    {
        var dimian = GameObject.Find("����");
        if(dimian != null)
        {
            var pos = dimian.transform.position;
            pos.y =- 0.1f; // ���õ���߶�
            dimian.transform.position = pos;
        }
        else
        {
            Debug.LogWarning("�������δ�ҵ�����ȷ������������Ϊ'����'�Ķ���");
        }
    }
}
