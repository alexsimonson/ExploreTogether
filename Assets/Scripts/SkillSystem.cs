using UnityEngine;
namespace ExploreTogether {
    [System.Serializable]
    public enum SkillName
    {
        Melee,
        Range,
        Magic,
        Health,
        Stamina,
        Alchemy,
        Blacksmithing,
        Communication,
        Cooking,
        Crafting,
        Engineering,
        Farming,
        Fishing,
        Hunting,
        Lumberjacking,
        Mining,
        Thieving
    }
	
    public class SkillInfo
	{
        public SkillName name;
        public int level;
        public int experience;
    }
	
    public class SkillSystem : ScriptableObject
	{
        public SkillInfo[] skills;
    
        public SkillSystem(){
            skills = new SkillInfo[System.Enum.GetValues(typeof(SkillName)).Length];
        }
        
        private const int MaxLevel = 100;
    
        // Increase experience in a specific skill
        public void GainExperience(SkillName skillName, int amount)
        {
            SkillInfo skill = GetSkill(skillName);
            if (skill != null)
            {
                skill.experience += amount;
                CheckLevelUp(skill);
            }
        }
    
        // Check if the skill has reached the required experience for leveling up
        private void CheckLevelUp(SkillInfo skill)
        {
            int expRequired = GetExperienceRequired(skill.level);
            if (skill.experience >= expRequired)
            {
                skill.experience -= expRequired;
                LevelUp(skill);
            }
        }
    
        // Level up the skill if it hasn't reached the maximum level
        private void LevelUp(SkillInfo skill)
        {
            if (skill.level < MaxLevel)
            {
                skill.level++;
                Debug.Log("Congratulations! " + skill.name.ToString() + " leveled up to " + skill.level.ToString());
            }
        }
    
        // Get the required experience for the given level (linear rate)
        private int GetExperienceRequired(int level)
        {
            // Modify this formula to adjust the rate of experience required per level
            return level * 100;
        }
    
        // Get the Skill object by skill name
        private SkillInfo GetSkill(SkillName skillName)
        {
            foreach (SkillInfo skill in skills)
            {
                if (skill.name == skillName)
                {
                    return skill;
                }
            }
            Debug.LogWarning("Skill not found: " + skillName.ToString());
            return null;
        }
    
        // Initialize the skills
        public void Initialize(SkillInfo[] saved_skills = null)
        {
            if (saved_skills == null)
            {
                // Set all skills with 0 experience and level 1
                foreach (SkillName skillName in System.Enum.GetValues(typeof(SkillName)))
                {
                    SkillInfo skill = new SkillInfo();
                    skill.name = skillName;
                    skill.level = 1;
                    skill.experience = 0;
                    skills[(int)skillName] = skill;
                }
                Debug.Log("Skills initialized");
            }
            else
            {
                // Replace the skills array with the saved skills
                skills = saved_skills;
                Debug.Log("Skills loaded from saved_skills");
            }
        }
    }
}
