using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SimpleJob : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var additionElement = new NativeArray<float>(2, Allocator.TempJob);
        additionElement[0] = 5;
        additionElement[1] = 10;
        var result = new NativeArray<float>(1, Allocator.TempJob);

        var additionJob = new VerySimpleAdditionJob
        {
            m_elements = additionElement,
            m_result = result
        };

        var jobHandle = additionJob.Schedule();
        jobHandle.Complete();
        print(result[0]);

        result.Dispose();
        additionElement.Dispose();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}






















public struct VerySimpleAdditionJob : IJob
{
    [NotNull] public NativeArray<float> m_elements;
    [NotNull] public NativeArray<float> m_result;
    
    public void Execute()
    {
        m_result[0] = m_elements[0] + m_elements[1];
    }
}

















