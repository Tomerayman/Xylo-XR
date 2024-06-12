using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicXR
{
    public class MyFirstSynth : MonoBehaviour
    {
        private static List<Tuple<string, float>> m_notes = new List<Tuple<string, float>>
        {
            Tuple.Create("C4", 261.63f),
            Tuple.Create("C#4/Db4", 277.18f),
            Tuple.Create("D4", 293.66f),
            Tuple.Create("D#4/Eb4", 311.13f),
            Tuple.Create("E4", 329.63f),
            Tuple.Create("F4", 349.23f),
            Tuple.Create("F#4/Gb4", 369.99f),
            Tuple.Create("G4", 392.00f),
            Tuple.Create("G#4/Ab4", 415.30f),
            Tuple.Create("A4", 440.00f),
            Tuple.Create("A#4/Bb4", 466.16f),
            Tuple.Create("B4", 493.88f),
            Tuple.Create("C5", 523.25f)
        };

        private static List<string> m_keys = new List<string>() { "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };

        private Dictionary<string, AudioSource> m_sources;

        private void Awake()
        {
            m_sources = new Dictionary<string, AudioSource>();

            for (int i = 0; i < m_keys.Count; i++)
            {
                var note = m_notes[i];
                var key = m_keys[i];

                GameObject go = new GameObject(note.Item1);
                go.transform.parent = transform;
                SineGenerator sg = go.AddComponent<SineGenerator>(); 
                sg.frequency = note.Item2;
                SynthOut so = go.AddComponent<SynthOut>();
                so.provider = sg;
                m_sources[key] = go.AddComponent<AudioSource>();
                m_sources[key].enabled = false;
            }
        }

        private void Update()
        {
            //foreach (var keySource in m_sources) 
            //{
            //    if (Input.GetKeyDown(keySource.Key))
            //    {
            //        keySource.Value.enabled = true;
            //    }
            //    else if (Input.GetKeyUp(keySource.Key))
            //    {
            //        keySource.Value.enabled = false;
            //    }
            //}
        }
    }

}
