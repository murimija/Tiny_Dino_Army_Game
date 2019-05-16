using UnityEngine;

public class EggController : MonoBehaviour
{
    [SerializeField] private GameObject dinoPref;
    private GameController gameController;

    private void Start()
    {
        gameController = GameController.instance;
        gameController.listOfEggs.Add(gameObject);
        
    }

    public void OpenEgg()
    {
        Instantiate(dinoPref, transform.position, Quaternion.identity);
        gameController.listOfEggs.Remove(gameObject);
        Destroy(gameObject);
    }

}