using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class StormBladeBehaviour : Projectile
    {
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.localScale = Vector3.one * skillData.ScaleMultiplier;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            gameObject.SetActive(true);
            _rigidbody.velocity = direction * skillData.ProjSpeed;
            Invoke(nameof(Release), skillData.Duration);
            Managers.Manager.I.Audio.Play(Sound.Effect, "StormBlade_Start");
        }

        public override void OnChangedSkillData(SkillData skillData)
        {
            CancelInvoke(nameof(Release));
            Invoke(nameof(Release), skillData.Duration);
        }
    }
}