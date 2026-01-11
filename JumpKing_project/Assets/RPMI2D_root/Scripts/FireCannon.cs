using UnityEngine;

public class FireCannon : MonoBehaviour
{

    [Header("Shoot Variables")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootPoint;

    [Header("Repeat Times")]
    [SerializeField] float firstShootTime;
    [SerializeField] float repeatShootTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(CannonShoot), firstShootTime, repeatShootTime);
    }

    void CannonShoot()
    {
        Instantiate(projectile, shootPoint.position, Quaternion.identity);
    }

}

    // Update is called once per frame
 
