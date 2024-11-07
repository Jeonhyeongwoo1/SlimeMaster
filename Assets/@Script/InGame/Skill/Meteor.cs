using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class Meteor : RepeatSkill
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
                
                Vector3 direction = Vector3.zero;
                string prefabName = CurrentLevel == Const.MAX_SKILL_Level
                    ? "MeteorProjectile_Final"
                    : "MeteorProjectile";
                GameObject prefab = GameManager.I.Resource.Instantiate(prefabName);
                var shootable = prefab.GetComponent<IGeneratable>();
                shootable.OnHit = OnHit;
                shootable.Generate(_owner.Position, direction, _skillData, _owner);

                await UniTask.WaitForSeconds(_skillData.AttackInterval, cancelImmediately: true);
            }
        }
    }
}