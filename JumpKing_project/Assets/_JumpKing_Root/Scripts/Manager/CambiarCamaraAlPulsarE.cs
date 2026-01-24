using UnityEngine;
using UnityEngine.InputSystem;

public class CambiarCamaraAlPulsarE : MonoBehaviour
{
    public Camera camara13;
    public Camera camara15;

    private bool jugadorDentro = false;

    void Update()
    {
        if (!jugadorDentro) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (camara13 != null)
                camara13.gameObject.SetActive(false);

            if (camara15 != null)
                camara15.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jugadorDentro = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jugadorDentro = false;
    }
}
