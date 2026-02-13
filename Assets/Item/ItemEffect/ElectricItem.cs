using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricItem : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private int electricMaxCount = 3;

    [SerializeField] private float controlConstant = 1.0f;
    private float jealousy;
    private int electricCurrentCount = 0;

    private Coroutine electricRoutine = null;
    private Monster currentMonster = null;

    private HashSet<Monster> hitMonsters = new HashSet<Monster>();
    private List<Vector3> pathPoints = new List<Vector3>();
    private string conductorItemId = "IT_035_Item035";
    private string contagionItemId = "IT_034_Item034";

    private bool isProcessing = false;

    public void SetJealousy(float jealousy) => this.jealousy = jealousy;

    private void Awake()
    {
        if (circleCollider != null) circleCollider.enabled = false;
    }

    private void OnEnable()
    {
        combat.OnAttackSuccessed += StartElectric;
    }

    private void OnDisable()
    {
        combat.OnAttackSuccessed -= StartElectric;
        StopAllCoroutines();
    }

    private void StartElectric(Monster monster, PlayerCombat.AttackType attackType)
    {
        if (isProcessing) return;

        bool isConductorTrigger = (attackType == PlayerCombat.AttackType.NormalAttacked &&
            PlayerItemStatController.FindItem(conductorItemId));
        bool isContagionTrigger = (attackType == PlayerCombat.AttackType.SpecialAttacked &&
            PlayerItemStatController.FindItem(contagionItemId));

        if (isConductorTrigger || isContagionTrigger)
            SpawnElectric(monster);
    }

    private void SpawnElectric(Monster monster)
    {
        pathPoints.Clear();
        hitMonsters.Clear();
        electricCurrentCount = 0;

        currentMonster = monster;

        isProcessing = true;
        ElectricAttack();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isProcessing || electricCurrentCount >= electricMaxCount) return;

        if (collision.TryGetComponent<Monster>(out Monster nextMonster))
        {
            if (hitMonsters.Contains(nextMonster)) return;

            if (electricRoutine != null) StopCoroutine(electricRoutine);

            currentMonster = nextMonster;
            ElectricAttack();
        }
    }

    private void ElectricAttack()
    {
        electricCurrentCount++;
        hitMonsters.Add(currentMonster);
        Debug.Log($"hitMonstersCount: {hitMonsters.Count}");
        Vector3 pos = currentMonster.transform.position;
        pos.z += 3;
        pathPoints.Add(pos);

        currentMonster.TakeDamage(jealousy * controlConstant);

        if (electricCurrentCount >= electricMaxCount)
        {
            ExecuteEffect();
        }
        else
        {
            transform.position = currentMonster.transform.position;
            circleCollider.enabled = false;
            circleCollider.enabled = true;

            if (electricRoutine != null) StopCoroutine(electricRoutine);
            electricRoutine = StartCoroutine(ElectricTimeout());
        }
    }

    private void ExecuteEffect()
    {
        if (electricRoutine != null) StopCoroutine(electricRoutine);
        circleCollider.enabled = false;
        isProcessing = false;
        GameObject effectObj = ObjectPooler.Instance.SpawnFromPool("ElectricEffect", Vector3.zero, Quaternion.identity);
        if (effectObj.TryGetComponent(out ElectricEffect effect))
        {
            effect.Draw(new List<Vector3>(pathPoints)); 
        }

        electricCurrentCount = 0;
    }

    IEnumerator ElectricTimeout()
    {
        yield return new WaitForSeconds(0.1f);
        ExecuteEffect();
    }
}