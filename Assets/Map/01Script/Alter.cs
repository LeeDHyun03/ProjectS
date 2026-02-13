using System;
using UnityEngine;

public class Alter : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public event Action<Vector3> OnBossSpawnTriggered;

    private void OnEnable()
    {
        if(GameManager.Instance != null)
        {
            OnBossSpawnTriggered += GameManager.Instance.SpawnBossMonster;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            OnBossSpawnTriggered -= GameManager.Instance.SpawnBossMonster;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GameManager.Instance.currentStage < 5) return;
        collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter character);
        if (character == null) return;
        if (!character.isInteracting) return;

        animator.SetTrigger("Activate");
    }

    public void OnBossSpawnEnded()
    {
        animator.SetTrigger("Deactivate");
    }
}
