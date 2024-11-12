using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Popup;
using SlimeMaster.Model;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Manager
{
    public class StageManager
    {
        public Map CurrentMap => _currentMap;
        public StageData StageData => _stageData;
        public WaveData WaveData => _waveData;
        
        private StageData _stageData;
        private WaveData _waveData;
        private MonsterSpawnPool _monsterSpawnPool;
        private Map _currentMap;
        private StageModel _stageModel;

        private EventManager _event;
        private ObjectManager _object;
        private ResourcesManager _resource;
        private CancellationTokenSource _waveTimerCts;
        
        public void Initialize(int stageIndex)
        {
            GameManager manager = GameManager.I;
            _event = manager.Event;
            _object = manager.Object;
            _resource = manager.Resource;
            
            _monsterSpawnPool = new MonsterSpawnPool();
            _stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            SetStageData(stageIndex);
            AddEvent();
        }

        private void AddEvent()
        {
            _event.AddEvent(GameEventType.DeadMonster, OnDeadMonster);
        }

        private bool TrySpawnGem(ref GemType gemType)
        {
            float smallGemRatio = _waveData.SmallGemDropRate;
            float greenGemRatio = _waveData.GreenGemDropRate;
            float blueGemRatio = _waveData.BlueGemDropRate;
            float yellowGemRatio = _waveData.YellowGemDropRate;

            float select = Random.value;
            if (select <= smallGemRatio)
            {
                gemType = GemType.SmallGem;
            }
            
            if (select <= greenGemRatio)
            {
                gemType = GemType.GreenGem;
            }
            
            if (select <= blueGemRatio)
            {
                gemType = GemType.BlueGem;
            }
            
            if(select <= yellowGemRatio)
            {
                gemType = GemType.YellowGem;
            }

            if (gemType == GemType.None)
            {
                // Debug.LogError($"failed get gemType random value {select}");
                return false;
            }

            return true;
        }

        private void SpawnDropItemByMonsterType(MonsterType monsterType, Vector3 spawnPosition)
        {
            switch (monsterType)
            {
                case MonsterType.Normal:
                    bool isPossibleSpawnDropItem = Random.value >= _waveData.nonDropRate;
                    if (!isPossibleSpawnDropItem)
                    {
                        return;
                    }

                    GemType gemType = GemType.None;
                    bool isSuccess = TrySpawnGem(ref gemType);
                    if (isSuccess)
                    {
                        GemController gem = GameManager.I.Object.MakeGem(gemType, spawnPosition);
                        _currentMap.AddItemInGrid(gem.transform.position, gem);
                    }

                    break;
                case MonsterType.Elete:
                    var dropItemIdList = _waveData.EliteDropItemId;

                    foreach (int id in dropItemIdList)
                    {
                        if (!GameManager.I.Data.DropItemDict.TryGetValue(id, out DropItemData dropItemData))
                        {
                            Debug.LogWarning($"failed spawn drop item {id}");
                            continue;
                        }
                        
                        string prefabName = dropItemData.DropItemType.ToString();
                        GameObject prefab = GameManager.I.Resource.Instantiate(prefabName);
                        var dropItem = prefab.GetComponent<DropItemController>();
                        dropItem.Spawn(spawnPosition);
                        dropItem.SetInfo(dropItemData);
                        _currentMap.AddItemInGrid(spawnPosition, dropItem);
                    }

                    break;
                case MonsterType.Boss:
                    break;
            }

            if (Random.value < Const.STAGE_SOULDROP_RATE)
            {
                SoulController soul = GameManager.I.Object.MakeSoul(spawnPosition);
                _currentMap.AddItemInGrid(spawnPosition, soul);
            }
        }

        private void OnDeadMonster(object value)
        {
            StageModel model = ModelFactory.CreateOrGetModel<StageModel>();
            model.killCount.Value++;
      
            var monster = (MonsterController)value;
            SpawnDropItemByMonsterType(monster.MonsterType, monster.Position);

            if (model.killCount.Value != 0 && model.killCount.Value % Const.MONSTER_KILL_BONUS_COUNT == 0)
            {
                GameManager.I.Object.Player.OnUpgradeStatByMonsterKill();
            }
        }
        
        private void SetStageData(int index)
        {
            _stageData = GameManager.I.Data.StageDict[1];
            _waveData = _stageData.WaveArray[0];
            _stageModel.currentWaveStep.Value = _waveData.WaveIndex;
            
            MakeMap();
        }

        private void MakeMap()
        {
            //맵, 몬스터 스폰, Wave
            GameObject map = _resource.Instantiate(_stageData.MapName, false);
            _currentMap = map.GetOrAddComponent<Map>();
            map.SetActive(true);
        }

        public async UniTask StartStage()
        {
            int waveCount = _stageData.WaveArray.Count;
            for (int i = 0; i < waveCount; i++)
            {
                await StartWave();
                
                _currentMap.DoChangeMap(waveCount, i);
                StopWave();
                int nextIndex = i + 1;
                if (nextIndex == waveCount)
                {
                    break;
                }

                _waveData = _stageData.WaveArray[nextIndex];
                _stageModel.currentWaveStep.Value = _waveData.WaveIndex;
                GameManager.I.Event.Raise(GameEventType.EndWave);
            }
            
            CompleteStage();
        }

        private void CompleteStage()
        {
            Time.timeScale = 0;
            var gameResultPopup = GameManager.I.UI.OpenPopup<UI_GameResultPopup>();

            TimeSpan playTimeSpan = DateTime.UtcNow - GameManager.I.GameStartTime;
            string playTime = playTimeSpan.ToString(@"mm\:ss");
            Debug.Log($"play Time {(DateTime.UtcNow - GameManager.I.GameStartTime).TotalSeconds} / {playTime}");
            gameResultPopup.UpdateUI(_stageData.StageLevel.ToString(), playTime, _stageData.ClearReward_Gold,
                _stageModel.killCount.ToString());
        }

        private async UniTask StartWave()
        {
            _monsterSpawnPool.SpawnMonsterAsync(_waveData.SpawnInterval, _waveData.MonsterId,
                _waveData.OnceSpawnCount,
                _waveData.FirstMonsterSpawnRate, _waveData.EleteId, _waveData.BossId).Forget();
         
            await WaveTimerAsync();   
            SpawnDropItem();
        }

        enum EDropItemType
        {
            Potion,
            Magnet,
            Bomb
        }

        private void SpawnDropItem()
        {
            int length = System.Enum.GetValues(typeof(EDropItemType)).Length;
            int random = Random.Range(0, length);
            string prefabName = ((EDropItemType)random).ToString();
            GameObject prefab = GameManager.I.Resource.Instantiate(prefabName);
            var dropItem = prefab.GetComponent<DropItemController>();
            
            Vector3 spawnPosition = Utils.GetPositionInDonut(_object.Player.transform, 5, 10);
            dropItem.Spawn(spawnPosition);

            DropItemData dropItemData = null;
            switch ((EDropItemType) random)
            {
                case EDropItemType.Potion:
                    if (GameManager.I.Data.DropItemDict.TryGetValue(Const.ID_POTION, out dropItemData))
                    {
                        dropItem.SetInfo(dropItemData);
                    }
                    break;
                case EDropItemType.Magnet:
                    if (GameManager.I.Data.DropItemDict.TryGetValue(Const.ID_MAGNET, out dropItemData))
                    {
                        dropItem.SetInfo(dropItemData);
                    }
                    break;
                case EDropItemType.Bomb:
                    if (GameManager.I.Data.DropItemDict.TryGetValue(Const.ID_BOMB, out dropItemData))
                    {
                        dropItem.SetInfo(dropItemData);
                    }
                    break;
            }
            
            _currentMap.AddItemInGrid(spawnPosition, dropItem);
        }

        public void StopWave()
        {
            _monsterSpawnPool.StopMonsterSpawn();
            Utils.SafeCancelCancellationTokenSource(ref _waveTimerCts);
        }

        private async UniTask WaveTimerAsync()
        {
            _waveTimerCts = new();
            CancellationToken token = _waveTimerCts.Token;
            int timer = (int) _waveData.RemainsTime;
            timer = 5;
            GameManager gameManager = GameManager.I;
            while (timer > 0)
            {
                if (gameManager.GameState == GameState.DeadPlayer)
                {
                    try
                    {
                        await UniTask.Yield(cancellationToken: _waveTimerCts.Token);
                        continue;
                    }
                    catch (Exception e) when (!(e is OperationCanceledException))
                    {
                        Debug.LogError("error wave time " + e);
                        break;
                    }
                }
                
                _stageModel.timer.Value = timer;
                timer--;
                try
                {
                    await UniTask.WaitForSeconds(1, cancellationToken: token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError("error wave time " + e);
                    break;
                }
            }
            
            Debug.Log("End");
        }
    }
}