using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarrelDamage : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Desactiva al jugador simulando que muere
            collision.gameObject.SetActive(false);

            // Inicia la rutina para reiniciar el nivel después de 3 segundos
            StartCoroutine(RestartLevel());
        }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);

        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
