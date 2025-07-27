using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inicio : MonoBehaviour
{
    public Text textoUI; // Asigna el texto desde el Inspector
    public float velocidadLatido = 2f; // Velocidad del pulso
    public float escalaMax = 1.2f;
    public float escalaMin = 0.9f;

    private Vector3 escalaInicial;
    private bool iniciado = false;

    void Start()
    {
        escalaInicial = textoUI.transform.localScale;
    }

    void Update()
    {
        if (!iniciado)
        {
            // Animación de latido
            float escala = Mathf.Lerp(escalaMin, escalaMax, (Mathf.Sin(Time.time * velocidadLatido) + 1f) / 2f);
            textoUI.transform.localScale = escalaInicial * escala;

            // Detectar cualquier tecla para iniciar
            if (Input.anyKeyDown)
            {
                iniciado = true;
                textoUI.gameObject.SetActive(false); // Oculta el texto
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
