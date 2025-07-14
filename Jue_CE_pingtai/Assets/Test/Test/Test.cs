using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;

public class Test : MonoBehaviour
{
    //private ERModularRoad road;
    //public Material bridgeMaterial;
    //public ERModularBase roadsystem;
    //public GameObject sideGameObject;

    public GameObject prefab_sphere;

    public GameObject bridg;


    // Start is called before the first frame update
    void Start()
    {
        //ERModularRoad road = roadsystem.CreateRoad("RoadWithSideObjects");

        CreatRoad();
        //CreatRoad_new();
        //TestFont();
    }


    private void TestFont()
    {
        // 在代码中打印字体包含的字符
        Font font = Resources.Load<Font>("font/AGENCYB");
        if (font != null && font.HasCharacter('中'))
        {
            Debug.Log("字体包含中文字符");
        }
        else
        {
            Debug.LogError("字体缺失中文支持！");
        }
    }
   
    private void CreatRoad()
    {
        // 1. 获取easyRoads3D API实例
        ERRoadNetwork roadNetwork = new ERRoadNetwork();
        //roadNetwork.roadNetwork
        // var sideNames = roadNetwork.roadNetwork.selectedObjects;
        // roadNetwork.roadNetwork.selectedRoadType = 5;
        // // 2. 创建道路对象
        // ERRoad road = new ERRoad();
        // road = roadNetwork.CreateRoad("yangxu");
        // var roads = roadNetwork.GetRoads();
        // List<ERSORoadExt> sides = addSide();

        // road.SetSideObjects(sides);

        //// Debug.Log(sideNames.Count);
        // 3. 设置道路控制点（必须至少3个点）
        Vector3[] controlPoints = new Vector3[6] {
             new Vector3(0,0, 0),
             new Vector3(0, 0, 50),
              new Vector3(0, 0, 60),
                            new Vector3(0, 0, 70),
             new Vector3(0, 0, 200),
             new Vector3(0,0,300)
         };
        // road.AddMarkers(controlPoints);

        // roadNetwork.BuildRoadNetwork();



        ERRoadType roadType = roadNetwork.roadNetwork.GetRoadTypeByName("Guidao_qiao");

        var side = roadNetwork.roadNetwork.QOQDQOOQDDQOOQ;

        //SideObject side_object = null;
        //foreach (SideObject item in side)
        //{
        //    if(item.name== "qiao_dun")
        //    {
        //        item.defaultStartOffset = 0;
        //        item.m_distance= 100;
        //        side_object = item;
                
        //    }
            
        //}

        //StartCoroutine(delayUpdate(roadNetwork, side_object));

        ERRoad road = roadNetwork.CreateRoad("new_road", roadType, controlPoints);

        //road.SetSideObjects(sides);
    }

    IEnumerator delayUpdate(ERRoadNetwork roadNetwork,SideObject side)
    {

        yield return new WaitForSeconds(2.0f);
        side.defaultStartOffset = 0;
        Debug.Log("执行了赋值");
        yield return new WaitForSeconds(2.0f);
        roadNetwork.roadNetwork.UpdateSideObjectsInScene();
        Debug.Log("执行了更新");

    }

    private List<ERSORoadExt> addSide()
    {
        List<ERSORoadExt> sides = new List<ERSORoadExt>();

        ERSORoadExt side = new ERSORoadExt();
        side.active = true;
        //side.sideObject.name = "side_new";
        SideObject sideObject = new SideObject();
       // sideObject.sourceObject = sideGameObject;
        side.sideObject = sideObject;
        side.sideObject.name = "side_new";
        sideObject.m_distance = 43;
        sideObject.sourceObject = bridg;
        sides.Add(side);

        return sides;
    }



    private void CreatRoad_new()
    {
        // 1. 获取easyRoads3D API实例
        //ERRoadNetwork roadNetwork = new ERRoadNetwork();
        //ERRoadType roadType = roadNetwork.roadNetwork.GetRoadTypeByName("Guidao_luji");
        // 2. 创建道路对象

        //// 3. 设置道路控制点（必须至少3个点）
        //Vector3[] controlPoints = new Vector3[4] {
        //    new Vector3(0,30, 0),
        //    new Vector3(0, 30, 100),
        //    new Vector3(0, 30, 200),
        //    new Vector3(150,30,300)
        //};

        ////ERRoad road = roadNetwork.CreateRoad("yangxu", roadType, controlPoints);
        ////Debug.Log(name);

        //CreaterRoad.CreatRoad_new(controlPoints, "Guidao_luji");

        //10米一个点去生成，生成200米距离

        int R = 10;
        int L = 20;
        float step = L/ 20;
        Vector3 start = new Vector3(0, 0, 0);
        Vector3 c_o = new Vector3(10, 0, 0);
        var t = Mathf.Atan2(start.z - c_o.z, start.x - c_o.x);


        List<Vector3> path = new List<Vector3>();
        for (int i = 0; i <= 20; i++)
        {
            Vector3 pos = new Vector3();
            var end = t - (step * i) / R;
            var x = c_o.x+ R * Mathf.Cos(end);
            var z = c_o.z + R * Mathf.Sin(end);
            pos.x = x;
            pos.y = 0;
            pos.z = z;

            path.Add(pos);
        }

        //CreaterRoad.CreatRoad_new(path.ToArray(), "Guidao_luji");
        int j = 0;
        foreach (var item in path)
        {    
            var cub = Instantiate(prefab_sphere, item, Quaternion.identity);
            cub.name = j.ToString();
            cub.transform.SetParent(this.transform);
            j++;
        }

    }

    
}
