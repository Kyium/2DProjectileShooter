using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SerializeField] private float MovePerFrame = 0.085f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Sprite aliveSprite;
    [SerializeField] private Sprite deadSprite;
    [SyncVar] public string team = "none";
    private float health = 100f;
    private float fuel = 150f;
    private float jetpack = 80f;
    private bool isDead = false;
    private bool activated = true;
    private readonly float FuelPerMoveFrame = 0.3f;
    private readonly float FuelPerJump = 1.0f;
    private readonly float JetpackPerFrame = 0.65f;
    private readonly float JetpackYVelocityPerFrame = 23;
    private readonly float JetpackMaxYVelocity = 30;
    // Start is called before the first frame update
    void Start()
    {
        if (activated)
        {
            activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (health < 0) health = 0;
        if (fuel < 0) fuel = 0;
        if (jetpack < 0) jetpack = 0;
        updateLocalUI();
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
            }
            else if (Input.GetKey(KeyCode.RightArrow) && fuel > 0)
            {
                var position = this.transform.position;
                position.x += MovePerFrame;
                fuel -= FuelPerMoveFrame;
                this.transform.position = position;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && this.GetComponent<Rigidbody2D>().velocity.y == 0 && fuel > 0)
            {
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 280));
                fuel -= FuelPerJump;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdSpawnProjectile();
            }
            if (Input.GetKey(KeyCode.DownArrow) && this.GetComponent<Rigidbody2D>().velocity.y != 0 && jetpack > 0)
            {
                if (this.GetComponent<Rigidbody2D>().velocity.y < JetpackMaxYVelocity)
                {
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JetpackYVelocityPerFrame));
                    jetpack -= JetpackPerFrame;
                }
            }
        }
    }

    [Command]
    private void CmdSpawnProjectile()
    {
        GameObject newProjectile = projectile;
        newProjectile = Instantiate(newProjectile,
            new Vector3(this.GetComponent<Transform>().position.x + 1,
                this.GetComponent<Transform>().position.y + 1, 0), Quaternion.identity);
        newProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 150));
        NetworkServer.Spawn(newProjectile);
    }

    [Command]
    private void CmdDestroyProjectile(GameObject projectileToDestroy)
    {
        NetworkServer.Destroy(projectileToDestroy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.name.Contains("Projectile"))
        {
            this.health -= collision.collider.gameObject.GetComponent<Projectile>().getDamage();
            deadCheck();
            if (isLocalPlayer) CmdDestroyProjectile(collision.collider.gameObject);
            updateLocalUI();
        }
    }

    private void deadCheck()
    {
        if (this.health <= 0 && !isDead && isLocalPlayer)
        {
            deactivate();
            isDead = true;
            CmdSetDead();
        }
    }

    [Command]
    private void CmdSetDead()
    {
        deactivate();
    }

    void updateLocalUI()
    {
        if (isLocalPlayer)
        {
            foreach (Text label in FindObjectOfType<Canvas>().GetComponentsInChildren<Text>())
            {
                if (label.name == "Health") label.text = "Health: " + health;
                else if (label.name == "Fuel") label.text = "Fuel: " + fuel.ToString("F1");
                else if (label.name == "Jetpack") label.text = "Jetpack: " + jetpack.ToString("F1");
            }

        }
    }

    private void activate()
    {
        activated = true;
        this.GetComponent<Rigidbody2D>().simulated = true;
        this.GetComponent<Transform>().localScale = new Vector3(0.3981007f, 0.6221094f, 1f);
        this.GetComponent<SpriteRenderer>().sprite = aliveSprite;
        if (isLocalPlayer) this.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void setTeam(string teamName)
    {
        team = teamName;
    }
    private void deactivate()
    {
        activated = false;
        this.GetComponent<Rigidbody2D>().simulated = false;
        this.GetComponent<Transform>().localScale = new Vector3(0.2f, 0.6f, 1f);
        this.GetComponent<SpriteRenderer>().sprite = deadSprite;
        this.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
