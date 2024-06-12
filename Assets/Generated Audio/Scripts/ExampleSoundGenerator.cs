using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ExampleSoundGenerator : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float amplitude;
    [SerializeField] private float frequency = 261.62f; // middle C

    private double m_phase;
    private int m_sampleRate;

    private void Awake()
    {
        m_sampleRate = AudioSettings.outputSampleRate;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        double phaseIncrement = frequency / m_sampleRate;   

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            float value = Mathf.Sin((float) m_phase * 2 * Mathf.PI) * amplitude;
            m_phase = (m_phase + phaseIncrement) % 1;

            for (int c = 0; c < channels; c++)
            {
                data[sample + c] = value;
            }
        }

    }
}
