using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject {
    public string id;
    public new string name;
    public string description;
    public float weight;
    public bool stack;
    public int max_stack_size;
    public Sprite icon;

    public void Interaction(){
        GameObject.FindWithTag("Player").GetComponent<Inventory>().AddItem(this);
    }
}
