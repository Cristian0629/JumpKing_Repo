using UnityEngine;

public class Meta2D : MonoBehaviour
{
    // Arrastra aquí el objeto que contiene el script VictoriaManager
    public VictoriaManager victoriaManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica que el objeto que entra tenga el Tag "Player"
        if (other.CompareTag("Player"))
        {
            victoriaManager.MostrarVictoria();
        }
    }
}
