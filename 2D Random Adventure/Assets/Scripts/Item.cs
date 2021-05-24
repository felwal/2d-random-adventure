using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public int id;
    float t = 0;

    //______________________________________________________
    //______________________________________________________

    void Update () {

        t += Time.deltaTime;

        if (t >= 200 || transform.position.y < 0) {
            Destroy(gameObject);
        } // destroy

    }

    // collisions
    private void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.tag == "Player") {
            Destroy(gameObject);
            Player.collectables[id]++;
        }

    }

}
