using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Manager
{
    [Serializable]
    public struct SpawnObjectData
    {
        public int id;
        public Type Type;
        public Vector3 spawnPosition;
    }
    
    public class ObjectManager
    {
        private EventManager _event;
        private ResourcesManager _resource;
        private DataManager _data;

        private PlayerController _player;
        
        public void Initialize(EventManager eventManager, ResourcesManager resourcesManager, DataManager dataManager, PlayerController playerController)
        {
            _event = eventManager;
            _resource = resourcesManager;
            _data = dataManager;

            _player = playerController;
            _player.Initialized();
            
            GameManager.I.Event.AddEvent(GameEventType.SpawnObject, OnSpawnObject);
        }

        private void OnSpawnObject(object value)
        {
            SpawnObjectData spawnObjectData = default;
            try
            {
                spawnObjectData = (SpawnObjectData)value;
            }
            catch (Exception e)
            {
                Debug.LogError($"failed parse error {e}");
                return;
            }

            if (spawnObjectData.Type == typeof(MonsterController))
            {
                CreatureData data = _data.CreatureDict[spawnObjectData.id];
                GameObject monsterObj = _resource.Instantiate(data.PrefabLabel);
                var monster = Utils.AddOrGetComponent<MonsterController>(monsterObj);
                Sprite sprite = _resource.Load<Sprite>(data.IconLabel);
                monster.Initialize(data, sprite);
                monster.Spawn(spawnObjectData.spawnPosition, _player);

            }
            
        }
    }
}
