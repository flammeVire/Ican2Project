using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Turn and PA",order = 1)]
    [SerializeField]int TurnMax;
    int TurnLeft;
    [SerializeField]int PAMax;
    public int PALeft;
    bool IsPlayerTurn;


    [Header("TMPro Ref", order = 2)]
    [SerializeField] TextMeshProUGUI PA_Text;
    [SerializeField] TextMeshProUGUI Turn_Text;

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
    public TileBase[] Ashes;
    public TileBase[] cursor;
    public TileBase[] Drain;

    Vector3Int[] AshesCellPosition = new Vector3Int[12];

    private void Start()
    {
        StartCoroutine(PlayerTurn());
        TurnLeft = TurnMax;
        PALeft = PAMax;
    }


    IEnumerator PlayerTurn()
    {
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
        StartCoroutine (PlayerTurn());
       
    }

    #region Button

    public void Spell1()
    {
        if(PALeft >= 1)
        {
            PALeft -= 1;
            UpdateText();
        }
        else
        {
            Debug.Log("pas assez de PA" + PALeft);
        }
    }

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
    //retour le type de tiles ET ses coordonnées dans le monde
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

        for (int i = 0; i < 4; i++)
        {
            TileBase AshesTile, HouseTile = null;
            (AshesTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Ashes_TileMap);
            (HouseTile, _) = GetTileAtWorldPosition(NextTilePosition[i], Dynamic_TileMap);
            if (AshesTile == null && !TileExistInArray(HouseTile, House))
            {
                CanMoveAdjacent[i] = true;
            }
            else
            {
                CanMoveAdjacent[i] = false;
            }

        }
        return (CanMoveAdjacent, NextTilePosition);
    }


    #endregion
}
