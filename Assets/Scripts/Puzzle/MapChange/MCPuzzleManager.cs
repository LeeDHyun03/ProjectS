using System.Collections.Generic;
using UnityEngine;

public class MCPuzzleManager : PuzzleManager
{
    public bool isPast = true;
    public List<MCBullet> allBullets = new List<MCBullet>();
    public override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public override void OnEnable()
    {
        base.OnEnable();
        foreach (var bullet in allBullets)
        {
            bullet.OnTriggerEnterBullet += PuzzleReset;
        }
    }
    public override void OnDisable()
    {
        base.OnDisable(); foreach (var bullet in allBullets)
        {
            bullet.OnTriggerEnterBullet -= PuzzleReset;
        }
    }
    public void ChangedMap()
    {
        isPast = !isPast;
    }
    public void SetBullets(MCBullet mC)
    {
               allBullets.Add(mC);
    }
}
