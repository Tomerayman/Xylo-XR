using MusicXR;
using MusicXR.Native;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SynthKey : MonoBehaviour
{
    public float frequency;
    public XRSimpleInteractable interactable;
    public AudioSource audioSource;
    public SampleGenerator sampleGenerator;
    public SynthOut synthOut;


    public void SetFrequency(float freq)
    {
        frequency = freq;
        sampleGenerator.frequency = freq;
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPressEnter);
        interactable.selectExited.AddListener(OnPressExit);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPressEnter);
        interactable.selectExited.RemoveListener(OnPressExit);
    }


    private void OnPressEnter(BaseInteractionEventArgs e = null)
        => audioSource.enabled = true;
    
    private void OnPressExit(BaseInteractionEventArgs e = null)
        => audioSource.enabled = false;

}
