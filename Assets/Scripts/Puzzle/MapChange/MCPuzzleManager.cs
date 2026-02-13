using System.Collections.Generic;
using UnityEngine;

public class MCPuzzleManager : PuzzleManager
{
    public bool isPast = true;
    public List<MCBullet> allBullets = new List<MCBullet>();
    Vector3 pastVec = new Vector3(0f, 0f, 0f);
    Vector3 presentVec = new Vector3(-45f, 0f, 0f);

    public override void OnDisable()
    {
        base.OnDisable(); 
        foreach (var bullet in allBullets)
        {
            bullet.OnTriggerEnterBullet -= PuzzleReset;
        }
    }
    public void ChangedMap()
    {
        isPast = !isPast;
        myMap.transform.position = isPast ? pastVec : presentVec;
    }
    public void SetBullets(MCBullet mC)
    {
        allBullets.Add(mC);
        mC.OnTriggerEnterBullet += PuzzleReset;
    }
}
