using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PressAnims : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Press_Animation()
    {
        animator.SetBool("isPress", true);
        // 사운드 추가
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return null;
        animator.SetBool("isPress", false);
    }

}
