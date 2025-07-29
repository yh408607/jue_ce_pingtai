using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public abstract class BaseLineData 
{
    /// <summary>
    /// 目前一共有五种
    /// 
    /// </summary>
    public string lineType;

    public float step_index =1;//单位长度
    public bool isquxian;
    public float huanqu_length;
    public float zuozhixian_length;
    public float youzhixian_length;

    protected Vector3 center
    {
        get
        {
            return new Vector3(yuan_R, 0, zuozhixian_length);
        }
    }

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

    public virtual bujian_data CalculatePointAndRotation(float licheng)
    {
        bujian_data data = new bujian_data();

        if (isquxian)
        {


            return CalculatePointAndRotation_Quxian(licheng);
        }
        else
        {

            return CalculatePointAndRotation_Zhixian(licheng);
        }

        return data;
    }

    /// <summary>
    /// 计算曲线上的路基点
    /// </summary>
    /// <param name="length">长度</param>
    /// <param name="start_pos">起始位置</param>
    /// <param name="step">细分段</param>
    /// <returns></returns>
    protected virtual List<Vector3> calculateQuxianPath(float length,Vector3 start_pos,int step)
    {
        float thetaStart = Mathf.Atan2(start_pos.z - center.z, start_pos.x - center.x);
        //float theta = length / yuan_R;
        //float thetaEnd = thetaStart - theta;

        List<Vector3> allPoints = new List<Vector3>();

        for (int i = 0; i < step; i++)
        {
            float l = length / step * i;
            float theta = l / yuan_R;
            float thetaEnd = thetaStart - theta;
            Vector3 pos = new Vector3(
                center.x + yuan_R * Mathf.Cos(thetaEnd), 0,
                center.z + yuan_R * Mathf.Sin(thetaEnd)
            );
            allPoints.Add(pos);
        }

        return allPoints;
    }

    /// <summary>
    /// 计算直线上的路径点
    /// </summary>
    /// <param name="length"></param>
    /// <param name="star_pos"></param>
    /// <param name="tangentDir"></param>
    /// <param name="thetaStart"></param>
    /// <returns></returns>
    protected virtual List<Vector3> calculateZhixianPath(float length,Vector3 star_pos, Vector3 tangentDir, float thetaStart )
    {
        List<Vector3> allPoints = new List<Vector3>();
        int step = (int)length;

        for (int i = 0; i < step; i++)
        {
            float l = length / step * i;
            float theta = l / yuan_R;
            float thetaEnd = thetaStart - theta;
            Vector3 pos = star_pos + tangentDir * 500;
            allPoints.Add(pos);
        }

        return allPoints;
    }
    /// <summary>
    /// 根据起始点位置，长度形成坐标点
    /// </summary>
    /// <param name="length"></param>
    /// <param name="start_pos"></param>
    /// <returns></returns>
    protected virtual Vector3 calculatePosForLength(float length,Vector3 start_pos)
    {
        Vector3 temp = new Vector3(0, 0, 0);
        float thetaStart = Mathf.Atan2(start_pos.z - center.z, start_pos.x - center.x);
        float theta = length / yuan_R;
        float thetaEnd = thetaStart - theta;//顺时针选择

        temp = new Vector3(
                    center.x + yuan_R * Mathf.Cos(thetaEnd), 0,
                    center.z + yuan_R * Mathf.Sin(thetaEnd)
        );
        return temp;
    }

    protected virtual bujian_data CalculatePointAndRotation_Quxian(float licheng)
    {
        bujian_data data = new bujian_data();
        //曲线计算曲线路径
        //Debug.LogError("曲线还未计算");
        var zhixian_quxian = zuozhixian_length + huanqu_length * 2 + yuan_length;
        var star_y = TrainController.Instance.Start_Hitgh;//初始高度

        if (licheng <= zuozhixian_length)
        {
            //计算直线
            //var star_y = TrainController.Instance.Start_Hitgh;//初始高度

            Vector3 pos = new Vector3(0, 0, licheng);
            Vector3 rota = new Vector3(0, 0, 0);

            data.positon = pos;
            data.rotation = rota;
        }
        else if (licheng > zuozhixian_length && licheng <= zhixian_quxian)
        {
            //里程在曲线内
            //var temp = zhixian_quxian - zuozhixian_length;
            var temp = licheng - zuozhixian_length;
            var start_pos = new Vector3(0, 0, zuozhixian_length);

            float thetaStart = Mathf.Atan2(start_pos.z - center.z, start_pos.x - center.x);
            float theta = temp / yuan_R;
            float thetaEnd = thetaStart - theta;//顺时针选择

            Vector3 pos = new Vector3(
                        center.x + yuan_R * Mathf.Cos(thetaEnd), 0,
                        center.z + yuan_R * Mathf.Sin(thetaEnd)
            );

            // 4. 计算切线方向（直线方向）
            //将弧度转为角度，
            float degree = thetaEnd * Mathf.Rad2Deg;
            var rota = new Vector3(0, 180 - degree, 0);

            data.positon = pos;
            data.rotation = rota;
        }
        else
        {
            //里程在曲线外，进入到右直线了
            var start_pos = new Vector3(0, star_y, zuozhixian_length);
            float thetaStart = Mathf.Atan2(start_pos.z - center.z, start_pos.x - center.x);
            float theta = (huanqu_length * 2 + yuan_length) / yuan_R;
            float thetaEnd = thetaStart - theta;//顺时针选择

            Vector3 endPos = new Vector3(
                        center.x + yuan_R * Mathf.Cos(thetaEnd), 0,
                        center.z + yuan_R * Mathf.Sin(thetaEnd)
            );

            // 4. 计算切线方向（直线方向）
            Vector3 tangentDir = new Vector3(Mathf.Sin(thetaEnd), 0, -Mathf.Cos(thetaEnd));
            var temp_length = licheng - zhixian_quxian;
            Vector3 point = endPos + tangentDir * temp_length;

            //将弧度转为角度，
            float degree = thetaEnd * Mathf.Rad2Deg;
            var rota = new Vector3(0, 180 - degree, 0);

            data.positon = point;
            data.rotation = rota;
        }

        return data;
    }

    protected virtual bujian_data CalculatePointAndRotation_Zhixian(float licheng)
    {
        bujian_data data = new bujian_data();
        //曲线计算曲线路径
        //Debug.LogError("曲线还未计算");
        var zhixian_quxian = zuozhixian_length + huanqu_length * 2 + yuan_length;

        //计算直线
        var star_y = TrainController.Instance.Start_Hitgh;//初始高度

        Vector3 pos = new Vector3(0, 0, licheng);
        Vector3 rota = new Vector3(0, 0, 0);

        data.positon = pos;
        data.rotation = rota;

        return data;
    }

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
