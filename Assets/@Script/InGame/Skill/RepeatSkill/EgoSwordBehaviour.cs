using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
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
            GameManager.I.Audio.Play(Sound.Effect, "EgoSword_Start");
            Invoke(nameof(Release), 3);
        }
    }
}