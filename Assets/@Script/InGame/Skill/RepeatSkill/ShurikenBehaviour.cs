using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ShurikenBehaviour : Projectile
    {
        public int BounceCount { get; private set; }
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.localScale = Vector3.one * skillData.ScaleMultiplier;
            gameObject.SetActive(true);
            UpdateVelocity(direction, skillData.ProjSpeed);
            Invoke(nameof(Release), 3);
            Managers.Manager.I.Audio.Play(Sound.Effect, "Shuriken_Start");
        }
        
        public void Bounce(Vector3 direction, float speed)
        {
            BounceCount++;
            UpdateVelocity(direction, speed);
        }

        private void UpdateVelocity(Vector3 direction, float speed)
        {
            _rigidbody.velocity = direction * speed;
        }

        private void OnDisable()
        {
            BounceCount = 0;
        }
    }
}