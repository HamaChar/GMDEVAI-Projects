using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject explosion;
    public float damage = 25f; // Add a damage variable
    
    void OnCollisionEnter(Collision col)
    {
        TankHealth hitTank = col.gameObject.GetComponent<TankHealth>();

        if (hitTank != null)
        {
            hitTank.TakeDamage(damage);
        }

        if (explosion != null) 
        {
            GameObject e = Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(e, 1.5f);
        }
        
        Destroy(this.gameObject);
    }
}