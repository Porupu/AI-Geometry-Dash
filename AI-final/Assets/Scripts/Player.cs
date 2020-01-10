using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 spawn;
    public static float moveSpeed = 5; //40
    public static float jumpHeight = 5f;
    public static float maxSpeed = 8.0f;

    public static int brainSize = 300;
    public Vector3[] brain = new Vector3[brainSize]; //brain stores vectors of movement, in our case mainly jumping (300 might be a bit too much)
    public int i = 0; //brain iterator
    public int lifespan = 15; //15 number of steps it takes before dying (increases each generation by the Population.cs)

    public bool reachedGoal = false;
    public bool dead = false;

    public Material alive;
    public Material notAlive;
    public Material champion;
   // public Shader alwaysOnTop; //makes the champion visible over the others

    public float fitness = 0;

    private float distToGround;
    public float distToGoalFromSpawn;
    public bool grounded = true;

    private Rigidbody rigidbody3d;

    private void Awake()
    {
        rigidbody3d = transform.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
       // lifespan = 1000;
        GenerateVectors();
        spawn = transform.position;
        distToGround = GetComponent<Collider>().bounds.extents.y;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            transform.position += new Vector3(1f, 0f, 0f) * Time.deltaTime * moveSpeed;
            if (transform.position.y < -30) Die(); //fall off map?
                                                   //if (transform.position.) .x or .z ?
                                                   //bounds and kill the player when hitting spikes
                                                   // else if (!)
            if (i >= brainSize || i >= lifespan) Die();
        }
    }

    public void MovePlayer()
    {
        if (!dead)
        {
            
            //GetComponent<Rigidbody>().velocity = Vector3.right * moveSpeed;
            //GetComponent<Rigidbody>().AddForce(Vector3.right * moveSpeed);
            //GetComponent<Rigidbody>().AddForce(Vector3.down * jumpHeight/6);
            if (grounded)
            {
                Debug.Log("Grounded");
                rigidbody3d.AddForce(brain[i], ForceMode.Impulse);
                //GetComponent<Rigidbody>().velocity = brain[i] * jumpHeight;
                grounded = false;
            }
            i++;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement
        }
    }
       

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Goal")
        {
            reachedGoal = true;
            Die();
        }
        if (other.gameObject.tag == "Spike")
        {
            Die();
            Debug.Log("Spikes");
        }
        if (other.gameObject.tag == "Ground")
        {
            grounded = true;
        }
        // collision with spikes
    }  
       
    public void Respawn()
    {
        transform.position = spawn;
        GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement in case it was falling
        i = 0;
        dead = false;
        reachedGoal = false;
        GetComponent<Renderer>().material = alive;
    }

    public void Die()
    {
        dead = true;
        GetComponent<Renderer>().material = notAlive;
        GetComponent<Rigidbody>().velocity = Vector3.zero; //stop movement
    }

    public void SetAsChampion()
    {
        GetComponent<Renderer>().material = champion;
        //GetComponent<Renderer>().material.shader = alwaysOnTop;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    private void GenerateVectors()
    {
        for (int j = 0; j < brainSize; j++)
        {
            
            brain[j] = new Vector3(0, Random.Range(0, 2) * jumpHeight, 0); //random vectors, x y z - gonna need to change that to be y to make them jump?     x = Random.Range(-10, 11)
            //Debug.Log(brain[j][1]);
        }
    }
}
