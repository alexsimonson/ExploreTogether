using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour {

    public void UpdateWeaponUI(Component sender, object data){
        Weapon equipped_weapon = (Weapon)data;
        if(equipped_weapon==null) return;
        var new_ui_str = "";
        if(data.GetType().ToString()=="Gun"){
            Gun equipped_gun = (Gun)equipped_weapon;
            if(equipped_gun==null) return;
            new_ui_str = equipped_gun.name + "\n" + equipped_gun.magazineRounds.ToString() + " | " + equipped_gun.bulletCount.ToString();
        }else{
            new_ui_str = equipped_weapon.name;
        }
        gameObject.GetComponent<Text>().text = new_ui_str;
    }
}