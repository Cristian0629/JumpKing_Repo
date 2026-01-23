using UnityEngine;

public class TornadoMover2D : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float amplitude = 2.5f;   // distancia hacia izquierda/derecha desde el punto inicial
    [SerializeField] float speed = 1.5f;       // velocidad del vaivén
    [SerializeField] bool startGoingRight = true;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        if (!startGoingRight) speed = -Mathf.Abs(speed);
        else speed = Mathf.Abs(speed);
    }

    void Update()
    {
        // PingPong suave: -1..1
        float t = Mathf.Sin(Time.time * speed);
        transform.position = startPos + Vector3.right * (t * amplitude);
    }
}
