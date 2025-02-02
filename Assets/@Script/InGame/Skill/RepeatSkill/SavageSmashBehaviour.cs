using SlimeMaster.Data;
using SlimeMaster.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class SavageSmashBehaviour : Projectile
    {
        [SerializeField] private GameObject[] _normalParticleObjectArray;
        [SerializeField] private GameObject[] _finalParticleObjectArray;

        private CreatureController _owner;
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            GameObject[] particleObjectArray =
                Level == Const.MAX_SKILL_Level ? _finalParticleObjectArray : _normalParticleObjectArray;

            _owner = owner; 
            for (int i = 0; i < skillData.NumProjectiles; i++)
            {
                if (particleObjectArray.Length == i)
                {
                    break;
                }
                
                particleObjectArray[i].SetActive(true);
            }
            
            gameObject.SetActive(true);
            Invoke(nameof(Release), 2);
            Managers.Manager.I.Audio.Play(Sound.Effect, "SavageSmash_Start");
        }

        public override void OnChangedSkillData(SkillData skillData)
        {
            transform.localScale = Vector3.one * skillData.ScaleMultiplier;
        }

        private void Update()
        {
            transform.position = _owner.Position;
        }

        public override void Release()
        {
            base.Release();

            foreach (GameObject go in _normalParticleObjectArray)
            {
                go.SetActive(false);
            }
            
            foreach (GameObject go in _finalParticleObjectArray)
            {
                go.SetActive(false);
            }
        }
    }
}