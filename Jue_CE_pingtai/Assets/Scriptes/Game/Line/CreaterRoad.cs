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
        road = roadNetwork.CreateRoad(name, roadType, controlPoints);

        return road.gameObject;
    }

    public static void CreatRoad_new(Vector3[] controlPoints, string roadName,string sideName,string sideLenght)
    {


    }
}
