using System.Collections.Generic;
using UnityEngine;

public class OAPuzzleManager : PuzzleManager
{
    public GameObject[] puzzleDifficulty = new GameObject[3];
    Transform currentDifficulty => puzzleDifficulty[difficulty].transform;
    Transform objects => currentDifficulty.GetChild(0);

    List<OAEnergy> energies = new List<OAEnergy>();
    List<OAActTile> RedActTiles = new List<OAActTile>();
    List<OAActTile> BlueActTiles = new List<OAActTile>();

    Vector3 startVec => currentDifficulty.GetChild(3).position;
    int redActiveCount = 0;
    int blueActiveCount = 0;

    private void Start()
    {
        for (int i = 0; i < objects.GetChild(0).childCount; i++)
        {
            var ctr = objects.GetChild(0).GetChild(i).GetComponent<OAEnergy>();
            ctr.SetPuzzleManager(this);
            ctr.OnActTrigger += HandleActTrigger; 
            energies.Add(ctr);
        }
        for (int i = 0; i < objects.GetChild(1).childCount; i++)
        {
            var ctr = objects.GetChild(1).GetChild(i).GetComponent<OAActTile>();
            ctr.SetPuzzleManager(this);
            if(ctr.myColor == OAActTileColor.Red)
                RedActTiles.Add(ctr);
            else
                BlueActTiles.Add(ctr);
        }
        player.position = startVec;
    }
    public override void Init(int level)
    {
        puzzleDifficulty[level].SetActive(true);
    }

    void HandleActTrigger(OAActTileColor color, bool isActive)
    {
        if (color == OAActTileColor.Red)
        {
            redActiveCount += isActive ? 1 : -1;
            redActiveCount = Mathf.Max(0, redActiveCount);

            if ((isActive && redActiveCount == 1) || (!isActive && redActiveCount == 0))
            {
                foreach (var a in RedActTiles) a.ToggleActive();
            }
        }
        else if (color == OAActTileColor.Blue)
        {
            blueActiveCount += isActive ? 1 : -1;
            blueActiveCount = Mathf.Max(0, blueActiveCount);

            if ((isActive && blueActiveCount == 1) || (!isActive && blueActiveCount == 0))
            {
                foreach (var a in BlueActTiles) a.ToggleActive();
            }
        }
    }

    private void OnDisable()
    {
        foreach (var a in energies)
        {
            a.OnActTrigger -= HandleActTrigger;
        }
    }
}