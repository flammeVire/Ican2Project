using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Header("Turn and PA",order = 1)]
    [SerializeField]int TurnMax;
    int TurnLeft;
    [SerializeField]int PAMax;
    public int PALeft;
    bool IsPlayerTurn;
    public bool UnfusionActive {  get; private set; }
    public int CitizensSaved;


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

    [Header("Tile")] 
    public TileBase[] House;
    public TileBase[] Citizens;
    public TileBase[] Holes;
    public TileBase[] Floors;
    public TileBase[] Waters;
    public TileBase[] Ashes;
    public TileBase[] cursor;
    public TileBase[] Drain;
    public TileBase[] NarrowStreet;
    public TileBase[] Gate;

    [SerializeField]Vector3Int[] AshesCellPosition = new Vector3Int[12];

    [Header("Boat")]
    [SerializeField] Vector3Int BoatPosition;
    public int PassagersOnBoat;
    public TileBase BoatTiles;
    bool IsBoatOnMap = true;

    private void Start()
    {
        AshesFirstPosition();

        TurnLeft = TurnMax;
        PALeft = PAMax;
        StartCoroutine(PlayerTurn());
    }


    IEnumerator PlayerTurn()
    {
        AshesPrevision();
        yield return null;
        PALeft = PAMax;
        IsPlayerTurn = true;
        UpdateText();
        yield return new WaitUntil(() => PALeft <= 0);
        yield return new WaitForSeconds(0.5f);
        TurnLeft -= 1;
        IsPlayerTurn = false;
        StartCoroutine(IaTurn());
    }
    IEnumerator IaTurn()
    {
        yield return null;
        AshesMovement();
        KillCitizens();
        KillHouses();
        StartCoroutine (PlayerTurn());
       
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
            }
            else
            {
                Dynamic_TileMap.SetTile(BoatPosition,BoatTiles);
            }

            IsBoatOnMap = !IsBoatOnMap;
            UpdateText();
        }
    }

    //Rain
    public void Spell2() 
    {
        if (PALeft >= 2)
        {
            PALeft -= 2;
            UpdateText();
        }
        else
        {
            Debug.Log("pas assez de PA :" + PALeft);
            
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
        Turn_Text.text = "Turn left: " + TurnLeft;
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
        
        // y = 4
    }
    
    void FireExpension()
    {
        int RandomExpension = UnityEngine.Random.Range(0,3);

        
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
        ///<summary>
        /// renvoye si la tiles peux bouger en a GAUCHE, BAS, DROITE, HAUT et la position d'arrivé
        /// </summary>

        //gauche bas droite gauche
        bool[] CanMoveAdjacent = new bool[4] {false,false,false,false};
        Vector3Int[] NextTilePosition = new Vector3Int[4] {cellPos+Vector3Int.left,cellPos+Vector3Int.down,cellPos+Vector3Int.right,cellPos+Vector3Int.up };

        //int MapsLimite = 6;


        for (int i = 0; i < 4; i++)
        {
            TileBase AshesTile = null, HouseTile = null,FloorTile = null;
            (AshesTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Ashes_TileMap);
            (HouseTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Dynamic_TileMap);
            (FloorTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Tile_TileMap);

            // && NextTilePosition[i].x > -MapsLimite && NextTilePosition[i].x <= (MapsLimite)
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
