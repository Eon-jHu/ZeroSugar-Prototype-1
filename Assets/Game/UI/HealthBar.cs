using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Camera camera;
    [SerializeField] private Image healthbarImage;
    
    void Awake()
    {
        camera ??= Camera.main;
    }

    public void UpdateHealthBar(float fillAmount)
    {
        healthbarImage.fillAmount = fillAmount;
    }

    void Update()
    {
        if (!camera)
            return;
        
        transform.LookAt(camera.transform);
    }
}
