using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class VerticalEdgeHighlight : MonoBehaviour
{

    public MeshFilter m_meshFilter;

    public GameObject m_edgePrefab;

    public float m_verticalHighlightThreshold = .01f;

    private List<GameObject> m_edges;

    private Vector3[] m_meshVertices;
    private int[] m_meshTriangles;
    private int m_nbTriangles;
    
    // Start is called before the first frame update
    void Start()
    {
        m_edges = new List<GameObject>();
        
        var mesh = m_meshFilter.mesh;
        m_meshVertices = mesh.vertices;
        m_meshTriangles = mesh.triangles;

        m_nbTriangles = m_meshTriangles.Length / 3;
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var entry in m_edges)
        {
            Destroy(entry);
        }
        
        m_edges.Clear();
        
        var jobTriangles = new NativeArray<Triangle>(m_nbTriangles, Allocator.TempJob);
        var jobResult = new NativeArray<HighlightEdge>(m_nbTriangles, Allocator.TempJob);


        var index = 0;

        var meshRotation = m_meshFilter.transform.rotation;

        for (int i = 0; i < m_meshTriangles.Length; i+=3)
        {
            var triangle = new Triangle
            {
                m_vertice1 = meshRotation * m_meshVertices[m_meshTriangles[i]],
                m_vertice2 = meshRotation * m_meshVertices[m_meshTriangles[i + 1]],
                m_vertice3 = meshRotation * m_meshVertices[m_meshTriangles[i + 2]],
            };

            jobTriangles[index] = triangle;
            index++;
        }

        var job = new VerticalEdgeDetectorJob
        {
            m_triangles = jobTriangles,
            m_result = jobResult,
            m_testValidationThreshold = m_verticalHighlightThreshold
        };


        var jobHandle = job.Schedule(jobTriangles.Length, 10);
        jobHandle.Complete();

        foreach (var result in jobResult)
        {
            if (result.m_edgePoint1 == Vector3.zero && result.m_edgePoint2 == Vector3.zero) continue;

            var edge = Instantiate(m_edgePrefab); // Need Pooling System
            m_edges.Add(edge);
            var linePoints = new Vector3[2];
            linePoints[0] = result.m_edgePoint1;
            linePoints[1] = result.m_edgePoint2;
            
            edge.GetComponent<LineRenderer>().SetPositions(linePoints);
        }

        jobResult.Dispose();
        jobTriangles.Dispose();
        
    }
}
