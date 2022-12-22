using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    public Text healthText;

    public void UpdateHealth(Component sender, object data){
        Debug.Log("Health is being updated????");
        if(data is int){
            int amount = (int) data;
            SetHealth(amount);
        }
    }

    private void SetHealth(int health){
        healthText.text = health.ToString() + " Health";
    }
}