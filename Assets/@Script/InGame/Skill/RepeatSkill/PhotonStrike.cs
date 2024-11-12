using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class PhotonStrike : RepeatSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            int projectileCount = _skillData.NumProjectiles;

            var list = GameManager.I.Object.GetNearestMonsterList(projectileCount);
            for (int i = 0; i < projectileCount; i++)
            {
                GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var generatable = prefab.GetComponent<IGeneratable>();
                Vector3 direction = _owner.GetDirection();
                var photonStrike = (generatable as PhotonStrikeBehaviour);
                if (list != null && list.Count > i)
                {
                    MonsterController monster = list[i];
                    if (monster.IsDeadState)
                    {
                        continue;
                    }
                    
                    photonStrike.SetTarget(monster);
                    direction = (monster.Position - _owner.Position).normalized;
                }

                generatable.OnHit = OnHit;
                generatable.Generate(_owner.Position, direction, _skillData, _owner);
                await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancellationToken: _skillLogicCts.Token);
            }
        }

        protected override void OnHit(Collider2D collider, Projectile projectile)
        {
            base.OnHit(collider, projectile);
            
            projectile.Release();
        }
    }
}