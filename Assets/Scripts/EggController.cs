using UnityEngine;

public class EggController : MonoBehaviour
{
    [SerializeField] private GameObject dinoPref;

    public void OpenEgg()
    {
        Instantiate(dinoPref, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}