using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour, IGameMode {
    
    public enum Mode{
        WaveSurvival
    }

    public Mode mode;

    public abstract bool TransitionPeriod();
    public abstract void Initialize();
    public abstract IEnumerator BeginTransitionPeriod();
    public abstract void SetupNextRound();
    public abstract void SpawnEnemies();
    public abstract void EnemyKilled();
}
