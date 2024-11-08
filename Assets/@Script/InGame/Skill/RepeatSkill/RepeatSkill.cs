using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public abstract class RepeatSkill : BaseSkill
    {
        protected CancellationTokenSource _skillLogicCts;
        
        public override async UniTaskVoid StartSkillLogicProcessAsync()
        {
            _skillLogicCts = new CancellationTokenSource();
            var token = _skillLogicCts.Token;
            while (_skillLogicCts != null || !_skillLogicCts.IsCancellationRequested)
            {
                try
                {
                    await UniTask.WaitForSeconds(_skillData.CoolTime, cancellationToken: token);
                    await UseSkill();
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
                    break;
                }
            }
        }

        protected virtual void OnHit(Collider2D collider, Projectile projectile)
        {
            if (Utils.TryGetComponentInParent(collider.gameObject, out CreatureController creature))
            {
                creature.TakeDamage(1);
                // Debug.Log(_owner.AttackDamage * _skillData.DamageMultiplier);
                // creature.TakeDamage(_owner.AttackDamage * _skillData.DamageMultiplier);
                // monster.TakeDamaged(100);
            }
        }
        
        protected abstract UniTask UseSkill();
    }
}