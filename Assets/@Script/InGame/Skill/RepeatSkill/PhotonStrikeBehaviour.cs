using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Controller;
using SlimeMaster.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Skill
{
    public class PhotonStrikeBehaviour : Projectile
    {
        [SerializeField] private MonsterController _monster;
        private float _speed;
        private Vector3 _direction;
        private float _range;

        public void SetTarget(MonsterController monster)
        {
            // Debug.Log("monster : " + monster + " " + monster.IsDead + " instance :" + monster.transform.GetInstanceID());
            _monster = monster;
        }
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData,
            CreatureController owner)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            _rigidbody.velocity = direction * skillData.ProjSpeed;
            _speed = skillData.ProjSpeed;
            // _range = skillData.RecognitionRange;
            _range = 15;
            CancelInvoke();
            Invoke(nameof(Release), 4);
            Managers.Manager.I.Audio.Play(Sound.Effect, "PhotonStrike_Start");
        }

        private void SetTargetMonster(float speed)
        {
            _monster = null;
            var monsterList = Managers.Manager.I.Object.GetNearestMonsterList(10);
            if (monsterList != null && monsterList.Count > 0)
            {
                var list = monsterList.FindAll(v => Vector2.Distance(v.Position, transform.position) < _range && !v.IsDeadState).ToList();
                if (list.Count > 0)
                {
                    _monster = list[Random.Range(0, list.Count)];
                    _direction = (_monster.Position - transform.position).normalized;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_monster == null || _monster.IsDeadState)
            {
                return;
            }
            
            Vector2 currentDirection = _rigidbody.velocity.normalized;
            _direction = (_monster.Position - transform.position).normalized;
            Vector2 lerp = Vector2.Lerp(currentDirection, _direction, Time.fixedDeltaTime * _speed);
            _rigidbody.velocity = lerp.normalized * _speed;
        }

        private void Update()
        {
            if (_monster == null || _monster.IsDeadState)
            {
                SetTargetMonster(_speed);
            }
        }
    }
}