using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLineRenender : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Vector3> pos_list;
    private LineRenderer line;
    public Color startColor = Color.white, endColor = Color.white;
    public float lineWidth = 1435 / 1000f;

    void Start()
    {
        pos_list = new List<Vector3>();

        for (int i = 0; i < 100; i++)
        {
            var pos = new Vector3(0, 0, i);
            pos_list.Add(pos);
        }

        line = GetComponent<LineRenderer>();
        line.alignment = LineAlignment.TransformZ;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = startColor;
        line.endColor = endColor;

        line.positionCount = pos_list.Count;
        line.SetPositions(pos_list.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
