using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShootProjectile : NetworkBehaviour
{
    [SerializeField] GameObject projectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdSpawnProjectile();
            }
        }
    }

    [Command]
    public void CmdSpawnProjectile()
    {
        GameObject newProjectile = projectile;
            newProjectile = Instantiate(newProjectile,
                new Vector3(this.GetComponent<Transform>().position.x + 1,
                    this.GetComponent<Transform>().position.y + 1, 0), Quaternion.identity);
            newProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 150));
            NetworkServer.Spawn(newProjectile);
        
    }
}
