using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] protected Slider healthSlider;
    float maxHealthValue = 100;
    private void Start()
    {
        healthSlider.maxValue = maxHealthValue;
        healthSlider.value = maxHealthValue;
    }

    public void SetCurrentHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void SetMaxHealthVale(float value)
    {
        maxHealthValue = value;
    }
}
