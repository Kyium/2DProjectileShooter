using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Proctile : NetworkBehaviour
{
    // Start is called before the first frame update
    private float damage = 10.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
        }
    }
}
