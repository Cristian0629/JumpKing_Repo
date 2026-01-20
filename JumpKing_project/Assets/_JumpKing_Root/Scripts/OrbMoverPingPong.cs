using UnityEngine;

public class OrbMoverPingPong : MonoBehaviour
{
    public enum Axis { Horizontal, Vertical }

    [Header("Movement")]
    [SerializeField] Axis axis = Axis.Vertical;
    [SerializeField] float distance = 2f;
    [SerializeField] float speed = 2f;

    [Header("Options")]
    [SerializeField] bool useLocalPosition = false;

    Vector3 startPos;

    void Start()
    {
        startPos = useLocalPosition ? transform.localPosition : transform.position;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * speed); // -1..1
        Vector3 offset = Vector3.zero;

        if (axis == Axis.Vertical) offset = Vector3.up * (t * distance);
        else offset = Vector3.right * (t * distance);

        if (useLocalPosition) transform.localPosition = startPos + offset;
        else transform.position = startPos + offset;
    }
}

