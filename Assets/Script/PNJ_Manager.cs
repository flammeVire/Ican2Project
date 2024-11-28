using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;


public class PNJ_Manager : MonoBehaviour
{
    public bool PlayerHasInterracting = false;
    [SerializeField] PNJ_Scriptable Pnj_Scriptable;
    [SerializeField] TextMeshProUGUI BulleDialogue;
    [SerializeField] GameObject Canva;
    string currentMessage = "";
    [SerializeField] float delay = 0.1f;
    
    public void ShowDialogue(bool hasItem)
    {
        string message;
        if (!hasItem) 
        {
             message = Pnj_Scriptable.Dialogue1;
        }
        else
        {
             message = Pnj_Scriptable.Dialogue2;
        }

        if (!PlayerHasInterracting) 
        {
            StartCoroutine(ShowLetterByLetter(message));
            PlayerHasInterracting=true;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowLetterByLetter(message));
        }
    }

    IEnumerator ShowLetterByLetter(string message)
    {
        Canva.SetActive(true);

        if (currentMessage.Length != message.Length)
        {
            for (int i = 0; i <= message.Length; i++)
            {
                currentMessage = message.Substring(0, i);
                BulleDialogue.text = currentMessage;
                yield return new WaitForSeconds(delay);
            }
        }
        
        yield return new WaitForSeconds(3);
        Canva.SetActive(false);

    }
}
