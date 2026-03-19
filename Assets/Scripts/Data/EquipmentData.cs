using UnityEngine;
using GachaRPG;

namespace GachaRPG
{
    // Template asset — defines a gear piece archetype (not an instance with rolled substats)
    [CreateAssetMenu(menuName = "GachaRPG/Equipment", fileName = "NewEquipment")]
    public class EquipmentData : ScriptableObject
    {
        [Header("Identity")]
        public string itemName;
        public GearSlot slot;
        public GearTier tier;
        public GearSet  set;

        [Header("Main Stat")]
        public StatType mainStat;
        public float    mainStatValue; // flat value (e.g. 400 ATK, or 0.08 for 8% ATK%)

        [Header("Drop Info")]
        [Tooltip("Which Hunt boss drops this gear set")]
        public string sourceHunt;
    }
}
