using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExploreTogether {
    public interface IGameMode{
        bool TransitionPeriod();
        void Initialize();
        void SetupNextRound();
        bool ShouldSpawnEnemy();
        void ResetGameMode();
        GameObject[] GetPlayerInventoryBackup();
        GameObject[] GetPlayerGearBackup();
        void ProgressGameMode();
        void SpawnMap();
        List<KeyValuePair<string, int>> GetScoreData();
        int GetUnspentScore();
    }
}