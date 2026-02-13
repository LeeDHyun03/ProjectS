using UnityEngine;

public class MonsterSounds : MonoBehaviour
{
    public void Archer1()
        => SfxManager.Instance.Play("Monster_Archer1");
    public void Archer2()
        => SfxManager.Instance.Play("Monster_Archer2");

    public void DarkMage()
        => SfxManager.Instance.Play("Monster_DarkMage");

    public void Knight()
        => SfxManager.Instance.Play("Monster_Knight");

    public void Mage1()
        => SfxManager.Instance.Play("Monster_Mage1");

    public void SpearMan()
        => SfxManager.Instance.Play("Monster_SpearMan");
}
