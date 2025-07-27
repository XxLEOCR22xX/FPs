using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;    // Prefab del enemigo
    public float spawnRadius = 10f;   // Radio del área de aparición
    public int maxEnemies = 10;       // Máximo de enemigos que pueden existir a la vez
    public float spawnInterval = 1f;  // Intervalo entre la generación de enemigos (en segundos)
    public Transform player;          // Referencia al jugador para seguirlo

    private List<GameObject> enemiesInRange = new List<GameObject>();  // Lista de enemigos en el área
    private Collider spawnAreaCollider;

    void Start()
    {
        spawnAreaCollider = GetComponent<Collider>();  // Obtener el collider del área de spawn
        StartCoroutine(ManageEnemySpawning());  // Comenzar la gestión continua de enemigos
        StartCoroutine(CheckEnemiesInRange());  // Comprobar enemigos fuera de rango
    }

    void Update()
    {
        // Actualizar la posición del spawn area para que siga al jugador
        transform.position = player.position;  // El área de spawn sigue al jugador
    }

    // Método para gestionar la creación continua de enemigos
    IEnumerator ManageEnemySpawning()
    {
        while (true)
        {
            // Si hay menos de 'maxEnemies', genera un nuevo enemigo
            if (enemiesInRange.Count < maxEnemies)
            {
                // Generar una posición aleatoria dentro del área de spawn (usando el spawnRadius)
                Vector3 randomPosition = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,  // Si estás en un plano 2D, puedes ajustarlo en Y según tus necesidades
                    Random.Range(-spawnRadius, spawnRadius)
                );

                // Asegurarse de que el enemigo esté dentro del collider
                randomPosition += spawnAreaCollider.transform.position;

                // Instanciar el enemigo
                GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
                enemiesInRange.Add(enemy);

                // Esperar un poco antes de generar el siguiente enemigo
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // Si ya hay el máximo de enemigos, esperar un poco antes de revisar nuevamente
                yield return null;
            }
        }
    }

    // Comprobar si los enemigos están fuera del rango y destruirlos
    IEnumerator CheckEnemiesInRange()
    {
        while (true)
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                GameObject enemy = enemiesInRange[i];

                // Comprobar si el enemigo está fuera del rango
                if (Vector3.Distance(enemy.transform.position, spawnAreaCollider.transform.position) > spawnRadius)
                {
                    Destroy(enemy);  // Destruir el enemigo si está fuera del rango
                    enemiesInRange.RemoveAt(i);  // Eliminarlo de la lista
                }
            }

            // Esperar antes de comprobar nuevamente
            yield return new WaitForSeconds(1f);
        }
    }

    // Método para destruir enemigos por disparo
    public void DestroyEnemyByShoot(GameObject enemy)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
            Destroy(enemy);
        }
    }
}
