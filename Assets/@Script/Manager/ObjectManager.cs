using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.View;
using SlimeMaster.Manager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI.Extensions;
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
    
    public class ObjectManager : IDisposable
    {
        public List<MonsterController> ActivateMonsterList => _activateMonsterList;
        public List<DropItemController> DroppedItemControllerList => _droppedItemControllerList;
        public PlayerController Player => _player;
        
        private EventManager _event;
        private ResourcesManager _resource;
        private DataManager _data;
        private PoolManager _pool;
        
        private List<MonsterController> _activateMonsterList = new();
        private List<DropItemController> _droppedItemControllerList = new();
        private PlayerController _player;
        
        public void Initialize()
        {
            GameManager manager = GameManager.I;
            _event = manager.Event;
            _resource = manager.Resource;
            _data = manager.Data;
            _pool = manager.Pool;
            
            AddEvent();
        }

        public void CreatePlayer()
        {
            GameObject playerPrefab = GameManager.I.Resource.Instantiate("Player");
            var player = Utils.AddOrGetComponent<PlayerController>(playerPrefab);
            _player = player;
            
            CreatureData creatureData = _data.CreatureDict[(int)CreatureType.Player];
            
            // creatureData.SkillTypeList.Add(10071);
            // creatureData.SkillTypeList.Add(10081);
            // creatureData.SkillTypeList.Add(10091);
            // creatureData.SkillTypeList.Add(10101);
            // creatureData.SkillTypeList.Add(10111);
            creatureData.SkillTypeList.Add(10131);
            
            List<SkillData> skillDataList = creatureData.SkillTypeList.Select(i => _data.SkillDict[i]).ToList();
            _player.Initialize(creatureData, _resource.Load<Sprite>(creatureData.IconLabel), skillDataList);
        }

        private void AddEvent()
        {
            GameManager.I.Event.AddEvent(GameEventType.SpawnMonster, OnSpawnObject);
            GameManager.I.Event.AddEvent(GameEventType.DeadMonster, OnDeadMonster);
            GameManager.I.Event.AddEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
        }

        public void Dispose()
        {
            // TODO release managed resources here
            RemoveEvent();
        }

        private void RemoveEvent()
        {
            GameManager.I.Event.RemoveEvent(GameEventType.SpawnMonster, OnSpawnObject);
            GameManager.I.Event.RemoveEvent(GameEventType.DeadMonster, OnDeadMonster);
            GameManager.I.Event.RemoveEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
        }
        
        public bool TryResetSupportSkillList()
        {
            if (_player.SoulAmount < Const.CHANGE_SUPPORT_SKILL_AMOUNT)
            {
                return false;
            }

            _player.SoulAmount -= Const.CHANGE_SUPPORT_SKILL_AMOUNT;
            _player.SkillBook.CurrentSupportSkillDataList = _player.SkillBook.GetRecommendSupportSkillDataList();
            return true;
        }

        public void AllObjectRelease()
        {
            _droppedItemControllerList.ForEach(v=> v?.Release());
            _activateMonsterList.ForEach(v=> v?.Release());
            _player.Release();
        }

        private void OnActivateDropItem(object value)
        {
            DropItemData dropItemData = (DropItemData)value;
            switch(dropItemData.DropItemType)
            {
                case DropableItemType.Bomb:
                    _activateMonsterList.ForEach(v=>
                    {
                        if (v.MonsterType == MonsterType.Normal)
                        {
                            v.ForceKill();
                        }
                    });
                    break;
                case DropableItemType.Magnet:
                    _droppedItemControllerList.ForEach(v =>
                    {
                        v.GetItem(_player.transform);
                    });
                    
                    GameManager.I.Stage.CurrentMap.Grid.RemoveAllItem(DropableItemType.Gem);
                    break;
            }
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

            if (spawnObjectData.Type == typeof(MonsterController) ||
                spawnObjectData.Type == typeof(EliteMonsterController) ||
                spawnObjectData.Type == typeof(BossMonsterController))
            {
                CreatureData data = _data.CreatureDict[spawnObjectData.id];
                GameObject monsterObj = _resource.Instantiate(data.PrefabLabel);
                var monster = Utils.AddOrGetComponent<MonsterController>(monsterObj);
                Sprite sprite = _resource.Load<Sprite>(data.IconLabel);
                List<SkillData> skillDataList = data.SkillTypeList.Select(i => _data.SkillDict[i]).ToList();
                monster.Initialize(data, sprite, skillDataList);
                monster.Spawn(spawnObjectData.spawnPosition + _player.Position, _player);
                _activateMonsterList.Add(monster);

                if (spawnObjectData.Type == typeof(EliteMonsterController))
                {
                    var uiGameScene = GameManager.I.UI.SceneUI as UI_GameScene;
                    uiGameScene.ShowMonsterInfo(MonsterType.Elete, data.DescriptionTextID, 1);

                    _player.OnKillEliteMonster();
                }
                else if(spawnObjectData.Type == typeof(BossMonsterController))
                {
                    var uiGameScene = GameManager.I.UI.SceneUI as UI_GameScene;
                    uiGameScene.ShowMonsterInfo(MonsterType.Boss, data.DescriptionTextID, 1);
                }
            }
        }

        public GemController MakeGem(GemType gemType, Vector3 spawnPosition)
        {
            GameObject prefab = _resource.Instantiate(Const.ExpGem);
            Sprite sprite = _resource.Load<Sprite>(gemType.ToString());
            var gem = prefab.GetOrAddComponent<GemController>();
            gem.Spawn(spawnPosition);
            gem.SetGemInfo(gemType, sprite);
            
            _droppedItemControllerList.Add(gem);
            return gem;
        }

        public SoulController MakeSoul(Vector3 spawnPosition)
        {
            GameObject prefab = _resource.Instantiate(Const.Soul);
            var soul = prefab.GetOrAddComponent<SoulController>();
            soul.Spawn(spawnPosition);
            _droppedItemControllerList.Add(soul);
            return soul;
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
        
        public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
        {
            string prefabName;
            if (isCritical)
                prefabName = "CriticalDamageFont";
            else
                prefabName = "DamageFont";

            GameObject go = _resource.Instantiate(prefabName);
            DamageFont damageText = go.GetOrAddComponent<DamageFont>();
            damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
        }

        private void OnDeadMonster(object value)
        {
            MonsterController monster = (MonsterController)value;
            _activateMonsterList.Remove(monster);
            _pool.ReleaseObject(monster.PrefabLabel, monster.gameObject);

            switch (monster.MonsterType)
            {
                case MonsterType.Normal:
                    break;
                case MonsterType.Elete:
                case MonsterType.Boss:
                    var uiGameScene = GameManager.I.UI.SceneUI as UI_GameScene;
                    uiGameScene.HideMonsterInfo(monster.MonsterType);
                    break;
            }
        }
    }
}
