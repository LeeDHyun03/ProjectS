using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private Image sword;
    [SerializeField] private Animator animator;


    public void Start_Anims()
    {
        panel.gameObject.SetActive(true);
        animator.SetBool("isStart", true);
    }
    

    public void DisableSword()
    {
        sword.gameObject.SetActive(false);
    }

    // 애니메이션 이벤트로 호출
    public void NextScene()
    {
        SceneManager.LoadScene("Field");
    }


}
