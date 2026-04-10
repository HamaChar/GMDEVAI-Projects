using UnityEngine;
using UnityEngine.SceneManagement;

public class TankHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Destroy tank when HP is reduced to 0
        if (currentHealth <= 0)
        {
            if(gameObject.tag == "Player")
                SceneManager.LoadScene("Tanks");
            Destroy(gameObject);
        }
    }

    // Calculates HP percentage for the FSM Flee logic
    public float GetHealthPercentage()
    {
        return (currentHealth / maxHealth) * 100f;
    }
}