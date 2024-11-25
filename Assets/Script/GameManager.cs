using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerPos;

    private void Update()
    {
        CamFollowing();
    }

    void CamFollowing()
    {
        transform.position = new Vector3(playerPos.position.x,playerPos.position.y,transform.position.z);
    }

}
