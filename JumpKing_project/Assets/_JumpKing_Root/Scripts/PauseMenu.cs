using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class ControladorPausa : MonoBehaviour
{
    public GameObject objetoMenuPausa; // Aquí arrastraremos tu imagen de pausa
    private bool juegoPausado = false;

    void Update()
    {
        // Si pulsas Escape o la tecla P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        objetoMenuPausa.SetActive(false); // Esconde el menú
        Time.timeScale = 1f;              // El tiempo vuelve a correr
        juegoPausado = false;

        // Bloquea el ratón de nuevo (opcional)
        Cursor.visible = false;
    }

    void Pausar()
    {
        objetoMenuPausa.SetActive(true);  // Muestra el menú
        Time.timeScale = 0f;              // Congela el juego
        juegoPausado = true;

        // Muestra el ratón para poder clicar botones
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f; // ¡IMPORTANTE! Si no lo haces, el menú principal estará congelado
        SceneManager.LoadScene("MenuPause"); //
    }
}
