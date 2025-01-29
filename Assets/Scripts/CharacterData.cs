using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
[System.Serializable]
public class CharacterData : ScriptableObject{
    public string name;
    public float health;
    public int experience;
}
