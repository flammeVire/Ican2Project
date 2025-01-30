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
    public Vector2 selectedPosition;
    public Vector3Int[] DrainPosition;
    public Vector3Int MouseInputTuto;

    TileBase[] Rain_Dynamicstiles = new TileBase[9];
    bool cancelRain = false;
    #endregion


    private void Update()
    {
        if (gameManager.IsPlayerTurn)
        {
            GetMousePos();
            if (gameManager.InTuto)
            {
                if (gameManager.RainActive)
                {
                    RainCursor();
                    InputRainTuto();
                }
                else
                {
                    InputTuto();
                }
            }
            else
            {
                if (gameManager.RainActive)
                {
                    RainCursor();
                    InputRain();
                }
                else
                {
                    InputTilesSelected();
                }
            }
        }
    }
    #region tuto
    void InputTuto()
    {
        if (Input.GetMouseButtonDown(0))
        {
            (_, MouseInputTuto) = gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Dynamic_TileMap);
            if (MouseInputTuto == new Vector3Int((int)gameManager.CoordTuto.x, (int)gameManager.CoordTuto.y, 0))
            {
                if (selected == null)
                {
                    Vector3Int tilePos;
                    (selected, tilePos) = gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Dynamic_TileMap);

                    if (selected != gameManager.BoatTiles && !gameManager.TileExistInArray(selected, gameManager.Fire))
                    {
                        selectedPosition = new Vector2(tilePos.x, tilePos.y);

                        if (selected != null)
                        {
                            gameManager.Cursor_TileMap.SetTile(tilePos, gameManager.DefaultCursor[0]);
                            StartCoroutine(gameManager.AnimateOneTile(tilePos, gameManager.DefaultCursor, gameManager.CursorDelay, gameManager.Cursor_TileMap));
                        }

                        //Debug.Log(selected);
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
    }

    void InputRainTuto()
    {
        if (Input.GetMouseButtonDown(0))
        {
            (_, MouseInputTuto) = gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Dynamic_TileMap);
            if (MouseInputTuto == new Vector3Int((int)gameManager.CoordTuto.x, (int)gameManager.CoordTuto.y, 0))
            {
                for (int i = 0; i < Rain_Dynamicstiles.Length; i++)
                {
                    (TileBase floor, _) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[i], gameManager.Tile_TileMap);
                    if (floor != null)
                    {
                        cancelRain = false;
                        break;
                    }
                    else
                    {
                        cancelRain = true;
                    }
                }
                if (!cancelRain)
                {
                    for (int i = 0; i < Rain_Dynamicstiles.Length; i++)
                    {
                        if (gameManager.TileExistInArray(Rain_Dynamicstiles[i], gameManager.Fire))
                        {
                            for (int j = gameManager.fireList.Count - 1; j >= 0; j--)
                            {
                                if (gameManager.fireList[j].Position == gameManager.RainPosition[i])
                                {
                                    gameManager.fireList.RemoveAt(j);
                                }
                            }
                            gameManager.Dynamic_TileMap.SetTile(gameManager.RainPosition[i], null);
                        }
                        gameManager.Rain_TileMap.SetTile(gameManager.RainPosition[i], null);
                    }
                    GameObject clone = Instantiate(gameManager.RainSprite,(Vector3)CursorOnScreen, Quaternion.identity);
                    clone.GetComponent<RainAnimation>().soundManagement = gameManager.Sound;
                    clone.GetComponent<RainAnimation>().sound();
                    gameManager.Spell2();
                    gameManager.PALeft -= gameManager.PaForRain;
                    gameManager.UpdateText();
                    gameManager.FirePrevision();
                }
                else
                {
                    gameManager.Spell2();
                }
            }
        }
    }


    #endregion
    void GetMousePos()
    {
        Vector3 screenPoint = (Input.mousePosition);
        screenPoint.z = 10.0f;
        CursorOnScreen = new Vector2(Camera.main.ScreenToWorldPoint(screenPoint).x, Camera.main.ScreenToWorldPoint(screenPoint).y);
    }

    void InputTilesSelected()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            gameManager.Cursor_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
            selected = null;
            selectedPosition = Vector2.zero;

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selected == null)
            {
                Vector3Int tilePos;
                (selected, tilePos) = gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Dynamic_TileMap);

                if (selected != gameManager.BoatTiles && !gameManager.TileExistInArray(selected,gameManager.Fire))
                {
                    selectedPosition = new Vector2(tilePos.x, tilePos.y);

                    if (selected != null)
                    {
                        gameManager.Cursor_TileMap.SetTile(tilePos, gameManager.DefaultCursor[0]);
                        StartCoroutine(gameManager.AnimateOneTile(tilePos,gameManager.DefaultCursor,gameManager.CursorDelay,gameManager.Cursor_TileMap));
                    }

                    //Debug.Log(selected);
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
                    if(selected == gameManager.House[0])
                    {
                    StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.House1MovingStart, cellPos, gameManager.House1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap,selected));
                    }
                    else if(selected == gameManager.House[1])
                    {
                        StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.House2MovingStart, cellPos, gameManager.House2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                    }
                    else if (selected == gameManager.House[2])
                    {
                        StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.House3MovingStart, cellPos, gameManager.House3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                    }
                    gameManager.Cursor_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), null);
                    gameManager.PALeft -= 1;
                    gameManager.UpdateText();
                }
                else if (gameManager.TileExistInArray(tileBase, gameManager.Holes))
                {
                    if (selected == gameManager.House[0])
                    {
                        StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.House1MovingStart, cellPos, gameManager.House1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected, gameManager.Floors[0],gameManager.Tile_TileMap));
                    }
                    else if (selected == gameManager.House[1])
                    {
                        StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.House2MovingStart, cellPos, gameManager.House2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected,gameManager.Floors[0], gameManager.Tile_TileMap));

                    }
                    else if (selected == gameManager.House[2])
                    {
                        StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.House3MovingStart, cellPos, gameManager.House3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected, gameManager.Floors[0], gameManager.Tile_TileMap));

                    }
                    gameManager.PALeft -= 1;
                    gameManager.UpdateText();
                }

            }
            gameManager.Sound.PlaySound(gameManager.Sound.HouseMovingSound, gameManager.Sound.SFXSource);

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
                        if (selected == gameManager.Citizens[0])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));
                        }
                        else if (selected == gameManager.Citizens[1])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                        }
                        else if (selected == gameManager.Citizens[2])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                        }
                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                    }
                    // drain
                    else if (gameManager.TileExistInArray(tileBase, gameManager.Drain))
                    {
                        if (selected == gameManager.Citizens[0])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));
                        }
                        else if (selected == gameManager.Citizens[1])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                        }
                        else if (selected == gameManager.Citizens[2])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                        }
                        gameManager.PALeft -= 1;
                        gameManager.PaForRain = 1;
                        gameManager.UpdateText();
                    }
                    //narrowStreet
                    else if (gameManager.TileExistInArray(tileBase, gameManager.NarrowStreet))
                    {
                        if (dynamicTile == null && tileSelected == gameManager.Citizens[0])
                        {
                            if (selected == gameManager.Citizens[0])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));
                            }
                            else if (selected == gameManager.Citizens[1])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                            }
                            else if (selected == gameManager.Citizens[2])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                            }
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                    }
                    //gate
                    else if (gameManager.TileExistInArray(tileBase, gameManager.Gate))
                    {
                        Debug.Log("gates");
                        //porte ouverte
                        if (tileBase == gameManager.Gate[1])
                        {
                            if (selected == gameManager.Citizens[0])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));
                            }
                            else if (selected == gameManager.Citizens[1])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                            }
                            else if (selected == gameManager.Citizens[2])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                            }
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                        //porte fermé
                        else if(tileBase == gameManager.Gate[0])
                        {
                            if(TotalOfCitizensSelected(tileSelected)+1 > 1)
                            {
                                gameManager.Tile_TileMap.SetTile(cellPos, gameManager.Gate[1]);
                                if (selected == gameManager.Citizens[0])
                                {
                                    StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));
                                }
                                else if (selected == gameManager.Citizens[1])
                                {
                                    StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                                }
                                else if (selected == gameManager.Citizens[2])
                                {
                                    StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected));

                                }
                                gameManager.PALeft -= 1;
                                gameManager.UpdateText();
                            }
                        }
                    }
                }


                else if (dynamicTile != null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {///
                    //cas citoyens sur citoyens
                    if (gameManager.TileExistInArray(dynamicTile, gameManager.Citizens))
                    {
                        if (gameManager.TileExistInArray(tileBase, gameManager.Floors) || gameManager.TileExistInArray(tileBase, gameManager.Drain))
                        {
                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);

                            TileBase tile = gameManager.Citizens[HowManyCitizensOnTiles(tileSelected, dynamicTile) - 1];
                            if (selected == gameManager.Citizens[0])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, selected,tile));
                            }
                            else if (selected == gameManager.Citizens[1])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap,selected, tile));

                            }
                            else if (selected == gameManager.Citizens[2])
                            {
                                StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap,selected, tile));

                            }
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                        }
                    }

                    else if (gameManager.TileExistInArray(tileBase, gameManager.Waters) && dynamicTile == gameManager.BoatTiles)
                    {
                        Debug.Log("ON A BOAT");
                        if (TotalOfCitizensSelected(tileSelected) + 1 <= 2 - gameManager.PassagersOnBoat)
                        {
                            Debug.Log("assez de place sur le bateau");
                            gameManager.PassagersOnBoat += TotalOfCitizensSelected(tileSelected) + 1;
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
                //cas sol
                if (dynamicTile == null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {
                    Debug.Log("dynamic tile == null");
                    //classic floor
                    if (gameManager.TileExistInArray(tileBase, gameManager.Floors))
                    {
                        Debug.Log("Floors");
                        ///
                        /*
                        if (selected == gameManager.Citizens[0])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen1MovingStart, cellPos, gameManager.Citizen1MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap,gameManager.Citizens[0]));
                        }
                        else if (selected == gameManager.Citizens[1])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen2MovingStart, cellPos, gameManager.Citizen2MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, gameManager.Citizens[0]));

                        }
                        else if (selected == gameManager.Citizens[2])
                        {
                            StartCoroutine(gameManager.Move(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizen3MovingStart, cellPos, gameManager.Citizen3MovingEnd, gameManager.MovingDelay, gameManager.Dynamic_TileMap, gameManager.Citizens[0]));

                        }
                        */
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)cellPos.x, (int)cellPos.y, 0), gameManager.Citizens[0]);
                        gameManager.PALeft -= 1;
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizens[TotalOfCitizensSelected(selected) - 1]);
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

                //cas dynamics
                else if (dynamicTile != null && IsTilesAdjacent(cellPos) && AshesTiles == null)
                {
                    //cas citoyens sur citoyens
                    if (gameManager.TileExistInArray(dynamicTile, gameManager.Citizens))
                    {
                        // recuperer combien de citoyens sur la case de départ
                        (TileBase Start_Tile, _) = gameManager.GetTileAtWorldPosition(selectedPosition, gameManager.Dynamic_TileMap);
                        int Total_Start = TotalOfCitizensSelected(Start_Tile);

                        // recuperer combien de citoyens sur la case d'arriver
                        (TileBase End_Tile, _) = gameManager.GetTileAtWorldPosition(cellPos, gameManager.Dynamic_TileMap);
                        int Total_End = TotalOfCitizensSelected(End_Tile);

                        //enlever -1 a la case de départ
                        gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizens[Total_Start - 1]);
                        //ajouter +1 a la case d'arrivé
                        gameManager.Dynamic_TileMap.SetTile(cellPos, gameManager.Citizens[Total_End + 1]);

                        gameManager.PALeft -= 1;
                        gameManager.UpdateText();
                        gameManager.Speel3();
                    }

                    else if (dynamicTile == gameManager.BoatTiles)
                    {
                        if (gameManager.PassagersOnBoat < 2)
                        {
                            (TileBase Start_Tile, _) = gameManager.GetTileAtWorldPosition(selectedPosition, gameManager.Dynamic_TileMap);
                            int Total_Start = TotalOfCitizensSelected(Start_Tile);


                            gameManager.Dynamic_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), gameManager.Citizens[Total_Start - 1]);


                            gameManager.PassagersOnBoat += 1;
                            gameManager.PALeft -= 1;
                            gameManager.UpdateText();
                            gameManager.Speel3();
                        }
                    }
                }
            }
            gameManager.Sound.PlaySound(gameManager.Sound.MovingSound,gameManager.Sound.SFXSource);
        }
        CitizensOnDrain();
        gameManager.Cursor_TileMap.SetTile(new Vector3Int((int)selectedPosition.x, (int)selectedPosition.y, 0), null);
        gameManager.AshesPrevision();
        gameManager.FirePrevision();
        selected = null;
        selectedPosition = Vector2.zero;
    }
    void RainCursor()
    {
        for(int i = 0; i < gameManager.RainPosition.Length; i++)
        {
            gameManager.Rain_TileMap.SetTile(gameManager.RainPosition[i], null);

        }
        (Rain_Dynamicstiles[0],gameManager.RainPosition[0]) = gameManager.GetTileAtWorldPosition(CursorOnScreen, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[1],gameManager.RainPosition[1]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[0] +Vector3Int.left, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[2],gameManager.RainPosition[2]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[0] + Vector3Int.right, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[3],gameManager.RainPosition[3]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[0] +Vector3Int.down, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[4],gameManager.RainPosition[4]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[0] +Vector3Int.up, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[5],gameManager.RainPosition[5]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[1] + Vector3Int.down, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[6],gameManager.RainPosition[6]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[1] + Vector3Int.up, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[7],gameManager.RainPosition[7]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[2] + Vector3Int.down, gameManager.Dynamic_TileMap);
        (Rain_Dynamicstiles[8],gameManager.RainPosition[8]) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[2] + Vector3Int.up, gameManager.Dynamic_TileMap);
        (_, Vector3Int cellpos) = gameManager.GetTileAtWorldPosition((Vector3)CursorOnScreen, gameManager.Cursor_TileMap);
        gameManager.Rain_TileMap.SetTile(cellpos, gameManager.RainCursor[0]);
        StartCoroutine(gameManager.AnimateOneTile(cellpos, gameManager.RainCursor, gameManager.CursorDelay,gameManager.Cursor_TileMap));
    }
    void InputRain()
    {
        if (Input.GetMouseButtonDown(1))
        {
            cancelRain = true;
            Debug.Log("cancel rain");
            
                gameManager.Spell2();
            for (int i = 0; i < Rain_Dynamicstiles.Length; i++)
            {

                if (gameManager.TileExistInArray(Rain_Dynamicstiles[i], gameManager.Fire))
                {
                    for (int j = gameManager.fireList.Count - 1; j >= 0; j--)
                    {
                        if (gameManager.fireList[j].Position == gameManager.RainPosition[i])
                        {
                            gameManager.fireList.RemoveAt(j);
                        }
                    }
                    gameManager.Dynamic_TileMap.SetTile(gameManager.RainPosition[i], null);
                }
                gameManager.Rain_TileMap.SetTile(gameManager.RainPosition[i], null);

            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < Rain_Dynamicstiles.Length; i++)
            {
                (TileBase floor, _) = gameManager.GetTileAtWorldPosition(gameManager.RainPosition[i], gameManager.Tile_TileMap);
                if (floor != null)
                {
                    cancelRain = false;
                    break;
                }
                else
                {
                    cancelRain = true;
                }
            }
            if (!cancelRain)
            {
                for (int i = 0; i < Rain_Dynamicstiles.Length; i++)
                {
                    if (gameManager.TileExistInArray(Rain_Dynamicstiles[i], gameManager.Fire))
                    {
                        for (int j = gameManager.fireList.Count - 1; j >= 0; j--)
                        {
                            if (gameManager.fireList[j].Position == gameManager.RainPosition[i])
                            {
                                gameManager.fireList.RemoveAt(j);
                            }
                        }
                        gameManager.Dynamic_TileMap.SetTile(gameManager.RainPosition[i], null);
                    }
                    gameManager.Rain_TileMap.SetTile(gameManager.RainPosition[i], null); 
                }
                GameObject clone = Instantiate(gameManager.RainSprite, (Vector3)CursorOnScreen, Quaternion.identity);
                clone.GetComponent<RainAnimation>().soundManagement = gameManager.Sound;
                clone.GetComponent<RainAnimation>().sound();
                gameManager.Spell2();
                gameManager.PALeft -= gameManager.PaForRain;
                gameManager.UpdateText();
                gameManager.FirePrevision();
            }
            else
            {
                gameManager.Spell2();
            }
        }
    }
    bool IsTilesAdjacent(Vector3Int cellpos)
    {
        float deltaX = Mathf.Abs(cellpos.x - selectedPosition.x);
        float deltaY = Mathf.Abs(cellpos.y - selectedPosition.y);

        if (deltaX > 1.5 || deltaY > 1.5)
        {
            return false; // Trop éloigné
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
    int HowManyCitizensOnTiles(TileBase CitizenTile_Start, TileBase CitizenTile_End)
    {
        int x, y;

        for (x = 1; x < gameManager.Citizens.Length + 1; x++)
        {
            if (gameManager.Citizens[x - 1] == CitizenTile_Start)
            {
                break;
            }
        }
        for (y = 1; y < gameManager.Citizens.Length + 1; y++)
        {
            if (gameManager.Citizens[y - 1] == CitizenTile_End)
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
    void CitizensOnDrain()
    {
        foreach (Vector3Int pos in DrainPosition) 
        {
            (TileBase D_tiles, _) = gameManager.GetTileAtWorldPosition(pos, gameManager.Dynamic_TileMap);
            if (gameManager.TileExistInArray(D_tiles, gameManager.Citizens))
            {
                gameManager.PaForRain = 1;
                Debug.Log("is on drain");
                break;
            }
             gameManager.PaForRain = 2;
        }
    }
}

