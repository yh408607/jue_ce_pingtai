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
        //        L1: 100,     // 初始直线长度
        //        L2: 50,      // 第一缓和曲线长度
        //        R: 200,      // 圆曲线半径
        //        L3: Math.PI * 100,  // 圆曲线长度（对应圆心角π弧度）
        //        L4: 50,      // 第二缓和曲线长度
        //        L5: 100,     // 最终直线长度
        //        pointsPerSegment: 100);

        //    // 输出前10个点的坐标
        //    for (int i = 0; i < 100; i++)
        //    {
        //        //Console.WriteLine($"Point {i}: X={curve[i].X:F2}, Y={curve[i].Y:F2}, Theta={curve[i].Theta:F2} rad");
        //        Debug.LogFormat("坐标点位Pint[{0}],x = {1},y={2},Thete={3}", i, curve[i].X, curve[i].Y, curve[i].Theta);
        //    }
        //}

        var starpoint = new Vector3(-1, 0, Mathf.Sqrt(3));
        calculateYuanPoint(starpoint, Mathf.PI/6, 2.0f);


        Debug.Log(Mathf.Atan2(0 - 0, -1 - 0));
    }


    //计算圆坐标点
    public List<Vector3> calculateYuanPoint(Vector3 startPoint, float alph_end, float yuan_R)
    {
        List<Vector3> points = new List<Vector3>();
        //计算圆心坐标
        var o_x = startPoint.x + yuan_R * Mathf.Sin(alph_end);
        var o_z = startPoint.z - yuan_R * Mathf.Cos(alph_end);

        var o = new Vector3(o_x, 0, o_z);

        Debug.LogFormat("起点坐标{0}", startPoint);
        Debug.LogFormat("圆心坐标点为{0}", o);


        //var c = licheng / step_index;
        //for (int i = 1; i < c; i++)
        //{
        //    var alph = (i * step_index) / yuan_R;
        //    var x_temp = o_x + yuan_R * (Mathf.Sin(alph_end) + Mathf.Sin(alph - alph_end));
        //    var z_temp = o_z + yuan_R * (-Mathf.Cos(alph_end) + Mathf.Cos(alph_end - alph));
        //}

        //先求极坐标
        var t = Mathf.Atan2(startPoint.z - o_z, startPoint.x - o_x);

        Debug.LogFormat("极坐标角度{0}", t);

        //顺时针转是减
        var x_temp = o_x + yuan_R * Mathf.Cos(t - (Mathf.PI / 3));
        var z_temp = o_z + yuan_R * Mathf.Sin(t - (Mathf.PI / 3));

        o = new Vector3(x_temp, 0, z_temp);
        Debug.LogFormat("顺时针旋转30°之后的坐标为{0}", o);


        return points;
    }

}


public class CurvePoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Theta { get; set; } // 当前点的切线方向（弧度）

    public CurvePoint(double x, double y, double theta)
    {
        X = x;
        Y = y;
        Theta = theta;
    }
}

public class CurveCalculator
{
    // 数值积分方法（辛普森法则）
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

    // 计算缓和曲线（螺旋线）坐标
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

            // 数值积分计算坐标变化
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

        // 1. 初始直线段L1 (Y轴方向)
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

        // 2. 第一缓和曲线L2（曲率从0到1/R）
        var spiral1 = CalculateClothoid(
            currentPoint,
            L2,
            0,
            1 / R,
            pointsPerSegment);
        fullCurve.AddRange(spiral1);
        currentPoint = spiral1[spiral1.Count - 1];

        // 3. 圆曲线L3
        double circleAngle = L3 / R;  // 圆心角
        double thetaStartCircle = currentPoint.Theta;
        double cx = currentPoint.X + R * Math.Cos(thetaStartCircle + Math.PI / 2); // 圆心坐标
        double cy = currentPoint.Y + R * Math.Sin(thetaStartCircle + Math.PI / 2);

        for (int i = 0; i <= pointsPerSegment; i++)
        {
            double angle = (double)i / pointsPerSegment * circleAngle;
            double x = cx + R * Math.Cos(thetaStartCircle - angle + Math.PI / 2);
            double y = cy + R * Math.Sin(thetaStartCircle - angle + Math.PI / 2);
            fullCurve.Add(new CurvePoint(x, y, thetaStartCircle - angle));
        }
        currentPoint = fullCurve[fullCurve.Count - 1];

        // 4. 第二缓和曲线L4（曲率从1/R到0）
        var spiral2 = CalculateClothoid(
            currentPoint,
            L4,
            1 / R,
            0,
            pointsPerSegment);
        fullCurve.AddRange(spiral2);
        currentPoint = spiral2[spiral2.Count - 1];

        // 5. 最终直线段L5
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

//// 使用示例
//class Program
//{
//    static void Main()
//    {
//        var curve = CurveCalculator.CalculateFullCurve(
//            L1: 100,     // 初始直线长度
//            L2: 50,      // 第一缓和曲线长度
//            R: 200,      // 圆曲线半径
//            L3: Math.PI * 100,  // 圆曲线长度（对应圆心角π弧度）
//            L4: 50,      // 第二缓和曲线长度
//            L5: 100,     // 最终直线长度
//            pointsPerSegment: 50);

//        // 输出前10个点的坐标
//        for (int i = 0; i < 10; i++)
//        {
//            Console.WriteLine($"Point {i}: X={curve[i].X:F2}, Y={curve[i].Y:F2}, Theta={curve[i].Theta:F2} rad");
//        }
//    }
//}