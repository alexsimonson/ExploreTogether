using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class Equipment : Item {
    
    public enum Type{
        Head,
        Neck,
        Torso,
        Leg,
        Glove,
        Shoe,
        Back,
        Eye,
        Coat,
        Ring,
        Weapon,
        Offhand
    }

    public Type type;
}
