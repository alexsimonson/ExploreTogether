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
        void SpawnMap(bool isLoading=false);
        List<KeyValuePair<string, int>> GetScoreData();
        int GetUnspentScore();
    }
}