using System;
using UnityEngine;

public class MCSeed : MonoBehaviour
{
    public event Action<bool> OnGrewTrigger;
    Collider2D myWater;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (myWater)
            return;

        if (other.name == "Water")
        {
            Debug.Log("물 들어옴");
            OnGrewTrigger?.Invoke(true);
            myWater = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != myWater)
            return;

        if (other.name == "Water")
        {
            Debug.Log("물 나감");
            OnGrewTrigger?.Invoke(false);
            myWater = null;
        }
    }
}