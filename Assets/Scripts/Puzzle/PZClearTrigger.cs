using System;
using UnityEngine;

public class PZClearTrigger : MonoBehaviour
{
    public event Action OnClear;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Flag")
        {
            OnClear?.Invoke();
        }

    }
}
