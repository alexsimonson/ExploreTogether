using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class Equipment : Item {
    
    public enum Type{
        Head,
        Cape,
        Neck,
        Body,
        Legs,
        Hands,
        Feet,
        Ring,
        Weapon,
        Shield
    }

    public Type type;
}
