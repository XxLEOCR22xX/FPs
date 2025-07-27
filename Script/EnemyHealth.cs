using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    EnemySpawner enemySpawner; // referencia interna al spawner

    void Start()
    {
        health = Random.Range(3, 11);
        enemySpawner = FindObjectOfType<EnemySpawner>(); // busca el spawner en la escena
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Daño recibido. Vida restante: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (enemySpawner != null)
        {
            enemySpawner.DestroyEnemyByShoot(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
