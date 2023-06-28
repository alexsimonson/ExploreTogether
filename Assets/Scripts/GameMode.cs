using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    [CreateAssetMenu(fileName = "New Game Mode", menuName = "Game Mode")]
    public abstract class GameMode : ScriptableObject, IGameMode {
        
        public enum Mode{
            WaveSurvival,
            DungeonCrawler,
            Demo
        }

        public Manager manager;
        public Mode mode;
        public int score = 0;
        public bool transition_period;
        public GameObject[] playerInventoryBackup;
        public GameObject[] playerGearBackup;

        public abstract bool TransitionPeriod();
        public abstract void Initialize();
        public abstract void SetupNextRound();
        public abstract bool ShouldSpawnEnemy();
        public abstract void ResetGameMode();
        public abstract GameObject[] GetPlayerInventoryBackup();
        public abstract GameObject[] GetPlayerGearBackup();
        public abstract void ProgressGameMode();
        public abstract void SpawnMap();
        public abstract List<KeyValuePair<string, int>> GetScoreData();
        public abstract int GetUnspentScore();
    }
}