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
        m_meshFilter = GetComponent<MeshFilter>(); //�õ�meshfilter���
        m_mesh = new Mesh(); //new һ��mesh

        Vector3[] vertices = new Vector3[] //�����б�ı���
        {
            //m_objA.transform.position, //����A������
            //m_objB.transform.position, //����B������
            //m_objC.transform.position  //����C������
            Vector3.zero,
            new Vector3(0,1,0),
            new Vector3(1,1,0),
                  new Vector3(1,0,0)
        };

        int[] triangles = new int[] { 0, 1, 2,0,2,3 }; //����A->B->C->A��˳������ ��������������

        m_mesh.vertices = vertices; //�Ѷ����б� �ŵ�mesh��
        m_mesh.triangles = triangles; //���������� �ŵ�mesh��

        m_meshFilter.mesh = m_mesh;   //��mesh�ŵ�meshfilter��   meshfilter������׸�renderer

    }
}
