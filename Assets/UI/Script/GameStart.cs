using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
/*    [SerializeField] private Image start;
    [SerializeField] private Animator animator;*/


    public void Start_Anims()
    {
        //animator.SetBool("isStart", true);
    }


    // 애니메이션 이벤트로 호출
    public void NextScene()
    {
        SceneManager.LoadScene("Field");
    }
}
