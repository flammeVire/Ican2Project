using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Turn and PA",order = 1)]
    [SerializeField]int TurnMax;
    int TurnLeft;
    [SerializeField]int PAMax;
    int PALeft;
    bool IsPlayerTurn;


    [Header("TMPro Ref", order = 2)]
    [SerializeField] TextMeshProUGUI PA_Text;
    [SerializeField] TextMeshProUGUI Turn_Text;


    private void Start()
    {
        StartCoroutine(PlayerTurn());
        TurnLeft = TurnMax;
        PALeft = PAMax;
    }


    IEnumerator PlayerTurn()
    {
        yield return null;
        PALeft = PAMax;
        IsPlayerTurn = true;
        UpdateText();
        yield return new WaitUntil(() => PALeft <= 0);
        yield return new WaitForSeconds(0.5f);
        TurnLeft -= 1;
        IsPlayerTurn = false;
        StartCoroutine(IaTurn());
    }
    IEnumerator IaTurn()
    {
        yield return null;
        
        StartCoroutine (PlayerTurn());
    }

    #region Button

    public void Spell1()
    {
        if(PALeft >= 1)
        {
            PALeft -= 1;
            UpdateText();
        }
        else
        {
            Debug.Log("pas assez de PA" + PALeft);
        }
    }

    public void Spell2() 
    {
        if (PALeft >= 2)
        {
            PALeft -= 2;
            UpdateText();
        }
        else
        {
            Debug.Log("pas assez de PA :" + PALeft);
            
        }
    }
    public void EndOfTurn()
    {
        PALeft = 0;
    }

    #endregion

    #region ShowText

    private void UpdateText()
    {
        PA_Text.text = "PA: " + PALeft;
        PA_Text.text = "PA: " + PALeft;
        Turn_Text.text = "Turn left: " + TurnLeft;
    }

    #endregion
}
