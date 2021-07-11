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
        public Vector3 m_edgePoint1;
        public Vector3 m_edgePoint2;
    }
    
    public struct VerticalEdgeDetectorJob : IJobParallelFor
    {
        public NativeArray<Triangle> m_triangles;
        public NativeArray<HighlightEdge> m_result;
        public float m_testValidationThreshold;
        
        public void Execute(int _index)
        {
            var testingVertice1 = m_triangles[_index].m_vertice1;
            var testingVertice2 = m_triangles[_index].m_vertice2;

            if (AreThoseVerticesVerticallyAligned(testingVertice1, testingVertice2))
            {
                m_result[_index] = new HighlightEdge
                {
                    m_edgePoint1 = testingVertice1,
                    m_edgePoint2 = testingVertice2
                };
            }
            
            testingVertice1 = m_triangles[_index].m_vertice2;
            testingVertice2 = m_triangles[_index].m_vertice3;

            if (AreThoseVerticesVerticallyAligned(testingVertice1, testingVertice2))
            {
                m_result[_index] = new HighlightEdge
                {
                    m_edgePoint1 = testingVertice1,
                    m_edgePoint2 = testingVertice2
                };
            }
            
            
            testingVertice1 = m_triangles[_index].m_vertice3;
            testingVertice2 = m_triangles[_index].m_vertice1;

            if (AreThoseVerticesVerticallyAligned(testingVertice1, testingVertice2))
            {
                m_result[_index] = new HighlightEdge
                {
                    m_edgePoint1 = testingVertice1,
                    m_edgePoint2 = testingVertice2
                };
            }
        }

        private bool AreThoseVerticesVerticallyAligned(Vector3 _testingVertice1, Vector3 _testingVertice2)
        {
            var vert1OnXZPlane = new float2(_testingVertice1.x, _testingVertice1.z);
            var vert2OnXZPlane = new float2(_testingVertice2.x, _testingVertice2.z);

            return math.distance(vert1OnXZPlane, vert2OnXZPlane) < m_testValidationThreshold;
        }
    }
}
