using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainAnimation: MonoBehaviour
{
    [SerializeField]Sprite[] sprites;
    [SerializeField] float delay;
    public SoundManagement soundManagement;
    private void Start()
    {
        StartCoroutine(rain());
    }

    IEnumerator rain()
    {
        for (int i = 0; i < sprites.Length; i++) 
        {
            yield return new WaitForSeconds(delay);
            this.GetComponent<SpriteRenderer>().sprite = sprites[i];
        }
        Destroy(this.gameObject);
    }
    
    public void sound()
    {
        soundManagement.PlaySound(soundManagement.RainSound, soundManagement.VoiceSource);
    }
}

