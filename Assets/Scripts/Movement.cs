using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private const float MovePerFrame = 0.04f;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            var position = this.transform.position;
            position.x -= MovePerFrame;
            this.transform.position = position;
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            var position = this.transform.position;
            position.x += MovePerFrame;
            this.transform.position = position;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 280));
        }
    }

}
