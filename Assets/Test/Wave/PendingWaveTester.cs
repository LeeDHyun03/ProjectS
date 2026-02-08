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
        Debug.Log("잠시 뒤 다시 필드로 복귀");
        yield return new WaitForSeconds(5);
        WaveManager.Instance.initializedByPuzzleTestScene = true;
        SceneManager.LoadScene("Field");
    }
}
