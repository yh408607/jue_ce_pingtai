using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Che_xian_sui_lineData : BaseLineData
{

    private float suidao_len;
    private suidao_line _suidao ;
    private Transform men;

    public Che_xian_sui_lineData(Severlinedata_paragm paragm)
    {
        step_index = 1;

        lineType = paragm.Substructural_Type;
        isquxian = paragm.CurveT == 0 ? false : true;
        zuozhixian_length = paragm.ST_LenL1;
        huanqu_length = paragm.HH_Len;
        yuan_length = paragm.CR_Len;
        yuan_R = paragm.CR;
        youzhixian_length = paragm.ST_LenR;

        //todo隧道的长度值需要模拟
        suidao_len = paragm.L_tunl;
        //suidao_len = 600;

        //计算坐标点
        calculatePath();
    }


    protected override void calculatePath()
    {
        linePath = new List<Baseline>();
        if (isquxian)
        {
            //todo曲线路径



        }
        else
        {
            //todo直线路径
            //缓和曲线长度不取值

            //地基部分
            huanqu_length = 0;
            var star_Pos = Vector3.zero;
            var end_pos = new Vector3(0, 0, zuozhixian_length);
            ludi_line ludi_1 = new ludi_line("ludi_1");
            ludi_1.path.Add(star_Pos);
            ludi_1.path.Add(end_pos);
            linePath.Add(ludi_1);
            star_Pos = end_pos;

            //隧道部分
            suidao_line suidao = new suidao_line("suidao");
            end_pos = new Vector3(0, 0, zuozhixian_length + suidao_len);
            suidao.path.Add(star_Pos);
            suidao.path.Add(end_pos);
            suidao.suidao_len = suidao_len;
            linePath.Add(suidao);
            star_Pos = end_pos;
            _suidao = suidao;

            //地基部分
            var totulLenth = zuozhixian_length + huanqu_length + yuan_length + huanqu_length + youzhixian_length;
            var temp = totulLenth - star_Pos.z;
            if (temp > 100)
            {
                end_pos = new Vector3(0, 0, totulLenth);
            }
            else
            {
                end_pos = new Vector3(0, 0, totulLenth + 300);
            }
            ludi_line ludi_2 = new ludi_line("luji");
            ludi_2.path.Add(star_Pos);
            ludi_2.path.Add(end_pos);
            linePath.Add(ludi_2);

        }
    }

    public override void calculatePath(string data)
    {
        
    }

    public override bujian_data CalculatePointAndRotation(float licheng)
    {
        bujian_data data = new bujian_data();


        if (isquxian)
        {
            //曲线计算曲线路径
            Debug.LogError("曲线还未计算");
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

    public override void CreatorRoad()
    {
        Debug.Log("开始创建车-线-隧");

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
                case "suidao":
                    var path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Suidao", item.Name);

                    //生成山体
                    creatorShan();
                    break;

                case "ludi_1":
                    path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;


                case "luji":

                    path = item.path.ToArray();
                    CreaterRoad.CreatRoad_new(path, "Guidao_luji", item.Name);
                    break;
                default:
                    break;
            }
        }
    }


    private void creatorShan()
    {
        //生成门
        //根据隧道长度来进行
        if (suidao_len > 300)
        {
            var star_door_prefab = Loader.LoadPrefab("Prefab/Cave/start_door");
            GameObject star_door = GameObject.Instantiate(star_door_prefab);
            men = star_door.transform;
            Vector3 star_pos = _suidao.start_pos;
            star_door.transform.position = new Vector3(star_pos.x, star_pos.y, star_pos.z + 60);
            var hole = star_door.transform.Find("hole");
            //Creatorhole(hole);
            TestRay();



            var end_door_prefab = Loader.LoadPrefab("Prefab/Cave/end_door");
            GameObject end_door = GameObject.Instantiate(end_door_prefab);
            var end_pos = _suidao.end_pos;
            end_door.transform.position = new Vector3(end_pos.x, end_pos.y, end_pos.z - 40);

            //生成三体
            var shan_prefab = Loader.LoadPrefab("Prefab/Cave/shan");
            GameObject shan = GameObject.Instantiate(shan_prefab);
            shan.transform.localScale = new Vector3(suidao_len, suidao_len, suidao_len);
            shan.transform.position = new Vector3(0, -20, suidao_len / 2);
        }
        else
        {

        }




    }


    public void TestRay()
    {
        var hole = men.transform.Find("hole");
        GameManager.Instance.StartCoroutine(Creatorhole(hole));
    }
  

    /// <summary>
    /// 挖洞
    /// </summary>
    private IEnumerator Creatorhole(Transform point )
    {
        yield return new WaitForSeconds(1.0f);
        // 从当前物体位置向前方发射射线
        Ray ray = new Ray(point.position, point.forward);
        RaycastHit hit;

        // 射线长度设为10单位
        if (Physics.Raycast(ray, out hit, 1000))
        {
            //Debug.Log("击中物体: " + hit.collider.gameObject.name);
            // 在场景视图中绘制射线（红色表示击中）
           // Debug.DrawLine(ray.origin, hit.point, Color.red);
            if (hit.collider.gameObject.tag == "shan")
            {
                var dyn = hit.collider.transform.parent.gameObject.GetComponent<DynamicHoleController>();
                dyn.AddHoleAtHitPoint(hit);
            }
        }
        else
        {
            //在场景视图中绘制射线（绿色表示未击中）
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.green);
        }
    }
}
