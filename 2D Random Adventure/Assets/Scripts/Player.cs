using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float jumpHeight = 2;
    float walkSpeed = 0.14f;
    bool grounded, jump;
    float t = 0;

    Camera cam;
    Color defaultCameraColor;
    public static Transform self;
    Transform selector;
    Vector3 spawnPosition;
    Rigidbody2D rb;

    public static int[] collectables = new int[C.amount];

    //______________________________________________________
    //______________________________________________________

    void Start() {

        for (int id = 1; id < collectables.Length; id++) {
            collectables[id] = 0;
        }

        cam = transform.Find("Main Camera").GetComponent<Camera>();
        selector = transform.Find("selector").transform;
        defaultCameraColor = cam.backgroundColor;
        self = transform;
        spawnPosition = transform.position;

        // set rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
  
    }
    private void FixedUpdate() {

        // jump
        if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && grounded) {
            //jump = true;
            rb.AddForce(new Vector2(0, 200)); // 400 keyDown
            // keep för egen fysik efter denan if
        }       

        // move horizantally
        if (Input.GetKey(KeyCode.D))   { transform.position += new Vector3(walkSpeed, 0); }   // right
        if (Input.GetKey(KeyCode.A))   { transform.position += new Vector3(-walkSpeed, 0); }  // left

        // die & respawn
        if (transform.position.y < -C.renderDistanceY-3) {
            cam.backgroundColor = defaultCameraColor;
            transform.position = spawnPosition;
            selector.localPosition = new Vector3(0, 0, -1);
            jump = false;
            t = 0; 
        }
        else if (transform.position.y < 0) {
            cam.backgroundColor += new Color(-0.01f, -0.01f, -0.01f);
        } // camera color


    }  
    
    // collisions
    private void OnCollisionStay2D(Collision2D other) {

        //ContactPoint2D contact = new ContactPoint2D();

        //bool top = false;
        //if (contact.point.x >= other.transform.position.y) { top = true; } // from top
        /*if (transform.position.y < other.transform.position.y) { } // from bottom
        if (transform.position.x > other.transform.position.x) { } // from right
        if (transform.position.x < other.transform.position.x) { } // from left*/

        if (other.gameObject.tag == "Solid" || other.gameObject.tag == "SemiSolid") {
            grounded = true;
            jump = false;
            t = 0;
        }

    }
    private void OnCollisionExit2D(Collision2D other) {

        if (other.gameObject.tag == "Solid" || other.gameObject.tag == "SemiSolid") {
            grounded = false;
            t = 0;
        }
    }

}

// destroy and remove rigidbodys & colliders