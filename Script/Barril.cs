using UnityEngine;

public class Barril : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BossController jefe = FindObjectOfType<BossController>();
            jefe.EliminarJugador();
            collision.gameObject.SetActive(false);
        }
    }
}
