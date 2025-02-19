using UnityEngine;

public interface IOccupier
{
    [field: SerializeField] public Transform OccupierTransform { get; }
}
