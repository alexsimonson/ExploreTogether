using UnityEngine;
using UnityEngine.UI;

namespace ExploreTogether {
    public class HealthUI : MonoBehaviour {
        public Text healthText;

        public void UpdateHealth(Component sender, object data){
            if(data is int){
                int amount = (int) data;
                SetHealth(amount);
            }
        }

        private void SetHealth(int health){
            healthText.text = health.ToString() + " Health";
        }
    }
}
