using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class StormBlade : RepeatSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            int count = 7;
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(45 + 45 * i, Vector3.forward) * _owner.GetDirection();
                for (int j = 0; j < projectileCount; j++)
                {
                    float angle = SkillData.AngleBetweenProj * (j - (SkillData.NumProjectiles - 1) / 2f);
                    Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                    GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    generatable.Generate(_owner.Position, direction, _skillData, _owner);
                }
                
                try
                {
                    await UniTask.WaitForSeconds(_skillData.AttackInterval, cancellationToken: _skillLogicCts.Token);
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
}