using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
    public class CharacterData : ScriptableObject{
        public string name;
        public GameMode.Mode game_mode;
        public Vector3 position;
        public float yRotation;
        public float health;
        public int experience;
        public List<int> inventory;
        public List<int> gear;
        public List<GeneratedNodeSerializable> dungeon_nodes;  // based on game modes, this may be null
    }
}
