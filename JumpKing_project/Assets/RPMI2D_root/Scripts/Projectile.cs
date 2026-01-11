using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Configuration")]
    [SerializeField] float speed;
    public bool isFacingRight;

    SpriteRenderer projectileRend; //Referencia al sprite de la bala
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectileRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        projectileMove();
     }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "CamConfiner" || collision.gameObject.tag == null)
                {
            gameObject.SetActive(false);
                }
    }
    void projectileMove()
    {
        if (isFacingRight) transform.Translate(Vector3.right * speed * Time.deltaTime);
        else
        {
            projectileRend.flipX = true; //Flipear el sprite de la bala
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }
}
