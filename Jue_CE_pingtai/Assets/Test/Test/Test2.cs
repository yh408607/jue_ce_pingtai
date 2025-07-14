using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //    var curve = CurveCalculator.CalculateFullCurve(
        //        L1: 100,     // ��ʼֱ�߳���
        //        L2: 50,      // ��һ�������߳���
        //        R: 200,      // Բ���߰뾶
        //        L3: Math.PI * 100,  // Բ���߳��ȣ���ӦԲ�ĽǦл��ȣ�
        //        L4: 50,      // �ڶ��������߳���
        //        L5: 100,     // ����ֱ�߳���
        //        pointsPerSegment: 100);

        //    // ���ǰ10���������
        //    for (int i = 0; i < 100; i++)
        //    {
        //        //Console.WriteLine($"Point {i}: X={curve[i].X:F2}, Y={curve[i].Y:F2}, Theta={curve[i].Theta:F2} rad");
        //        Debug.LogFormat("�����λPint[{0}],x = {1},y={2},Thete={3}", i, curve[i].X, curve[i].Y, curve[i].Theta);
        //    }
        //}

        var starpoint = new Vector3(-1, 0, Mathf.Sqrt(3));
        calculateYuanPoint(starpoint, Mathf.PI/6, 2.0f);


        Debug.Log(Mathf.Atan2(0 - 0, -1 - 0));
    }


    //����Բ�����
    public List<Vector3> calculateYuanPoint(Vector3 startPoint, float alph_end, float yuan_R)
    {
        List<Vector3> points = new List<Vector3>();
        //����Բ������
        var o_x = startPoint.x + yuan_R * Mathf.Sin(alph_end);
        var o_z = startPoint.z - yuan_R * Mathf.Cos(alph_end);

        var o = new Vector3(o_x, 0, o_z);

        Debug.LogFormat("�������{0}", startPoint);
        Debug.LogFormat("Բ�������Ϊ{0}", o);


        //var c = licheng / step_index;
        //for (int i = 1; i < c; i++)
        //{
        //    var alph = (i * step_index) / yuan_R;
        //    var x_temp = o_x + yuan_R * (Mathf.Sin(alph_end) + Mathf.Sin(alph - alph_end));
        //    var z_temp = o_z + yuan_R * (-Mathf.Cos(alph_end) + Mathf.Cos(alph_end - alph));
        //}

        //��������
        var t = Mathf.Atan2(startPoint.z - o_z, startPoint.x - o_x);

        Debug.LogFormat("������Ƕ�{0}", t);

        //˳ʱ��ת�Ǽ�
        var x_temp = o_x + yuan_R * Mathf.Cos(t - (Mathf.PI / 3));
        var z_temp = o_z + yuan_R * Mathf.Sin(t - (Mathf.PI / 3));

        o = new Vector3(x_temp, 0, z_temp);
        Debug.LogFormat("˳ʱ����ת30��֮�������Ϊ{0}", o);


        return points;
    }

}


public class CurvePoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Theta { get; set; } // ��ǰ������߷��򣨻��ȣ�

    public CurvePoint(double x, double y, double theta)
    {
        X = x;
        Y = y;
        Theta = theta;
    }
}

public class CurveCalculator
{
    // ��ֵ���ַ���������ɭ����
    private static double SimpsonIntegral(Func<double, double> f, double a, double b, int n = 1000)
    {
        double h = (b - a) / n;
        double sum = f(a) + f(b);
        for (int i = 1; i < n; i++)
        {
            double x = a + i * h;
            sum += f(x) * (i % 2 == 0 ? 2 : 4);
        }
        return sum * h / 3;
    }

