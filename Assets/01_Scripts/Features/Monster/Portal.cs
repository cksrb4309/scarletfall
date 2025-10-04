using UnityEngine;

public class Portal : MonoBehaviour
{
    public Monster monster;
    private void Start()
    {
        transform.parent = null;
    }
    public void EnableMonster()
    {
        monster.StartMonster();
    }
    public void EndPortal()
    {
        gameObject.SetActive(false);
    }
}
