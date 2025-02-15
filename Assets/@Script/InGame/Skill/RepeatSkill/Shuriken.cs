using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Controller;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class Shuriken : RepeatSkill
    {
        protected override string HitSoundName => "Shuriken_Hit";

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                var list = Managers.Manager.I.Object.GetNearestMonsterList(projectileCount);
                if (list == null)
                {
                    break;
                }

                Vector3 direction = (list[i].transform.position - _owner.Position).normalized;
                GameObject prefab = Managers.Manager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var shootable = prefab.GetComponent<IGeneratable>();
                shootable.OnHit = OnHit;
                shootable.Generate(_owner.Position, direction, _skillData, _owner);
                
                await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancelImmediately: true);
            }
        }
        
        protected override void OnHit(Collider2D collider, Projectile projectile)
        {
            if (collider.TryGetComponent(out MonsterController monster))
            {
                monster.TakeDamage(_owner.AttackDamage * _skillData.DamageMultiplier, _owner);

                var energyBolt = projectile as ShurikenBehaviour;
                if (energyBolt != null)
                {
                    if (energyBolt.BounceCount >= _skillData.NumBounce)
                    {
                        projectile.Release();
                        return;
                    }
                    
                    Vector3 direction = projectile.Velocity.normalized;
                    List<Transform> list =
                        Managers.Manager.I.Object.GetMonsterAndBossTransformListInFanShape(_owner.transform, direction);
                    if (list == null)
                    {
                        return;
                    }
                    
                    direction = (list[0].transform.position - _owner.transform.position).normalized;
                    energyBolt.Bounce(direction, _skillData.BounceSpeed);
                }
            }
        }
    }
}