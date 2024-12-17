using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseInWorld : MonoBehaviour
{
    public Vector2 CursorOnScreen;
    [SerializeField]Tilemap Tile_TileMap;
    [SerializeField]Tilemap Dynamic_TileMap;
    [SerializeField]Tilemap Cursor_TileMap;
    TileBase selected = null;
    Vector2 selectedPosition;

    private void Update()
    {
        GetMousePos();
        InputTilesSelected();
    }

    void GetMousePos()
    {
        Vector3 screenPoint = (Input.mousePosition);
        screenPoint.z = 10.0f;
        CursorOnScreen = new Vector2(Camera.main.ScreenToWorldPoint(screenPoint).x, Camera.main.ScreenToWorldPoint(screenPoint).y);
    }

    void InputTilesSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selected == null)
            {
                Debug.Log("Input 0");
                selected = GetTileAtWorldPosition(CursorOnScreen,Dynamic_TileMap);
                selectedPosition = CursorOnScreen;
                Debug.Log(selected);
                Debug.Log(new Vector2(selectedPosition.x,selectedPosition.y));
            }
            else
            {
                if (selected.name == "House")
                {
                    Move_DynamicTiles();
                }
                
            }
        }
    }

    TileBase GetTileAtWorldPosition(Vector3 worldPos,Tilemap MapWanted)
    {
        Vector3Int cellPosition = Dynamic_TileMap.WorldToCell(worldPos);
        TileBase tile = MapWanted.GetTile(cellPosition);
        return tile;
    }


    Vector2Int MoveVector()
    {
        float deltaX = Mathf.Abs(CursorOnScreen.x - selectedPosition.x);
        float deltaY = Mathf.Abs(CursorOnScreen.y - selectedPosition.y);

        if (deltaX > 1.5 || deltaY > 1.5)
        {
            return new Vector2Int(0,0); // Trop éloigné
        }

        if (deltaX > deltaY) // Gauche/Droite
        {
            return CursorOnScreen.x < selectedPosition.x ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);
        }
        else if (deltaY > deltaX) // Haut/Bas
        {
            return CursorOnScreen.y > selectedPosition.y ? new Vector2Int(0,1) : new Vector2Int(0, -1);
        }

        return new Vector2Int(0, 0);
    }

    void Move_DynamicTiles()
    {
        if(MoveVector() != Vector2Int.zero)
        {
            if(GetTileAtWorldPosition(CursorOnScreen,Tile_TileMap).name == "ClassicFloor")
            {
                Debug.Log("move to classic floor");
                Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x,(int)selectedPosition.y,0),null);
                Dynamic_TileMap.SetTile(new Vector3Int((int)CursorOnScreen.x,(int)CursorOnScreen.y,0),selected);
            }
            else if(selected.name == "House")
            {
                if(GetTileAtWorldPosition(CursorOnScreen, Tile_TileMap).name == "Hole")
                {
                    //return true;
                }
            }
            else if(selected.name == "1citizen" ||  selected.name == "2citizen" || selected.name == "3citizen")
            {

            }
        }
       // return false;
    }

}
