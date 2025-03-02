using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointDisplayUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text actionPointText;

    public void SetActionPointDisplay(int actionsPerTurn, int actionsRemaining)
    {
        actionPointText.text = actionsRemaining.ToString();
        fillImage.fillAmount = (float)actionsRemaining / actionsPerTurn;
    }
}
