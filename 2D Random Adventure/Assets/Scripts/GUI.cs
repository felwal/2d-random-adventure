using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUI : MonoBehaviour {

    public int[] spotBlock = new int[8];
    GameObject[] itemSpot = new GameObject[8];
    Text[] itemText = new Text[8];

    public Transform scrollSelector;
    public static int spotSelected;

    //______________________________________________________
    //______________________________________________________

    void Start () {

        C.SpotBlock = spotBlock;
        scrollSelector.localPosition = new Vector2(-122.5f, 0);
        spotSelected = 1;

        // set spot, text & color
        for (int spot = 0; spot < 8; spot++) {

            itemSpot[spot] = GameObject.Find("item slot (" + spot + ")");
            itemText[spot] = GameObject.Find("item slot (" + spot + ")/Text").GetComponent<Text>();

            itemSpot[spot].GetComponent<RawImage>().color = Terraform.color[spotBlock[spot]];

        }

    }
    void Update () {

        MoveSelected();

        // update text
        for (int spot = 0; spot < 8; spot++) {

            if (Player.collectables[spotBlock[spot]] != 0) {
                itemText[spot].text = Player.collectables[spotBlock[spot]] + "";
            }
            else {
                itemText[spot].text = "";
            }
        }

    }

    // interactions
    private void MoveSelected() {

        if (Input.GetKey(KeyCode.Alpha1)) {
            scrollSelector.localPosition = new Vector2(-122.5f, 0);
            spotSelected = 0;
        } // 0
        if (Input.GetKey(KeyCode.Alpha2)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35, 0);
            spotSelected = 1;
        } // 1
        if (Input.GetKey(KeyCode.Alpha3)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35*2, 0);
            spotSelected = 2;
        } // 2
        if (Input.GetKey(KeyCode.Alpha4)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35*3, 0);
            spotSelected = 3;
        } // 3
        if (Input.GetKey(KeyCode.Alpha5)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35*4, 0);
            spotSelected = 4;
        } // 4
        if (Input.GetKey(KeyCode.Alpha6)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35*5, 0);
            spotSelected = 5;
        } // 5
        if (Input.GetKey(KeyCode.Alpha7)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35*6, 0);
            spotSelected = 6;
        } // 6
        if (Input.GetKey(KeyCode.Alpha8)) {
            scrollSelector.localPosition = new Vector2(-122.5f+35*7, 0);
            spotSelected = 7;
        } // 7

    }

}
