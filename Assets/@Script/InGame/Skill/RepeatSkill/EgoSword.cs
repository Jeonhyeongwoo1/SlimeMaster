using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class EgoSword : RepeatSkill
    {
        protected override string HitSoundName => "EgoSword_Hit";

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected async override UniTask UseSkill()
        {
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = _skillData.AngleBetweenProj * (i - (_skillData.NumProjectiles - 1) / 2f);
                var direction = Quaternion.AngleAxis(angle, Vector3.forward) * _owner.GetDirection();
                string label = CurrentLevel == Const.MAX_SKILL_Level
                    ? "EgoSwordProjectile_Final"
                    : _skillData.PrefabLabel;
                
                GameObject prefab = GameManager.I.Resource.Instantiate(label);
                IGeneratable generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.Generate(_owner.Position, direction, _skillData, _owner);
                
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