using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class SkillBook
    {
        public List<BaseSkill> ActivateSkillList => _skillList.FindAll(v => v.IsLearn);
        
        private List<BaseSkill> _skillList = new();
        private CreatureController _owner;

        public SkillBook(CreatureController owner, List<SkillData> skillList)
        {
            _owner = owner;
            AddSkillList(skillList);   
        }

        private void AddSkillList(List<SkillData> skillList)
        {
            //미리 모든 스킬들을 초기화 시켜서 셋업
            foreach (SkillData skillData in skillList)
            {
                BaseSkill skill = null;
                AddSkill(skillData, ref skill);
                if (skill != null)
                {
                    _skillList.Add(skill);
                }
            }
        }

        public void UseAllSkillList()
        {
            _skillList.ForEach(v =>
            {
                UpgradeOrAddSkill(v.SkillData);
            });
        }
        
        public void UpgradeOrAddSkill(SkillData skillData)
        {
            var skill = _skillList.Find(v => v.SkillData.DataId == skillData.DataId);
            if (skill.IsMaxLevel)
            {
                Debug.LogWarning("is max level :" + skill.SkillData.DataId);
                return;
            }

            if (!skill.IsLearn)
            {
                skill.Learn();
                UseSkill(skill);
                return;
            }
            
            SkillData upgradeSkillData = GameManager.I.Data.SkillDict[skillData.DataId + 1];
            skill.UpdateSkillData(upgradeSkillData);
            UseSkill(skill);
        }
        
        private void AddSkill(SkillData skillData, ref BaseSkill baseSkill)
        {
            SkillType skillType = (SkillType)skillData.DataId;
            string skillName = $"{typeof(BaseSkill).Namespace}.{skillType}";
            Debug.Log(skillName);
            try
            {
                var skill = Activator.CreateInstance(Type.GetType(skillName)) as BaseSkill;
                if (skill == null)
                {
                    return;
                }

                skill.Initialize(skillType, _owner, skillData);
                baseSkill = skill;
            }
            catch (Exception e)
            {
                
            }
          
        }
        
        public void UseSkill(BaseSkill baseSkill)
        {
            baseSkill.StopSkillLogic();
            baseSkill.StartSkillLogicProcessAsync();
        }

        public void StopAllSkillLogic()
        {
            _skillList.ForEach(v=> v.StopSkillLogic());
        }

        public List<BaseSkill> GetRecommendSkillList(int recommendSkillCount = 3)
        {
            List<BaseSkill> list = ActivateSkillList.Count == Const.MAX_SKILL_COUNT
                ? ActivateSkillList.FindAll(v => v.CurrentLevel < Const.MAX_SKILL_Level)
                : _skillList.FindAll(v => v.CurrentLevel < Const.MAX_SKILL_Level);
            
            list.Shuffle();
            return list.Take(recommendSkillCount).ToList();
        }
    }
}