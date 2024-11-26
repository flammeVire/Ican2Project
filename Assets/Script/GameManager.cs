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

    public Transform[] centerofQuartier;
    
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

    private void Update()
    {
        CameraZoom(true);
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
                transform.position = centerofQuartier[0].position;
                break;
            case QuartierDispo.Quartier2:
                transform.position = centerofQuartier[1].position;

                break;
            case QuartierDispo.Quartier3:
                transform.position = centerofQuartier[2].position;

                break;
            case QuartierDispo.Quartier4:
                transform.position = centerofQuartier[3].position;

                break;
            default:
                Debug.Log("Wrong Quartier");
                break;
        }
    }
    #endregion
}
