using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicHoleController : MonoBehaviour
{
    //[Header("Hole Settings")]
    //public float holeRadius = 0.1f;
    //public float featherWidth = 0.02f;
    //public int maxHoles = 16;
    //public LayerMask interactableLayer;

    //[Header("Visual Feedback")]
    //public GameObject holeMarkerPrefab;
    //public bool showMarkers = true;

    private Material material;
    private Renderer objectRenderer;
    //private List<Vector2> holeUVPositions = new List<Vector2>();
    //private List<float> holeRadii = new List<float>();
    //private List<GameObject> holeMarkers = new List<GameObject>();

    public float holeSize = 0.1f;

    void Start()
    {
        objectRenderer =this.transform.Find("shan_child").GetComponent<Renderer>();
        material = objectRenderer.material;
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.name == "shan_child")
        //        {
        //            AddHoleAtHitPoint(hit);
        //        }
        //    }
        //}
    }

    public void AddHoleAtHitPoint(RaycastHit hit)
    {
        material.SetVector("_HoleData",new Vector4(hit.textureCoord.x, hit.textureCoord.y, holeSize, 0));
        Debug.Log("开始挖了没");
    }

    /// <summary>
    /// 挖洞方法
    /// </summary>
    public void Createrhole()
    {
        var hole_pos = this.transform.Find("hole");
        StartCoroutine(Creatorhole(hole_pos));
    }

    /// <summary>
    /// 挖洞
    /// </summary>
    private IEnumerator Creatorhole(Transform point)
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

void OnDestroy()
    {
        // 清理材质实例
        if (Application.isPlaying)
        {
            Destroy(material);
        }
    }
}