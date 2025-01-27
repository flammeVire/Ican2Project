using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagement : MonoBehaviour
{
    [SerializeField] public AudioClip MovingSound;
    [SerializeField] public AudioClip DeathSound;
    [SerializeField] public AudioClip ButtonSound;
    [SerializeField] public AudioClip MyTurnSound;
    [SerializeField] public AudioClip IATurnSound;
    [SerializeField] public AudioClip HouseMovingSound;
    [SerializeField] public AudioClip SpawnFireSound;
    [SerializeField] public AudioClip FireSound;
    [SerializeField] public AudioClip SpawnVoidSound;
    [SerializeField] public AudioClip AshSound;
    [SerializeField] public AudioClip BoatComingSound;
    [SerializeField] public AudioClip BoatLeavingSound;
    [SerializeField] public AudioClip RainSound;

    [SerializeField] public AudioSource SFXSource;
    [SerializeField] public AudioSource AshSource;
    [SerializeField] public AudioSource FireSource;
    [SerializeField] public AudioSource VoiceSource;
    
    public void PlaySound (AudioClip clip,AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }


}
