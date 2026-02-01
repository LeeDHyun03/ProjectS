using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class MonsterDetection : MonoBehaviour
{
    public enum DetectionState
    {
        Attack,
        Chase,
        Cognizance,
        None
    }

    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float chaseInRange = 15f;
    [SerializeField] private float chaseOutRange = 20f;
    [SerializeField] private float cognizanceRange = 25f;

    [SerializeField] private DetectionState currentDetectionState;

    [SerializeField] private CircleCollider2D detectionCollider;

    public event Action<DetectionState, Character> OnDetectionStateChanged;

    private float distanceToTarget = 0;
    private Character targetCharacter = null;
    private bool isPlayerSide = false;

    public void SetupDectectionRange(
        float newAttackRange,
        float newChaseInRange,
        float newChaseOutRange,
        float newCognizanceRange)
    {
        attackRange = newAttackRange;
        chaseInRange = newChaseInRange;
        chaseOutRange = newChaseOutRange;
        cognizanceRange = newCognizanceRange;
    }

    public void SetIsPlayerSide(bool newIsPlayerSide)
    {
        isPlayerSide = newIsPlayerSide;
    }

    private void Start()
    {
        detectionCollider.radius = cognizanceRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == gameObject) return;
        if (!isPlayerSide)
        {
            collision.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);
            if (player == null)
                return;
            else targetCharacter = player;
        }
        else
        {
            collision.TryGetComponent<Monster>(out Monster monster);
            if (monster == null)
                return;
            else targetCharacter = monster;
        }
        detectionCollider.enabled = false;
    }

    void Update()
    {
        if (targetCharacter == null && currentDetectionState != DetectionState.None)
        {
            currentDetectionState = DetectionState.None;
            OnDetectionStateChanged?.Invoke(currentDetectionState, null);
            return;
        }
        if (targetCharacter != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, targetCharacter.transform.position);
            DetectionState previousState = currentDetectionState;
            if (distanceToTarget <= attackRange)
            {
                currentDetectionState = DetectionState.Attack;
            }
            else if (distanceToTarget <= chaseInRange && chaseInRange > 0)
            {
                currentDetectionState = DetectionState.Chase;
            }
            else if (distanceToTarget <= cognizanceRange && cognizanceRange > 0)
            {
                currentDetectionState = DetectionState.Cognizance;
            }
            else if(distanceToTarget > chaseOutRange && chaseOutRange > 0)
            {
                currentDetectionState = DetectionState.None;
                targetCharacter = null;
                detectionCollider.enabled = true;
            }
            if (previousState != currentDetectionState)
            {
                OnDetectionStateChanged?.Invoke(currentDetectionState, targetCharacter);
            }
        }
    }
}