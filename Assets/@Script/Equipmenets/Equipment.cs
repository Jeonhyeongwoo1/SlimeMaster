using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;

namespace SlimeMaster.Equipmenets
{
    public class Equipment
    {
        public int Level => _level;
        public string DataId => _equipmentData.DataId;
        public EquipmentData EquipmentData => _equipmentData;
        public string UID => _uid;
        
        private EquipmentData _equipmentData;
        private bool _isEquipped;
        private int _level;
        private string _uid;
        
        public Equipment(EquipmentData equipmentData, bool isEquipped, int level, string uid)
        {
            _isEquipped = isEquipped;
            _equipmentData = equipmentData;
            _level = level;
            _uid = uid;
        }
        
        public bool IsEquippedByType(EquipmentType equipmentType)
        {
            return _equipmentData.EquipmentType == equipmentType && _isEquipped;
        }
        
        public bool IsEquipped()
        {
            return _isEquipped;
        }

        public void LevelUp()
        {
            _level++;
        }

        public bool IsPossibleMerge(Equipment equipment)
        {
            switch (_equipmentData.MergeEquipmentType1)
            {
                case MergeEquipmentType.Grade when equipment.EquipmentData.EquipmentGrade ==
                                                   (EquipmentGrade)System.Enum.Parse(typeof(EquipmentGrade), _equipmentData.MergeEquipment1):
                case MergeEquipmentType.ItemCode when _equipmentData.MergeEquipment1 == equipment.DataId:
                    return true;
            }
            
            switch (_equipmentData.MergeEquipmentType2)
            {
                case MergeEquipmentType.Grade when equipment.EquipmentData.EquipmentGrade ==
                                                   (EquipmentGrade)System.Enum.Parse(typeof(EquipmentGrade), _equipmentData.MergeEquipment2):
                case MergeEquipmentType.ItemCode when _equipmentData.MergeEquipment2 == equipment.DataId:
                    return true;
            }
            
            return false;
        }

        public EquipmentType GetEquipmentType()
        {
            return _equipmentData.EquipmentType;
        }
    }
}