    // ���㻺�����ߣ������ߣ�����
    private static List<CurvePoint> CalculateClothoid(
        CurvePoint startPoint,
        double length,
        double startCurvature,
        double endCurvature,
        int points)
    {
        List<CurvePoint> result = new List<CurvePoint>();
        double deltaCurvature = endCurvature - startCurvature;

        for (int i = 0; i <= points; i++)
        {
            double s = (double)i / points * length;
            double curvature = startCurvature + deltaCurvature * s / length;

            // ��ֵ���ּ�������仯
            double dx = SimpsonIntegral(u => Math.Cos(startPoint.Theta + startCurvature * u + 0.5 * deltaCurvature / length * u * u), 0, s);
            double dy = SimpsonIntegral(u => Math.Sin(startPoint.Theta + startCurvature * u + 0.5 * deltaCurvature / length * u * u), 0, s);

            double newX = startPoint.X + dx;
            double newY = startPoint.Y + dy;
            double newTheta = startPoint.Theta + startCurvature * s + 0.5 * deltaCurvature / length * s * s;

            result.Add(new CurvePoint(newX, newY, newTheta));
        }
        return result;
    }

    public static List<CurvePoint> CalculateFullCurve(
        double L1, double L2, double R, double L3, double L4, double L5,
        int pointsPerSegment = 100)
    {
        List<CurvePoint> fullCurve = new List<CurvePoint>();

        // 1. ��ʼֱ�߶�L1 (Y�᷽��)
        CurvePoint currentPoint = new CurvePoint(0, 0, Math.PI / 2);
        for (int i = 0; i <= pointsPerSegment; i++)
        {
            double s = (double)i / pointsPerSegment * L1;
            fullCurve.Add(new CurvePoint(
                0,
                s,
                Math.PI / 2));
        }
        currentPoint = new CurvePoint(0, L1, Math.PI / 2);

        // 2. ��һ��������L2�����ʴ�0��1/R��
        var spiral1 = CalculateClothoid(
            currentPoint,
            L2,
            0,
            1 / R,
            pointsPerSegment);
        fullCurve.AddRange(spiral1);
        currentPoint = spiral1[spiral1.Count - 1];

        // 3. Բ����L3
        double circleAngle = L3 / R;  // Բ�Ľ�
        double thetaStartCircle = currentPoint.Theta;
        double cx = currentPoint.X + R * Math.Cos(thetaStartCircle + Math.PI / 2); // Բ������
        double cy = currentPoint.Y + R * Math.Sin(thetaStartCircle + Math.PI / 2);

        for (int i = 0; i <= pointsPerSegment; i++)
        {
            double angle = (double)i / pointsPerSegment * circleAngle;
            double x = cx + R * Math.Cos(thetaStartCircle - angle + Math.PI / 2);
            double y = cy + R * Math.Sin(thetaStartCircle - angle + Math.PI / 2);
            fullCurve.Add(new CurvePoint(x, y, thetaStartCircle - angle));
        }
        currentPoint = fullCurve[fullCurve.Count - 1];

        // 4. �ڶ���������L4�����ʴ�1/R��0��
        var spiral2 = CalculateClothoid(
            currentPoint,
            L4,
            1 / R,
            0,
            pointsPerSegment);
        fullCurve.AddRange(spiral2);
        currentPoint = spiral2[spiral2.Count - 1];

        // 5. ����ֱ�߶�L5
        for (int i = 0; i <= pointsPerSegment; i++)
        {
            double s = (double)i / pointsPerSegment * L5;
            fullCurve.Add(new CurvePoint(
                currentPoint.X + s * Math.Cos(currentPoint.Theta),
                currentPoint.Y + s * Math.Sin(currentPoint.Theta),
                currentPoint.Theta));
        }

        return fullCurve;
    }
}

//// ʹ��ʾ��
//class Program
//{
//    static void Main()
//    {
//        var curve = CurveCalculator.CalculateFullCurve(
//            L1: 100,     // ��ʼֱ�߳���
//            L2: 50,      // ��һ�������߳���
//            R: 200,      // Բ���߰뾶
//            L3: Math.PI * 100,  // Բ���߳��ȣ���ӦԲ�ĽǦл��ȣ�
//            L4: 50,      // �ڶ��������߳���
//            L5: 100,     // ����ֱ�߳���
//            pointsPerSegment: 50);

//        // ���ǰ10���������
//        for (int i = 0; i < 10; i++)
//        {
//            Console.WriteLine($"Point {i}: X={curve[i].X:F2}, Y={curve[i].Y:F2}, Theta={curve[i].Theta:F2} rad");
//        }
//    }
//}