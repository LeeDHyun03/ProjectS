using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PendingWaveTester : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ReturnToFieldAtferSeconds());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ReturnToFieldAtferSeconds()
    {
        Dbg.L("잠시 뒤 다시 필드로 복귀함");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Field");
    }
}
