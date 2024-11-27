using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;
using SlimeMaster.Managers;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ChainLightningBehaviour :Projectile
    {
        public Vector3 EndPosition;
        
        [SerializeField] private ParticleSystem _mainParticle;
        [SerializeField] private ParticleSystem _subParticle;
        
        private RaycastHit2D[] _hitArray = new RaycastHit2D[10];
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData,
            CreatureController owner)
        {
            transform.position = spawnPosition;
            transform.localScale = Vector3.one * skillData.ScaleMultiplier;
            
            var main = _mainParticle.main;
            var main2 = _subParticle.main;

            float distance = Vector3.Distance(transform.position, EndPosition);
            main.startSizeX = main2.startSizeX = distance;
            main.startSizeY = main2.startSizeY = 8;

            direction = (EndPosition - spawnPosition).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x);
            main.startRotation = main2.startRotation = (angle * -1f);

            int rayDistance = (int)distance;
            int hitCount =
                Physics2D.BoxCastNonAlloc(spawnPosition, Vector2.one, 0, direction, _hitArray,
                    rayDistance, Layer.BossAndMonster);
                    
            for (int j = 0; j < hitCount; j++)
            {
                RaycastHit2D hit = _hitArray[j];
                OnHit.Invoke(hit.collider, this);
            }
            
            Managers.Manager.I.Audio.Play(Sound.Effect, "ChainLightning_Start");
            gameObject.SetActive(true);
        }
    }
}