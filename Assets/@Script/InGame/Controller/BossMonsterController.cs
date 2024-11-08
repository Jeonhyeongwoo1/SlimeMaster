using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public enum BossStateType
    {
        None,
        Idle,
        Move,
        Skill,
        Dead
    }
    
    public class BossMonsterController : MonsterController
    {

        [SerializeField] private BossStateType _bossStateType;

        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);

            _bossStateType = BossStateType.Skill;
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            
            float ratio = _currentHp == 0 ? 0 : _currentHp / _creatureData.MaxHp;
            GameManager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
        }
    }
}