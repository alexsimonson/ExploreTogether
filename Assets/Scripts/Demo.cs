using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : GameMode, IGameMode {

    void Start(){
        mode = Mode.Demo;
    }

    public override bool TransitionPeriod(){
        return transition_period;
    }

    public override void Initialize(){
        return;
    }

    public override void SetupNextRound(){
        return;
    }

    public override bool ShouldSpawnEnemy(){
        return false;
    }

    public override void ResetGameMode(){
        return;
    }

    public override GameObject[] GetPlayerInventoryBackup(){
        return null;
    }

    public override GameObject[] GetPlayerGearBackup(){
        return null;
    }

    public override void ProgressGameMode(){
        return;
    }

    public override void SpawnMap(){
        Debug.Log("Spawning map");
        return;
    }

    public override List<KeyValuePair<string, int>> GetScoreData(){
        return null;
    }

    public override int GetUnspentScore(){
        return -1;
    }

}