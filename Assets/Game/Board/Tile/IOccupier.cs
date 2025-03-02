using UnityEngine;

public interface IOccupier
{
    public Transform OccupierTransform { get; }

    public void TakeDamage(int damage);
}
