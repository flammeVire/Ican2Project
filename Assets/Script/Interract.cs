using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interract : MonoBehaviour
{
    static public GameObject InterractObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
            if (collision.gameObject.tag == "Interractable")
            {
                InterractObject = collision.gameObject;
            Debug.Log("InterractableOn");
            }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interractable")
        {
            if(collision.gameObject.GetComponent<FixedJoint2D>() == null)
            {
                InterractObject = null;
            }
            
            Debug.Log("InterractableOff");
        }
    }
}
