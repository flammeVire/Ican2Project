using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Management : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public GameObject CurrentinterractObj;
    bool IsInterracting;

    private void Update()
    {
        Movement();
        InterractInput();
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical");
        if (!IsInterracting)
        {
            rb.velocity = new Vector2(x, y) * speed;
            if (x > 0)
            {
                //GetComponent<SpriteRenderer>().flipX = false;
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            else if (x < 0)
            {
                // GetComponent<SpriteRenderer>().flipX = true;
                transform.rotation = Quaternion.Euler(0, 0, 90);

            }
            if (y < 0)
            {
                GetComponent<SpriteRenderer>().flipY = true;
                transform.rotation = Quaternion.Euler(0, 0, 180);

            }
            else if (y > 0)
            {
                GetComponent<SpriteRenderer>().flipY = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);

            }
        }
        else
        {
            rb.velocity = new Vector2(x, y) * (speed /2);
        }
    }

    #region InterractInput
    void InterractInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            CurrentinterractObj = Interract.InterractObject;

            if (CurrentinterractObj != null)
            {
                if (CurrentinterractObj.layer == 7)
                {
                    if (CurrentinterractObj.TryGetComponent<FixedJoint2D>(out FixedJoint2D component))
                    {
                        Debug.Log("component == null");
                        Destroy(component);
                        IsInterracting = false;
                        StartCoroutine(StopBoxMoving(CurrentinterractObj.GetComponent<Rigidbody2D>()));

                    }
                    else
                    {
                        Joint2D joint = CurrentinterractObj.AddComponent<FixedJoint2D>();
                        joint.connectedBody = rb;
                        IsInterracting = true;
                        CurrentinterractObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    }
                }
                else if (CurrentinterractObj.layer == 6)
                {
                    //definir ce qui se passe quand on rentre dans un pnj
                    //sois afficher bulle de dialogue
                    // sois parle tout seul quand on est proche
                }
            }
            else
            {
                Joint2D ObjPush = FindAnyObjectByType<FixedJoint2D>();
                if (ObjPush.connectedBody == this)
                {
                    Debug.Log("component == null");
                    StartCoroutine(StopBoxMoving(ObjPush.GetComponent<Rigidbody2D>()));
                    
                    IsInterracting = false;
                    Destroy(ObjPush);
                }
            }
        }
    }

    IEnumerator StopBoxMoving(Rigidbody2D objrb)
    {
        objrb.bodyType = RigidbodyType2D.Static;
        yield return null;
        objrb.bodyType = RigidbodyType2D.Kinematic;
    }
    #endregion
}
