using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Camera cam;
    [SerializeField] float DefaultZoom;
    [SerializeField] float DefaultUnZoom;
    [SerializeField] float ActualZoom;
    [SerializeField] float SpeedZoom;

    [Header("Player")]
    [SerializeField] GameObject Player;
    [SerializeField] Transform NextPlayerPos;

    [Header("Quartier Data")]
    [SerializeField] Transform[] centerofQuartier;
    [SerializeField] float[] SizeOfCam;
    
    public QuartierDispo ActualQuartier;
    public enum QuartierDispo
    {
        Quartier1
            , Quartier2
            , Quartier3
            , Quartier4
            , Intro
            , Outro
    }

    private void Start()
    {
        Player_Tp(NextPlayerPos);
        CameraOnQuartier(ActualQuartier);
    }

    private void Update()
    {
        //CameraZoom(true);
    }

    #region CameraManagement
    

    void CameraZoom(bool zoomed)
    {
        if (Input.GetAxisRaw("Fire1")>0)
        {
            Debug.Log("Unzoom");
            if (ActualZoom <= DefaultUnZoom)
            {
                ActualZoom = ActualZoom + Time.deltaTime * SpeedZoom;
            }
            else
            {
                ActualZoom = DefaultUnZoom;
            }
                
        }
        else 
        {
            Debug.Log("zooming");
            if(ActualZoom >= DefaultZoom)
            {
                ActualZoom = ActualZoom - Time.deltaTime * SpeedZoom;
            }
            else
            {
                ActualZoom = DefaultZoom;
            }
        }

        cam.orthographicSize = ActualZoom;
    }

    public void CameraOnQuartier(QuartierDispo quartier)
    {
        switch (quartier)
        {
            case QuartierDispo.Quartier1:
                transform.position = new Vector3(centerofQuartier[0].position.x,centerofQuartier[0].position.y,-100);
                cam.orthographicSize = SizeOfCam[0];
                break;
            case QuartierDispo.Quartier2:
                transform.position = new Vector3(centerofQuartier[1].position.x, centerofQuartier[1].position.y, -100);
                cam.orthographicSize = SizeOfCam[1];

                break;
            case QuartierDispo.Quartier3:
                transform.position = new Vector3(centerofQuartier[2].position.x, centerofQuartier[2].position.y, -100);
                cam.orthographicSize = SizeOfCam[2];

                break;
            case QuartierDispo.Quartier4:
                transform.position = new Vector3(centerofQuartier[3].position.x, centerofQuartier[3].position.y, -100);
                cam.orthographicSize = SizeOfCam[3];

                break;
            default:
                Debug.Log("Wrong Quartier");
                break;
        }
    }

    #endregion

    #region Game Management

    public void Player_Tp(Transform transform)
    {
        Vector3 top_pos = new Vector3(transform.position.x,transform.position.y, 0);
        Player.transform.position = top_pos;
    }

    #endregion
}
