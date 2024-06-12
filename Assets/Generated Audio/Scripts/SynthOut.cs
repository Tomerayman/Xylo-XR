using UnityEngine;

namespace MusicXR
{
    public class SynthOut : MonoBehaviour
    {
        public SynthProvider provider;

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (channels != 2)
            {
                Debug.LogError("Synthic only works with unity STEREO output mode.");
                return;
            }

            if (provider == null) return;
            provider.FillBuffer(data);
        }
    }
}