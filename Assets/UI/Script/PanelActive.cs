using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelActive : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private Image content;

    private bool isActive = false;

    public void ClickButton()
    {
        isActive = !isActive;
        StartCoroutine(DisableDelay());
    }

    private IEnumerator DisableDelay()
    {
        yield return null;
        panel.gameObject.SetActive(isActive);
        content.gameObject.SetActive(isActive);
    }
}
