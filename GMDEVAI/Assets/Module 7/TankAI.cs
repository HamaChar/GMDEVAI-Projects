using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAI : MonoBehaviour
{
    Animator anim;
    public GameObject player;
    public GameObject bullet;
    public GameObject turret;

    public GameObject GetPlayer()
    {
        return player;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        // Existing distance tracking
        anim.SetFloat("distance", Vector3.Distance(transform.position, player.transform.position));

        TankHealth healthScript = GetComponent<TankHealth>();
        if (healthScript != null)
        {
            anim.SetFloat("healthPercent", healthScript.GetHealthPercentage());
        }
    }

    void Fire()
    {
        GameObject b = Instantiate(bullet, turret.transform.position, turret.transform.rotation);
        b.GetComponent<Rigidbody>().AddForce(turret.transform.forward * 1500);
    }

    public void StopFiring()
    {
        CancelInvoke("Fire");
    }

    public void StartFiring()
    {
        InvokeRepeating("Fire", 0.5f, 0.5f);
    }
}