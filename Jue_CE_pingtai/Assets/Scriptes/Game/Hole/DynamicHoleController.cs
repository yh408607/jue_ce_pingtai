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

        // 初始化Shader参数
        //material.SetInt("_HoleCount", 0);
        //material.SetFloat("_Feather", featherWidth);

        //// 初始化数组
        //Vector4[] holeCenters = new Vector4[maxHoles];
        //float[] radii = new float[maxHoles];
        //material.SetVectorArray("_HoleCenters", holeCenters);
        //material.SetFloatArray("_HoleRadii", radii);
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
        //Debug.Log("开始挖了没");
    }

    //void UpdateShaderHoles()
    //{
    //    Vector4[] holeCenters = new Vector4[maxHoles];
    //    float[] radii = new float[maxHoles];

    //    for (int i = 0; i < holeUVPositions.Count; i++)
    //    {
    //        holeCenters[i] = holeUVPositions[i];
    //        radii[i] = holeRadii[i];
    //    }

    //    material.SetInt("_HoleCount", holeUVPositions.Count);
    //    material.SetVectorArray("_HoleCenters", holeCenters);
    //    material.SetFloatArray("_HoleRadii", radii);
    //}

    //public void ClearAllHoles()
    //{
    //    holeUVPositions.Clear();
    //    holeRadii.Clear();
    //    UpdateShaderHoles();

    //    foreach (var marker in holeMarkers)
    //    {
    //        Destroy(marker);
    //    }
    //    holeMarkers.Clear();
    //}

    void OnDestroy()
    {
        // 清理材质实例
        if (Application.isPlaying)
        {
            Destroy(material);
        }
    }
}