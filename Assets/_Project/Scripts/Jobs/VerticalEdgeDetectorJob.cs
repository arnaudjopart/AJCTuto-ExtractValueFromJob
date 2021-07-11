using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Jobs
{
    public struct Triangle
    {
        public Vector3 m_vertice1;
        public Vector3 m_vertice2;
        public Vector3 m_vertice3;
    }

    public struct HighlightEdge
    {
        public Vector3 m_edge1;
        public Vector3 m_edge2;
    }

    public struct VerticalEdgeDetectorJob : IJobParallelFor
    {
        public NativeArray<Triangle> m_triangles;
        public NativeArray<HighlightEdge> m_result;
        public float m_testValidationThreshold;

        public void Execute(int _index)
        {
            var testedVertice1 = m_triangles[_index].m_vertice1;
            var testedVertice2 = m_triangles[_index].m_vertice2;
            
            if (AreThoseVerticesVerticallyAligned(testedVertice1,testedVertice2) )
            {
                m_result[_index] = new HighlightEdge
                {
                    m_edge1 = testedVertice1,
                    m_edge2 = testedVertice2
                };
            }
            
            testedVertice1 = m_triangles[_index].m_vertice2;
            testedVertice2 = m_triangles[_index].m_vertice3;
            
            if (AreThoseVerticesVerticallyAligned(testedVertice1,testedVertice2))
            {
                m_result[_index] = new HighlightEdge
                {
                    m_edge1 = testedVertice1,
                    m_edge2 = testedVertice2
                };
            }
            
            testedVertice1 = m_triangles[_index].m_vertice3;
            testedVertice2 = m_triangles[_index].m_vertice1;

            if (AreThoseVerticesVerticallyAligned(testedVertice1,testedVertice2 ))
            {
                m_result[_index] = new HighlightEdge
                {
                    m_edge1 = testedVertice1,
                    m_edge2 = testedVertice2
                };
            }
        }

        private bool AreThoseVerticesVerticallyAligned(float3 _testedVertice1, float3 _testedVertice2)
        {
            var vert1OnXZPlane = new float2(_testedVertice1.x,_testedVertice1.z);
            var vert2OnXZPlane = new float2(_testedVertice2.x,_testedVertice2.z);

            return math.distance(vert1OnXZPlane, vert2OnXZPlane) < m_testValidationThreshold;
        }
    }
}