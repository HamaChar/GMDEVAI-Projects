using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;    // Drag your Bullet prefab here in the Inspector
    public Transform spawnPoint;       // Drag the empty GameObject acting as the barrel tip here
    public float shootForce = 1500f;   // How fast the bullet travels

    void Update()
    {
        // Shoot when the Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBullet();
        }
    }

    void FireBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        
        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * shootForce);
        }
    }
}