using Unity.Burst;
using Unity.Mathematics;

namespace MusicXR.Math
{
    public delegate double WaveFunc(double x);

    [BurstCompile]
    public struct MathFunction
    {
        public FunctionPointer<WaveFunc> waveFunc;
    }

    public enum WaveformType
    {
        Sine,
        Triangle,
        Sawtooth,
        Square
    }

    [BurstCompile]
    public static class WaveformFunctions
    {
        [BurstCompile]
        public static double Sine(double x) => math.sin(x);

        [BurstCompile]
        public static double Triangle(double x) => 2.0 * math.abs(2.0 * (x / (2.0 * math.PI) - math.floor(x / (2.0 * math.PI) + 0.5))) - 1.0;

        [BurstCompile]
        public static double Sawtooth(double x) => 2.0 * (x / (2.0 * math.PI) - math.floor(x / (2.0 * math.PI) + 0.5));

        [BurstCompile]
        public static double Square(double x) => math.sign(math.sin(x));
    }

}

