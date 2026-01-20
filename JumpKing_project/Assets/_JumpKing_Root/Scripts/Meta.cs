using UnityEngine;

public class Meta : MonoBehaviour
{
    // Este método detecta cuando algo entra en el cuadrado de la meta
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ¿El objeto que entró tiene la etiqueta "Player"?
        if (other.CompareTag("Player"))
        {
            // Busca el script VictoriaManager que ya configuraste antes
            VictoriaManager gestor = FindObjectOfType<VictoriaManager>();

            if (gestor != null)
            {
                gestor.MostrarVictoria(); // Llama a la función que enciende la foto
            }
        }
    }
}

