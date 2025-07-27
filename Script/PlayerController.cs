using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    float xforce;
    float zforce;

    float yaw = 0f;
    float pitch = 0f;

    [SerializeField] float moveSpeed = 2;
    [SerializeField] float lookSpeed = 2;
    [SerializeField] GameObject cam;
    Rigidbody rb;

    [SerializeField] Vector3 boxSize;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float jumpForce = 5;

    [SerializeField] GameObject bullethole;
    RaycastHit hit;
    [SerializeField] float fireRate = 0.1f;
    bool canFire = true;

    AudioSource aud;
    [SerializeField] ParticleSystem gunfire;

    bool isGrounded;

    // 🔫 Ammo variables
    int maxAmmo = 150;
    int currentAmmo;

    bool isReloading = false;

    // ✅ UI Text para mostrar balas
    [SerializeField] Text ammoText;

    // 👾 Nivel y enemigos eliminados
    [SerializeField] int enemiesToKill = 5; // cantidad para pasar de nivel
    int enemiesKilled = 0; // contador de eliminados

    [SerializeField] Text levelMessageText; // asigna en el inspector el Text del Canvas

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;

        currentAmmo = maxAmmo; // inicia con todas las balas

        UpdateAmmoUI();

        if (levelMessageText != null)
        {
            levelMessageText.enabled = false; // oculta el mensaje al inicio
        }
    }

    void Update()
    {
        LookAround();
        PlayerMovement();
        GroundCheck();

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetMouseButton(0) && canFire && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                Debug.Log("Sin balas, recarga...");
            }
        }

        // ⏩ Recarga con clic derecho
        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            Reload();
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void PlayerMovement()
    {
        xforce = Input.GetAxis("Horizontal") * moveSpeed;
        zforce = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 move = transform.right * xforce + transform.forward * zforce;
        move.y = rb.velocity.y;

        rb.velocity = move;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Player jumped");
    }

    void Shoot()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 300f))
        {
            aud.Play();
            gunfire.Play();

            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealthLogic(hit.collider.gameObject);
            }
            else
            {
                Instantiate(bullethole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
        }

        currentAmmo--; // resta una bala al disparar
        Debug.Log("Balas restantes: " + currentAmmo);
        UpdateAmmoUI();

        canFire = false;
        Invoke(nameof(FireRateReset), fireRate);
    }

    void EnemyHealthLogic(GameObject enemy)
    {
        // Busca si ya tiene vida asignada
        if (enemy.TryGetComponent<EnemyLife>(out EnemyLife enemyLife))
        {
            enemyLife.health--;

            Debug.Log("Enemigo golpeado. Vida restante: " + enemyLife.health);

            if (enemyLife.health <= 0)
            {
                Destroy(enemy);
                Debug.Log("Enemigo eliminado");

                enemiesKilled++;

                if (enemiesKilled >= enemiesToKill)
                {
                    StartCoroutine(LoadNextLevel());
                }
            }
        }
        else
        {
            // Si no tiene vida asignada, se la asigna (entre 3 y 10) y le resta 1 por el disparo
            int vidaInicial = UnityEngine.Random.Range(3, 11);
            enemyLife = enemy.AddComponent<EnemyLife>();
            enemyLife.health = vidaInicial - 1;

            Debug.Log("Enemigo generado con " + vidaInicial + " de vida. Vida restante tras disparo: " + enemyLife.health);

            if (enemyLife.health <= 0)
            {
                Destroy(enemy);
                Debug.Log("Enemigo eliminado");

                enemiesKilled++;

                if (enemiesKilled >= enemiesToKill)
                {
                    StartCoroutine(LoadNextLevel());
                }
            }
        }
    }

    void FireRateReset()
    {
        gunfire.Stop();
        canFire = true;
    }

    void GroundCheck()
    {
        isGrounded = Physics.BoxCast(transform.position, boxSize, -transform.up, transform.rotation, maxDistance, layerMask);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    void Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        Debug.Log("Recarga completa. Balas: " + currentAmmo);
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Balas: " + currentAmmo + "/ " + maxAmmo;
        }
    }

    IEnumerator LoadNextLevel()
    {
        if (levelMessageText != null)
        {
            levelMessageText.text = "Cargando siguiente nivel...";
            levelMessageText.enabled = true;
        }

        yield return new WaitForSeconds(5f);

        if (levelMessageText != null)
        {
            levelMessageText.enabled = false;
        }

        Debug.Log("Siguiente nivel cargado (agrega tu lógica aquí)");
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // Reinicia el contador para el siguiente nivel
        enemiesKilled = 0;
    }

    // 👾 Clase interna para guardar vida de cada enemigo
    private class EnemyLife : MonoBehaviour
    {
        public int health;
    }
}
