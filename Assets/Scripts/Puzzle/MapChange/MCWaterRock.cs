using UnityEngine;

public class MCWaterRock : MonoBehaviour
{
    [SerializeField] MCWater myWater;
    [SerializeField] private Vector3 offset = new Vector3(45f, 0, 0);
    Vector3 waterPos => myWater.transform.position;

    public void MoveToWater()
    {
        transform.position = waterPos + offset;
    }
}
