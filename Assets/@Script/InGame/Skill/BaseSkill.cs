using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
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
        public float AccumulatedDamage { get; private set; }
        protected virtual string HitSoundName { get; set; } 
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

            if (_owner.CreatureType == CreatureType.Player)
            {
                Managers.Manager.I.Event.Raise(GameEventType.LearnSkill, SkillData);
            }
        }

        public abstract UniTask StartSkillLogicProcessAsync();
        public abstract void StopSkillLogic();
        protected abstract UniTask UseSkill();

        public virtual void OnChangedSkillData()
        {
            
        }
        
        protected virtual void OnHit(Collider2D collider, Projectile projectile)
        {
            if (Utils.TryGetComponentInParent(collider.gameObject, out CreatureController creature))
            {
                // Debug.Log(_owner.AttackDamage * _skillData.DamageMultiplier);
                float damage = _owner.AttackDamage * _skillData.DamageMultiplier;
                creature.TakeDamage(damage, _owner);
                AccumulatedDamage += damage;
                
                Managers.Manager.I.Audio.Play(Sound.Effect, HitSoundName);
            }
        }
    }
}