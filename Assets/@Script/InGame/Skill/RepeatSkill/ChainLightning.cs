using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Controller;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ChainLightning : RepeatSkill
    {
        private List<IGeneratable> _list = new();
        protected override string HitSoundName => "ChainLightning_Hit";
        
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            if (_list.Count > 0)
            {
                _list.ForEach(v=>
                {
                    if (v.ProjectileMono.gameObject)
                    {
                        v.Release();
                    }
                });
                _list.Clear();   
            }
        }

        private void GenerateChainLighting(int j, int maxCount, float minDistance, float maxDistance,
            Vector3 targetPosition, ref List<MonsterController> chainedMonsterList)
        {
            if (j == maxCount)
            {
                return;
            }
            
            List<MonsterController> monsterList =
                Managers.Manager.I.Object.ActivateMonsterList
                    .OrderBy(a => (targetPosition - a.transform.position).sqrMagnitude).ToList();
            if (monsterList.Count == 0)
            {
                Debug.Log("monsterList count 0");
                return;
            }

            bool isChainedMonster = false;
            foreach (MonsterController monster in monsterList)
            {
                float distance = Vector3.Distance(targetPosition, monster.Position);
                // if (distance > maxDistance)
                // {
                //     break;
                // }
                
                if (minDistance <= distance && distance <= maxDistance)
                {
                    if (chainedMonsterList.Contains(monster))
                    {
                        continue;
                    }

                    isChainedMonster = true;
                    chainedMonsterList.Add(monster);
                    break;
                }
            }

            if (isChainedMonster)
            {
                GenerateChainLighting(++j, _skillData.NumBounce, minDistance, maxDistance,
                    chainedMonsterList[^1].Position, ref chainedMonsterList);
            }
        }

        protected override async UniTask UseSkill()
        {
            var nearestMonsterList = Managers.Manager.I.Object.GetNearestMonsterList(_skillData.NumProjectiles);
            if (nearestMonsterList == null)
            {
                return;
            }
            
            foreach (MonsterController monsterController in nearestMonsterList)
            {
                Debug.Log(monsterController.Position);
            }
         
            _list.Clear();   
            for (int i = 0; i < nearestMonsterList.Count; i++)
            {
                float minDistance = 0;
                float maxDistance = _skillData.BounceDist + 1;
                var chainedMonsterList = new List<MonsterController>();
                chainedMonsterList.Add(nearestMonsterList[i]);
                GenerateChainLighting(0, _skillData.NumBounce - 1, minDistance, maxDistance,
                    nearestMonsterList[i].Position, ref chainedMonsterList);
                
                for (var j = 0; j < chainedMonsterList.Count; j++)
                {
                    GameObject prefab = Managers.Manager.I.Resource.Instantiate(_skillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    (generatable as ChainLightningBehaviour).EndPosition = chainedMonsterList[j].Position;
                    Vector3 spawnPosition = j == 0 ? _owner.Position : chainedMonsterList[j - 1].Position;
                    generatable.Generate(spawnPosition, Vector3.zero, _skillData, _owner);
                    _list.Add(generatable);
                }
            }
            
            try
            {
                await UniTask.WaitForSeconds(0.4f, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
            }
            
            _list.ForEach(v=> v.Release());
        }
    }
}