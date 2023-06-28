using UnityEngine;
using UnityEngine.UI;

namespace ExploreTogether {
    public class SkillUI : MonoBehaviour
	{
        public Manager manager;
        public ScrollRect scrollRect;
        public GameObject SkillInfoUI_prefab;
        public GameObject content;
    
        private void Awake()
        {
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            content = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            SkillInfoUI_prefab = Resources.Load("Prefabs/SkillInfoUI", typeof(GameObject)) as GameObject;
        }
    
        // Display the skills in the scroll view
        private void DisplaySkills()
        {
            // Clear existing skill items
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
    
            // Instantiate skill items for each skill
            // foreach (SkillInfo skill in manager.player_skills.skills)
            // {
            //     GameObject skillItem = Instantiate(SkillInfoUI_prefab, new Vector3(0, 0, 0), Quaternion.identity);
            //     UpdateSkillItem(skillItem, skill);
            //     skillItem.transform.SetParent(content.transform);
            // }
        }
    
        // Update the skill item with the relevant SkillInfo
        private void UpdateSkillItem(GameObject skillItem, SkillInfo skill)
        {
            Text skillNameText = skillItem.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            Text skillLevelText = skillItem.transform.GetChild(0).GetChild(1).GetComponent<Text>();
            Text skillExperienceText = skillItem.transform.GetChild(0).GetChild(2).GetComponent<Text>();
    
            skillNameText.text = skill.name.ToString();
            skillLevelText.text = "Level: " + skill.level.ToString();
            skillExperienceText.text = "Experience: " + skill.experience.ToString();
        }
    
        public void Initialize(){
            DisplaySkills();
        }
    }
}