using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class Item : ScriptableObject {
        public int id;
        public new string name;
        public string description;
        public float weight;
        public bool stack;
        public int max_stack_size;
        public Sprite icon;
    }
}
