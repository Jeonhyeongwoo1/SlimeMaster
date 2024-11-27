using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class EliteMonsterController : MonsterController
    {
        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);
        }

        public override void Spawn(Vector3 spawnPosition, PlayerController player)
        {
            base.Spawn(spawnPosition, player);
            transform.localScale = new Vector3(2f, 2f, 2f);
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            base.TakeDamage(damage, attacker);

            float ratio = HP == 0 ? 0 : HP / _creatureData.MaxHp;
            Managers.Manager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
        }
    }
}
