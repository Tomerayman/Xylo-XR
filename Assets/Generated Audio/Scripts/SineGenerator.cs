using MusicXR.Native.Buffers;
using MusicXR.Native.Data;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace MusicXR
{
    [BurstCompile]
    public class SineGenerator : SynthProvider
    {
        [Range(16.35f, 7902.13f)] public float frequency = 261.62f; // middle C
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;

        private static BurstSineDelegate _burstSine;

        private double _phase;
        private int _sampleRate;

        private void Awake()
        {
            _sampleRate = AudioSettings.outputSampleRate;
            _burstSine ??= BurstCompiler.CompileFunctionPointer<BurstSineDelegate>(BurstSine).Invoke;
        }

        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            _phase = _burstSine(ref buffer, _phase, _sampleRate, amplitude, frequency);
        }

        private delegate double BurstSineDelegate(ref SynthBuffer buffer,
            double phase, int sampleRate, float amplitude, float frequency);

        [BurstCompile]
        private static double BurstSine(ref SynthBuffer buffer,
            double phase, int sampleRate, float amplitude, float frequency)
        {
            // calculate how much the phase should change after each sample
            double phaseIncrement = frequency / sampleRate;

            for (int sample = 0; sample < buffer.Length; sample++)
            {
                // calculate and set buffer sample
                buffer[sample] = new StereoData((float) (math.sin(phase * 2 * math.PI) * amplitude));

                // increment _phase value for next iteration
                phase = (phase + phaseIncrement) % 1;
            }

            // return the updated phase
            return phase;
        }
    }
}