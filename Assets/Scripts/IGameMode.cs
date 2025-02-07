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
        // adding these in from maze.cs
        bool SpawnItem(int _item_id, Vector3? _position=null);
        Vector3 RandomSpawnPointInGenerated(int _padding=10);
        Vector3 SpawnPointAtIndex(int _index);
        void SpawnResource(string _object_prefab, Vector3? _position=null, string _name="spawned prefab");
        void SpawnEnemy(GameObject _enemy_prefab, Vector3? _position=null, string _name="spawned_enemy_name");
        List<MazeSpawnedItemSerializable> ExportSpawnedItems();
        bool ImportSavedItems(List<MazeSpawnedItemSerializable> _load_spawned_items);
        List<MazeSpawnedObjectSerializable> ExportSpawnedEnemies();
        bool ImportSavedEnemies(List<MazeSpawnedObjectSerializable> _load_spawned_enemies);
        List<MazeSpawnedObjectSerializable> ExportSpawnedResources();
        bool ImportSavedResources(List<MazeSpawnedObjectSerializable> _load_spawned_resources);
        bool FindAndRemoveItem(int _item_id, Vector3 _position);
        bool AddSpawnedItem(int _item_id, Vector3 _drop_position);
        bool FindAndRemoveEnemy(Transform _remove_transform);
    }
}