using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GlobalLight : MonoBehaviour
{
    public static GlobalLight Instance;

    private Light2D light2D;

    void Awake()
    {
        Instance = this;
        light2D = GetComponent<Light2D>();
    }

    public void SetColor(Color color)
    {
        light2D.color = color;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
