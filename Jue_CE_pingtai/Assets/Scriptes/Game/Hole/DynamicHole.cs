using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicHole : MonoBehaviour
{
    [Header("孔洞设置")]
    public Vector2 holePosition = new Vector2(0.5f, 0.5f);
    public float holeRadius = 0.12f;
    public float featherWidth = 0.05f;

    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        UpdateShaderProperties();
    }

    void Update()
    {
        // 如果需要实时更新（比如跟随鼠标）
        // UpdateShaderProperties();

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SetHole(hit.textureCoord, holeRadius, featherWidth);
            }
        }
    }

    public void SetHole(Vector2 position, float radius, float feather)
    {
        holePosition = position;
        holeRadius = radius;
        featherWidth = feather;
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {
        material.SetVector("_HolePos", new Vector4(holePosition.x, holePosition.y, 0, 0));
        material.SetFloat("_HoleRadius", holeRadius);
        material.SetFloat("_Feather", featherWidth);
    }


}