using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using _Project.Scripts.Jobs;

namespace _Project.Scripts.Mono
{
    public class VerticalEdgeHighlight : MonoBehaviour
    {
        public MeshFilter m_renderer;
        public GameObject m_edgePrefab;

        public float m_verticalHighlightTreshold = .01f;

        private int m_frameSplit;
        private List<GameObject> m_edges;

        private void Start()
        {
            m_edges = new List<GameObject>();
        }

        private void Update()
        {
            m_frameSplit += 1;
            if (m_frameSplit % 10 != 0) return;
            m_frameSplit = 0;
            
            foreach (var entry in m_edges)
            {
                Destroy(entry);
            }
            
            m_edges.Clear();

            var mesh = m_renderer.mesh;
            var meshVertices = mesh.vertices;
            var meshTriangles = mesh.triangles;

            var nbTriangles = meshTriangles.Length / 3;
            
            var jobTriangles = new NativeArray<Triangle>(nbTriangles, Allocator.TempJob);
            var jobResult = new NativeArray<HighlightEdge>(nbTriangles, Allocator.TempJob);

            var index = 0;

            var meshRotation = m_renderer.transform.rotation;
            
            for (var i = 0; i < meshTriangles.Length; i+=3)
            {
                
                var triangle = new Triangle
                {
                    m_vertice1 = meshRotation*meshVertices[meshTriangles[i]],
                    m_vertice2 = meshRotation* meshVertices[meshTriangles[i+1]],
                    m_vertice3 = meshRotation*meshVertices[meshTriangles[i+2]],
                    
                };
                jobTriangles[index] = triangle;
                index++;
            }
            
            
            var job = new VerticalEdgeDetectorJob
            {
                m_triangles = jobTriangles,
                m_result = jobResult,
                m_testValidationThreshold = m_verticalHighlightTreshold
            };
            
            
            var jobhandle = job.Schedule(jobTriangles.Length,10);
            jobhandle.Complete();
            
           
            foreach (var result in jobResult)
            {
                if (result.m_edge1 == Vector3.zero && result.m_edge1 == Vector3.zero) continue;
                
                var edge = Instantiate(m_edgePrefab);
                m_edges.Add(edge);
                var linePoints = new Vector3[2];
                linePoints[0] = result.m_edge1;
                linePoints[1] = result.m_edge2;
                edge.GetComponent<LineRenderer>().SetPositions(linePoints);
            }
            
            jobResult.Dispose();
            jobTriangles.Dispose();

        }
        
    }
}



