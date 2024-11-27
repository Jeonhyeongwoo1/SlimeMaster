using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Entity;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class BasicAttack : SequenceSkill
    {
        protected override async UniTask UseSkill()
        {
            if (_targetCreature == null)
            {
                return;
            }

            Vector3 _offset = new Vector3(0, 0.25f, 0);
            GameObject obj = Managers.Manager.I.Resource.Instantiate(nameof(SkillRange));
            obj.transform.SetParent(_owner.transform);
            obj.transform.localPosition = _offset;
            obj.SetActive(true);
            var skillRange = obj.GetComponent<SkillRange>();

            float radius = _skillData.ProjRange + _targetCreature.CircleColliderRadius;
            float duration =  skillRange.SetCircle(radius);

            await UniTask.WaitForSeconds(duration);
            float distance = Vector3.Distance(_owner.Position, _targetCreature.Position);
            if (distance <= radius)
            {
                _targetCreature.TakeDamage(_owner.AttackDamage * _skillData.DamageMultiplier, _owner);
            }
            
            _owner.UpdateStateAndAnimation(CreatureStateType.Skill, "Attack");
            await UniTask.WaitForSeconds(0.5f, cancellationToken: _skillLogicCts.Token);

            // Hit Effect
            GameObject HitEffectObj = Managers.Manager.I.Resource.Instantiate(Const.BossSmashHitEffect);
            HitEffectObj.transform.SetParent(_owner.transform);
            HitEffectObj.transform.localPosition = Vector3.zero;
            HitEffectObj.transform.localScale = Vector3.one * (radius * 0.3f);
            HitEffectObj.SetActive(true);
            
            try
            {
                await UniTask.WaitForSeconds(0.7f, cancellationToken: _skillLogicCts.Token);
            
                Managers.Manager.I.Pool.ReleaseObject(Const.BossSmashHitEffect, HitEffectObj);
            
                await UniTask.WaitForSeconds(_skillData.AttackInterval, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
            }
        }
    }
}