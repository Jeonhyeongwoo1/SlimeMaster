using System;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
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
            Invoke(nameof(Sleep), 2);
        }

        private void Update()
        {
            transform.position = _owner.Position;
        }

        public override void Sleep()
        {
            base.Sleep();

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