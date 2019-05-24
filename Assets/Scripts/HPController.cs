using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    [SerializeField] private int maxHealthPoints;
    public int currentHealthPoints;
    [SerializeField] private GameObject healthBarPref;
    [SerializeField] private Image healthBarImage;
    private bool helthBarExsist;
    private bool isDead;
    private SceneChanger sceneChanger;
    private void Start()
    {
        currentHealthPoints = maxHealthPoints;
        helthBarExsist = false;
        sceneChanger = SceneChanger.instance;
    }

    public void takeDamage(int damage)
    {
        if (!helthBarExsist)
        {
            healthBarPref.SetActive(true);
            helthBarExsist = true;
        }

        currentHealthPoints -= damage;

        healthBarImage.fillAmount = (float) currentHealthPoints / maxHealthPoints;

        if (currentHealthPoints <= 0 && !isDead)
        {
            death();
            isDead = true;
        }
    }

    private void death()
    {
        if (gameObject.CompareTag("Egg"))
            gameObject.GetComponent<EggController>().OpenEgg();
        else if (gameObject.CompareTag("PlayersDino"))
            gameObject.GetComponent<DinoController>().Death();
        else if (gameObject.CompareTag("Enemy"))
            gameObject.GetComponent<EnemyController>().Death();
        else if (gameObject.CompareTag("Boss"))
        {
            gameObject.GetComponent<EnemyController>().Death();
            WinGame();
        }
    }

    private void WinGame()
    {
        sceneChanger.GoToScene("WinScreen");
    }
    
}