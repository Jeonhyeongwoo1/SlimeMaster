using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace SlimeMaster.InGame.Controller
{
    public class BossMonsterController : MonsterController
    {
        [SerializeField] private Animator _animator;
        
        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);

            _creatureStateType = CreatureStateType.Skill;
        }

        public override void Spawn(Vector3 spawnPosition, PlayerController player)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            _player = player;
        }

        public void UpdateState(CreatureStateType stateType) => _creatureStateType = stateType;

        private void UpdateAnimation(string animationName)
        {
            _animator.Play(animationName);
        }
        
        public override void UpdateStateAndAnimation(CreatureStateType stateType, string animationName)
        {
            UpdateState(stateType);
            UpdateAnimation(animationName);
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            base.TakeDamage(damage, attacker);

            float ratio = HP == 0 ? 0 : HP / _creatureData.MaxHp;
            GameManager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
        }
    }
}