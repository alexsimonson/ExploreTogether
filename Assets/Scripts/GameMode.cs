using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Mode", menuName = "Game Mode")]
public abstract class GameMode : ScriptableObject, IGameMode {
    
    public enum Mode{
        WaveSurvival,
        DungeonCrawler,
        Demo
    }

    public Mode mode;
    public bool transition_period;

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
