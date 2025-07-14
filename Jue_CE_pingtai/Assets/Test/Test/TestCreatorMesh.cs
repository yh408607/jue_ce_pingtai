using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreatorMesh : MonoBehaviour
{

    Mesh m_mesh;
    MeshFilter m_meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        creatorMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void creatorMesh()
    {
        m_meshFilter = GetComponent<MeshFilter>(); //得到meshfilter组件
        m_mesh = new Mesh(); //new 一个mesh

        Vector3[] vertices = new Vector3[] //顶点列表的变量
        {
            //m_objA.transform.position, //加入A点坐标
            //m_objB.transform.position, //加入B点坐标
            //m_objC.transform.position  //加入C点坐标
            Vector3.zero,
            new Vector3(0,1,0),
            new Vector3(1,1,0),
                  new Vector3(1,0,0)
        };

        int[] triangles = new int[] { 0, 1, 2,0,2,3 }; //按照A->B->C->A的顺序连接 放入三角序列中

        m_mesh.vertices = vertices; //把顶点列表 放到mesh中
        m_mesh.triangles = triangles; //把三角序列 放到mesh中

        m_meshFilter.mesh = m_mesh;   //把mesh放到meshfilter中   meshfilter会把它抛给renderer

    }
}
