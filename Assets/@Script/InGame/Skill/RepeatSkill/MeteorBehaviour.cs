using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Skill;
using SlimeMaster.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Entity
{
    public class MeteorBehaviour : Projectile
    {
        private Vector3 _destinationPosition;
        private GameObject _shadowObject;
        private const float _spawnDistance = 30;
        private float _explosionRange = 1.5f;
        private Collider2D[] _colliderArray = new Collider2D[100];
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            /*
             *  위치: 카메라가 보이지 않는 오른쪽 상단에서 스폰 -> 목적지까지 이동
             *  범위: 캐릭터 주변 범위를 기준
             */

            int randomRange = 10;
            Vector3 destinationPosition = owner.Position + new Vector3(Random.Range(-randomRange, randomRange),
                Random.Range(-randomRange, randomRange));
            _destinationPosition = destinationPosition;

            float angle = 45 * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * _spawnDistance;
            float y = Mathf.Sin(angle) * _spawnDistance;
            spawnPosition = destinationPosition + new Vector3(x, y);
            direction = (destinationPosition - spawnPosition).normalized;
            
            transform.position = spawnPosition;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            transform.localScale = Vector3.one * skillData.ScaleMultiplier;
            gameObject.SetActive(true);

            _rigidbody.velocity = direction * skillData.ProjSpeed;

            GameObject shadowPrefab = GameManager.I.Resource.Instantiate(Const.MeteorShadow);
            shadowPrefab.transform.position = destinationPosition;
            shadowPrefab.SetActive(true);
            _shadowObject = shadowPrefab;
            GameManager.I.Audio.Play(Sound.Effect, "Meteor_Start");
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, _destinationPosition);
            float shadowScale = Mathf.Lerp(1, 2.2f, 1 - distance / 30);
            _shadowObject.transform.localScale = Vector3.one * shadowScale;
            if (distance < 0.3f)
            {
                Explosion();
            }
        }

        private void Explosion()
        {
            string hitEffectPrefabName = Level == Const.MAX_SKILL_Level ? "MeteorHitEffect_Final" : "MeteorHitEffect";
            GameObject prefab = GameManager.I.Resource.Instantiate(hitEffectPrefabName);
            prefab.SetActive(true);
            prefab.transform.position = _destinationPosition;
            
            int count = Physics2D.OverlapCircleNonAlloc(_destinationPosition, _explosionRange, _colliderArray,
                Layer.BossAndMonster);

            for (int i = 0; i < count; i++)
            {
                Collider2D collider = _colliderArray[i];
                OnHit?.Invoke(collider, this);
            }
            
            Release();
        }

        public override void Release()
        {
            base.Release();
            GameManager.I.Pool.ReleaseObject(_shadowObject.name, _shadowObject);
            _shadowObject.SetActive(false);
        }
    }
}