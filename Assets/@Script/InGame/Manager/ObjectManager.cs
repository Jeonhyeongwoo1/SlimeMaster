using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Enum;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public List<MonsterController> ActivateMonsterList => _activateMonsterList;
        private EventManager _event;
        private ResourcesManager _resource;
        private DataManager _data;
        private PoolManager _pool;
        
        private List<MonsterController> _activateMonsterList = new();
        private PlayerController _player;
        
        public void Initialize(PlayerController playerController)
        {
            GameManager manager = GameManager.I;
            _event = manager.Event;
            _resource = manager.Resource;
            _data = manager.Data;
            _pool = manager.Pool;

            _player = playerController;

            CreatureData creatureData = _data.CreatureDict[(int)CreatureType.Player];
            
            creatureData.SkillTypeList.Add(10071);
            creatureData.SkillTypeList.Add(10081);
            creatureData.SkillTypeList.Add(10091);
            creatureData.SkillTypeList.Add(10101);
            creatureData.SkillTypeList.Add(10111);
            creatureData.SkillTypeList.Add(10131);
            
            List<SkillData> skillDataList = creatureData.SkillTypeList.Select(i => _data.SkillDict[i]).ToList();
            _player.Initialize(creatureData, _resource.Load<Sprite>(creatureData.IconLabel), skillDataList);
            AddEvent();
        }

        private void AddEvent()
        {
            GameManager.I.Event.AddEvent(GameEventType.SpawnObject, OnSpawnObject);
            GameManager.I.Event.AddEvent(GameEventType.DeadMonster, OnDeadMonster);
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
                _activateMonsterList.Add(monster);
            }
        }

        public List<MonsterController> GetMonsterInRange(float minDistance, float maxDistance, Vector3 targetPosition)
        {
            List<MonsterController> monsterList =
                _activateMonsterList.OrderBy(a => (targetPosition - a.transform.position).sqrMagnitude).ToList();

            if (monsterList.Count == 0)
            {
                return null;
            }
         
            var list = new List<MonsterController>();   
            foreach (MonsterController monster in monsterList)
            {
                float distance = Vector3.Distance(targetPosition, monster.Position);
                if (distance > maxDistance)
                {
                    break;
                }
                
                if (minDistance <= distance && distance <= maxDistance)
                {
                    list.Add(monster);
                }
            }

            return list;
        }

        public List<MonsterController> GetNearestMonsterList(int count = 1)
        {
            List<MonsterController> monsterList =
                _activateMonsterList.OrderBy(a => (_player.Position - a.transform.position).sqrMagnitude).ToList();

            int min = math.min(count, monsterList.Count);
            if (min == 0)
            {
                return null;
            }
            
            monsterList = monsterList.Take(min).ToList();

            //카운터가 몬스터 리스트보다 크면 몬스터를 더 추가해준다.
            while (count > monsterList.Count)
            {
                monsterList.Add(monsterList[Random.Range(0, monsterList.Count)]);
            }

            return monsterList;
        }

        private Collider2D[] _collider2D = new Collider2D[20];
        private List<Transform> _cachedMonsterList = new();
        
        public List<Transform> GetMonsterAndBossTransformListInFanShape(Transform target, Vector3 direction, float angle = 180)
        {
            _cachedMonsterList.Clear();
            int count = Physics2D.OverlapCircleNonAlloc(target.position, 10, _collider2D,
                LayerMask.GetMask("Monster", "Boss"));
            
            for (int i = 0; i < count; i++)
            {
                var collider = _collider2D[i];
                Vector3 inVector = (collider.transform.position - target.position).normalized;
                float dot = Vector3.Dot(inVector, direction);
                float theta = MathF.Acos(dot);
                float degree = theta * Mathf.Rad2Deg;

                if (degree <= angle / 2)
                {
                    _cachedMonsterList.Add(collider.transform);
                }
            }
            
            if (_cachedMonsterList.Count == 0)
            {
                return null;
            }

            List<Transform> monsterList =
                _cachedMonsterList.OrderBy(a => (_player.Position - a.transform.position).sqrMagnitude).ToList();
            return monsterList;
        }

        private void OnDeadMonster(object value)
        {
            MonsterController monster = (MonsterController)value;
            _activateMonsterList.Remove(monster);
            _pool.ReleaseObject(monster.PrefabLabel, monster.gameObject);
        }
    }
}
