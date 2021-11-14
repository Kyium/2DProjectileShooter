using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] const float MovePerFrame = 0.03f;
    [SerializeField] GameObject projectile;
    [SerializeField] private Sprite deadSprite;
    private float health = 100f;
    private float fuel = 150f;
    private float jetpack = 80f;
    private bool isDead = false;
    private float FuelPerMoveFrame = 0.1f;
    private float FuelPerJump = 1.0f;
    private float JetpackPerFrame = 0.3f;
    private float JetpackYVelocityPerFrame = 7;
    private float JetpackMaxYVelocity = 140;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (health < 0) health = 0;
        if (fuel < 0) fuel = 0;
        if (jetpack < 0) jetpack = 0;
    }

    private void HandleInput()
    {
        if (!isDead && isLocalPlayer)
        {
            if (Input.GetKey(KeyCode.LeftArrow) && fuel > 0)
            {
                var position = this.transform.position;
                position.x -= MovePerFrame;
                fuel -= FuelPerMoveFrame;
                this.transform.position = position;
                updateLocalUI();
            }
            else if (Input.GetKey(KeyCode.RightArrow) && fuel > 0)
            {
                var position = this.transform.position;
                position.x += MovePerFrame;
                fuel -= FuelPerMoveFrame;
                this.transform.position = position;
                updateLocalUI();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && this.GetComponent<Rigidbody2D>().velocity.y == 0 && fuel > 0)
            {
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 280));
                fuel -= FuelPerJump;
                updateLocalUI();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdSpawnProjectile();
            }
            if (Input.GetKey(KeyCode.UpArrow) && this.GetComponent<Rigidbody2D>().velocity.y != 0 && jetpack > 0)
            {
                if (this.GetComponent<Rigidbody2D>().velocity.y < JetpackMaxYVelocity)
                {
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JetpackYVelocityPerFrame));
                    jetpack -= JetpackPerFrame;
                    updateLocalUI();
                }
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
            updateLocalUI();
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

    void updateLocalUI()
    {
        if (isLocalPlayer)
        {
            foreach (Text label in FindObjectOfType<Canvas>().GetComponentsInChildren<Text>())
            {
                if (label.name == "Health") label.text = "Health: " + health;
                if (label.name == "Fuel") label.text = "Fuel: " + fuel.ToString("F1");
                if (label.name == "Jetpack") label.text = "Jetpack: " + jetpack.ToString("F1");
            }

        }
    }
}
