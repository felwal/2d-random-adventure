using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    int x, distanceX;
    int y, distanceY;
    float t = 0;

    Collider2D cd;
    Rigidbody2D rb;

    //________________________________________________
    //________________________________________________

    private void Start() {

        x = (int)transform.position.x;
        y = (int)transform.position.y;

        cd = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();


    }
    void Update() {

        // destroy after 0.2
        t += Time.deltaTime;
        if (t >= 0.2) { 
            Terraform.xyInfo[x][y] = 0;
            Terraform.xyBlock[x][y] = 0;
            Destroy(gameObject);
            }

        //distanceX = (int)Mathf.Abs(Player.self.position.x - x);
        //distanceY = (int)Mathf.Abs(Player.self.position.y - y - 1);


        /*
        // out of view - destroy 
        if (distanceX > Control.renderDistanceX || distanceY > Control.renderDistanceY /*|| Terraform.xyInfo[x][y] == 0) {

            Terraform.xyBlock[x][y] = 0;
            Destroy(gameObject);
        }*/

        
        // remove collider
        //if (distanceX > 4 || distanceY > 4)    { cd.enabled = false; rb.simulated = false; }
        //else                                   { cd.enabled = true; rb.simulated = true; }
        

	}


}
