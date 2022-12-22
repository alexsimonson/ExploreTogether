using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour, IGameMode {
    
    public enum Mode{
        WaveSurvival,
        DungeonCrawler
    }

    public Mode mode;

    public abstract bool TransitionPeriod();
    public abstract void Initialize();
    public abstract IEnumerator BeginTransitionPeriod();
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
