using UnityEngine;
using System;

public class PZConfinerTrigger : MonoBehaviour
{
    public event Action OnConfinerTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Confiner")
        {
            OnConfinerTrigger?.Invoke();
        }
    }
}
