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
            Debug.Log(playerMovement.IsMoving);
            return playerMovement.IsMoving;
        }
    }
}