using UnityEngine;

public class VictoriaManager : MonoBehaviour
{
    // Esta variable guardará tu foto de victoria
    public GameObject panelVictoria;

    // Esta función "enciende" la foto cuando ganas
    public void MostrarVictoria()
    {
        panelVictoria.SetActive(true); // Activa el objeto que habías desactivado
        Time.timeScale = 0f;          // Detiene el tiempo del juego
        Cursor.visible = true;         // Muestra el ratón
        Cursor.lockState = CursorLockMode.None;
    }
}
