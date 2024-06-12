using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SynthKeyboard : MonoBehaviour
{
    [SerializeField] private SynthKey firstWhiteKey;
    [SerializeField] private GameObject blackKeyPrefab;
    [SerializeField] private Transform keysParent;
    [SerializeField] private float firstKeyFrequency;
    [SerializeField] private float whiteKeySpacing;


    List<float> keyScale = new List<float> { 1, 1, 0.5f, 1, 1, 1, 0.5f };

    private void Awake()
    {
        firstWhiteKey.SetFrequency(firstKeyFrequency);
                
        
        
        CreateOctaves(firstWhiteKey, keyScale, 3);
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
        Vector3 blackKeyHeightInterval = Vector3.up * whiteKeySpacing / 5f;


        foreach (float gap in scale)
        {
            prevKey = resultOctave.Last();
            
            if (gap == 1)
            {
                blackKey = Instantiate(blackKeyPrefab,
                    prevKey.transform.position + (keyPosInterval / 2) + blackKeyHeightInterval,
                    Quaternion.identity, keysParent).GetComponent<SynthKey>();
                blackKey.SetFrequency(SemitoneAbove(prevKey.frequency));
                resultOctave.Add(blackKey);
            }

            whiteKey = Instantiate(firstWhiteKey, prevKey.transform.position + keyPosInterval, 
                Quaternion.identity, keysParent).GetComponent<SynthKey>();
            whiteKey.SetFrequency((gap == 1) ? ToneAbove(prevKey.frequency) : SemitoneAbove(prevKey.frequency));
            resultOctave.Add(whiteKey);

        }

        return resultOctave;
    }




    // Function to calculate frequency of a note a tone above the given frequency
    public static float ToneAbove(float frequency)
    {
        // Define the frequency ratio for a tone (two semitones)
        float toneRatio = (float) Math.Pow(2, 2.0 / 12);

        // Calculate and return the frequency of the note a tone above
        return frequency * toneRatio;
    }

    // Function to calculate frequency of a note a semitone above the given frequency
    public static float SemitoneAbove(float frequency)
    {
        // Define the frequency ratio for a semitone
        float semitoneRatio = (float)Math.Pow(2, 1.0 / 12);

        // Calculate and return the frequency of the note a semitone above
        return frequency * semitoneRatio;
    }

}

//[Serializable]
//public class SynthKeyData
//{
//    public GameObject obj;
//    public double frequency;
//    public bool isBlack;
//}
