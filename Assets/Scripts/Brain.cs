using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
	public class Brain : MonoBehaviour{
		
		public List<Task> tasks = new List<Task>();

		public void AddTask(Task task){
			if(tasks.Contains(task)){
				Debug.Log("Task already exists");
			}else{
				Debug.Log("Adding task");
				tasks.Add(task);
			}
		}

		public void RemoveTask(Task task){
			if(tasks.Contains(task)){
				Debug.Log("The list contains this exact task");
				tasks.Remove(task);
			}else{
				Debug.Log("The list doesn't contain that task");
			}
		}
	}
}
