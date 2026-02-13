using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PressAnims : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Press_Animation()
    {
        SfxManager.Instance.Play("UI_Click");
        animator.SetBool("isPress", true);
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return null;
        animator.SetBool("isPress", false);
    }

}
