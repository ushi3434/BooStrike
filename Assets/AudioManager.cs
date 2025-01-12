using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM;

    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip releaseJetSound1; //Žã‚¢
    [SerializeField] private AudioClip releaseJetSound2;
    [SerializeField] private AudioClip releaseJetSound3;
    [SerializeField] private AudioClip releaseJetSound4; //‹­‚¢
    [SerializeField] private AudioClip chargeJetSound;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayDeadSound()
    {
        audioSource.PlayOneShot(deadSound,0.9f);
    }

    public void PlayReleaseJet(float ratio)
    {
        if (ratio < 0.25f)
            audioSource.PlayOneShot(releaseJetSound1);
        else if (ratio < 0.5f)
            audioSource.PlayOneShot(releaseJetSound2);
        else if (ratio < 0.75f)
            audioSource.PlayOneShot(releaseJetSound3);
        else
            audioSource.PlayOneShot(releaseJetSound4);
    }

    public void PlayChargeJet()
    {
        audioSource.PlayOneShot(chargeJetSound,0.6f);
    }

    public void StopBGM()
    {
        BGM.Stop();
    }

}
