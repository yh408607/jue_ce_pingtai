using EasyRoads3Dv3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaterRoad 
{
    public static ERRoad road;

    /// <summary>
    /// 创建road
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="roadName"></param>
    public static GameObject CreatRoad_new(Vector3[] controlPoints,string roadName,string name)
    {
        // 1. 获取easyRoads3D API实例
        ERRoadNetwork roadNetwork = new ERRoadNetwork();
        ERRoadType roadType = roadNetwork.roadNetwork.GetRoadTypeByName(roadName);
        if (roadType == null)
        {
            Debug.LogErrorFormat("获取的RoadName不存在，请检查，名字为{0}", roadName);
            return null;
        }

        // 2. 创建道路对象
        if (controlPoints.Length < 2)
        {
            Debug.LogErrorFormat("path数量太小小于2个，现在的数量为{0}个", controlPoints.Length);
        }
            
        
        try
        {
            road = roadNetwork.CreateRoad(name, roadType, controlPoints);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("创建道路失败，请检查参数是否正确，当前道路名字为{0},道路类型为{1},path的数量为{2}", name, roadType.ToString(), controlPoints[1]);
            throw;
        }

        return road.gameObject;
    }

    public static void CreatRoad_new(Vector3[] controlPoints, string roadName,string sideName,string sideLenght)
    {


    }
}
