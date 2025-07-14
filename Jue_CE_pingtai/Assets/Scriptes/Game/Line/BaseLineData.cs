using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLineData 
{
    /// <summary>
    /// 目前一共有五种
    /// 
    /// </summary>
    public string lineType;

    public float step_index;//单位长度
    public bool isquxian;
    public float huanqu_length;
    public float zuozhixian_length;
    public float youzhixian_length;


    /// <summary>
    /// 圆半径
    /// </summary>
    public float yuan_R;

    /// <summary>
    ///  圆曲线长度
    /// </summary>
    public float yuan_length;

    public List<Baseline> linePath;

    /// <summary>
    /// 创建road
    /// </summary>
    public abstract void CreatorRoad();

    /// <summary>
    /// 计算路径点
    /// </summary>
    protected abstract void calculatePath();

    /// <summary>
    /// 计算路径点
    /// </summary>
    /// <param name="data"></param>
    public abstract void calculatePath(string data);

    public abstract bujian_data CalculatePointAndRotation(float licheng);

}

/// <summary>
/// 基本线路
/// </summary>
public class Baseline
{
    public string Name;
    public int ID;
    public List<Vector3> path = new List<Vector3>();
}

/// <summary>
/// 地基线路
/// </summary>
public class ludi_line : Baseline
{
    public ludi_line(string name)
    {
        this.Name = name;
    }
}

//桥线路
public class qiao_line : Baseline
{
    public int qiaodun_number;
    public List<Vector3> qiaodun_pos = new List<Vector3>();

    public qiao_line(string name)
    {
        this.Name = name;

    }
}

/// <summary>
/// 隧道线路
/// </summary>
public class suidao_line : Baseline
{
    public Vector3 start_pos
    {
        get => path[0];
    }
    public Vector3 end_pos
    {
        get => path[path.Count - 1];
    }
    public float suidao_len;


    public suidao_line(string name)
    {
        this.Name = name;
    }
}
