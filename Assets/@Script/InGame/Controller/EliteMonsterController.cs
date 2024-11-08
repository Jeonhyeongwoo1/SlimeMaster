using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class EliteMonsterController : MonsterController
    {
        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);
        }

        public override void Spawn(Vector3 spawnPosition, PlayerController playerController)
        {
            base.Spawn(spawnPosition, playerController);
            transform.localScale = new Vector3(2f, 2f, 2f);
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            
            float ratio = _currentHp == 0 ? 0 : _currentHp / _creatureData.MaxHp;
            GameManager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
        }
    }
}
