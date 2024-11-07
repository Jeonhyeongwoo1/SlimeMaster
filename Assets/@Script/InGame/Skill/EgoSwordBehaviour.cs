using Script.InGame.Skill;
using SlimeMaster.Data;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class EgoSwordBehaviour : Projectile
    {
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            gameObject.SetActive(true);
            _rigidbody.velocity = direction * skillData.ProjSpeed;
            Invoke(nameof(Sleep), 3);
        }
    }
}