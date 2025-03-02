using System;
using TMPro;
using UnityEngine;

public class TurnStartUIText : MonoBehaviour
{
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private float notificationLifeTime = 2.0f;
    
    private void Awake()
    {
        TurnBasedSystem.OnPlayerStartTurn += PlayerStartTurnNotify;
        TurnBasedSystem.OnEnemyStartTurn += EnemyStartTurnNotify;
    }

    private void PlayerStartTurnNotify()
    {
        notificationText.text = "Player Turn Started!";
        notificationText.gameObject.SetActive(true);

        this.Wait(notificationLifeTime, () =>
        {
            notificationText.gameObject.SetActive(false);
        });
    }
    
    private void EnemyStartTurnNotify()
    {
        notificationText.text = "Enemy Turn Started!";
        notificationText.gameObject.SetActive(true);

        this.Wait(notificationLifeTime, () =>
        {
            notificationText.gameObject.SetActive(false);
        });    
    }
    
    private void OnDestroy()
    {
        TurnBasedSystem.OnPlayerStartTurn -= PlayerStartTurnNotify;
        TurnBasedSystem.OnEnemyStartTurn -= EnemyStartTurnNotify;    
    }
}
