using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{
    [SerializeField] Camera currentCamera;
    [SerializeField] Camera nextCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER TOCADO POR: " + other.name);

        if (!other.CompareTag("Player")) return;

        Debug.Log("ES EL PLAYER -> CAMBIO CAMARA");

        if (currentCamera != null) currentCamera.gameObject.SetActive(false);
        else Debug.Log("currentCamera ES NULL");

        if (nextCamera != null) nextCamera.gameObject.SetActive(true);
        else Debug.Log("nextCamera ES NULL");
    }
}
