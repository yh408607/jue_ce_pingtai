using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainData
{
    //这是每一个里程的数据列车数据
    private List<LichengDta> licheng_data = new List<LichengDta>();
    public int data_Count
    {
        get
        {
            return licheng_data.Count;
        }
    }

    //public List<LichengDta> Licheng_data { get => licheng_data; }

    public LichengDta GetLicheng_Data(int index)
    {
        if(index >= licheng_data.Count)
        {
            Debug.LogFormat("当前获取的索引为{0},数据的长度为{1}", index, licheng_data.Count);
            return null;
        }

        return licheng_data[index];
    }


    public void GenerLichengData(LichengDta lichengDta)
    {
        licheng_data.Add(lichengDta);
    }
}

/// <summary>
/// 总共里程数据
/// </summary>
public class LichengDta
{
    public float licheng;

    //private List<string[]> che_xiang_data;

    public List<chexiangData> chexiangdata_list { get; private set; }//每个车厢数据

    public LichengDta(string[] datas)
    {
        List<string[]> che_xiang_data = new List<string[]>();
        chexiangdata_list = new List<chexiangData>();


        if (!float.TryParse(datas[0], out licheng))
        {
            Debug.LogErrorFormat("里程数无法转为float{0}", datas[0]);
        }

        for (int i = 1; i < datas.Length; i+=42)
        {
            int remaining = datas.Length - i;
            int currentChunkSize = Mathf.Min(42, remaining);
            string[] chunk = new string[currentChunkSize];
            Array.Copy(datas, i, chunk, 0, currentChunkSize);
            che_xiang_data.Add(chunk);
        }

        foreach (string[] item in che_xiang_data)
        {
            chexiangData chexiangData = new chexiangData(item);
            chexiangdata_list.Add(chexiangData);
        }
    }
}

/// <summary>
/// 每个里程中的车厢数据
/// </summary>
public class chexiangData
{

    public bujian_data cheti_data { get; }
    public bujian_data qian_goujia_data { get; }
    public bujian_data hou_goujia_data { get; }
    public bujian_data lun_1_data
    {
        get;
    }
    public bujian_data lun_2_data
    {
        get;
    }
    public bujian_data lun_3_data
    {
        get;
    }
    public bujian_data lun_4_data
    {
        get;
    }

    public chexiangData()
    {

    }


    public chexiangData(string[] datas)
    {
        List<string[]> temp_datas = new List<string[]>();

        for (int i = 0; i < datas.Length; i += 6)
        {
            int remaining = datas.Length - i;
            int currentChunkSize = Mathf.Min(6, remaining);
            string[] chunk = new string[currentChunkSize];
            Array.Copy(datas, i, chunk, 0, currentChunkSize);

            //var data = new chexiangData();
            temp_datas.Add(chunk);
        }

        cheti_data = jiexidata("cheti", temp_datas[0]);
        qian_goujia_data = jiexidata("qiangoujia", temp_datas[1]);
        hou_goujia_data = jiexidata("hougoujia", temp_datas[2]);

        var p_lun_1_z = datas[18];
        var p_lun_2_z = datas[19];
        var p_lun_3_z = datas[20];
        var p_lun_4_z = datas[21];

        var p_lun_1_x = datas[22];
        var p_lun_2_x = datas[23];
        var p_lun_3_x = datas[24];
        var p_lun_4_x = datas[25];

        var p_lun_1_y = datas[30];
        var p_lun_2_y = datas[31];
        var p_lun_3_y = datas[32];
        var p_lun_4_y = datas[33];

        var r_lun_1_y = datas[26];
        var r_lun_2_y = datas[27];
        var r_lun_3_y = datas[28];
        var r_lun_4_y = datas[29];

        var r_lun_1_z = datas[34];
        var r_lun_2_z = datas[35];
        var r_lun_3_z = datas[36];
        var r_lun_4_z = datas[37];

        var r_lun_1_x = datas[38];
        var r_lun_2_x = datas[39];
        var r_lun_3_x = datas[40];
        var r_lun_4_x = datas[41];

        lun_1_data = jiexidata("chelun_1",p_lun_1_x, p_lun_1_y, p_lun_1_z, r_lun_1_x, r_lun_1_y, r_lun_1_z);
        lun_2_data = jiexidata("chelun_2",p_lun_2_x, p_lun_2_y, p_lun_2_z, r_lun_2_x, r_lun_2_y, r_lun_2_z);
        lun_3_data = jiexidata("chelun_3",p_lun_3_x, p_lun_3_y, p_lun_3_z, r_lun_3_x, r_lun_3_y, r_lun_3_z);
        lun_4_data = jiexidata("chelun_4",p_lun_4_x, p_lun_4_y, p_lun_4_z, r_lun_4_x, r_lun_4_y, r_lun_4_z);

    }

    private bujian_data jiexidata(string name,string[] data_step)
    {
        bujian_data data = new bujian_data();
        data.Name = name;

        var pox_z = float.Parse(data_step[0]);
        var pos_x = float.Parse(data_step[1]);
        var pos_y = float.Parse(data_step[2]);

        var rota_y = float.Parse(data_step[3]);
        var rota_x = float.Parse(data_step[4]);
        var roata_z =float.Parse(data_step[5]);

        data.positon = new Vector3(pos_x, pos_y, pox_z);
        data.rotation = new Vector3(rota_x, rota_y, roata_z);

        data.p_x = data_step[1];
        data.p_y = data_step[2];
        data.p_z = data_step[0];

        data.r_x = data_step[4];
        data.r_y = data_step[3];
        data.r_z = data_step[5];

        return data;
    }

    private bujian_data jiexidata(string Name,string p_x,string p_y,string p_z, string r_x, string r_y, string r_z)
    {
        bujian_data data = new bujian_data();

        //var x_p =(float) Math.Round(float.Parse(p_x), 2);
        //var y_p =(float) Math.Round(float.Parse(p_y), 2);
        //var z_p =(float) Math.Round(float.Parse(p_z), 2);

        //var x_r =(float) Math.Round(float.Parse(r_x), 2);
        //var y_r =(float) Math.Round(float.Parse(r_y), 2);
        //var z_r =(float) Math.Round(float.Parse(r_z), 2);

        var x_p = float.Parse(p_x);
        var y_p = float.Parse(p_y);
        var z_p = float.Parse(p_z);

        var x_r = float.Parse(r_x);
        var y_r = float.Parse(r_y);
        var z_r = float.Parse(r_z);

        data.positon = new Vector3(x_p, y_p, z_p);
        data.rotation = new Vector3(x_r, y_r, z_r);

        data.p_x = p_x;
        data.p_y = p_y;
        data.p_z = p_z;
        data.r_x = r_x;
        data.r_y = r_y;
        data.r_z = r_z;

        return data;
    }

}

/// <summary>
/// 部件的数据
/// </summary>
public struct bujian_data
{
    public string Name;

    public string p_x;
    public string p_y;
    public string p_z;

    public string r_x;
    public string r_y;
    public string r_z;

    public Vector3 positon;
    public Vector3 rotation;
}

/// <summary>
/// 放大系数
/// </summary>
public struct fangdaxishu
{
    public int pos_xishu;//位移的放大系数
    public int rota_xishu;//旋转的放大系数
}