using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CurveAndStraightPath : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public Vector3 startPoint = Vector3.zero; // 起始点 (x0, y0)
    public float radius = 10f;               // 圆曲线半径 R
    public float curveLength = 15.70796f;    // 圆曲线长度 L (90度弧长示例)
    public float straightLength = 20f;       // 直线长度
    public GameObject pointPrefab;           // 用于标记点的预制体（可选）
    public bool drawWithLineRenderer = true; // 是否用 LineRenderer 绘制
    public bool isClockwise = true;//顺时针或者逆时针

    private void Start()
    {
        //GeneratePath();
        testCurveLine();

    }

    void GeneratePath()
    {
        float thetaStart = Mathf.Atan2(startPoint.z - center.z, startPoint.x - center.x);

        // 3. 计算圆曲线的终点角度和坐标
        float theta = curveLength / radius;
        float thetaEnd = isClockwise ? thetaStart - theta : thetaStart + theta;
        Vector3 endPoint = new Vector3(
            center.x + radius * Mathf.Cos(thetaEnd),0,
            center.z + radius * Mathf.Sin(thetaEnd)
        );

        // 4. 计算切线方向（直线方向）
        Vector3 tangentDir = isClockwise ?
            new Vector3(Mathf.Sin(thetaEnd), 0,-Mathf.Cos(thetaEnd)) : // CW
            new Vector3(-Mathf.Sin(thetaEnd), 0,Mathf.Cos(thetaEnd));  // CCW

        // 5. 计算所有点（圆曲线 + 直线）
        List<Vector3> allPoints = new List<Vector3>();

        // (A) 圆曲线部分（每隔1度采样）
        int numCurvePoints = Mathf.FloorToInt(theta * Mathf.Rad2Deg) + 1;
        for (int i = 0; i <= numCurvePoints; i++)
        {
            float t = i / (float)numCurvePoints;
            float angle = isClockwise ? thetaStart - t * theta : thetaStart + t * theta;
            Vector3 point = new Vector3(
                center.x + radius * Mathf.Cos(angle),0,
                center.z + radius * Mathf.Sin(angle));
            allPoints.Add(new Vector3(point.x, 0, point.z));
        }

        // (B) 直线部分（每隔1单位长度采样）
        int numStraightPoints = Mathf.FloorToInt(straightLength) + 1;
        for (int i = 1; i <= numStraightPoints; i++)
        {
            Vector3 point = endPoint + tangentDir * i;
            allPoints.Add(new Vector3(point.x, 0, point.z));
        }


        // 4. 可视化（生成 GameObject 或 LineRenderer）
        if (drawWithLineRenderer)
        {
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = allPoints.Count;
            lineRenderer.SetPositions(allPoints.ToArray());
            lineRenderer.startWidth = 0.9f;
            lineRenderer.endWidth = 0.9f;
            lineRenderer.material = new Material(Shader.Find("Standard"));
            //lineRenderer.startColor = Color.blue;
            //lineRenderer.endColor = Color.blue;
            //lineRenderer.alignment = LineAlignment.TransformZ;


        }

        // 可选：生成 GameObject 标记点
        if (pointPrefab != null)
        {
            foreach (Vector3 point in allPoints)
            {
                Instantiate(pointPrefab, point, Quaternion.identity);
            }
        }
    }

    void testCurveLine()
    {
        Severlinedata_paragm _Paragm = new Severlinedata_paragm();
        _Paragm.CR = 100;
        _Paragm.CR_Len = 200;
        _Paragm.CurveT = 0;
        _Paragm.ST_LenL1 = 100;
        _Paragm.HH_Len = 50;
        _Paragm.ST_LenR = 500;

        _Paragm.Len_Bri = new float[4]
        {
            32,32,32,32
        };

        _Paragm.XB1 = new float[0];
        _Paragm.YB1 = new float[0];
        _Paragm.ZB1 = new float[0];

        Che_xian_qiao_lineData test = new Che_xian_qiao_lineData(_Paragm);
        float l = Mathf.PI * _Paragm.CR / 2;
        //var temp_list = test.calculateQuxianPath(l, new Vector3(0,0,100), 100);

        //for (int i = 0; i < temp_list.Count; i++)
        //{
        //    GameObject obj = Instantiate(pointPrefab, temp_list[i], Quaternion.identity);
        //    obj.name = i.ToString();

        //}

    }
}
