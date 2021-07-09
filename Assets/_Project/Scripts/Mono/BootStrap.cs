using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Scripts.Mono
{
    public class BootStrap : MonoBehaviour
    {
        public MeshFilter m_renderer;

        public GameObject edgePrefab;

        public List<GameObject> m_edges;
        

        void Update()
        {
            foreach (var entry in m_edges)
            {
                Destroy(entry);
            }
            m_edges.Clear();
            
            var meshVertices = m_renderer.mesh.vertices;
            var meshtriangle = m_renderer.mesh.triangles;

            var nbTriangles = meshtriangle.Length / 3;
            
            var triangleses = new NativeArray<Triangles>(nbTriangles, Allocator.TempJob);
            var resultEdges = new NativeArray<HighlightEdge>(nbTriangles, Allocator.TempJob);

            var index = 0;
            
            for (var i = 0; i < meshtriangle.Length; i+=3)
            {
                
                var triangle = new Triangles
                {
                    _vertice1 = m_renderer.transform.rotation*meshVertices[meshtriangle[i]],
                    _vertice2 =m_renderer.transform.rotation* meshVertices[meshtriangle[i+1]],
                    _vertice3 = m_renderer.transform.rotation*meshVertices[meshtriangle[i+2]],
                    
                };
                triangleses[index] = triangle;
                index++;
            }
            
            
            var job = new SomeJob
            {
                _triangles = triangleses,
                _edges = resultEdges
            };
            
            
            var jobhandle = job.Schedule(triangleses.Length,10);
            jobhandle.Complete();
            triangleses.Dispose();
           
            foreach (var VARIABLE in resultEdges)
            {
                if (VARIABLE._edge1 != Vector3.zero || VARIABLE._edge1 != Vector3.zero)
                {
                    var edge = Instantiate(edgePrefab);
                    m_edges.Add(edge);
                    var linePoints = new Vector3[2];
                    linePoints[0] = VARIABLE._edge1;
                    linePoints[1] = VARIABLE._edge2;
                    edge.GetComponent<LineRenderer>().SetPositions(linePoints);
                }
            }
            
            resultEdges.Dispose();

        }
        
    }


    public struct SomeJob : IJobParallelFor
    {
        public NativeArray<Triangles> _triangles;
        public NativeArray<HighlightEdge> _edges;
        
        public void Execute(int index)
        {
            
            var testedVertice1 = _triangles[index]._vertice1;
            var testedVertice2 = _triangles[index]._vertice2;
            
            if (AreThoseVerticesVeritallyAligned(testedVertice1,testedVertice2) )
            {
                _edges[index] = new HighlightEdge
                {
                    _edge1 = testedVertice1,
                    _edge2 = testedVertice2
                };
            }
            
            testedVertice1 = _triangles[index]._vertice2;
            testedVertice2 = _triangles[index]._vertice3;
            
            if (AreThoseVerticesVeritallyAligned(testedVertice1,testedVertice2))
            {
                _edges[index] = new HighlightEdge
                {
                    _edge1 = testedVertice1,
                    _edge2 = testedVertice2
                };
            }
            
            testedVertice1 = _triangles[index]._vertice3;
            testedVertice2 = _triangles[index]._vertice1;

            if (AreThoseVerticesVeritallyAligned(testedVertice1,testedVertice2 ))
            {
                _edges[index] = new HighlightEdge
                {
                    _edge1 = testedVertice1,
                    _edge2 = testedVertice2
                };
            }
        }

        private bool AreThoseVerticesVeritallyAligned(float3 _testedVertice1, float3 _testedVertice2)
        {
            var vert1OnXZPlane = new float2(_testedVertice1.x,_testedVertice1.z);
            var vert2OnXZPlane = new float2(_testedVertice2.x,_testedVertice2.z);

            return math.distance(vert1OnXZPlane, vert2OnXZPlane) < .01f;
        }
    }

    
    
    
}


public struct Triangles
{
    public float3 _vertice1;
    public float3 _vertice2;
    public float3 _vertice3;
    
}

public struct HighlightEdge
{
    public Vector3 _edge1;
    public Vector3 _edge2;
}

