using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.Popup;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Manager
{
    public class GameManager
    {
        public Map CurrentMap => _currentMap;
        public StageData StageData => _stageData;
        public WaveData WaveData => _waveData;
        public GameState GameState => _gameState;
        
        private StageData _stageData;
        private WaveData _waveData;
        private MonsterSpawnPool _monsterSpawnPool;
        private Map _currentMap;
        private StageModel _stageModel;

        private EventManager _event = Managers.Manager.I.Event;
        private ObjectManager _object = Managers.Manager.I.Object;
        private ResourcesManager _resource = Managers.Manager.I.Resource;
        private CancellationTokenSource _waveTimerCts;
        private GameState _gameState;
        
        public DateTime GameStartTime { get; private set; }
        
        public void Initialize()
        {
            _stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            _monsterSpawnPool = new MonsterSpawnPool();
            AddEvent();
        }

        private void AddEvent()
        {
            _event.AddEvent(GameEventType.DeadMonster, OnDeadMonster);
            _event.AddEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            _event.AddEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);
        }
        
        private void OnResurrectionPlayer(object value)
        {
            UpdateGameState(GameState.Start);
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
                        GemController gem = Managers.Manager.I.Object.MakeGem(gemType, spawnPosition);
                        _currentMap.AddItemInGrid(gem.transform.position, gem);
                    }

                    break;
                case MonsterType.Elete:
                    var dropItemIdList = _waveData.EliteDropItemId;

                    foreach (int id in dropItemIdList)
                    {
                        if (!Managers.Manager.I.Data.DropItemDict.TryGetValue(id, out DropItemData dropItemData))
                        {
                            Debug.LogWarning($"failed spawn drop item {id}");
                            continue;
                        }
                        
                        string prefabName = dropItemData.DropItemType.ToString();
                        GameObject prefab = Managers.Manager.I.Resource.Instantiate(prefabName);
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
                SoulController soul = Managers.Manager.I.Object.MakeSoul(spawnPosition);
                _currentMap.AddItemInGrid(spawnPosition, soul);
            }
        }

        private void OnDeadMonster(object value)
        {
            StageModel model = ModelFactory.CreateOrGetModel<StageModel>();
            model.killCount.Value++;
            Managers.Manager.I.GameContinueData.killCount++;
      
            var monster = (MonsterController)value;
            SpawnDropItemByMonsterType(monster.MonsterType, monster.Position);

            if (model.killCount.Value != 0 && model.killCount.Value % Const.MONSTER_KILL_BONUS_COUNT == 0)
            {
                Managers.Manager.I.Object.Player.OnUpgradeStatByMonsterKill();
            }
        }
        
        private void SetStageData(StageData stageData, int waveIndex = 0)
        {
            _stageData = stageData;
            _waveData = _stageData.WaveArray[waveIndex];
            _stageModel.currentWaveStep.Value = _waveData.WaveIndex;
        }

        private void MakeMap()
        {
            //맵, 몬스터 스폰, Wave
            GameObject map = _resource.Instantiate(_stageData.MapName, false);
            _currentMap = map.GetOrAddComponent<Map>();
            map.SetActive(true);
        }

        public async UniTaskVoid StartStage(StageData stageData, int waveIndex)
        {
            SetStageData(stageData, waveIndex);
            MakeMap();
            _object.CreatePlayer();
            await StartStageAsync();
        }

        private async UniTask StartStageAsync()
        {
            GameStartTime = DateTime.UtcNow;
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
                Managers.Manager.I.GameContinueData.waveIndex = _waveData.WaveIndex;
                Managers.Manager.I.Event.Raise(GameEventType.EndWave);
            }
            
            CompleteStage();
        }

        private async void CompleteStage()
        {
            Time.timeScale = 0;
            var gameResultPopup = Managers.Manager.I.UI.OpenPopup<UI_GameResultPopup>();

            TimeSpan playTimeSpan = DateTime.UtcNow - GameStartTime;
            string playTime = playTimeSpan.ToString(@"mm\:ss");
            gameResultPopup.UpdateUI(_stageData.StageLevel.ToString(), playTime, _stageData.ClearReward_Gold,
                _stageModel.killCount.ToString());
            
            _object.AllObjectRelease();

            var response = await ServerHandlerFactory.Get<IUserClientSender>().StageClearRequest(_stageData.StageIndex);
            if (response.responseCode != ServerErrorCode.Success)
            {
                switch (response.responseCode)
                {
                    case ServerErrorCode.FailedFirebaseError:
                    case ServerErrorCode.FailedGetUserData:
                        //Alert
                        Debug.LogError("Failed :" + response.errorMessage);
                        return;
                }
            }

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.AddItemValue(response.ItemData.ItemId, response.ItemData.ItemValue);
            userModel.UpdateStage(response.StageData.StageIndex, response.StageData.WaveDataList.Last().WaveIndex);
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
            GameObject prefab = Managers.Manager.I.Resource.Instantiate(prefabName);
            var dropItem = prefab.GetComponent<DropItemController>();
            
            Vector3 spawnPosition = Utils.GetPositionInDonut(_object.Player.transform, 5, 10);
            dropItem.Spawn(spawnPosition);

            DropItemData dropItemData = null;
            switch ((EDropItemType) random)
            {
                case EDropItemType.Potion:
                    if (Managers.Manager.I.Data.DropItemDict.TryGetValue(Const.ID_POTION, out dropItemData))
                    {
                        dropItem.SetInfo(dropItemData);
                    }
                    break;
                case EDropItemType.Magnet:
                    if (Managers.Manager.I.Data.DropItemDict.TryGetValue(Const.ID_MAGNET, out dropItemData))
                    {
                        dropItem.SetInfo(dropItemData);
                    }
                    break;
                case EDropItemType.Bomb:
                    if (Managers.Manager.I.Data.DropItemDict.TryGetValue(Const.ID_BOMB, out dropItemData))
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
            Managers.Manager manager = Managers.Manager.I;
            while (timer > 0)
            {
                if (GameState == GameState.DeadPlayer)
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
        
        private void OnDeadPlayer(object value)
        {
            UpdateGameState(GameState.DeadPlayer);
        }
        
        private void UpdateGameState(GameState gameState) => _gameState = gameState;
    }
}