using System;
using System.Collections.Generic;
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
using SlimeMaster.Shared.Data;
using SlimeMaster.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public void GameEnd()
        {
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.Reset();
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            stageModel.Reset();
            StopStage();
            Managers.Manager.I.Object.GameEnd();
            SceneManager.LoadScene(SceneType.LobbyScene.ToString());
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
            Debug.LogError("MakeMap");
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
            _stageTaskSource = new();
            var task = UniTask.WhenAll(_stageTaskSource.Task, StartStageAsync());
        }

        private UniTaskCompletionSource _stageTaskSource;
        public void StopStage()
        {
            if (_stageTaskSource != null)
            {
                _stageTaskSource.TrySetResult();
            }
            
            StopWave();
            Utils.SafeCancelCancellationTokenSource(ref _waveTimerCts);
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

            var response = await ServerHandlerFactory.Get<IUserClientSender>().StageClearRequest(new StageClearRequest() { stageIndex = _stageData.StageIndex});
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

        #region PathFinding

        private readonly Vector3Int[] _dirArray =
        {
            new Vector3Int(1, 0),
            new Vector3Int(0, 1),
            new Vector3Int(-1, 0),
            new Vector3Int(0, -1),
            new Vector3Int(1, 1),
            new Vector3Int(1, -1),
            new Vector3Int(-1, 1),
            new Vector3Int(-1, -1),
        };

        private Dictionary<Vector3Int, CreatureController> _cellDict = new Dictionary<Vector3Int, CreatureController>();
        
        public List<Vector3Int> PathFinding(Vector3Int startPosition, Vector3Int destPosition, int maxDepth = 10)
        {
            // 상하좌우 + 대각선 (8 방향)
            //int[] cost = { 10, 10, 10, 10, 14, 14, 14, 14 }; // 대각선 이동은 비용 14

            Dictionary<Vector3Int, int> bestDict = new Dictionary<Vector3Int, int>();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            PriorityQueue<Node> queue = new PriorityQueue<Node>();
            Dictionary<Vector3Int, Vector3Int> pathDict = new Dictionary<Vector3Int, Vector3Int>();

            //목적지에 도착하지 못할 경우에 그나마 가장 가까운 위치로 보낸다.
            int closedH = int.MaxValue;
            Vector3Int closePos = Vector3Int.zero;
            int depth = 0;

            {
                int h = Mathf.Abs(destPosition.x - startPosition.x) + Mathf.Abs(destPosition.y - startPosition.y);
                Node startNode = new Node(h, depth, startPosition);
                queue.Push(startNode);
                bestDict[startPosition] = h;
                pathDict[startPosition] = startPosition;

                closedH = h;
                closePos = startPosition;
            }

            while (!queue.IsEmpty())
            {
                Node node = queue.Top();
                queue.Pop();

                Vector3Int nodePos = new Vector3Int(node.x, node.y, 0);
                if (nodePos == destPosition)
                {
                    // closePos = nodePos;
                    break;
                }

                if (visited.Contains(nodePos))
                {
                    continue;
                }
                
                visited.Add(nodePos);
                depth = node.depth;

                // Debug.Log($"{depth}");
                if (depth == maxDepth)
                {
                    break;
                }
                
                for (int i = 0; i < _dirArray.Length; i++)
                {
                    int nextY = node.y + _dirArray[i].y;
                    int nextX = node.x + _dirArray[i].x;
                    if (!CanGo(nextX, nextY))
                    {
                        continue;
                    }

                    // Debug.Log("Can");
                    Vector3Int nextPos = new Vector3Int(nextX, nextY, 0);
                    //int g = cost[i] + node.g;
                    int h = Mathf.Abs(destPosition.x - nextPos.x) + Mathf.Abs(destPosition.y - nextPos.y);
                    if (bestDict.ContainsKey(nextPos) && bestDict[nextPos] <= h)
                    {
                        continue;
                    }

                    bestDict[nextPos] = h;
                    queue.Push(new Node(h, depth + 1, nextPos));
                    pathDict[nextPos] = nodePos;
                    
                    if (closedH > h)
                    {
                        closedH = h;
                        closePos = nextPos;
                    }
                }
            }

            List<Vector3Int> list = new List<Vector3Int>();
            Vector3Int now = destPosition;

            if (!pathDict.ContainsKey(now))
            {
                now = closePos;
                if (!pathDict.ContainsKey(now))
                {
                    Debug.LogError($"Pathfinding Error: No valid path found to {destPosition} or closest position {closePos}");
                    return new List<Vector3Int>(); 
                }
            }

            int count = 0;
            while (pathDict[now] != now)
            {
                if (!list.Contains(now))
                {
                    list.Add(now);
                }
            
                count++;
                now = pathDict[now];
                if (count > maxDepth)
                {
                    break;
                }
            }

            list.Add(now);
            list.Reverse();
            // Debug.Log($"now {depth} {now} / {destPosition} / {closePos} / {startPosition} / {list.Count}");
            return list;
        }

        public bool CanGo(int x, int y, bool ignoreObject = false)
        {
            if (ignoreObject)
            {
                return true;
            }
            
            if (_cellDict.TryGetValue(new Vector3Int(x, y), out var creature))
            {
                return false;
            }
            
            return true;
        }

        public void AddCreatureInGrid(Vector3Int position, CreatureController creature)
        {
            if (_cellDict.ContainsKey(position))
            {
                return;
            }
            
            _cellDict[position] = creature;
        }

        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return _currentMap.Grid.Grid.WorldToCell(worldPosition);
        }

        public Vector3 CellToWorld(Vector3Int cellPosition)
        {
            return _currentMap.Grid.Grid.CellToWorld(cellPosition);
        }
        
        #endregion
    }
}