using Roguelike.Items;
using System.Collections;
using UnityEngine;

public class ItemDataTestRunner : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return ItemDataManager.Instance.LoadAsync();

        Debug.Log($"[TEST] IsLoaded={ItemDataManager.Instance.IsLoaded}");
        Debug.Log($"[TEST] Items={ItemDataManager.Instance.ItemsById.Count}");
    }
}
