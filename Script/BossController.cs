using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public Transform jugador;
    public GameObject barrilPrefab;
    public Transform puntoDisparo;
    public float velocidad = 3f;
    public float tiempoEntreBarriles = 3f;
    public float fuerzaBarril = 500f;

    public float tiempoMaximo = 120f;

    private float tiempoRestante;
    private bool juegoTerminado = false;

    [SerializeField] Image pantallaNegra;
    [SerializeField] Text mensajeFinal;

    void Start()
    {
        tiempoRestante = tiempoMaximo;
        StartCoroutine(DispararBarriles());
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Movimiento hacia el jugador
        Vector3 direccion = (jugador.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        // Temporizador
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            Ganar();
        }
    }

    IEnumerator DispararBarriles()
    {
        while (!juegoTerminado)
        {
            yield return new WaitForSeconds(tiempoEntreBarriles);

            if (jugador != null)
            {
                GameObject barril = Instantiate(barrilPrefab, puntoDisparo.position, Quaternion.identity);
                Rigidbody rb = barril.GetComponent<Rigidbody>();
                Vector3 direccion = (jugador.position - puntoDisparo.position).normalized;
                rb.AddForce(direccion * fuerzaBarril);
            }
        }
    }

    public void EliminarJugador()
    {
        if (!juegoTerminado)
        {
            Perder();
        }
    }

    void Ganar()
    {
        juegoTerminado = true;
        pantallaNegra.gameObject.SetActive(true);
        mensajeFinal.text = "¡GANASTE!";
        mensajeFinal.color = Color.green;
        StartCoroutine(FadeIn());
    }

    void Perder()
    {
        juegoTerminado = true;
        pantallaNegra.gameObject.SetActive(true);
        mensajeFinal.text = "PERDISTE";
        mensajeFinal.color = Color.red;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color color = pantallaNegra.color;
        color.a = 0f;
        pantallaNegra.color = color;

        while (pantallaNegra.color.a < 1f)
        {
            color.a += Time.deltaTime * 0.5f;
            pantallaNegra.color = color;
            yield return null;
        }

        mensajeFinal.gameObject.SetActive(true);

        // Espera 3 segundos antes de ir a la escena 0
        yield return new WaitForSeconds(3f);

    // Cargar escena llamada "menu"
    SceneManager.LoadScene(0);
    }
}
