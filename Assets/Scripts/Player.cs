using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    private readonly float FuelPerJump = 1.0f;
    private readonly float FuelPerMoveFrame = 0.1f;
    private readonly float JetpackMaxYVelocity = 10;
    private readonly float JetpackPerFrame = 0.3f;
    private readonly float JetpackYVelocityPerFrame = 20;
    private bool activated = true;
    [SerializeField] private Sprite aliveSprite;
    [SerializeField] private Sprite deadSprite;
    private float fuel = 150f;
    private float health = 100f;
    private bool isDead;
    private float jetpack = 80f;
    [SerializeField] private readonly float MovePerFrame = 0.055f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject selectButton;

    // Start is called before the first frame update
    private void Start()
    {
        if (activated) activate();
    }

    [Command]
    private void CmdSpawnServerControls()
    {
        selectButton = Instantiate(selectButton, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(selectButton, connectionToClient);
        selectButton.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    // Update is called once per frame
    private void Update()
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
                var position = transform.position;
                position.x -= MovePerFrame;
                fuel -= FuelPerMoveFrame;
                transform.position = position;
                updateLocalUI();
            }
            else if (Input.GetKey(KeyCode.RightArrow) && fuel > 0)
            {
                var position = transform.position;
                position.x += MovePerFrame;
                fuel -= FuelPerMoveFrame;
                transform.position = position;
                updateLocalUI();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && GetComponent<Rigidbody2D>().velocity.y == 0 &&
                fuel > 0)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 280));
                fuel -= FuelPerJump;
                updateLocalUI();
            }

            if (Input.GetKeyDown(KeyCode.Space)) CmdSpawnProjectile();

            if (Input.GetKey(KeyCode.DownArrow) && GetComponent<Rigidbody2D>().velocity.y != 0 &&
                jetpack > 0)
                if (GetComponent<Rigidbody2D>().velocity.y < JetpackMaxYVelocity)
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JetpackYVelocityPerFrame));
                    jetpack -= JetpackPerFrame;
                    updateLocalUI();
                }

            if (Input.GetKeyDown(KeyCode.X)) // Debug controls
            {
                fuel += 100;
                jetpack += 100;
                updateLocalUI();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                transform.position = Vector3.zero;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && isDead)
        {
            health = 100;
            activate();
            CmdSetAlive();
        }
    }

    [Command]
    private void CmdSpawnProjectile()
    {
        var newProjectile = projectile;
        newProjectile = Instantiate(newProjectile,
            new Vector3(GetComponent<Transform>().position.x + 0.5f,
                GetComponent<Transform>().position.y + 0.5f, 0), Quaternion.identity);
        newProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(225, 150));
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
            health -= collision.collider.gameObject.GetComponent<Projectile>().getDamage();
            deadCheck();
            CmdDestroyProjectile(collision.collider.gameObject);
            updateLocalUI();
        }
    }

    private void deadCheck()
    {
        if (health <= 0 && !isDead)
        {
            deactivate();
            if (isLocalPlayer) CmdSetDead();
        }
    }

    [Command]
    private void CmdSetDead()
    {
        deactivate();
    }

    [Command]
    private void CmdSetAlive()
    {
        activate();
    }

    private void updateLocalUI()
    {
        if (isLocalPlayer)
        {
            if (health < 0) health = 0;
            if (fuel < 0) fuel = 0;
            if (jetpack < 0) jetpack = 0;
            foreach (var label in FindObjectOfType<Canvas>().GetComponentsInChildren<Text>())
            {
                if (label.name == "Health") label.text = "Health: " + health;
                if (label.name == "Fuel") label.text = "Fuel: " + fuel.ToString("F1");
                if (label.name == "Jetpack") label.text = "Jetpack: " + jetpack.ToString("F1");
            }
        }
    }

    private void activate()
    {
        activated = true;
        isDead = false;
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<SpriteRenderer>().sprite = aliveSprite;
        if (isLocalPlayer) GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void deactivate()
    {
        activated = false;
        isDead = true;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<SpriteRenderer>().sprite = deadSprite;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}