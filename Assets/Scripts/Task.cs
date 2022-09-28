using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task {

    public string title;
    public string description;
    public string reward;
    public string requirements;   // functional checks that must be completed to consider a task "complete"

    public Task(string title, string description, string reward, string requirements){
        this.title = title;
        this.description = description;
        this.reward = reward;
        this.requirements = requirements;
    }
}
