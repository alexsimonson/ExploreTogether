using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject{
    public string characterName;
    public int characterLevel;
    public float health;
    public int experiencePoints;
}
