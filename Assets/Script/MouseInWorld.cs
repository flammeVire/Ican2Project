using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseInWorld : MonoBehaviour
{
    #region Data
    public GameManager gameManager;

    public Vector2 CursorOnScreen;
    TileBase selected = null;
    Vector2 selectedPosition;

    #endregion


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
                (selected, tilePos) = gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Dynamic_TileMap);

                if (selected != gameManager.BoatTiles)
                {
                    selectedPosition = new Vector2(tilePos.x, tilePos.y);

                    if (selected != null)
                    {
                        gameManager.Cursor_TileMap.SetTile(tilePos, gameManager.cursor[0]);
                    }

                    Debug.Log(selected);
                    Debug.Log(selectedPosition);
                }
                else
                {
                    selected = null;
                }
            }
            else
            {
                if (gameManager.TileExistInArray(selected, gameManager.House) || gameManager.TileExistInArray(selected, gameManager.Citizens))
                {
                    Move_Dynamics(selected);
                }

            }
        }
    }




    void Move_Dynamics(TileBase tileSelected)
    {
        (TileBase tileBase, Vector3Int cellPos) = (gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Tile_TileMap));
        (TileBase dynamicTile, _) = gameManager.GetTileAtWorldPosition(cellPos, gameManager.Dynamic_TileMap);
        (TileBase AshesTiles, _) = gameManager.GetTileAtWorldPosition(cellPos, gameManager.Ashes_TileMap);


        //cas maison
        if (gameManager.TileExistInArray(tileSelected, gameManager.House))
        {
            if (dynamicTile == null && AshesTiles == null)
            {
                if (gameManager.TileExistInArray(tileBase, gameManager.Floors))
                {
                    gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                    gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), selected);
                    gameManager.Cursor_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), null);
                    gameManager.PALeft -= 1;
                    gameManager.UpdateText();
                }
                else if (gameManager.TileExistInArray(tileBase, gameManager.Holes))
                {
                    gameManager.Tile_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Floors[0]);
                    gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                    gameManager.PALeft -= 1;
                    gameManager.UpdateText();
                }

            }
        }

        //cas citoyens
        else if (gameManager.TileExistInArray(tileSelected, gameManager.Citizens))
        {
            //cas normal
            if (!gameManager.UnfusionActive)
            {
                if (dynamicTile == null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {
                    //classic floor
                    if (gameManager.TileExistInArray(tileBase, gameManager.Floors))
                    {
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), selected);
                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                    }
                    // drain
                    else if (gameManager.TileExistInArray(tileBase, gameManager.Drain))
                    {
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), selected);
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                    }
                    //narrowStreet
                    else if (gameManager.TileExistInArray(tileBase, gameManager.NarrowStreet))
                    {
                        if (dynamicTile == null && tileSelected == gameManager.Citizens[0])
                        {
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), selected);
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                    }
                }

                
                else if (dynamicTile != null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {
                    //cas citoyens sur citoyens
                    if (gameManager.TileExistInArray(dynamicTile, gameManager.Citizens))
                    {
                        if (gameManager.TileExistInArray(tileBase, gameManager.Floors) || gameManager.TileExistInArray(tileBase, gameManager.Drain))
                        {
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Citizens[HowManyCitizensOnTiles(tileSelected, dynamicTile) - 1]);
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                    }

                    else if (gameManager.TileExistInArray(tileBase, gameManager.Waters) && dynamicTile == gameManager.BoatTiles)
                    {
                        Debug.Log("ON A BOAT");
                        if (TotalOfCitizensSelected(tileSelected)+1 <= 2 - gameManager.PassagersOnBoat) 
                        {
                            Debug.Log("assez de place sur le bateau");
                            gameManager.PassagersOnBoat += TotalOfCitizensSelected(tileSelected)+1;
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                        else
                        {
                            Debug.Log("LE BATEAU EST PLEIN");
                        }

                    }
                }
            }

            //cas defusion
            else if (gameManager.UnfusionActive && TotalOfCitizensSelected(tileSelected) != 0)
            {
                Debug.Log("Move unfuiosn");
                if (dynamicTile == null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {
                    Debug.Log("dynamic tile == null");
                    //classic floor
                    if (gameManager.TileExistInArray(tileBase, gameManager.Floors))
                    {
                        Debug.Log("Floors");
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizens[TotalOfCitizensSelected(selected) - 1]);
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Citizens[0]);
                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                    }

                    // drain
                    else if (gameManager.TileExistInArray(tileBase, gameManager.Drain))
                    {
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizens[TotalOfCitizensSelected(selected) - 1]);
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Citizens[0]);
                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                    }
                    //narrowStreet
                    else if (gameManager.TileExistInArray(tileBase, gameManager.NarrowStreet))
                    {
                        if (dynamicTile == null)
                        {
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizens[TotalOfCitizensSelected(selected) - 1]);
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Citizens[0]);
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                    }
                    gameManager.Speel3();
                }
                /*
                else if (dynamicTile != null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {
                    //cas citoyens sur citoyens
                    if (gameManager.TileExistInArray(dynamicTile, gameManager.Citizens))
                    {
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Citizens[HowManyCitizensOnTiles(tileSelected, dynamicTile) - 1]);
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                    }
                }*/
            }
        }
       
        
        gameManager.Cursor_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
        gameManager.AshesPrevision();
        selected = null;
        selectedPosition = Vector2.zero;
    }

    bool IsTilesAdjacent(Vector3Int cellpos)
    {
        float deltaX = Mathf.Abs(cellpos.x - selectedPosition.x);
        float deltaY = Mathf.Abs(cellpos.y - selectedPosition.y);

        if (deltaX > 1.5 || deltaY > 1.5)
        {
            return false; // Trop �loign�
        }

        if (deltaX > deltaY) // Gauche/Droite
        {
            return true;
        }
        else if (deltaY > deltaX) // Haut/Bas
        {
            return true;
        }

        return false;
    }
    int HowManyCitizensOnTiles(TileBase citizenSelected, TileBase citizenTile)
    {
        int x, y;

        for (x = 1; x < gameManager.Citizens.Length + 1; x++)
        {
            if (gameManager.Citizens[x - 1] == citizenSelected)
            {
                break;
            }
        }
        for (y = 1; y < gameManager.Citizens.Length + 1; y++)
        {
            if (gameManager.Citizens[y - 1] == citizenTile)
            {
                break;
            }
        }
        Debug.Log("il y a " + (x + y) + "citoyens");
        return x + y;
    }
    int TotalOfCitizensSelected(TileBase TileSelected)
    {
        int number = 0;

        for (int i = 0; i < gameManager.Citizens.Length; i++)
        {
            if (TileSelected == gameManager.Citizens[i])
            {
                number = i;
                break;
            }
        }
        Debug.Log("total of citizens selected = " + number);
        return number;
    }

}

