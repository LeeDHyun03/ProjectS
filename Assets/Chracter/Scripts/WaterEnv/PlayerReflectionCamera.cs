using UnityEngine;

public class PlayerReflectionCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        var s = GameObject.Find("PlayerCharacter").transform.position;
        transform.position = new Vector3(s.x, s.y, transform.position.z);
    }
}
