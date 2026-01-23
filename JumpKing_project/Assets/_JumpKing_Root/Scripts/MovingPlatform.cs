using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float speed; // Velocidad de la plataforma
    [SerializeField] int startingPoint; // Numero para determinar el indice del punto de la plataforma
    [SerializeField] Transform[] points; // Array de puntos de posición hacia los lados que la plataforma se moverá.
    private int i; // Indice del Array
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Setear la posicion inicial de la plataforma a uno de los puntos, asignando a startin point a un valor numerico.
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; // Aumenta el indice, cambia de objetivo.
            if (i == points.Length) // Chequea si la plataforma ha llegado al último punto
            {
                i = 0;
            }
        }
        // Mueve la plataforma a la posición del punto guardado en el Array en el espacio con valor igual a "i"
        transform.position = Vector2.MoveTowards(transform.position,points[i].position,speed * Time.deltaTime );
    }
}
