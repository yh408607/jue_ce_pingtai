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
        throw new System.NotImplementedException();
    }

    public override void CreatorRoad()
    {
        throw new System.NotImplementedException();
    }

    protected override void calculatePath()
    {
        linePath = new List<Baseline>();
        if (isquxian)
        {

        }
        else
        {
            //这里全是路基
        }
    }
}
