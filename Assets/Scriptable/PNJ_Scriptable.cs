using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pnj_data", menuName = "ScriptableObjects/Pnj_data", order = 1)]
public class PNJ_Scriptable : ScriptableObject
{
    [TextArea] public string Dialogue1;
    [TextArea] public string Dialogue2;

}
