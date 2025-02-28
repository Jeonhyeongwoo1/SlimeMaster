using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Skill
{
    public class SkillBook
    {
        public List<BaseSkill> ActivateSkillList => _activateSkillList.FindAll(v => v.IsLearn);
        public int SupportSkillCount => _activateSupportSkillDataList.Count;
        public List<SupportSkill> ActivateSupportSkillDataList => _activateSupportSkillDataList;
        public List<SupportSkillData> CurrentSupportSkillDataList
        {
            get
            {
                if (_currentSupportSkillDataList == null || _currentSupportSkillDataList.Count == 0)
                {
                    _currentSupportSkillDataList =
                        Managers.Manager.I.Object.Player.SkillBook.GetRecommendSupportSkillDataList();
                }

                return _currentSupportSkillDataList;
            }
            set
            {
                _currentSupportSkillDataList ??= new List<SupportSkillData>();
                _currentSupportSkillDataList = value;
            }
        }

        private List<BaseSkill> _activateSkillList = new();
        private List<SupportSkill> _activateSupportSkillDataList = new();
        private CancellationTokenSource _useSequenceSkillCts;
        private CreatureController _owner;
        public List<SupportSkillData> _lockSupportSkillDataList = new(Const.SUPPORT_ITEM_USEABLECOUNT);
        private List<SupportSkillData> _currentSupportSkillDataList;

        public SkillBook(CreatureController owner, List<SkillData> skillList)
        {
            _owner = owner;
            AddSkillList(skillList);
        }

        private void AddSkillList(List<SkillData> skillList)
        {
            // foreach (var skillData in skillList)
            // {
            //     Debug.Log(skillData.DataId);
            // }

            //미리 모든 스킬들을 초기화 시켜서 셋업
            foreach (SkillData skillData in skillList)
            {
                BaseSkill skill = null;
                AddSkill(skillData, ref skill);
                if (skill != null)
                {
                    _activateSkillList.Add(skill);
                }
            }
        }

        public void UseAllSkillList(bool useRepeatSkill, bool useSequenceSkill, CreatureController targetCreature)
        {
            if (useRepeatSkill)
            {
                var repeatSkillList = _activateSkillList.FindAll(v => v is RepeatSkill);
                repeatSkillList.ForEach(v =>
                {
                    if (v.IsLearn)
                    {
                        UseSkill(v);
                    }
                });
            }

            if (useSequenceSkill)
            {
                UseSequenceSkill(targetCreature);
            }
        }

        private async void UseSequenceSkill(CreatureController targetCreature)
        {
            var sequenceSkillList = _activateSkillList.FindAll(v => v is SequenceSkill);
            if (sequenceSkillList.Count == 0)
            {
                return;
            }
            
            sequenceSkillList.ForEach(v=>
            {
                UpgradeOrAddSkill(v.SkillData);
                (v as SequenceSkill).SetTargetCreature(targetCreature);
            });
            
            _useSequenceSkillCts = new CancellationTokenSource();
            
            try
            {
                //잠깐의 유예기간을 준다.
                await UniTask.WaitForSeconds(1f, cancellationToken: _useSequenceSkillCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSequenceSkill)} log : {e}");
                return;
            }
            
            int index = 0;
            while (_useSequenceSkillCts != null && !_useSequenceSkillCts.IsCancellationRequested)
            {
                var sequenceSkill = sequenceSkillList[index];
                try
                {
                    await sequenceSkill.StartSkillLogicProcessAsync();
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSequenceSkill)} log : {e}");
                    break;
                }
                    
                index++;
                index %= sequenceSkillList.Count;
                
            }
        }

        public List<SupportSkillData> GetLevelSupportSkillDataList()
        {
            return _activateSupportSkillDataList.Select(x => x.SupportSkillData)
                .Where(x => x.SupportSkillType == SupportSkillType.LevelUp).ToList();
        }

        public List<SupportSkillData> GetMonsterKillSupportSkillDataList()
        {
            return _activateSupportSkillDataList.Select(x => x.SupportSkillData)
                .Where(x => x.SupportSkillType == SupportSkillType.MonsterKill).ToList();
        }

        public List<SupportSkillData> GetMonsterEliteSupportSkillDataList()
        {
            return _activateSupportSkillDataList.Select(x => x.SupportSkillData)
                .Where(x => x.SupportSkillType == SupportSkillType.EliteKill).ToList();
        }

        public bool IsLearnSkill(SkillData skillData)
        {
            BaseSkill baseSkill = _activateSkillList.Find(v => v.SkillData == skillData);
            return baseSkill == null ? false : baseSkill.IsLearn;
        }

        public bool IsExistSkillInActivateSkillList(BaseSkill skill)
        {
            return _activateSkillList.Exists(v => v == skill);
        }
        
        public void StopSequenceSkill()
        {
            Utils.SafeCancelCancellationTokenSource(ref _useSequenceSkillCts);
        }

        public int GetActivateSkillLevel(int skillId)
        {
            var skill = ActivateSkillList.Find(v => v.SkillData.DataId == skillId);
            if (skill == null)
            {
                Debug.LogError("Failed get skill id " + skillId);
                return 0;
            }

            return skill.CurrentLevel;
        }

        public bool TryResurrection(ref SupportSkill skillData)
        {
            var skillList = _activateSupportSkillDataList.FindAll(
                v => v.SupportSkillData.SupportSkillName == SupportSkillName.Resurrection);
            if (skillList.Count == 0)
            {
                return false;
            }

            skillList.Sort((a, b) => a.SupportSkillData.SupportSkillGrade.CompareTo(b.SupportSkillData.SupportSkillGrade));
            SupportSkill skill = skillList[^1];
            skillData = skill;
            return true;
        }

        public void UsedResurrectionSupportSkill(SupportSkill resurrectionSupportSkill)
        {
            _activateSupportSkillDataList.Remove(resurrectionSupportSkill);
        }

        public void UpgradeOrAddSkill(SkillData skillData)
        {
            if (skillData == null)
            {
                Debug.LogError("SkillData is null");
                return;
            }
            
            var skill = _activateSkillList.Find(v => v.SkillData.DataId == skillData.DataId);
            if (skill == null)
            {
                AddSkill(skillData, ref skill);
                _activateSkillList.Add(skill);
            }
            
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
            
            SkillData upgradeSkillData = Managers.Manager.I.Data.SkillDict[skillData.DataId + 1];
            skill.UpdateSkillData(upgradeSkillData);
            UseSkill(skill);
        }

        public void UpdateSkill(SupportSkillData supportSkill)
        {
            foreach (BaseSkill skill in _activateSkillList)
            {
                if (skill.SkillType.ToString() == supportSkill.SupportSkillName.ToString())
                {
                    skill.SkillData.NumBounce += supportSkill.NumBounce;
                    skill.SkillData.NumPenerations += supportSkill.NumPenerations;
                    skill.SkillData.ProjRange += supportSkill.ProjRange;
                    skill.SkillData.NumProjectiles += supportSkill.NumProjectiles;
                    skill.SkillData.RoatateSpeed += supportSkill.RoatateSpeed;
                    skill.SkillData.ProjectileSpacing += supportSkill.ProjectileSpacing;
                    skill.SkillData.Duration += supportSkill.Duration;
                    skill.SkillData.AttackInterval += supportSkill.AttackInterval;
                    skill.SkillData.ScaleMultiplier += supportSkill.ScaleMultiplier;
                    
                    skill.OnChangedSkillData();
                }
            }
        }

        public void AddSupportSkill(SupportSkillData supportSkillData)
        {
            var skill = new SupportSkill(supportSkillData);
            _activateSupportSkillDataList.Add(skill);

            Managers.Manager.I.GameContinueData.supportSkillDataList = _activateSupportSkillDataList.Select(x=> x.SupportSkillData).ToList();
        }

        public List<SupportSkillData> GetRecommendSupportSkillDataList()
        {
            /*
             *  총 4개의 리스트를 뽑느다.
             *   1. 고정 시킨 스킬을 먼저 get
             *   2. 남은 것들 중에서 랜덤으로 등급을 먼저 선출
             *   3. 선출된 등급을 기준으로 스킬을 get
             *   4. 스킬이 없으면 그 가장 아랫단계의 등급부터 다시 스킬을 찾아 스킬이 있으면 Get 없으면 reutrn
             */

            int count = Const.SUPPORT_ITEM_USEABLECOUNT;
            var recommendSkillDataList = new List<SupportSkillData>();
            recommendSkillDataList.AddRange(_lockSupportSkillDataList);

            count -= recommendSkillDataList.Count;
            for (int i = 0; i < count; i++)
            {
                SupportSkillGrade grade = GetRandomSupportSkillGrade();
                List<SupportSkillData> skillList = GetSupportSkillData(grade, recommendSkillDataList);
                if (skillList.Count > 0)
                {
                    int random = Random.Range(0, skillList.Count);
                    recommendSkillDataList.Add(skillList[random]);
                }
                else
                {
                    AddRecommendSkillData(0, ref recommendSkillDataList);            
                }
            }

            return recommendSkillDataList;
        }

        private void AddRecommendSkillData(int depth, ref List<SupportSkillData> skillDataList)
        {
            int length = System.Enum.GetNames(typeof(SupportSkillGrade)).Length;
            if (depth > length)
            {
                return;
            }

            var list = GetSupportSkillData((SupportSkillGrade)depth, skillDataList);
            if (list.Count > 0)
            { 
                int random = Random.Range(0, list.Count);
                skillDataList.Add(list[random]);
                return;
            }

            AddRecommendSkillData(depth + 1, ref skillDataList);
        }

        private List<SupportSkillData> GetSupportSkillData(SupportSkillGrade grade, List<SupportSkillData> addedSupportSkillDataList)
        {
            var skillDataList = Managers.Manager.I.Data.SupportSkillDataDict.Values.ToList();
            var excludeList =
                new HashSet<SupportSkillData>().Concat(_activateSupportSkillDataList.Select(c => c.SupportSkillData))
                    .Concat(_lockSupportSkillDataList)
                    .Concat(addedSupportSkillDataList);
            
            return skillDataList.Where(x => !excludeList.Contains(x) && x.SupportSkillGrade == grade).ToList();
        }

        private SupportSkillGrade GetRandomSupportSkillGrade()
        {
            float select = Random.value;
            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Legend])
            {
                return SupportSkillGrade.Legend;
            }

            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Epic])
            {
                return SupportSkillGrade.Epic;
            }

            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Rare])
            {
                return SupportSkillGrade.Rare;
            }

            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Uncommon])
            {
                return SupportSkillGrade.Uncommon;
            }

            return SupportSkillGrade.Common;
        }

        private SkillType GetSkillType(int id)
        {
            SkillType skillType = SkillType.None;
            if (id > (int)SkillType.BossSkill)
            {
                Array array = System.Enum.GetValues(typeof(SkillType));
                foreach (var obj in array)
                {
                    int value = (int)obj;
                    int range = 4 + value;
                    if (value <= id && id <= range)
                    {
                        return (SkillType)value;
                    }
                }
            }
            else
            {
                return (SkillType)id;
            }

            Debug.LogError("Failed get skill type : " + id);
            return SkillType.None;
        }
        
        private void AddSkill(SkillData skillData, ref BaseSkill baseSkill)
        {
            SkillType skillType = GetSkillType(skillData.DataId);
            if (skillType == SkillType.None)
            {
                Debug.LogWarning("SkillType is None / skill id : " + skillData.DataId);
                return;
            }
            
            string skillName = $"{typeof(BaseSkill).Namespace}.{skillType}";
            var skill = Activator.CreateInstance(Type.GetType(skillName)) as BaseSkill;
            if (skill == null)
            {
                Debug.LogWarning("skill is null : " + skillData.DataId);
                return;
            }

            skill.Initialize(skillType, _owner, skillData);
            baseSkill = skill;
        }
        
        public void UseSkill(BaseSkill baseSkill)
        {
            baseSkill.StopSkillLogic();
            baseSkill.StartSkillLogicProcessAsync().Forget();

            List<SupportSkill> supportSkillList = _activateSupportSkillDataList.FindAll(v =>
                v.SupportSkillData.SupportSkillName.ToString() == baseSkill.SkillType.ToString());
            
            foreach (SupportSkill supportSkill in supportSkillList)
            {
                UpdateSkill(supportSkill.SupportSkillData);
            }
        }

        public void StopAllSkillLogic()
        {
            _activateSkillList.ForEach(v=> v.StopSkillLogic());
            StopSequenceSkill();
        }

        public List<BaseSkill> GetRecommendSkillList(int recommendSkillCount = 3)
        {
            List<BaseSkill> list = ActivateSkillList.Count == Const.MAX_SKILL_COUNT
                ? ActivateSkillList.FindAll(v => v.CurrentLevel < Const.MAX_SKILL_Level)
                : _activateSkillList.FindAll(v => v.CurrentLevel < Const.MAX_SKILL_Level);
            
            list.Shuffle();
            return list.Take(recommendSkillCount).ToList();
        }
    }
}