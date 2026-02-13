using UnityEngine;

namespace WaterWalkEffectCreator
{
    [RequireComponent(typeof(PlayerMovement))]
    public class Player : Base
    {
        private PlayerMovement playerMovement;
        void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        protected override bool CheckIfInMovementState()
        {
            return playerMovement.IsMoving;
        }
    }
}