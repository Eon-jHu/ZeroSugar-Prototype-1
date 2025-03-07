using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Tile : MonoBehaviour
{
    public List<Tile> neighbourTiles = new();
    [field: SerializeField] public GameObject TileRangeIndicator { get; private set; }

    [field: SerializeField]
    public IOccupier Occupier { get; private set; }
    private Transform occupierTransform;
    public Color previousColor;

    [SerializeField] private Color hoverGreen;
    [SerializeField] private Color hoverYellow;
    [SerializeField] private Color hoverRed;
    public bool isInFeedbackMode; // This is true for when the tile is in feedback mode; holding a color for 0.25s

    public static event Action<Tile> OnCenterTileAssigned;
    public static event Action<Tile> OnTileClicked; // Static event that carries the clicked tile

    public bool isAOE;
    public bool isCross;
    //public bool isOccupied;

    private void Awake()
    {
        Board.OnTileInitRequired += InitializeTile;
    }

    private void Start()
    {
        isInFeedbackMode = false;
    }

    public bool OccupyTile(IOccupier occupier, Tile fromTile, bool snapToTile = false)
    {
        // if tile is already occupied return false.
        // todo check for unit death--Occupier might not be null
        if (Occupier != null)
            return false;
        
        // leave current tile so that it can be occupied by another unit.
        if (fromTile)
        {
            fromTile.LeaveTile(occupier);
        }
        
        Occupier = occupier;
        occupierTransform = occupier.OccupierTransform;

        /* Note that this probably works best when the player mesh/model is a separate from the Player root object. Then you can ensure
         That the feet of the player model is at 0,0,0.*/
        if (snapToTile)
        {
            Occupier.OccupierTransform.position = transform.position;
        }
        
        return true;
    }

    public void LeaveTile(IOccupier leavingOccupier)
    {
        if (leavingOccupier == Occupier)
        {
            Occupier = null;
            occupierTransform = null;
        }
    }

    private void InitializeTile(Board board)
    {
        foreach (var tile in FindObjectsOfType<Tile>())
        {
            if (tile == this)
                continue;
            
            float distanceToOtherTile = Vector3.Distance(transform.position, tile.transform.position);

            if (distanceToOtherTile <= 1.42f * Board.Instance.TileSize)
            {
                neighbourTiles.Add(tile);
            }
        }
        
        // register tile with the board
        board.AddTile(this);

        // if the tile is at origin, this is the center tile.
        if (transform.name.ToLower().Contains("center tile"))
        {
            board.CenterTile = this;
            OnCenterTileAssigned?.Invoke(this);
        }
    }

    // can use this for enemy spawn positions
    public bool IsBoundaryTile()
    {
        return neighbourTiles.Count < 8;
    }

    public bool IsOccupied()
    {
        return Occupier != null;
    }

    public void ShowTileIndicator(bool isThrownRange = false)
    {
        TileRangeIndicator.SetActive(true);
        Color color = isThrownRange ? hoverYellow : hoverGreen;
        TileRangeIndicator.GetComponent<Renderer>().material.color = color;
        previousColor = color;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                SetHoverColor();
            }
        }
    }


    private void OnMouseUp()
    {
        Debug.Log(OnTileClicked);

        if (OnTileClicked != null)
        {
            isInFeedbackMode = true;
            StartCoroutine(ClickFeedback());
            OnTileClicked.Invoke(this);
        }
    }

    private void ResetColor()
    {
        if (isInFeedbackMode) return;

        Renderer render = TileRangeIndicator.GetComponent<Renderer>();
        render.material.color = previousColor;
    }

    public void HighlightNeighborTiles(bool highlight)
    {
        foreach (var neighbor in neighbourTiles)
        {
            neighbor.TileRangeIndicator.SetActive(highlight);
            neighbor.TileRangeIndicator.GetComponent<Renderer>().material.color = highlight ? hoverRed : previousColor;
        }
    }

    private IEnumerator ClickFeedback()
    {
        SetHoverColor(); // Change the color to red
        yield return new WaitForSeconds(1f); // Wait for 1 second
        ResetColor(); // Revert back to the previous color.
        TileRangeIndicator.SetActive(false);
        isInFeedbackMode = false;
    }

    private void OnMouseEnter()
    {
        if (!TileRangeIndicator.activeSelf)
            return;



        if (isAOE || isCross)
        {
            foreach (var neighbor in neighbourTiles)
            {
                //disable diagonal

                if (isCross)
                {
                    SetHoverColor();
                    if (Vector3.Distance(this.transform.position, neighbor.transform.position) > 2 * 1.05)
                        continue; 
                }

                neighbor.TileRangeIndicator.SetActive(true);
                neighbor.TileRangeIndicator.GetComponent<Renderer>().material.color = hoverRed; // Use a different color
            } 
        }
        else
            SetHoverColor();

    }

    private void SetHoverColor()
    {
        Renderer render = TileRangeIndicator.GetComponent<Renderer>();
        render.material.color = hoverRed;
    }
    
    private void OnMouseExit()
    {
        ResetColor();

        if (isAOE || isCross)
        {
            foreach (var neighbor in neighbourTiles)
            {
                neighbor.ResetColor();
                //neighbor.TileRangeIndicator.SetActive(false); // Hide if necessary
            } 
        }


    }

    public void SimulateHoverEffect(bool activate)
    {
        if (activate)
        {
            SetHoverColor();
            foreach (var neighbor in neighbourTiles)
            {
                neighbor.TileRangeIndicator.SetActive(true);
                neighbor.TileRangeIndicator.GetComponent<Renderer>().material.color = hoverYellow;
            }
        }
        else
        {
            ResetColor();
            foreach (var neighbor in neighbourTiles)
            {
                neighbor.ResetColor();
                neighbor.TileRangeIndicator.SetActive(false);
            }
        }
    }



    private void OnDestroy()
    {
        Board.OnTileInitRequired -= InitializeTile; 
    }

    private void Update()
    {
        //isOccupied = IsOccupied();
    }
}
