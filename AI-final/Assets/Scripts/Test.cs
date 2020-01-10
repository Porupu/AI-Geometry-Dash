using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Rigidbody rigidbody3d;
    public static float moveSpeed = 5; //40
    public static float jumpHeight = 5f;
    public bool grounded = true;

    private void Awake()
    {
        rigidbody3d = transform.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(1f, 0f, 0f) * Time.deltaTime * moveSpeed;

        if (Input.GetMouseButtonDown(0) && grounded)
        {
            rigidbody3d.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
            //GetComponent<Rigidbody>().velocity = brain[i] * jumpHeight;
            grounded = false;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Spike")
        {

            Debug.Log("Spikes");
        }
        if (other.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
}
