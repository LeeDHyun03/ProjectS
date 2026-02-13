using TMPro;
using UnityEngine;

public class ItemKeywordInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keywordName;
    [SerializeField] private TextMeshProUGUI keywordDesc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(string name, string description)
    {
        keywordName.text = name;
        keywordDesc.text = description;
    }
}
