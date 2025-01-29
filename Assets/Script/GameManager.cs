 using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    [Header("Turn and PA", order = 1)]
    int Turn;
    [SerializeField] int PAMax;
    public int PALeft;
    bool IsPlayerTurn;
    public SoundManagement Sound;

    public bool UnfusionActive { get; private set; }
    public int CitizensSaved;
    public int RainRaduis;
    [Header("TMPro Ref", order = 2)]
    [SerializeField] TextMeshProUGUI PA_Text;
    [SerializeField] TextMeshProUGUI Turn_Text;
    [SerializeField] TextMeshProUGUI Boat_Text;
    [SerializeField] GameObject Unfusion_Button;

    [Header("TileMap", order = 3)]
    public Tilemap Tile_TileMap;
    public Tilemap Dynamic_TileMap;
    public Tilemap Cursor_TileMap;
    public Tilemap Ashes_TileMap;
    public Tilemap Rain_TileMap;

    [Header("Tile")]
    public TileBase[] House;
    public TileBase[] Citizens;
    public TileBase[] Holes;
    public TileBase[] Floors;
    public TileBase[] Waters;
    public TileBase BackAsh;
    public TileBase[] Ashes;
    public TileBase[] Fire;
    public TileBase[] Drain;
    public TileBase[] NarrowStreet;
    public TileBase[] Gate;
    [SerializeField] Vector3Int[] AshesCellPosition = new Vector3Int[12];
    [SerializeField] Vector3Int[] oldAshesCellPosition = new Vector3Int[12];

    [Header("Boat")]
    [SerializeField] Vector3Int BoatPosition;
    public int PassagersOnBoat;
    public TileBase BoatTiles;
    bool IsBoatOnMap = true;

    [Header("Rain")]
    public Vector3Int[] RainPosition = new Vector3Int[9];
    public bool RainActive { get; private set; }
    public int PaForRain = 2;

    [Header("Attack")]
    public List<FireExpansion> fireList;
    public int turnForFireAtk;
    public List<Vector3Int> VoidPosition;
    public int turnForVoidAtk;

    [Header("AnimationTile")]
    public TileBase[] WaterTiles;
    public float WaterDelay;
    public TileBase[] DefaultCursor;
    public TileBase[] RainCursor;
    public TileBase[] AshCursor;
    public TileBase[] FireCursor;
    public float CursorDelay;
    public float AshDelay;
    public TileBase[] House1MovingStart;
    public TileBase[] House1MovingEnd;
    public float MovingDelay;

    Coroutine[] OldAshCursorAnimation = new Coroutine[12];
    Coroutine[] OldAshAnimation = new Coroutine[12];

    private void Start()
    {
        AshesFirstPosition();

        Turn = 1;
        PALeft = PAMax;
        StartCoroutine(PlayerTurn());
        StartCoroutine(FireAttack());
        StartCoroutine(VoidAttack());
        StartCoroutine(AnimateWater(WaterTiles, WaterDelay));
    }
    IEnumerator PlayerTurn()
    {
        //Sound.PlaySound(Sound.MyTurnSound, Sound.SFXSource);
        AshesPrevision();
        yield return null;
        PALeft = PAMax;
        IsPlayerTurn = true;
        UpdateText();
        yield return new WaitUntil(() => PALeft <= 0);
        yield return new WaitForSeconds(0.5f);
        IsPlayerTurn = false;
        StartCoroutine(IaTurn());
    }
    IEnumerator IaTurn()
    {
        //Sound.PlaySound(Sound.IATurnSound, Sound.SFXSource);
        yield return new WaitForSeconds(1f);
        AshesMovement();
        KillHouses();
        GetLastAshes();
        KillCitizens();
        KillFire();
        if (Turn >= turnForFireAtk)
        {
            FireExpand();
            FirePrevision();
        }
        Turn += 1;
        //CheckIfAshIsTheLast();



        StartCoroutine(PlayerTurn());
    }

    IEnumerator FireAttack()
    {
        yield return new WaitUntil(() => Turn == turnForFireAtk-1);

        for (int i = 0; i < fireList.Count; i++)
        {
            if (CanAddFireOnMap(fireList[i].Position))
            {
                Cursor_TileMap.SetTile(fireList[i].Position, FireCursor[0]);
            }
        }

        yield return new WaitUntil(() => Turn == turnForFireAtk);
        for (int i = 0; i < fireList.Count; i++)
        {
            if (CanAddFireOnMap(fireList[i].Position))
            {
                Dynamic_TileMap.SetTile(fireList[i].Position, Fire[0]);
            }
        }
    }

    IEnumerator VoidAttack()
    {
        yield return new WaitUntil(() => Turn == turnForVoidAtk);
        for (int i = 0; i < VoidPosition.Count; i++)
        {
            Dynamic_TileMap.SetTile(VoidPosition[i], null);
            Tile_TileMap.SetTile(VoidPosition[i], Holes[0]);
        }
    }

    #region Button

    //Boat
    public void Spell1()
    {
        if (PALeft >= 1)
        {
            PALeft -= 1;
            if (IsBoatOnMap)
            {
                Dynamic_TileMap.SetTile(BoatPosition, null);
                CitizensSaved += PassagersOnBoat;
                PassagersOnBoat = 0;
                Sound.PlaySound(Sound.BoatLeavingSound, Sound.SFXSource);
            }
            else
            {
                Dynamic_TileMap.SetTile(BoatPosition, BoatTiles);
                Sound.PlaySound(Sound.BoatComingSound, Sound.SFXSource);
            }

            IsBoatOnMap = !IsBoatOnMap;
            UpdateText();
        }
    }

    //Rain
    public void Spell2()
    {
        Debug.Log("Pa for rain = " + PaForRain);
        if (PALeft >= PaForRain)
        {
            RainActive = !RainActive;
            UpdateText();
        }

    }

    //Unfusion
    public void Speel3()
    {
        UnfusionActive = !UnfusionActive;

        Color color = UnfusionActive ? Color.green : Color.red;
        Debug.Log("unfusion == " + UnfusionActive);
        Unfusion_Button.GetComponent<Image>().color = color;
    }
    public void EndOfTurn()
    {
        PALeft = 0;
    }

    #endregion

    #region ShowText

    public void UpdateText()
    {
        PA_Text.text = "PA: " + PALeft;
        PA_Text.text = "PA: " + PALeft;
        Turn_Text.text = "tour écoulé: " + Turn;
    }

    #endregion

    #region tile management

    #region Ashes
    void AshesFirstPosition()
    {
        Vector3 ashescellpos = new Vector3(-5.5f, 4.5f, 0);
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            (TileBase ashes, Vector3Int arrayPosition) = GetTileAtWorldPosition(ashescellpos, Ashes_TileMap);
            if (TileExistInArray(ashes, Ashes))
            {
                AshesCellPosition[i] = arrayPosition;
                OldAshAnimation[i] = StartCoroutine(AnimateOneTile(AshesCellPosition[i], Ashes, AshDelay, Ashes_TileMap));
                ashescellpos += Vector3.right;
            }
            else
            {
                Debug.LogError("ISSUES ASHES CELL NOT FOUND");
            }
        }
    }

    void AshesMovement()
    {
        // i = relative pos.x in tilemap
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            //set older tiles
            oldAshesCellPosition[i] = AshesCellPosition[i];
            Cursor_TileMap.SetTile(oldAshesCellPosition[i], null);

            //Get Expansion
            bool[] canmove = new bool[4];
            Vector3Int[] nextpos = new Vector3Int[4];
            (canmove, nextpos) = CanTilesMoveAdjacent(AshesCellPosition[i]);

            // j = all 4 direction
            for (int j = 0; j < nextpos.Length; j++)
            {
                if (canmove[j])
                {
                    Ashes_TileMap.SetTile(nextpos[j], BackAsh);
                    //if j expand to : DOWN
                    if (j == 1)
                    {
                        AshesCellPosition[i] = nextpos[j];
                    }
                }
            }
            if (oldAshesCellPosition[i] != AshesCellPosition[i])
            {
                Ashes_TileMap.SetTile(oldAshesCellPosition[i], BackAsh);
            }
        }
        /*
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {

            if (ancienPosition[i] == AshesCellPosition[i])
            {
                for (int collum = 0; collum < AshesCellPosition.Length; collum++)
                {
                    Vector3 pos = new Vector3(AshesCellPosition[i].x, AshesCellPosition[i].y - collum, AshesCellPosition[i].z);
                    (TileBase cellTile, Vector3Int cellpos) = GetTileAtWorldPosition(pos, Ashes_TileMap);
                    if (cellTile != null && cellpos.y != AshesCellPosition[i].y)
                    {
                        AshesCellPosition[i] = cellpos;
                        break;
                    }
                }

            }
        }
        */
        
        
    }

    void GetLastAshes()
    {
        Vector3Int[] pos = new Vector3Int[12];

        //parcourir la map de bas en haut 
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            pos[i] = new Vector3Int(AshesCellPosition[i].x, -4, 0);
            for (int Up = 0; Up < 9; Up++)
            {
                (TileBase A_Tile, Vector3Int A_Pos) = GetTileAtWorldPosition(pos[i], Ashes_TileMap);
                // si on trouve une cendre c'est la bonne
                if (A_Tile != null)
                {
                    AshesCellPosition[i] = A_Pos;
                    break;
                }
                pos[i] += Vector3Int.up;
            }
        }
        ChangeLastAshes();
    }

    void ChangeLastAshes()
    {
        
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            // Vérifier si la position a changé
            if (oldAshesCellPosition[i] != AshesCellPosition[i])
            {
                Ashes_TileMap.SetTile(AshesCellPosition[i], Ashes[0]);
                OldAshAnimation[i] = StartCoroutine(AnimateOneTile(AshesCellPosition[i], Ashes, AshDelay, Ashes_TileMap));
            }
        
        
            Vector3Int pos = AshesCellPosition[i];
            for (int j = 0; j < 9; j++)
            {
                pos += Vector3Int.up;
                (TileBase tile, _) = GetTileAtWorldPosition(pos, Ashes_TileMap);
                if (tile != null)
                {
                    Ashes_TileMap.SetTile(pos, BackAsh);
                }
            }
        }
    }

    public void AshesPrevision()
    {
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            bool[] canmove = new bool[4];
            Vector3Int[] nextpos = new Vector3Int[4];
            (canmove, nextpos) = CanTilesMoveAdjacent(AshesCellPosition[i]);

            for (int j = 0; j < nextpos.Length; j++)
            {
                if (canmove[j])
                {
                    Cursor_TileMap.SetTile(nextpos[j], AshCursor[0]);
                    OldAshCursorAnimation[i] = StartCoroutine(AnimateOneTile(nextpos[j], AshCursor, CursorDelay, Cursor_TileMap));
                }
                else
                {
                    OldAshCursorAnimation[i] = null;
                }
            }
            (TileBase A_Tile, _) = GetTileAtWorldPosition(AshesCellPosition[i], Ashes_TileMap);
            if (A_Tile != null)
            {
                Cursor_TileMap.SetTile(AshesCellPosition[i], null);
            }

        }

    }
    #endregion

    #region Dynamics

    void KillCitizens()
    {
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            (TileBase tile, Vector3Int cellpos) = GetTileAtWorldPosition(AshesCellPosition[i],Dynamic_TileMap);
            if (TileExistInArray(tile, Citizens))
            {
                Dynamic_TileMap.SetTile(cellpos, null);
                Debug.Log("Citizens Die on :" + cellpos);
            }
        }
    }

    void KillFire()
    {
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            (TileBase tile, Vector3Int cellpos) = GetTileAtWorldPosition(AshesCellPosition[i], Dynamic_TileMap);
            if (TileExistInArray(tile, Fire))
            {
                Dynamic_TileMap.SetTile(cellpos, null);
                Debug.Log("Citizens Die on :" + cellpos);
            }
        }
        FirePrevision();
    }

    void KillHouses()
    {
        for (int lenght = 0; lenght < 12; lenght++) 
        {
            for (int height = 0; height < 9; height++) 
            {
                Vector3Int mapPos = new Vector3Int(-6 + lenght, - 4 + height, 0);
                (TileBase D_tiles, _) = GetTileAtWorldPosition(mapPos, Dynamic_TileMap);

                if (TileExistInArray(D_tiles, House))
                {
                    int allboolean = 0;
                    (bool[] canMove, _) = CanTilesMoveAdjacent(mapPos);
                    foreach (bool boolean in canMove)
                    {
                        if (!boolean)
                        {
                            allboolean++;
                        }
                    }
                    if (allboolean == 4)
                    {
                        Dynamic_TileMap.SetTile(mapPos, null);
                        Ashes_TileMap.SetTile(mapPos, BackAsh);
                        AshesCellPosition[lenght] = mapPos;
                    }
                }
            }
        }
        
    }

    /// need to fix
    #region fire
    //Check if tile map are available for fire 
    bool CanAddFireOnMap(Vector3Int cellpos)
    {
        TileBase D_tile = Dynamic_TileMap.GetTile(cellpos);
        TileBase T_tile = Tile_TileMap.GetTile(cellpos);
        
        if (T_tile == null || TileExistInArray(D_tile, House) || TileExistInArray(T_tile, Drain) || TileExistInArray(D_tile, Fire) || TileExistInArray(T_tile,Waters))
        {
            return false;
        }
        return true;
    }

    //check if fire can spawn on nextpos
    void FireExpand()
    {
        List<FireExpansion> fireListClone = new List<FireExpansion>(fireList);
        foreach (FireExpansion fires in fireListClone)
        {
            fires.nextDirectionWanted = fires.nextDirectionOrder[0];
            for (int i = 0; i < fires.haveAlreadyExpand.Length; i++)
            {
                //feu ne s'est pas deja rependu et la direction voulu est la bonne
                if (!fires.haveAlreadyExpand[i])
                {
                    fires.nextDirectionWanted = fires.nextDirectionOrder[i];
                    //cas fire peut se propager
                    if (CanAddFireOnMap(FireWantedPosition(fires))) 
                    {
                        fires.haveAlreadyExpand[i] = true;
                        SetCloneFireToList(fires);
                        break;
                    }
                    else
                    {
                        Debug.Log("fail");
                    }
                }
            }
        }
    }

    Vector3Int FireWantedPosition(FireExpansion fire)
    {
        Vector3Int nextpos = new Vector3Int();
        switch (fire.nextDirectionWanted)
        {
            case FireExpansion.direction.North:
                nextpos = fire.Position + Vector3Int.up;
                break;
            case FireExpansion.direction.South:
                nextpos = fire.Position + Vector3Int.down;
                break;
            case FireExpansion.direction.West:
                nextpos = fire.Position + Vector3Int.left;
                break;
            case FireExpansion.direction.Est:
                nextpos = fire.Position + Vector3Int.right;
                break;
        }
        return nextpos;
    }

    void SetCloneFireToList(FireExpansion fire)
    {
        FireExpansion clone = Instantiate(fire);
        clone.Position = FireWantedPosition(fire);
        for (int i = 0; i < clone.haveAlreadyExpand.Length; i++)
        {
            clone.haveAlreadyExpand[i] = false;
        }
        ChangeArrayDirection(clone);

        fireList.Add(clone);
        Dynamic_TileMap.SetTile(FireWantedPosition(fire), Fire[0]);
        Cursor_TileMap.SetTile(FireWantedPosition(fire),null);
    }

    void ChangeArrayDirection(FireExpansion fire)
    {
        if (fire.nextDirectionOrder[0] == FireExpansion.direction.North)
        {
            fire.nextDirectionOrder[0] = FireExpansion.direction.Est;
            fire.nextDirectionOrder[2] = FireExpansion.direction.South;
            fire.nextDirectionOrder[1] = FireExpansion.direction.West;
            fire.nextDirectionOrder[3] = FireExpansion.direction.North;
        }
        else if (fire.nextDirectionOrder[0] == FireExpansion.direction.South) 
        {
            fire.nextDirectionOrder[0] = FireExpansion.direction.West;
            fire.nextDirectionOrder[1] = FireExpansion.direction.Est;
            fire.nextDirectionOrder[2] = FireExpansion.direction.North;
            fire.nextDirectionOrder[3] = FireExpansion.direction.South;
        }
        else if (fire.nextDirectionOrder[0] == FireExpansion.direction.West)
        {
            fire.nextDirectionOrder[0] = FireExpansion.direction.South;
            fire.nextDirectionOrder[1] = FireExpansion.direction.North;
            fire.nextDirectionOrder[2] = FireExpansion.direction.Est;
            fire.nextDirectionOrder[3] = FireExpansion.direction.West;
        }
        else if (fire.nextDirectionOrder[0] == FireExpansion.direction.Est) 
        {
            fire.nextDirectionOrder[0] = FireExpansion.direction.North;
            fire.nextDirectionOrder[1] = FireExpansion.direction.South;
            fire.nextDirectionOrder[2] = FireExpansion.direction.West;
            fire.nextDirectionOrder[3] = FireExpansion.direction.Est;
        }
        fire.nextDirectionWanted = fire.nextDirectionOrder[0];
    }


    void removeFirePrevision()
    {
        for (int lenght = 0; lenght < 12; lenght++)
        {
            for (int height = 0; height < 9; height++)
            {
                Vector3Int mapPos = new Vector3Int(-6 + lenght, -4 + height, 0);
                (TileBase C_tiles, _) = GetTileAtWorldPosition(mapPos, Cursor_TileMap);

                if (TileExistInArray(C_tiles, FireCursor))
                {
                   Cursor_TileMap.SetTile(mapPos,null);
                }
            }
        }
    }
    public void FirePrevision()
    {
        removeFirePrevision();
        List<FireExpansion> fireListClone = new List<FireExpansion>(fireList);
        foreach (FireExpansion fires in fireListClone)
        {
            fires.nextDirectionWanted = fires.nextDirectionOrder[0];
            for (int i = 0; i < fires.haveAlreadyExpand.Length; i++)
            {
                //feu ne s'est pas deja rependu et la direction voulu est la bonne
                if (!fires.haveAlreadyExpand[i])
                {
                    fires.nextDirectionWanted = fires.nextDirectionOrder[i];
                    //cas fire peut se propager
                    if (CanAddFireOnMap(FireWantedPosition(fires)))
                    {
                        Cursor_TileMap.SetTile(FireWantedPosition(fires), FireCursor[0]);
                        break;
                    }
                    else
                    {
                        Debug.Log("fail");
                    }
                }
            }
        }
    }
    #endregion

    #endregion


    #region Tile utility function
    public (TileBase, Vector3Int) GetTileAtWorldPosition(Vector3 worldPos, Tilemap MapWanted)
    {
        Vector3Int cellPosition = MapWanted.WorldToCell(worldPos);
        TileBase tile = MapWanted.GetTile(cellPosition);
        return (tile, cellPosition);
    }
    public bool TileExistInArray(TileBase tile, Array array)
    {
        foreach (TileBase item in array)
        {
            if (item == tile) return true;
        }

        return false;
    }
    (bool[], Vector3Int[])CanTilesMoveAdjacent(Vector3Int cellPos)
    {
        //gauche bas droite gauche
        bool[] CanMoveAdjacent = new bool[4] {false,false,false,false};
        Vector3Int[] NextTilePosition = new Vector3Int[4] {cellPos+Vector3Int.left,cellPos+Vector3Int.down,cellPos+Vector3Int.right,cellPos+Vector3Int.up };

       


        for (int i = 0; i < 4; i++)
        {
            TileBase AshesTile = null, HouseTile = null,FloorTile = null;
            (AshesTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Ashes_TileMap);
            (HouseTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Dynamic_TileMap);
            (FloorTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Tile_TileMap);

            if (FloorTile != null)
            {
                if (AshesTile == null && !TileExistInArray(HouseTile, House))
                {
                    CanMoveAdjacent[i] = true;
                }
                else
                {
                    CanMoveAdjacent[i] = false;
                }
            }

        }
        return (CanMoveAdjacent, NextTilePosition);
    }

    #endregion

    #endregion

    #region animation

    IEnumerator AnimateWater(TileBase[] arrayOfTile, float Delay)
    {
        Vector3Int cellPos = new Vector3Int(-6, -4, 0);
        TileBase tile = Tile_TileMap.GetTile(cellPos);

        if (tile != null)
        {
            int arrayLenght = arrayOfTile.Length;
            for (int i = 0; i < arrayLenght; i++)
            {
                if (tile == arrayOfTile[arrayLenght-1])
                {
                    tile = arrayOfTile[0];
                    break;
                }

                else if (tile == arrayOfTile[i])
                {
                    tile = arrayOfTile[i + 1];
                    break;
                }
            }
        }
        int pos = cellPos.x;
        for (int i = 0; i < 12; i++)
        {
            cellPos.x = pos + i;
            
            Tile_TileMap.SetTile(cellPos, tile);
        }
        yield return new WaitForSeconds(Delay);
        StartCoroutine(AnimateWater(arrayOfTile, Delay));
    }

    public IEnumerator AnimateOneTile(Vector3Int cellPos, TileBase[] arrayOfTile, float Delay,Tilemap tileMap)
    {
        TileBase tile = tileMap.GetTile(cellPos);
        //Debug.Log("Animate on tile on " + cellPos);
        if (tile != null)
        {
            int arrayLenght = arrayOfTile.Length;
            for (int i = 0; i < arrayLenght; i++)
            {
                if (tile == arrayOfTile[arrayLenght - 1])
                {
                    tile = arrayOfTile[0];
                    break;
                }

                else if (tile == arrayOfTile[i])
                {
                    tile = arrayOfTile[i + 1];
                    break;
                }
            }
            tileMap.SetTile(cellPos, tile);
            yield return new WaitForSeconds(Delay);
            StartCoroutine(AnimateOneTile(cellPos,arrayOfTile,Delay,tileMap));
        }
    }

    public IEnumerator Move(Vector3Int StartPos, TileBase[] arrayOfStartTile,Vector3Int EndPos, TileBase[] arrayofEndTile,float TotalDelay,Tilemap tilemap, TileBase defaultTile)
    {
        int totalOfFrame = arrayOfStartTile.Length + arrayofEndTile.Length;

        for(int i = 0;i < arrayOfStartTile.Length; i++)
        {
            tilemap.SetTile(StartPos, arrayOfStartTile[i]);
            yield return new WaitForSeconds(TotalDelay/totalOfFrame);
        }
        tilemap.SetTile(StartPos,null);
        
            yield return new WaitForSeconds(0.5f);
        for(int i = 0;i < arrayofEndTile.Length; i++)
        {
            tilemap.SetTile(EndPos,arrayofEndTile[i]);
            yield return new WaitForSeconds(TotalDelay/totalOfFrame);
        }
        tilemap.SetTile(EndPos, defaultTile);

    }
    #endregion
}
