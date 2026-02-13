using TMPro;
using UnityEngine;

public class PortalEntranceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text puzzleName;
    [SerializeField] private TMP_Text difficult;

    public void SetPuzzleInfo(string puzzleName, string difficultName)
    {
        this.puzzleName.text = puzzleName;
        this.difficult.text = difficultName;
    }
}
