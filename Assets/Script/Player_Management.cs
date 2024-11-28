using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Management : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public GameObject BoxInterractObj;

    [Header("Ash Data")]
    public GameObject AshHalo;
    float LifeTime;
    public float MaxLifeTime;
    public bool inAsh;

    bool IsInterracting = false;
    bool BoxMoving = false;
    bool HaveItemNeeded;

    private void Update()
    {
        Movement();
        InterractInput();

        if (inAsh)
        {
            Suffocating();
            
        }
        else
        {
            LifeTime = MaxLifeTime;
        }

        
        HaloReducing(inAsh);
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
        else // bouge en ligne droite uniquement
        {
           // rb.velocity = new Vector2(x, y) * (speed /2);

            if(BoxInterractObj.transform.rotation != Quaternion.Euler(0,0,0))
            {
                Debug.Log("different de 0");
                rb.velocity = new Vector2(0, y) * speed/2;

            }
            else
            {
                Debug.Log("== de 0");
                rb.velocity = new Vector2(x, 0) * speed /2;
                
            }
            
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
                        Debug.Log("have item ==" + HaveItemNeeded);
                            Interract.InterractObject.GetComponent<PNJ_Manager>().ShowDialogue(HaveItemNeeded);
                    }
                    //cas boite
                    else if(Interract.InterractObject.layer == 7)
                    {
                        Joint2D joint = Interract.InterractObject.AddComponent<FixedJoint2D>();
                        joint.connectedBody = rb;
                        BoxMoving = true;
                        Interract.InterractObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                        BoxInterractObj = Interract.InterractObject;
                        transform.position = Interract.InterractObject.transform.position;
                    }
                    //cas Object
                    else if(Interract.InterractObject.layer == 10)
                    {
                        HaveItemNeeded = true;
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

    #region ash

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ash")
        {
            Debug.Log("InAsh");
            inAsh = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ash")
        {
            Debug.Log("InAsh");
            inAsh = false;
        }
    }

    void Suffocating()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0) 
        {
            Debug.Log("Player Is Dead");
        }
    }

    void HaloReducing(bool Actif)
    {
        if (Actif)
        {
            float LifeInPercent = LifeTime*10 / 100;
            float MaskInPercent = AshHalo.GetComponent<DynamicHole>().MaskSize * LifeInPercent;
            AshHalo.GetComponent<DynamicHole>().maskSize = MaskInPercent;

            AshHalo.GetComponent<DynamicHole>().targetObject = AshHalo;
        }
        else 
        {
            AshHalo.GetComponent<DynamicHole>().targetObject = AshHalo.GetComponent<DynamicHole>().FakePoint;

            AshHalo.GetComponent<DynamicHole>().maskSize = AshHalo.GetComponent<DynamicHole>().MaskSize;
        }
    }


    #endregion
}
