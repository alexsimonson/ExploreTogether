using System.Collections;
using UnityEngine;
public interface IGameMode{
    bool TransitionPeriod();
    void Initialize();
    IEnumerator BeginTransitionPeriod();
    void SetupNextRound();
    void SpawnEnemies();
    void EnemyKilled();
    void ResetGameMode();
}