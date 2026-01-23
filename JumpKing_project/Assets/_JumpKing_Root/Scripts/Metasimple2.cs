using UnityEngine;

public class MetaSimple : MonoBehaviour
{
    public GameObject panelVictoria; // Aquí arrastrarás tu panel luego

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto que entra tiene el Tag "Player"
        if (collision.CompareTag("Player"))
        {
            panelVictoria.SetActive(true); // Enciende la pantalla
            Time.timeScale = 0f;           // Pausa el juego
            Cursor.visible = true;         // Muestra el ratón
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
