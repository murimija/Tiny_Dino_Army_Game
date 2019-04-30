using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    [SerializeField] private int maxHealthPoints;
    public int currentHealthPoints;
    [SerializeField] private GameObject healthBarPref;
    [SerializeField] private Image healthBarImage;
    private bool helthBarExsist;

    private void Start()
    {
        currentHealthPoints = maxHealthPoints;
        helthBarExsist = false;
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

        if (currentHealthPoints <= 0)
        {
            death();
        }
    }

    private void death()
    {
        if (gameObject.CompareTag("Egg"))
            gameObject.GetComponent<EggController>().OpenEgg();
        if (gameObject.CompareTag("PlayersDino"))
            gameObject.GetComponent<DinoController>().Death();
        if (gameObject.CompareTag("Enemy"))
            gameObject.GetComponent<EnemyController>().Death();

        //Destroy(gameObject);
    }
}