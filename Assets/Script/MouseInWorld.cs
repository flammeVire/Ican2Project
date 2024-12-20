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
    [SerializeField] TileBase Floor, Ashes, Hole,Cursor;
    TileBase selected = null;
    Vector2 selectedPosition;
    public GameManager gameManager;


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
       // Debug.Log("cursor:" + CursorOnScreen); 
    }

    void InputTilesSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selected == null)
            {
                Vector3Int tilePos;
                Debug.Log("Input 0");
                (selected,tilePos) = GetTileAtWorldPosition(CursorOnScreen,Dynamic_TileMap);
                CursorSelector(tilePos, Cursor);
                selectedPosition = new Vector2(tilePos.x, tilePos.y);
                Debug.Log(selected);
                Debug.Log(selectedPosition);
            }
            else
            {
                if (selected.name == "House")
                {
                    Move_House();
                }
                
            }
        }
    }

    (TileBase,Vector3Int) GetTileAtWorldPosition(Vector3 worldPos,Tilemap MapWanted)
    {
        Vector3Int cellPosition = MapWanted.WorldToCell(worldPos);
        
        TileBase tile = MapWanted.GetTile(cellPosition);
        

        return (tile,cellPosition);
    }



    void Move_House()
    {
        (TileBase tileBase, Vector3Int cellPos) = (GetTileAtWorldPosition(CursorOnScreen, Tile_TileMap));
        if (tileBase.name == "ClassicFloor")
        {
            (TileBase dynamicTile, _) = GetTileAtWorldPosition(cellPos, Dynamic_TileMap);
            if (dynamicTile == null)
            {
                Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), selected);
                CursorSelector(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), Cursor);
                gameManager.PALeft -= 1;
                gameManager.UpdateText();
            }
            
            selected = null;
            selectedPosition = Vector2.zero;
        }
        else if (tileBase.name == "Hole")
        {
            Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
            Tile_TileMap.SetTile(new Vector3Int((int) cellPos.x, (int) cellPos.y, 0),Floor);
        }
    }

    void CursorSelector(Vector3Int cellPos, TileBase cursor)
    {
        Cursor_TileMap.SetTile(cellPos, cursor);
    }


    Vector2Int MoveVector()
    {
        float deltaX = Mathf.Abs(CursorOnScreen.x - selectedPosition.x);
        float deltaY = Mathf.Abs(CursorOnScreen.y - selectedPosition.y);

        if(deltaX > 1.5 || deltaY > 1.5)
        {
            return new Vector2Int(0,0); // Trop éloigné
        }

        if(deltaX > deltaY) // Gauche/Droite
        {
            return CursorOnScreen.x < selectedPosition.x ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);
        }
        else if(deltaY > deltaX) // Haut/Bas
        {
            return CursorOnScreen.y > selectedPosition.y ? new Vector2Int(0,1) : new Vector2Int(0, -1);
        }

        return new Vector2Int(0, 0);
    }
}
