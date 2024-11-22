using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class PoisonField : RepeatSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);        
        }

        protected override async UniTask UseSkill()
        {
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = _skillData.AngleBetweenProj * i;
                Vector3 position = Quaternion.AngleAxis(angle, Vector3.forward) * _owner.GetDirection() *
                                   _skillData.ProjRange;

                string prefabName = CurrentLevel == 6 ? "PoisonFieldProjectile_Final" : "PoisonFieldProjectile";
                GameObject prefab = GameManager.I.Resource.Instantiate(prefabName);
                var generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.Level = CurrentLevel;
                Vector3 spawnPosition = _owner.Position + position;
                generatable.Generate(spawnPosition, Vector3.zero, _skillData, _owner);

                try
                {
                    await UniTask.WaitForSeconds(_skillData.AttackInterval, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                    StopSkillLogic();
                    StartSkillLogicProcessAsync().Forget();
                    return;
                }
            }

            try
            {
                await UniTask.WaitForSeconds(_skillData.Duration, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
                StartSkillLogicProcessAsync().Forget();
            }
        }
    }
}