using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public abstract class BaseSkill
    {
        public int CurrentLevel => _currentLevel;
        public SkillData SkillData => _skillData;
        public bool IsMaxLevel => _currentLevel == Const.MAX_SKILL_Level;
        public bool IsLearn => _currentLevel > 0;
        public SkillType SkillType => _skillType;
        
        protected SkillData _skillData;
        protected SkillType _skillType;
        protected CreatureController _owner;
        private int _currentLevel;

        public virtual void Initialize(SkillType skillType, CreatureController owner, SkillData skillData)
        {
            _skillType = skillType;
            _owner = owner;
            _skillData = skillData;
            _currentLevel = 0;
        }

        public void UpdateSkillData(SkillData skillData)
        {
            _skillData = skillData;
            _currentLevel++;
        }

        public void Learn()
        {
            _currentLevel = 1;
        }

        public abstract UniTask StartSkillLogicProcessAsync(CancellationTokenSource cts = null);
        public abstract void StopSkillLogic();
        protected abstract UniTask UseSkill();
        
        protected virtual void OnHit(Collider2D collider, Projectile projectile)
        {
            if (Utils.TryGetComponentInParent(collider.gameObject, out CreatureController creature))
            {
                // Debug.Log(_owner.AttackDamage * _skillData.DamageMultiplier);
                creature.TakeDamage(_owner.AttackDamage * _skillData.DamageMultiplier);
                // monster.TakeDamaged(100);
            }
        }
    }
}