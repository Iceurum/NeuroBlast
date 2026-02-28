using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die(true);
    }

    public virtual void Die(bool killedByPlayer)
    {
        if (killedByPlayer)
            GameManager.Instance.AddEnemyDestroyed();
        else
            GameManager.Instance.AddEnemyEscaped();

        Destroy(gameObject);
    }
}