using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameOverManager script is running!");
        gameOverCanvas = GameObject.Find("GameOverCanvas");
        gameOverCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.Instance.PlayerAliveCheck() == false)
        {
            gameOverCanvas.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameTest");
    }
}
