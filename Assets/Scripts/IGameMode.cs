using System.Collections;
using UnityEngine;
public interface IGameMode{
    bool TransitionPeriod();
    void Initialize();
    
    IEnumerator BeginTransitionPeriod();
    void SetupNextRound();
    void SpawnEnemies();
    bool ShouldSpawnEnemy();
    void EnemyKilled();
    void ResetGameMode();
    GameObject[] GetPlayerInventoryBackup();
    GameObject[] GetPlayerGearBackup();
    void ProgressGameMode();
    void SpawnMap();
}