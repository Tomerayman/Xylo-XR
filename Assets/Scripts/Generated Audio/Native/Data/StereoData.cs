using System.Runtime.InteropServices;

namespace MusicXR.Native.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StereoData
    {
        public float LeftChannel;
        public float RightChannel;

        public StereoData(float value)
        {
            LeftChannel = value;
            RightChannel = value;
        }
    }
}