using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataPanel : BaseUI
{
    private Dropdown dropdown;
    private Coroutine coroutine;
    private CheXiang chexiang;


    public override void Init()
    {
        base.Init();

        dropdown = m_UiUitil.Get("fandabeishu")._trans.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(dropdownCallback);


        m_UiUitil.Get("cheiang_data/cheti_text")._text.text = "";
        m_UiUitil.Get("cheiang_data/qiangoujia_text")._text.text = "";
        m_UiUitil.Get("cheiang_data/hougoujia_text")._text.text = "";
        m_UiUitil.Get("cheiang_data/chelun_1_text")._text.text = "";
        m_UiUitil.Get("cheiang_data/chelun_2_text")._text.text = "";
        m_UiUitil.Get("cheiang_data/chelun_3_text")._text.text = "";
        m_UiUitil.Get("cheiang_data/chelun_4_text")._text.text = "";
    }


    public void ShowData(string chexiangName,CheXiang chexiangData)
    {
        if (chexiangData.chexiangData == null) return;
        Show();
        chexiang = chexiangData;
        if (coroutine!=null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(delayShowData());
    }


    IEnumerator delayShowData()
    {
        //Debug.LogFormat("显示了{0}的车厢数据", chexiang.CheXiangID);
        while (chexiang!=null)
        {
            yield return new WaitForSeconds(0.1f);

            m_UiUitil.Get("cheiang_data/title")._text.text = string.Format("第{0}节车振动数据", chexiang.CheXiangID+1);

            m_UiUitil.Get("cheiang_data/cheti_text")._text.text = getString("车体", chexiang.chexiangData.cheti_data);
            m_UiUitil.Get("cheiang_data/qiangoujia_text")._text.text = getString("前构架", chexiang.chexiangData.qian_goujia_data);
            m_UiUitil.Get("cheiang_data/hougoujia_text")._text.text = getString("后构架", chexiang.chexiangData.hou_goujia_data);
            m_UiUitil.Get("cheiang_data/chelun_1_text")._text.text = getString("车轮1", chexiang.chexiangData.lun_1_data);
            m_UiUitil.Get("cheiang_data/chelun_2_text")._text.text = getString("车轮2", chexiang.chexiangData.lun_2_data);
            m_UiUitil.Get("cheiang_data/chelun_3_text")._text.text = getString("车轮3", chexiang.chexiangData.lun_3_data);
            m_UiUitil.Get("cheiang_data/chelun_4_text")._text.text = getString("车轮4", chexiang.chexiangData.lun_4_data);
        }
    }

    public override void Hide()
    {
        base.Hide();
        StopAllCoroutines();
    }


    private void dropdownCallback(int index)
    {
        Debug.LogFormat("选择了放大系数{0}", index);
        switch (index)
        {
            case 0:
                GameManager.Instance.fangdaxishu.pos_xishu = 1;
                GameManager.Instance.fangdaxishu.rota_xishu = 1;
                break;
            case 1:
                //放大系数为1000倍，保留俩位小数

                GameManager.Instance.fangdaxishu.pos_xishu = 10;
                GameManager.Instance.fangdaxishu.rota_xishu = 1000;

                break;
            case 2:
                //放大系数为2000倍，保留俩位小数
                GameManager.Instance.fangdaxishu.pos_xishu = 30;
                GameManager.Instance.fangdaxishu.rota_xishu = 5000;
                break;
            case 3:
                //放大系数为3000倍，保留2位小数
                GameManager.Instance.fangdaxishu.pos_xishu = 50;
                GameManager.Instance.fangdaxishu.rota_xishu = 10000;
                break;
            default:
                break;
        }
    }


    private string getString(string name_info,bujian_data bujian)
    {
        string temp_name = "位移:"; 
        if(GameManager.Instance.data_type.Contains("加速度"))
        {
            temp_name = "加速度:";
        }
        else if(GameManager.Instance.data_type.Contains("位移"))
        {
            temp_name = "位移:";
        }


        var x_p = (float)Math.Round(float.Parse(bujian.p_x), 4);
        var y_p = (float)Math.Round(float.Parse(bujian.p_y), 4);
        var z_p = (float)Math.Round(float.Parse(bujian.p_z), 4);

        var x_r = (float)Math.Round(float.Parse(bujian.r_x), 4);
        var y_r = (float)Math.Round(float.Parse(bujian.r_y), 4);
        var z_r = (float)Math.Round(float.Parse(bujian.r_z), 4);


        //string p_Z = name_info + "纵向" + temp_name + bujian.p_z;
        //string p_x = name_info + "横向" + temp_name + bujian.p_x;
        //string p_y = name_info + "垂向" + temp_name + bujian.p_y;
        //string r_x = name_info + "点头角度：" + bujian.r_x;
        //string r_y = name_info + "摇头角度：" + bujian.r_y;
        //string r_z = name_info + "侧滚角度：" + bujian.r_z;


        string p_Z = name_info + "纵向" + temp_name + z_p;
        string p_x = name_info + "横向" + temp_name + x_p;
        string p_y = name_info + "垂向" + temp_name + y_p;
        string r_x = name_info + "点头角度：" + x_r;
        string r_y = name_info + "摇头角度：" + y_r;
        string r_z = name_info + "侧滚角度：" + z_r;

        string temp = p_Z + '\n' + p_x + '\n' + p_y + '\n' + r_x + '\n' + r_y + '\n' + r_z;
        return temp;
    }
}
