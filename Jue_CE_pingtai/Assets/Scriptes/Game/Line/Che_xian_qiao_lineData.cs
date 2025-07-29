using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Che_xian_qiao_lineData :BaseLineData
{

    public List<Vector3> point_list;

    private List<qiao_dun> _bridge_list;
    /// <summary>
    /// 桥的数据
    /// </summary>
    public struct qiao_dun
    {
        /// <summary>
        /// 桥的长度
        /// </summary>
        public float qiao_length;

        /// <summary>
        /// 桥的沉降位移
        /// </summary>
        public Vector3 chengjiang_pos;

        
    }

    private List<Vector3> paths;


    public Che_xian_qiao_lineData()
    {
        paths = new List<Vector3>();
        point_list = new List<Vector3>();
        step_index = 1;
    }

    public Che_xian_qiao_lineData(Severlinedata_paragm _Paragm)
    {
        paths = new List<Vector3>();
        point_list = new List<Vector3>();
        step_index = 1;


        lineType = _Paragm.Substructural_Type;
        isquxian =_Paragm.CurveT==0 ? false : true;
        zuozhixian_length = _Paragm.ST_LenL1;
        huanqu_length = _Paragm.HH_Len;
        yuan_length =_Paragm.CR_Len;
        yuan_R = _Paragm.CR;
        youzhixian_length = _Paragm.ST_LenR;

        //计算沉降，沉降需要考虑桥墩的位置
        float[] bride_len = _Paragm.Len_Bri;
        float[] x_bs = _Paragm.XB1;
        float[] y_bs = _Paragm.YB1;
        float[] z_bs = _Paragm.ZB1; 

        //设置沉降点
        SetBridgeData(x_bs, y_bs, z_bs, bride_len);

        //计算路径与各个端的元素
        calculatePath("");
    }


    /// <summary>
    /// 设置桥的数据
    /// </summary>
    public void SetBridgeData(float[] xb,float[] yb,float[] zb,float[] bridges)
    {
        _bridge_list = new List<qiao_dun>();

        var length = bridges.Length;

        if (xb.Length < length)
        {
            xb = setXb_yb_zb(xb, length+1);
            yb = setXb_yb_zb(yb, length+1);
            zb = setXb_yb_zb(zb, length+1);
        }

        for (int i = 0; i <= length; i++)
        {
            var qiao_dun = new qiao_dun();
            if(i== 0)
            {
                qiao_dun.qiao_length = 0;
            }
            else
            {
                qiao_dun.qiao_length = bridges[i-1];
            }

            Vector3 pos = new Vector3(yb[i], zb[i], xb[i]);
            qiao_dun.chengjiang_pos = pos;
            _bridge_list.Add(qiao_dun);
        }
    }

    //桥墩判断，是否有设置沉降，没有则自动填满
    private float[] setXb_yb_zb(float[] xb,int length)
    {
        float[] temp = new float[length];

        for (int j = 0; j < length; j++)
        {
            if (j < xb.Length)
            {
                temp[j] = xb[j];
            }
            else
            {
                temp[j] = 0;
            }
        }
        return temp;
    }

    /// <summary>
    /// 计算路径点
    /// </summary>
    public override void calculatePath(string data)
    {
        linePath = new List<Baseline>();
        if (isquxian)
        {
            //曲线生成,左直线是地基
            //曲线的起始点

            //计算曲线总长度
            var temp_tutul_length = huanqu_length + yuan_length + huanqu_length;
            float totalAngleRad = temp_tutul_length / yuan_R;

            Vector3 startPoint = new Vector3(0, 0, zuozhixian_length);

            //判断桥梁总长度是否大于曲线全部距离，如果大于，则桥梁有一部分在直线上，如果小于则桥梁全部在曲线上，需要计算曲线上路基的长度
            float brigth_length = 0;
            for (int i = 0; i < _bridge_list.Count; i++)
            {
                brigth_length += _bridge_list[i].qiao_length;
            }

            if(brigth_length> temp_tutul_length)
            {
                //大于总曲线的化，则桥梁有一部分在直线上
                var temp = (brigth_length - temp_tutul_length) / 2;

                var star_Pos = Vector3.zero;
                var end_pos = new Vector3(0, 0, zuozhixian_length);
                ludi_line ludi_1 = new ludi_line("luji");
                ludi_1.path.Add(star_Pos);
                ludi_1.path.Add(end_pos);
                linePath.Add(ludi_1);

                star_Pos = end_pos;
                //小部分的直线桥梁
                end_pos = new Vector3(0, 0, star_Pos.z + temp);
                qiao_line qiao_1 = new qiao_line("qiao");
                qiao_1.path.Add(star_Pos);
                qiao_1.path.Add(end_pos);
                linePath.Add(qiao_1);
                star_Pos = end_pos;

                //计算曲线坐标
                // 转换为度数
                qiao_line qiao_2 = new qiao_line("qiao");
                float totalAngleDeg = totalAngleRad * Mathf.Rad2Deg;

                // 计算点的数量（每隔1度一个点）
                int numPoints = Mathf.FloorToInt(totalAngleDeg) + 1; ;
                // 存储所有点的位置
                Vector3[] points = new Vector3[numPoints];
                for (int i = 0; i < numPoints; i++)
                {
                    // 当前角度（弧度）
                    float angleRad = i * Mathf.Deg2Rad;
                    // 计算坐标
                    var _x = star_Pos.x + yuan_R * Mathf.Sin(angleRad);
                    var _z = star_Pos.z + yuan_R * (1 - Mathf.Cos(angleRad));
                    points[i] = new Vector3(_x, 0, _z);
                    var point  = new Vector3(_x, 0, _z);
                    qiao_2.path.Add(point);
                }
                linePath.Add(qiao_2);
                star_Pos = points[numPoints - 1];
                //开始计算直线部分
                //有部分直线是桥梁
                // 1. 计算圆曲线的终点和切线方向

                float x_end = star_Pos.x + yuan_R * Mathf.Sin(totalAngleRad);
                float z_end = star_Pos.z + yuan_R * (1 - Mathf.Cos(totalAngleRad));
                Vector2 tangentDir = new Vector2(Mathf.Cos(totalAngleRad), Mathf.Sin(totalAngleRad)).normalized;

                var x = x_end + temp * tangentDir.x;
                var z = z_end + temp * tangentDir.y;
                end_pos = new Vector3(x, 0, z);
                qiao_line qiao_3 = new qiao_line("qiao");
                qiao_3.path.Add(star_Pos);
                qiao_3.path.Add(end_pos);
                linePath.Add(qiao_3);
                star_Pos = end_pos;
                //计算直线部分
                x = x_end + 500 * tangentDir.x;
                z = z_end + 500 * tangentDir.y;
                end_pos = new Vector3(x, 0, z);
                ludi_line ludi_2 = new ludi_line("luji");
                ludi_2.path.Add(star_Pos);
                ludi_2.path.Add(end_pos);
                linePath.Add(ludi_2);
            }
            else
            {
                //如果桥长度小于曲线长度，桥在曲线内，桥就在曲线的中间位置
                //那么曲线就由地基曲线-桥梁曲线-地基曲线-地基直线四部分组成
                //地基曲线的长度 =（曲线总长度-桥梁长度）/2


                var temp = (temp_tutul_length - brigth_length) / 2;

                //计算直线部分
                var star_Pos = Vector3.zero;
                var end_pos = new Vector3(0, 0, zuozhixian_length);
                ludi_line ludi_1 = new ludi_line("luji_1");
                ludi_1.path.Add(star_Pos);
                ludi_1.path.Add(end_pos);

                linePath.Add(ludi_1);

                star_Pos = end_pos;

                //曲线部分有一段地基
                List<Vector3> luji_2_path = calculateQuxianPath(temp, star_Pos, (int)temp);
                ludi_line ludi_2 = new ludi_line("luji_2");
                ludi_2.path = luji_2_path;
                var temp_indx = luji_2_path.Count - 1;
                star_Pos = luji_2_path[temp_indx];

                linePath.Add(ludi_2);

                ////桥梁部分
                //List<Vector3> qiao_path = calculateQuxianPath(brigth_length, star_Pos, (int)brigth_length);
                //qiao_line qiao = new qiao_line("qiao");
                //qiao.path = qiao_path;
                //temp_indx = qiao_path.Count - 1;
                //star_Pos = qiao_path[temp_indx];
                ////计算桥墩的坐标与旋转角度
                //qiao.qiaodun_number = _bridge_list.Count;
                //计算桥墩的位置与方向
                qiao_line qiao = new qiao_line("qiao");
                List<Vector3> pos = new List<Vector3>();
                List<Vector3> qiao_dun_pos = new List<Vector3>();
                if (_bridge_list.Count > 0)
                {
   
                    for (int i = 0; i < _bridge_list.Count; i++)
                    {
                        var qiao_length  = _bridge_list[i].qiao_length;
                        List<Vector3> qiao_path = calculateQuxianPath(qiao_length, star_Pos, (int)qiao_length);
                        //计算沉降
                        temp_indx = qiao_path.Count - 1;
                        if(temp_indx<= 0)
                        {
                            qiao_dun_pos.Add(star_Pos);
                        }
                        else
                        {
                            qiao_dun_pos.Add(qiao_path[temp_indx]);
                            qiao_path[temp_indx] = qiao_path[temp_indx] + _bridge_list[i].chengjiang_pos;
                            star_Pos = qiao_path[temp_indx];
                        }              

                        pos.AddRange(qiao_path);
                        //计算桥墩的位置与旋转角度
                        // qiao.qiaodun_number = _bridge_list.Count;

                    }
                }

                qiao.path = pos;
                qiao.qiaodun_number = _bridge_list.Count;
                qiao.qiaodun_pos = qiao_dun_pos;
                temp_indx = pos.Count - 1;
                star_Pos = pos[temp_indx];

                linePath.Add(qiao);

                // linePath.Add(qiao);

                //地基曲线部分
                List<Vector3> luji_3_path = calculateQuxianPath(temp, star_Pos, (int)temp);
                ludi_line ludi_3 = new ludi_line("luji_1");
                ludi_3.path = luji_3_path;
                temp_indx = luji_3_path.Count - 1;
                star_Pos = luji_3_path[temp_indx];
                linePath.Add(ludi_3);

                //直线部分
                float thetaStart = Mathf.Atan2(startPoint.z - center.z, startPoint.x - center.x);
                float thetaEnd = thetaStart - totalAngleRad;
                Vector3 tangentDir = new Vector3(Mathf.Sin(thetaEnd), 0, -Mathf.Cos(thetaEnd));
                end_pos = star_Pos + tangentDir * 500;
                ludi_line ludi_4 = new ludi_line("luji_2");
                ludi_4.path.Add(star_Pos);
                ludi_4.path.Add(end_pos);
                linePath.Add(ludi_4);
            }
        }
        else
        {
            huanqu_length = 0;
            //直线生成
            //直线情况，地基到Z-H点，后面就开始是桥梁，计算桥梁的总长度，每个桥梁桥墩的位置与沉降关系，桥梁后面还是地基
            //地基部分 左直线+直缓点
            var star_Pos = Vector3.zero;
            var end_pos = new Vector3(0, 0, zuozhixian_length);
            ludi_line ludi_1 = new ludi_line("luji_1");
            ludi_1.path.Add(star_Pos);
            ludi_1.path.Add(end_pos);

            linePath.Add(ludi_1);
            star_Pos = end_pos;

            //桥梁部分
            if (_bridge_list.Count > 0)
            {
                qiao_line qiao = new qiao_line("qiao");
                qiao.qiaodun_number = _bridge_list.Count;
                //star_Pos = end_pos;
                List<Vector3> pos = new List<Vector3>();
                for (int i = 0; i < _bridge_list.Count; i++)
                {
                    var temp_pos = star_Pos + new Vector3(0, 0, _bridge_list[i].qiao_length) + _bridge_list[i].chengjiang_pos;
                    pos.Add(temp_pos);
                    star_Pos = new Vector3(temp_pos.x, 0, temp_pos.z);
                }
                qiao.path = pos;
                qiao.qiaodun_pos = pos;

                linePath.Add(qiao);
            }


            //地基部分
            star_Pos = new Vector3(star_Pos.x, 0, star_Pos.z);
            //检测剩余长度
            var totulLenth = zuozhixian_length + huanqu_length + yuan_length + huanqu_length + youzhixian_length;
            var temp = totulLenth - star_Pos.z;
            if (temp > 100)
            {
                end_pos = new Vector3(0, 0, totulLenth);
            }
            else
            {
                end_pos = new Vector3(0, 0, totulLenth+300);
            }
            ludi_line ludi_2 = new ludi_line("luji_2");
            ludi_2.path.Add(star_Pos);
            ludi_2.path.Add(end_pos);


            linePath.Add(ludi_2);
        }
    }

    /// <summary>
    /// 计算路径点
    /// </summary>
    protected override void calculatePath()
    {
        if (isquxian)
        {
            Vector3 one = Vector3.zero;
            //计算直线Z_H位置
            Vector3 Z_H = new Vector3(0, 0, one.z + zuozhixian_length);

            //计算缓曲线H_Y位置
            var temp = calculateHuanHequxianPath(huanqu_length, yuan_R);
            var x =Z_H.x+ temp.x;
            var z =Z_H.z+  temp.z;
            var H_Y = new Vector3(x, 0, z);

            //圆曲线终点 Y_H位置
            float theta = yuan_length / yuan_R;
            float alpha_end = huanqu_length / (2 * yuan_R);
            var Y_H = calculateYuanquxianPath(H_Y, alpha_end, yuan_length, yuan_R);

            //计算圆曲线到缓和曲线H_Z的位置
            float alpha_circular_end = alpha_end + theta;
            temp = calculateHuanHequxianPath(huanqu_length, yuan_R);
            var x_temp = -temp.x;
            var z_temp = temp.z;
            x = Y_H.x + x_temp * Mathf.Cos(alpha_circular_end) - z_temp * Mathf.Sin(alpha_circular_end);
            z = Y_H.z + x_temp * Mathf.Sin(alpha_circular_end) + z_temp * Mathf.Cos(alpha_circular_end);
            var H_Z = new Vector3(x, 0, z);

            //继续第二段直线的位置youzhi
            float alpha_transition2_end = alpha_circular_end + huanqu_length / (2 * yuan_R);
            x = H_Z.x + youzhixian_length * Mathf.Cos(alpha_transition2_end);
            z = H_Z.z + youzhixian_length * Mathf.Sin(alpha_transition2_end);
            Vector3 youzhi = new Vector3(x, 0, z);

            paths.Add(one);
            paths.Add(Z_H);
            paths.Add(H_Y);
            paths.Add(Y_H);
            paths.Add(H_Z);
            paths.Add(youzhi);

            Debug.LogFormat("这是一条曲线，起点位置{0},Z_H点位置{1}，H_Y点位置{2}，Y_H点位置为{3}，H_Z点位置{4}，终点位置为{5}", one, Z_H, H_Y, Y_H, H_Z, youzhi);

        }
        else
        {
            Vector3 one = Vector3.zero;
            Vector3 Z_H = new Vector3(0, 0,one.z +zuozhixian_length);
            Vector3 H_Y = new Vector3(0, 0, Z_H.z+ huanqu_length);
            Vector3 Y_H = new Vector3(0, 0, H_Y.z + yuan_length);
            Vector3 H_Z = new Vector3(0, 0, Y_H.z + huanqu_length);
            Vector3 youzhi = new Vector3(0, 0, H_Z.z + youzhixian_length);

            float totul_length = zuozhixian_length + huanqu_length + yuan_length + huanqu_length + youzhixian_length;
            Vector3 totule = new Vector3(0, 0, one.z + totul_length);

            paths.Add(one);
            paths.Add(totule);


        }
    }


    /// <summary>
    /// 计算缓和曲线H_Y点或者H_Z点坐标
    /// </summary>
    private   Vector3 calculateHuanHequxianPath(float length_quxian,float R)
    {
        Vector3 point = new Vector3();
        //这里需要考虑，参照系方向的问题。
        var temp = Mathf.Pow(length_quxian, 3) / (40 * Mathf.Pow(R, 2));
        float z = length_quxian - temp;
        float x = Mathf.Pow(length_quxian, 2) / (6 * R);

        point.x = x;
        point.y = 0;
        point.z = z;

        return point;
    }

    /// <summary>
    /// 计算圆曲线坐标
    /// </summary>
    /// <param name="length_c"></param>
    /// <param name="R"></param>
    /// <returns></returns>
    private Vector3 calculateYuanquxianPath(Vector3 star_point,float alpha_end, float length_c,float R)
    {

        var temp = length_c / R;


        float x_circular;
        float z_circular;

        var x_c = star_point.x + (R * Mathf.Sin(Mathf.PI / 2 - alpha_end));
        var z_c = star_point.z - (R * Mathf.Cos(Mathf.PI / 2 - alpha_end));

        //var x_c = star_point.x - (R * Mathf.Sin(alpha_end));
        //var z_c = star_point.z + (R * Mathf.Cos(alpha_end));

        Debug.LogFormat("圆心坐标为X{0},y{1}", x_c, z_c);

        //x_circular = x_c + (R * Mathf.Sin(alpha_end - temp));
        //z_circular = z_c - (R * Mathf.Cos(alpha_end - temp));

        var d_x = star_point.x - x_c;
        var d_z = star_point.z - z_c;
        x_circular = x_c + d_x * Mathf.Cos(temp) + d_z * Mathf.Sin(temp);
        z_circular = z_c - d_z * Mathf.Sin(temp) + d_z * Mathf.Cos(temp);


        var point = new Vector3(x_circular, 0, z_circular);
        return point;
    }


    /// <summary>
    /// 根据里程输出对应的坐标
    /// </summary>
    /// <param name="licheng"></param>
    public void CalculateListPiont(float licheng)
    {
        if (licheng <= zuozhixian_length)
        {
            //var c = licheng / step_index;

            //for (int i = 0; i < c; i++)
            //{
            //    var z = i * step_index;//沿Z轴方向前进
            //    Vector3 point = new Vector3(0, 0, z);
            //    point_list.Add(point);
            //}


            point_list = calculateLinePoint(licheng);

            return;
        }
        else if (zuozhixian_length < licheng && licheng <= zuozhixian_length + huanqu_length)
        {
            //先要减掉直线距离计算计算距离的坐点
            var temp_list = calculateLinePoint(zuozhixian_length);
            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }
            //再计算缓和曲线的坐标点
            var quxian_licheng = licheng - zuozhixian_length;
            var start_point = new Vector3(0, 0, zuozhixian_length);
            temp_list = calculateQuxianPoint(start_point, quxian_licheng);
            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }

            return;
        }else if( zuozhixian_length + huanqu_length<licheng && licheng<= zuozhixian_length + huanqu_length + yuan_length)
        {
            //计算左直线，左缓和曲线的坐标点

            var temp_list = calculateLinePoint(zuozhixian_length);
            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }
            //再计算缓和曲线的坐标点
            var quxian_licheng = huanqu_length;
            var start_point = new Vector3(0, 0, zuozhixian_length);
            temp_list = calculateQuxianPoint(start_point, quxian_licheng);
            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }

            //计算圆曲线上的坐标点

            //计算剩余里程
            var shengyu_licheng = licheng - (zuozhixian_length + huanqu_length);
            var starpoint = point_list[point_list.Count - 1];
            var alph = huanqu_length / 2 * yuan_R;
            temp_list = calculateYuanPoint(starpoint, alph, shengyu_licheng);

            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }

            return;
        }
        else if(zuozhixian_length + huanqu_length + yuan_length<licheng && licheng<= zuozhixian_length + huanqu_length + yuan_length+huanqu_length)
        {
            //里程到了直线-缓和曲线-圆曲线-缓和曲线

            //计算左直线，左缓和曲线的坐标点

            var temp_list = calculateLinePoint(zuozhixian_length);
            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }
            //再计算缓和曲线的坐标点
            var quxian_licheng = huanqu_length;
            var start_point = new Vector3(0, 0, zuozhixian_length);
            temp_list = calculateQuxianPoint(start_point, quxian_licheng);
            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }

            //计算圆曲线上的坐标点

            //计算剩余里程
            var shengyu_licheng = yuan_length;
            var starpoint = point_list[point_list.Count - 1];
            var alph = huanqu_length / 2 * yuan_R;
            temp_list = calculateYuanPoint(starpoint, alph, shengyu_licheng);

            foreach (var item in temp_list)
            {
                point_list.Add(item);
            }

            //todo没做完，继续做

        }
    }

    /*
    /// <summary>
    /// 根据里程计算当前的坐标点与朝向
    /// </summary>
    /// <param name="licheng"></param>
    /// <returns></returns>
    public override bujian_data CalculatePointAndRotation(float licheng)
    {
        bujian_data data = new bujian_data();

        if (isquxian)
        {
            //曲线计算曲线路径
            //Debug.LogError("曲线还未计算");
            var zhixian_quxian = zuozhixian_length + huanqu_length * 2 + yuan_length;

            if (licheng <= zuozhixian_length)
            {
                //计算直线
                var star_y = TrainController.Instance.Start_Hitgh;//初始高度

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
                var rota = new Vector3(0,180-degree,0);

                data.positon = pos;
                data.rotation = rota;
            }
            else
            {
                //里程在曲线外，进入到右直线了
                var start_pos = new Vector3(0, 0, zuozhixian_length);
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
        }
        else
        {
            //计算直线
            var star_y = TrainController.Instance.Start_Hitgh;//初始高度

            Vector3 pos = new Vector3(0, 0, licheng);
            Vector3 rota = new Vector3(0, 0, 0);

            data.positon = pos;
            data.rotation = rota;

        }

        return data;
    }
    */
    //计算左直线的坐标点
    private List<Vector3> calculateLinePoint(float licheng)
    {
        var c = licheng / step_index;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < c; i++)
        {
            var z = i * step_index;//沿Z轴方向前进
            Vector3 point = new Vector3(0, 0, z);
            points.Add(point);
        }

        return points;
    }


    //计算左缓和曲线坐标点
    private List<Vector3> calculateQuxianPoint(Vector3 starpoint,float licheng)
    {
        List<Vector3> points = new List<Vector3>();
        var c = licheng / step_index;

        for (int i = 1; i < c; i++)
        {
            var l = i * step_index;
            var z = l - (Mathf.Pow(l, 5) / (40 * Mathf.Pow(yuan_R, 2) * Mathf.Pow(licheng, 2)));

            var x = Mathf.Pow(l, 3) / (6 * yuan_R * licheng);

            var x_temp = starpoint.x + x;

            var z_temp = starpoint.z + z;

            var temp_point = new Vector3(x_temp, 0, z_temp);

            points.Add(temp_point);
        }

        return points;
    }


    //计算圆坐标点
    public List<Vector3> calculateYuanPoint(Vector3 startPoint,float alph_end,float licheng)
    {
        List<Vector3> points = new List<Vector3>();
        //计算圆心坐标
        var o_x = startPoint.x + yuan_R * Mathf.Sin(alph_end);
        var o_z = startPoint.z - yuan_R * Mathf.Cos(alph_end);

        var o = new Vector3(o_x, 0, o_z);
        Debug.LogFormat("圆心坐标点为{0}", o);


        //先求极坐标
        var t = Mathf.Atan2(startPoint.z - o_z, startPoint.x - o_x);
        var c = licheng / step_index;
        for (int i = 1; i < c; i++)
        {
            var alph = (i * step_index) / yuan_R;
            var x_temp = o_x + yuan_R * Mathf.Cos(t - alph);
            var z_temp = o_z + yuan_R * Mathf.Sin(t - alph);

            var tmep_c = new Vector3(x_temp, 0, z_temp);
            points.Add(tmep_c);

        }
        return points;
    }

    /// <summary>
    /// 生成道路
    /// </summary>
    public override void CreatorRoad()
    {
        //这是生成初始端
        Vector3[] ludi_point = new Vector3[2] {
            new Vector3(0,0,-500),
            new Vector3(0, 0,0),
        };

        CreaterRoad.CreatRoad_new(ludi_point, "Guidao_luji", "初始段");
        
        foreach (var item in linePath)
        {
            switch (item.Name)
            {
                case "luji_1":
                    var path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;

                case "luji_2":
                    path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;
                case "qiao":
                    path = item.path.ToArray();
                    GameObject road = CreaterRoad.CreatRoad_new(path, "Guidao_qiao", item.Name);
                    //根据位置生成桥墩
                    var temp = item as qiao_line;
                    var chengjiang_pos = temp.qiaodun_pos;
                    foreach (var pos in chengjiang_pos)
                    {
                        var prefab = Loader.LoadPrefab("Prefab/qiao_dun");
                        var qiao_dun =GameObject.Instantiate(prefab, pos, Quaternion.identity);
                        qiao_dun.transform.SetParent(road.transform);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}


