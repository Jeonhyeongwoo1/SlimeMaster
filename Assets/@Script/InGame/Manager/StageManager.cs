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
using SlimeMaster.Model;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Manager
{
    public class StageManager
    {
        public Map CurrentMap => _currentMap;
        
        private PlayerController _player;
        private StageData _stageData;
        private WaveData _waveData;
        private MonsterSpawnPool _monsterSpawnPool;
        private Map _currentMap;
        private StageModel _stageModel;

        private EventManager _event;
        private ObjectManager _object;
        private ResourcesManager _resource;
        private CancellationTokenSource _waveTimerCts;
        
        public void Initialize(int stageIndex, PlayerController playerController)
        {
            GameManager manager = GameManager.I;
            _event = manager.Event;
            _object = manager.Object;
            _resource = manager.Resource;
            
            _player = playerController;
            _monsterSpawnPool = new MonsterSpawnPool(_player);
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

        private void OnDeadMonster(object value)
        {
            bool isPossibleSpawnDropItem = Random.value >= _waveData.nonDropRate;
            if (!isPossibleSpawnDropItem)
            {
                return;
            }

            GemType gemType = GemType.None;
            bool isSuccess = TrySpawnGem(ref gemType);
            if (isSuccess)
            {
                var monster = (MonsterController)value;
                GemController gem = MakeGem(gemType, monster.Position);
                _currentMap.AddItemInGrid(gem.transform.position, gem);
            }

            StageModel model = ModelFactory.CreateOrGetModel<StageModel>();
            model.killCount.Value++;
        }

        private GemController MakeGem(GemType gemType, Vector3 spawnPosition)
        {
            GameObject prefab = _resource.Instantiate(Const.ExpGem);
            Sprite sprite = _resource.Load<Sprite>(gemType.ToString());
            var gem = prefab.GetOrAddComponent<GemController>();
            gem.Initialize(sprite, gemType, spawnPosition);
            
            return gem;
        }

        private void SetStageData(int index)
        {
            _stageData = GameManager.I.Data.StageDict[index];
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
                
                StopWave();
                int nextIndex = i + 1;
                if (nextIndex == waveCount)
                {
                    break;
                }

                _waveData = _stageData.WaveArray[nextIndex];
                _stageModel.currentWaveStep.Value = _waveData.WaveIndex;
            }
            
            StageClear();
        }

        private void StageClear()
        {
            
        }

        private async UniTask StartWave()
        {
            _monsterSpawnPool.StartMonsterSpawn(_waveData.SpawnInterval, _waveData.MonsterId, _waveData.OnceSpawnCount,
                _waveData.FirstMonsterSpawnRate);
            await WaveTimerAsync();
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
            while (timer > 0)
            {
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
        }
    }
}