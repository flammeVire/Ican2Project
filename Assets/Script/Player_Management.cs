using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Management : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public GameObject BoxInterractObj;

    bool IsInterracting = false;
    bool BoxMoving = false;
    private void Update()
    {
        Movement();
        InterractInput();
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal"), y = Input.GetAxis("Vertical");
        if (!BoxMoving)
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
            //le joueur n'est pas en interraction
            if (!IsInterracting)
            {
                //un objet est dans la zone d'interraction
                if(Interract.InterractObject != null)
                {
                    //cas pnj
                    if (Interract.InterractObject.layer == 6)
                    {
                        Interract.InterractObject.GetComponent<PNJ_Manager>().ShowDialogue();
                    }
                    //cas boite
                    else if(Interract.InterractObject.layer == 7)
                    {
                        Joint2D joint = Interract.InterractObject.AddComponent<FixedJoint2D>();
                        joint.connectedBody = rb;
                        BoxMoving = true;
                        Interract.InterractObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                        BoxInterractObj = Interract.InterractObject;
                    }
                    IsInterracting = true;
                }
                
            }
            //le joueur est en interraction
            else
            {
                IsInterracting = false;
                //si le joueur deplacer une boite
                if (BoxMoving)
                {
                    if(BoxInterractObj != null && BoxInterractObj.TryGetComponent<Joint2D>(out Joint2D component))
                    {
                        Interract.InterractObject = null;
                        Debug.Log(BoxInterractObj.GetComponent<Rigidbody2D>());
                        StartCoroutine(StopBoxMoving(BoxInterractObj.GetComponent<Rigidbody2D>()));
                        Destroy(component);
                        BoxMoving = false;
                    }
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
