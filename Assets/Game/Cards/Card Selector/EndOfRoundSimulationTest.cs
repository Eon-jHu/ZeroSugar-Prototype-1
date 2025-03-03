using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfRoundSimulationTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AsyncOperation op = SceneManager.LoadSceneAsync("Card Select", LoadSceneMode.Additive);

            op.completed += SelectionSceneReady;
        }
    }

    private void SelectionSceneReady(AsyncOperation op)
    {
        CardSelector.Instance.SelectCard();
    }
}
