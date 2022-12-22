using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour {

    public void UpdateWeaponUI(Component sender, object data){
        // if(data.GetType().ToString()!="ItemSlot"){
        //     Debug.LogError("Invalid update data type");
        //     return;
        // }
        Debug.Log("Updating weapon UI");
        Debug.Log(data);
        gameObject.GetComponent<Text>().text = data.ToString();
        return;
        // ItemSlot slot = (ItemSlot)data;
        // Equipment equipped_weapon = (Equipment)slot.item;
        // if(equipped_weapon!=null){
        //     if(equipped_weapon.type==Equipment.Type.Weapon){
        //         Debug.Log("Fuck you");
        //         // Gun equipped_gun = (Gun)equipped_weapon;
        //         // if(equipped_gun==null){
        //         //     gameObject.GetComponent<Text>().text = equipped_weapon.name;
        //         // }else{
        //         //     gameObject.GetComponent<Text>().text = equipped_gun.name + "\n" + equipped_gun.magazineRounds + " | " + equipped_gun.bulletCount;
        //         // }
        //     }
        // }
    }
}