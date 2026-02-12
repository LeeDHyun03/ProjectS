using UnityEngine;

namespace WaterWalkEffectCreator
{
    [RequireComponent(typeof(MonsterMovement))]
    public class Monster : Base
    {
        private MonsterMovement monsterMovement;
        void Start()
        {
            monsterMovement = GetComponent<MonsterMovement>();
        }
        protected override bool CheckIfInMovementState()
        {
            return !monsterMovement.GetWaiting();
        }
    }
}