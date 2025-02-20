using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

namespace MusicXR
{
    public class AudioOut : MonoBehaviour
    {
        public List<SynthProvider> providers = new List<SynthProvider>();

        private NativeArray<float> dataArray;
        private NativeArray<float> tempBuffer;
        private float[] tempArray;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (channels != 2)
            {
                Debug.LogError("AudioOut only works with Unity STEREO output mode.");
                return;
            }

            if (providers.Count == 0) return;

            // Ensure dataArray and tempBuffer are correctly initialized
            if (!dataArray.IsCreated || dataArray.Length != data.Length)
            {
                if (dataArray.IsCreated) dataArray.Dispose();
                if (tempBuffer.IsCreated) tempBuffer.Dispose();

                dataArray = new NativeArray<float>(data.Length, Allocator.Persistent);
                tempBuffer = new NativeArray<float>(data.Length, Allocator.Persistent);
                tempArray = new float[data.Length];
            }

            // Clear the tempBuffer before use
            NativeArray<float>.Copy(new NativeArray<float>(data.Length, Allocator.Temp), tempBuffer);

            // Fill each provider's buffer and add to tempBuffer
            for (int i = 0; i < providers.Count; i++)
            {
                if (!providers[i].isActive) continue;

                providers[i].FillBuffer(tempArray);

                for (int j = 0; j < tempBuffer.Length; j++)
                {
                    tempBuffer[j] += tempArray[j];
                }
            }

            // Schedule a job to aggregate buffers
            var aggregateJob = new AggregateJob
            {
                inputBuffer = tempBuffer,
                outputBuffer = dataArray
            };

            JobHandle aggregateHandle = aggregateJob.Schedule();

            // Complete the job and copy the result to the managed array
            aggregateHandle.Complete();
            dataArray.CopyTo(data);
        }

        private void OnDestroy()
        {
            if (dataArray.IsCreated) dataArray.Dispose();
            if (tempBuffer.IsCreated) tempBuffer.Dispose();
        }

        [BurstCompile]
        private struct AggregateJob : IJob
        {
            [ReadOnly] public NativeArray<float> inputBuffer;
            public NativeArray<float> outputBuffer;

            public void Execute()
            {
                for (int i = 0; i < outputBuffer.Length; i++)
                {
                    outputBuffer[i] = Mathf.Clamp01(outputBuffer[i] + inputBuffer[i]);
                }
            }
        }
    }
}
