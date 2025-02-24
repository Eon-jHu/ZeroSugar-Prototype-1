using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AoEType
{
    Single,
    Line,
    Cross,
    Cone,
    Circle
}

public enum EDamageVariance
{
    None,
    Critical,
    Random,
    RangeToPlayer,
    RangeToCenter
}

[CreateAssetMenu(fileName = "NewCardData", menuName = "Card/Data")]
public class CardData : ScriptableObject
{
    public Sprite cardSprite;

    public string cardName;
    public int damage;
    public EDamageVariance damageVariance;
    public float varianceValue;
    public int actionCost;

    public int minRange;
    public int maxRange;
    public AoEType aoeType;
    public int aoeArea;

    public int pushDistance;
    public int pullDistance;
    public bool pushPullToCenter;
}
