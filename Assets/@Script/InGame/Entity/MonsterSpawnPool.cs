using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Entity
{
    public class MonsterSpawnPool : IDisposable
    {
        private float _spawnInterval;
        private CancellationTokenSource _spawnCts;
        private PlayerController _player;

        public MonsterSpawnPool(PlayerController player)
        {
            _player = player;

            AddEvent();
        }

        private void AddEvent()
        {
            GameManager.I.Event.AddEvent(GameEventType.GameOver, OnGameOver);
        }

        private void OnGameOver(object value)
        {
            Utils.SafeCancelCancellationTokenSource(ref _spawnCts);
        }

        public void StartMonsterSpawn(float spawnInterval, List<int> monsterIdList, int onceSpawnCount,
            float firstMonsterSpanRate)
        {
            EnemySpawnAsync(spawnInterval, monsterIdList, onceSpawnCount, firstMonsterSpanRate).Forget();
        }

        public async UniTaskVoid EnemySpawnAsync(float spawnInterval, List<int> monsterIdList, int onceSpawnCount,
            float firstMonsterSpanRate)
        {
            _spawnCts = new CancellationTokenSource();

            while (true)
            {
                try
                {
                    await UniTask.WaitForSeconds(spawnInterval, cancellationToken: _spawnCts.Token);
                    
                    for (int i = 0; i < onceSpawnCount; i++)
                    {
                        float select = Random.value;
                        int monsterId =
                            monsterIdList[select <= firstMonsterSpanRate ? 0 : Random.Range(0, monsterIdList.Count)];
                        float angle = 360 / (i + 1);
                        var spawnPosition = GetCirclePosition(angle) + _player.transform.position;

                        SpawnObjectData spawnObjectData = new SpawnObjectData();
                        spawnObjectData.spawnPosition = spawnPosition;
                        spawnObjectData.id = monsterId;
                        spawnObjectData.Type = typeof(MonsterController);
                        GameManager.I.Event.Raise(GameEventType.SpawnObject, spawnObjectData);
                    }
                    
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"{nameof(EnemySpawnAsync)} error {e}");
                    await UniTask.WaitForSeconds(2f);
                    // Debug.Log("Restart enemy spawn");
                    // EnemySpawnAsync().Forget();
                    break;
                }
            }
        }

        private Vector3 GetCirclePosition(float angle)
        {
            float radius = 10;
            angle += Random.Range(0, 180);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            return new Vector3(x, y);
        }

        public void Dispose()
        {
            Utils.SafeCancelCancellationTokenSource(ref _spawnCts);
        }
    }
}