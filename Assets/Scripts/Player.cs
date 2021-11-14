using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] const float MovePerFrame = 0.04f;
    [SerializeField] GameObject projectile;
    [SerializeField] private Sprite deadSprite;
    private float health = 100;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!isDead && isLocalPlayer)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var position = this.transform.position;
                position.x -= MovePerFrame;
                this.transform.position = position;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                var position = this.transform.position;
                position.x += MovePerFrame;
                this.transform.position = position;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && this.GetComponent<Rigidbody2D>().velocity.y == 0)
            {
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 280));
            }
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

    [Command]
    public void CmdDespawnProjectile(GameObject projectileToDestroy)
    {
        NetworkServer.Destroy(projectileToDestroy);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.name.Contains("Projectile"))
        {
            this.health -= collision.collider.gameObject.GetComponent<Projectile>().getDamage();
            deadCheck();
            CmdDespawnProjectile(collision.collider.gameObject);
            if (isLocalPlayer)
            {
                FindObjectOfType<Canvas>().GetComponentInChildren<Text>().text = "Health: " + health;
            }
        }
    }

    private void deadCheck()
    {
        if (this.health <= 0 && !isDead)
        {
            this.GetComponent<SpriteRenderer>().sprite = deadSprite;
            this.GetComponent<Transform>().localScale = new Vector3(0.2f, 0.6f, 1f);
            this.GetComponent<Rigidbody2D>().simulated = false;
            isDead = true;
        }
    }
}
