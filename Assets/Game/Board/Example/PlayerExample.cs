using UnityEngine;

public class PlayerExample : MonoBehaviour, IOccupier
{
    public Transform OccupierTransform => transform;

    private void Awake()
    {
        Tile.OnCenterTileAssigned += PlacePlayer;
    }

    private void PlacePlayer(Tile tile)
    {
        tile.OccupyTile(this, null, true);
    }

    private void OnDestroy()
    {
        Tile.OnCenterTileAssigned -= PlacePlayer;
    }
}
