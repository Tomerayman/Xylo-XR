using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MusicXR
{
    [RequireComponent(typeof(AudioOut))]
    public class SynthKeyboard : MonoBehaviour
    {
        [SerializeField] private SynthKey firstWhiteKey;
        [SerializeField] private GameObject blackKeyPrefab;
        [SerializeField] private Transform keysParent;
        [SerializeField] private float firstKeyFrequency;
        [SerializeField] private float whiteKeySpacing;
        public AudioOut KeyboardAudioOut {  get; private set; }

        List<float> keyScale = new List<float> { 1, 1, 0.5f, 1, 1, 1, 0.5f };
        private float blackKeyHeightInterval;

        private void Awake()
        {
            firstWhiteKey.SetFrequency(firstKeyFrequency);
            KeyboardAudioOut = GetComponent<AudioOut>();
            blackKeyHeightInterval = whiteKeySpacing / 5f;

            CreateOctaves(firstWhiteKey, keyScale, 1);
        }

        public void CreateOctaves(SynthKey firstKey, List<float> scale, int octaves)
        {
            SynthKey last = firstKey;
            while (octaves > 0)
            {
                last = CreateKeysOctave(last, scale).Last();
                octaves--;
            }
        }

        public List<SynthKey> CreateKeysOctave(SynthKey firstKey, List<float> scale)
        {
            List<SynthKey> resultOctave = new List<SynthKey>() { firstKey };
            SynthKey prevKey, blackKey, whiteKey;
            Vector3 keyPosInterval = Vector3.right * whiteKeySpacing;

            foreach (float gap in scale)
            {
                prevKey = resultOctave.Last();

                if (gap == 1)
                {
                    blackKey = CreateBlackKey(resultOctave, prevKey, keyPosInterval);
                }

                whiteKey = CreateWhiteKey(resultOctave, prevKey, keyPosInterval, gap);

            }

            return resultOctave;
        }


        private SynthKey CreateWhiteKey(List<SynthKey> resultOctave, SynthKey prevKey, Vector3 keyPosInterval, float gap)
        {
            Vector3 position = prevKey.transform.position + keyPosInterval;
            return CreateKey(firstWhiteKey.gameObject, resultOctave, prevKey, position, gap == 0.5f);
        }


        private SynthKey CreateBlackKey(List<SynthKey> resultOctave, SynthKey prevKey, Vector3 keyPosInterval)
        {
            Vector3 position = prevKey.transform.position + (keyPosInterval / 2) + Vector3.up * blackKeyHeightInterval;
            return CreateKey(blackKeyPrefab, resultOctave, prevKey, position, true);
        }

        private SynthKey CreateKey(GameObject keyPrefab, List<SynthKey> resultOctave, SynthKey prevKey, Vector3 position, bool isSemitone)
        {
            SynthKey key = Instantiate(keyPrefab, position, Quaternion.identity, keysParent).GetComponent<SynthKey>();
            key.SetFrequency(isSemitone ? SemitoneAbove(prevKey.frequency) : ToneAbove(prevKey.frequency));
            KeyboardAudioOut.providers.Add(key.sampleGenerator);
            key.audioOut = KeyboardAudioOut;
            resultOctave.Add(key);
            return key;
        }


        // Function to calculate frequency of a note a tone above the given frequency
        private static float ToneAbove(float frequency)
        {
            // Define the frequency ratio for a tone (two semitones)
            float toneRatio = (float)System.Math.Pow(2, 2.0 / 12);

            // Calculate and return the frequency of the note a tone above
            return frequency * toneRatio;
        }


        // Function to calculate frequency of a note a semitone above the given frequency
        public static float SemitoneAbove(float frequency)
        {
            // Define the frequency ratio for a semitone
            float semitoneRatio = (float)System.Math.Pow(2, 1.0 / 12);

            // Calculate and return the frequency of the note a semitone above
            return frequency * semitoneRatio;
        }

    }
}


