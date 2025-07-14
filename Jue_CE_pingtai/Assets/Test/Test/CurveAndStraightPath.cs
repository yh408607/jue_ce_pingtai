using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CurveAndStraightPath : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public Vector3 startPoint = Vector3.zero; // ��ʼ�� (x0, y0)
    public float radius = 10f;               // Բ���߰뾶 R
    public float curveLength = 15.70796f;    // Բ���߳��� L (90�Ȼ���ʾ��)
    public float straightLength = 20f;       // ֱ�߳���
    public GameObject pointPrefab;           // ���ڱ�ǵ��Ԥ���壨��ѡ��
    public bool drawWithLineRenderer = true; // �Ƿ��� LineRenderer ����
    public bool isClockwise = true;

    private void Start()
    {
        GeneratePath();
    }

    void GeneratePath()
    {
        float thetaStart = Mathf.Atan2(startPoint.z - center.z, startPoint.x - center.x);

        // 3. ����Բ���ߵ��յ�ǶȺ�����
        float theta = curveLength / radius;
        float thetaEnd = isClockwise ? thetaStart - theta : thetaStart + theta;
        Vector3 endPoint = new Vector3(
            center.x + radius * Mathf.Cos(thetaEnd),0,
            center.z + radius * Mathf.Sin(thetaEnd)
        );

        // 4. �������߷���ֱ�߷���
        Vector3 tangentDir = isClockwise ?
            new Vector3(Mathf.Sin(thetaEnd), 0,-Mathf.Cos(thetaEnd)) : // CW
            new Vector3(-Mathf.Sin(thetaEnd), 0,Mathf.Cos(thetaEnd));  // CCW

        // 5. �������е㣨Բ���� + ֱ�ߣ�
        List<Vector3> allPoints = new List<Vector3>();

        // (A) Բ���߲��֣�ÿ��1�Ȳ�����
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

        // (B) ֱ�߲��֣�ÿ��1��λ���Ȳ�����
        int numStraightPoints = Mathf.FloorToInt(straightLength) + 1;
        for (int i = 1; i <= numStraightPoints; i++)
        {
            Vector3 point = endPoint + tangentDir * i;
            allPoints.Add(new Vector3(point.x, 0, point.z));
        }


        // 4. ���ӻ������� GameObject �� LineRenderer��
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

        // ��ѡ������ GameObject ��ǵ�
        if (pointPrefab != null)
        {
            foreach (Vector3 point in allPoints)
            {
                Instantiate(pointPrefab, point, Quaternion.identity);
            }
        }
    }
}
