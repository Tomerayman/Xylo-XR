using MusicXR.Math;
using MusicXR.Native.Buffers;
using MusicXR.Native.Data;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace MusicXR.Native
{
    [BurstCompile]
    public class SampleGenerator : SynthProvider
    {
        [SerializeField] private WaveformType waveformType = WaveformType.Square;

        [Range(16.35f, 7902.13f)] public float frequency = 261.62f; // middle C
        [SerializeField, Range(0, 1)] private float amplitude = 0.5f;

        private static BurstSampleDelegate _burstSample;

        private double _phase;
        private int _sampleRate;
        private MathFunction mathFunction;

        private void Awake()
        {
            _sampleRate = AudioSettings.outputSampleRate;

            // Initialize the math function with the selected waveform type
            switch (waveformType)
            {
                case WaveformType.Sine:
                    mathFunction.waveFunc = BurstCompiler.CompileFunctionPointer<WaveFunc>(WaveformFunctions.Sine);
                    break;
                case WaveformType.Triangle:
                    mathFunction.waveFunc = BurstCompiler.CompileFunctionPointer<WaveFunc>(WaveformFunctions.Triangle);
                    break;
                case WaveformType.Sawtooth:
                    mathFunction.waveFunc = BurstCompiler.CompileFunctionPointer<WaveFunc>(WaveformFunctions.Sawtooth);
                    break;
                case WaveformType.Square:
                    mathFunction.waveFunc = BurstCompiler.CompileFunctionPointer<WaveFunc>(WaveformFunctions.Square);
                    break;
                default:
                    mathFunction.waveFunc = BurstCompiler.CompileFunctionPointer<WaveFunc>(WaveformFunctions.Sine);
                    break;
            }

            _burstSample ??= BurstCompiler.CompileFunctionPointer<BurstSampleDelegate>(BurstSample).Invoke;
        }

        protected override void ProcessBuffer(ref SynthBuffer buffer)
        {
            _phase = _burstSample(ref buffer, _phase, _sampleRate, amplitude, frequency, mathFunction);
        }

        private delegate double BurstSampleDelegate(ref SynthBuffer buffer,
            double phase, int sampleRate, float amplitude, float frequency, MathFunction mathFunction);

        [BurstCompile]
        private static double BurstSample(ref SynthBuffer buffer,
            double phase, int sampleRate, float amplitude, float frequency, MathFunction mathFunction)
        {
            // calculate how much the phase should change after each sample
            double phaseIncrement = frequency / sampleRate;

            for (int sample = 0; sample < buffer.Length; sample++)
            {
                // calculate and set buffer sample using the provided math function
                buffer[sample] = new StereoData((float)(mathFunction.waveFunc.Invoke(phase * 2 * math.PI) * amplitude));

                // increment _phase value for next iteration
                phase = (phase + phaseIncrement) % 1;
            }

            // return the updated phase
            return phase;
        }
    }
}
