using UnityEngine;

public class TornadoZone2D : MonoBehaviour
{
    [Header("Hold & Launch")]
    [SerializeField] Transform holdPoint;
    [SerializeField] float launchUpSpeed = 14f;
    [SerializeField] float launchSideCarry = 1f;

    [Header("Inside Visual Motion")]
    [SerializeField] float orbitRadius = 0.15f;
    [SerializeField] float orbitSpeed = 720f;

    public Transform HoldPoint => holdPoint != null ? holdPoint : transform;
    public float LaunchUpSpeed => launchUpSpeed;
    public float LaunchSideCarry => launchSideCarry;
    public float OrbitRadius => orbitRadius;
    public float OrbitSpeed => orbitSpeed;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // ✅ AHORA LLAMA AL PlayerController (que ya tiene EnterTornado)
        var pc = other.GetComponent<PlayerController>();
        //if (pc != null)
            //pc.EnterTornado(this);
    }
}
