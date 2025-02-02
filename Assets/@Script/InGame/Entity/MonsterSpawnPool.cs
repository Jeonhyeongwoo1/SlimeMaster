using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Entity
{
    public class MonsterSpawnPool
    {
        private float _spawnInterval;
        private CancellationTokenSource _spawnCts;

        public MonsterSpawnPool()
        {
            AddEvent();
        }

        private void AddEvent()
        {
            Managers.Manager.I.Event.AddEvent(GameEventType.GameOver, OnGameOver);
        }

        private void OnGameOver(object value)
        {
            Utils.SafeCancelCancellationTokenSource(ref _spawnCts);
        }
        
        public void StopMonsterSpawn()
        {
            Utils.SafeCancelCancellationTokenSource(ref _spawnCts);
        }

        public async UniTask SpawnMonsterAsync(float spawnInterval, List<int> monsterIdList, int onceSpawnCount,
            float firstMonsterSpanRate, List<int> eleteIdList, List<int> bossIdList)
        {
            _spawnCts = new CancellationTokenSource();

            if (bossIdList != null && bossIdList.Count > 0)
            {
                for (int i = 0; i < bossIdList.Count; i++)
                {
                    RaiseSpawnMonster(i, bossIdList[i], typeof(BossMonsterController));
                }
                
                Managers.Manager.I.Event.Raise(GameEventType.SpawnedBoss);
            }

            if (eleteIdList != null && eleteIdList.Count > 0)
            {
                for (var i = 0; i < eleteIdList.Count; i++)
                {
                    RaiseSpawnMonster(i, eleteIdList[i], typeof(EliteMonsterController));
                }
            }
            
            while (true)
            {
                try
                {
                    await UniTask.WaitForSeconds(spawnInterval, cancellationToken: _spawnCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"{nameof(SpawnMonsterAsync)} error {e}");
                    break;
                }
                
                Debug.Log("Spawn");
                for (int i = 0; i < onceSpawnCount; i++)
                {
                    float select = Random.value;
                    int monsterId =
                        monsterIdList[select <= firstMonsterSpanRate ? 0 : Random.Range(0, monsterIdList.Count)];
                    RaiseSpawnMonster(i, monsterId, typeof(MonsterController));
                }
            }
        }

        private void RaiseSpawnMonster(int index, int id, Type monsterType)
        {
            float angle = 360 / (index + 1);
            var spawnPosition = GetCirclePosition(angle);
            SpawnObjectData spawnObjectData = new SpawnObjectData();
            spawnObjectData.spawnPosition = spawnPosition;
            spawnObjectData.id = id;
            spawnObjectData.Type = monsterType;
            Managers.Manager.I.Event.Raise(GameEventType.SpawnMonster, spawnObjectData);
        }

        private Vector3 GetCirclePosition(float angle)
        {
            float radius = Random.Range(20, 50);
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