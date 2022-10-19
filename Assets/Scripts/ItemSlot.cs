using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : ScriptableObject {
    public Item item;
    public int stack_size;
    public int index;

    public void Initialize(Item _item, int _stack_size, int _index){
        item = _item;
        stack_size = _stack_size;
        index = _index;
    }
}