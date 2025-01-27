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
    public TileBase[] Ashes;
    public TileBase[] cursor;
    public TileBase[] Fire;
    public TileBase[] Drain;
    public TileBase[] NarrowStreet;
    public TileBase[] Gate;

    [SerializeField] Vector3Int[] AshesCellPosition = new Vector3Int[12];
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


    private void Start()
    {
        AshesFirstPosition();

        Turn = 1;
        PALeft = PAMax;
        StartCoroutine(PlayerTurn());
        StartCoroutine(FireAttack());
        StartCoroutine(VoidAttack());

    }
    IEnumerator PlayerTurn()
    {
        Sound.PlaySound(Sound.MyTurnSound, Sound.SFXSource);
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
        Sound.PlaySound(Sound.IATurnSound, Sound.SFXSource);
        yield return new WaitForSeconds(1f);
        AshesMovement();
        KillCitizens();
        KillHouses();
        Turn += 1;
        if (Turn >= turnForFireAtk)
        {
            FireExpand();
        }
        StartCoroutine (PlayerTurn());
    }

    IEnumerator FireAttack()
    {
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
        for(int i = 0; i < VoidPosition.Count; i++)
        {
            Dynamic_TileMap.SetTile(VoidPosition[i],null);
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
                Dynamic_TileMap.SetTile(BoatPosition,null);
                CitizensSaved += PassagersOnBoat;
                PassagersOnBoat = 0;
                Sound.PlaySound(Sound.BoatLeavingSound,Sound.SFXSource);
            }
            else
            {
                Dynamic_TileMap.SetTile(BoatPosition,BoatTiles);
                Sound.PlaySound(Sound.BoatComingSound,Sound.SFXSource);
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

        Color color = UnfusionActive ?  Color.green : Color.red ;
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
        Vector3Int[] ancienPosition = new Vector3Int[12];

        // i = all ashes tiles
        for (int i = 0; i < AshesCellPosition.Length; i++)
        {
            ancienPosition[i] = AshesCellPosition[i];

            bool[] canmove = new bool[4];
            Vector3Int[] nextpos = new Vector3Int[4];
            (canmove, nextpos) = CanTilesMoveAdjacent(AshesCellPosition[i]);


            // j = all 4 direction
            for (int j = 0; j < nextpos.Length; j++)
            {
                if (canmove[j])
                {
                    Ashes_TileMap.SetTile(nextpos[j], Ashes[0]);

                    if (j == 1)
                    {
                        AshesCellPosition[i] = nextpos[j];
                    }
                }


            }
        }

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
                    Cursor_TileMap.SetTile(nextpos[j], cursor[1]);
                }
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

    void KillHouses()
    {
        for (int row = 0;row < AshesCellPosition.Length; row++)
        {
            for (int col = 0; col < AshesCellPosition.Length; col++)
            {
                Vector3Int mapPos = new Vector3Int(-6+row,4-col,0);
                (TileBase dynamic, _) = GetTileAtWorldPosition(mapPos,Dynamic_TileMap);

                if (TileExistInArray(dynamic, House))
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
                        Debug.Log("HOUSE IN " + mapPos + "is surronded by ashes");
                        Dynamic_TileMap.SetTile(mapPos,null);
                        Ashes_TileMap.SetTile(mapPos, Ashes[0]);
                    }
                }
            }
        }
    }
    


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
}
