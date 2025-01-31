using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
    [System.Serializable]
    public class CharacterData : ScriptableObject{
        public string name;
        public GameMode.Mode game_mode;
        public float health;
        public int experience;
        public List<int> inventory;
        public List<int> gear;
    }
}
