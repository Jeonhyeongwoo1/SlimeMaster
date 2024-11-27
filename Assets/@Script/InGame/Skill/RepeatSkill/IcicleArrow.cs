using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class IcicleArrow : RepeatSkill
    {
        protected override string HitSoundName => "IcicleArrow_Hit";

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override void OnHit(Collider2D collider, Projectile projectile)
        {
            base.OnHit(collider, projectile);

            if (projectile is IcicleArrowBehaviour icicleArrowBehaviour)
            {
                icicleArrowBehaviour.PenerationCount--;
            }
        }

        protected async override UniTask UseSkill()
        {
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = _skillData.AngleBetweenProj * (i - (_skillData.NumProjectiles - 1) / 2f);
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * _owner.GetDirection();
                GameObject prefab = Managers.Manager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var shootable = prefab.GetComponent<IGeneratable>();
                shootable.OnHit = OnHit;
                shootable.Generate(_owner.Position, direction, _skillData, _owner);
            }
        }
    }
}