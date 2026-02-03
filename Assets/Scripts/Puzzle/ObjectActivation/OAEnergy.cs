using UnityEngine;
using System;

public class OAEnergy : OATileController
{
    public bool onButton; 
    public event Action<OAActTileColor, bool> OnActTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onButton = true;
        ActChange(collision.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onButton = false;
        ActChange(collision.gameObject, false);
    }

    void ActChange(GameObject obj, bool isActive)
    {
        var c = obj.name;

        if (c == "RedAct_Button")
        {
            OnActTrigger?.Invoke(OAActTileColor.Red, isActive);
        }
        else if (c == "BlueAct_Button")
        {
            OnActTrigger?.Invoke(OAActTileColor.Blue, isActive);
        }
    }
}