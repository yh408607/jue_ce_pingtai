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

        //todo����ĳ���ֵ��Ҫģ��
        suidao_len = paragm.L_tunl;
        //suidao_len = 600;

        //���������
        calculatePath();
    }

    
    public override void calculatePath(string data)
    {
        if (isquxian)
        {

        }else
        {

        }
    }

    /// <summary>
    /// ������̼��㵱ǰ��������볯��
    /// </summary>
    /// <param name="licheng"></param>
    /// <returns></returns>
    public override bujian_data CalculatePointAndRotation(float licheng)
    {
        bujian_data data = new bujian_data();


        return data;

    }

    public override void CreatorRoad()
    {
        Debug.Log("��ʼ������-��-��-�� ·��");
    }

    protected override void calculatePath()
    {



        if (isquxian)
        {

        }
        else
        {

        }
    }
}
