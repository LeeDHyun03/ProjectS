using System;
using System.Collections;
using UnityEngine;

public class Alter : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public event Action<Vector3> OnBossSpawnTriggered;

    private bool alreadySpawnedBossMonster = false;

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
        if (alreadySpawnedBossMonster) return;
        if (GameManager.Instance.currentStage < 5) return;
        collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter character);
        if (character == null) return;
        if (!character.isInteracting) return;
        alreadySpawnedBossMonster = true;
        animator.SetTrigger("Activate");
        OnBossSpawnTriggered?.Invoke(transform.position);
        StartCoroutine(OnBossSpawnEnded());
    }

    public IEnumerator OnBossSpawnEnded()
    {
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("Deactivate");
        StopAllCoroutines();
    }
}
