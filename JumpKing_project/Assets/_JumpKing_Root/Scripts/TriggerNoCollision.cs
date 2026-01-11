using UnityEngine;

public class TriggerNoCollision : MonoBehaviour
{
    private void Awake()
    {
        // Asegurar que el collider es trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
}
