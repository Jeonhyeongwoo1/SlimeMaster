using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Entity;
using UnityEngine;

namespace SlimeMaster.InGame.Manager
{
    public class StageManager
    {
        private PlayerController _player;
        private StageData _stageData;
        private WaveData _waveData;
        private MonsterSpawnPool _monsterSpawnPool;
        private int _waveIndex;
        
        public void Initialize(int stageIndex, PlayerController playerController)
        {
            _player = playerController;
            _monsterSpawnPool = new MonsterSpawnPool(_player);
            SetStageData(stageIndex);
        }

        private void SetStageData(int index)
        {
            _stageData = GameManager.I.Data.StageDict[index];
            _waveData = _stageData.WaveArray[_waveIndex];
            _waveIndex++;
            
            MakeStage();
        }

        private void MakeStage()
        {
            //맵, 몬스터 스폰, Wave
            GameObject map = GameManager.I.Resource.Instantiate(_stageData.MapName, false);
            map.SetActive(true);
        }

        public void StartStage()
        {
            _monsterSpawnPool.StartMonsterSpawn(_waveData.SpawnInterval, _waveData.MonsterId, _waveData.OnceSpawnCount,
                _waveData.FirstMonsterSpawnRate);
        }
    }
}