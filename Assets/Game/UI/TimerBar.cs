using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Slider timer;
    public void UpdateHealthBar(float fillAmount)
    {
        timer.value = fillAmount;
    }
    // Start is called before the first frame update
    void Start()
    {
        timer.maxValue = TurnBasedSystem.Instance.getStartingTime();
        timer.value = TurnBasedSystem.Instance.timer;
    }

    // Update is called once per frame
    void Update()
    {
        timer.value = TurnBasedSystem.Instance.timer;
    }
}
