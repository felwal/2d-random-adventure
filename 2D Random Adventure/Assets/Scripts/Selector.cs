using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {

    readonly int z = -1;
    int x, lastX, xLocalInt;
    int y, lastY, yLocalInt;
    float xLocal, yLocal;

    float t = 0;
    public int range = 3;
    public float toolEfficiency = 1;
    static float[] blockHardness;
    static int[] blockDrop;

    Transform itemParent;

    //______________________________________________________
    //______________________________________________________

    void Start() {

        blockHardness = new float[] { 0,
            2f,             // 1_stone
            0.5f,           // 2_grass
            0.5f,           // 3_dirt
            Mathf.Infinity, // 4_bedrock
            0.1f,           // 5_air
            2f,             // 6_cobblestone
            1f,             // 7_log
            0.2f,           // 8_leaves
            1f              // 9_wood
        };
        blockDrop = new int[] { 0,
            6,
            3,
            3,
            4,
            5,
            6,
            9,
            8,
            9
        };

        transform.localPosition = new Vector3Int(0, 0, z);
        itemParent = GameObject.Find("Items").transform;

        lastX = x;
        lastY = y;

    }
	void FixedUpdate() {

        x = (int)Mathf.Round(transform.position.x);
        y = (int)Mathf.Round(transform.position.y);
        xLocal = transform.localPosition.x;
        yLocal = transform.localPosition.y;
        xLocalInt = (int)Mathf.Round(xLocal);
        yLocalInt = (int)Mathf.Round(yLocal);

        MoveSelector();
        LightBlock();
        BreakBlock();
        PlaceBlock();

    }

    // interactons
    private void MoveSelector() {

        // move float
        if (Input.GetKey(KeyCode.RightArrow) && xLocal < range) {
            transform.localPosition += new Vector3(0.2f, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && xLocal > -range) {
            transform.localPosition += new Vector3(-0.2f, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow) && yLocal < range)  {
            transform.localPosition += new Vector3(0, 0.2f, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow) && yLocal > -range) {
            transform.localPosition += new Vector3(0, -0.2f, 0);
        }

        // round to int
        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            transform.localPosition = new Vector3Int(xLocalInt, yLocalInt, z);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            transform.localPosition = new Vector3Int(xLocalInt, yLocalInt, z);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            transform.localPosition = new Vector3Int(xLocalInt, yLocalInt, z);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            transform.localPosition = new Vector3Int(xLocalInt, yLocalInt, z);
        }

    }
    private void LightBlock() {

        // change lightning
        if (lastX != x || lastY != y) { // när flyttas till ny koordinat

            // highlight
            if (BlockReachable(x, y)) {
                Color colorSelected = Terraform.color[Terraform.xyBlock[x][y]] + new Color(0.1f, 0.1f, 0.1f);
                Terraform.xyObject[x][y].GetComponent<SpriteRenderer>().color = colorSelected;
            }

            // downlight
            if (BlockExist(lastX, lastY)) {
                Terraform.xyObject[lastX][lastY].GetComponent<SpriteRenderer>().color = Terraform.color[Terraform.xyBlock[lastX][lastY]];
            }

            // set new last
            lastX = x;
            lastY = y;
        }

    }
    private void BreakBlock() {

        // cancel breaking
        if (Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.LeftAlt)) { // eller om flyttar till annat block
            if (BlockBreakable(x, y)) {
                Color colorSelected = Terraform.color[Terraform.xyBlock[x][y]] + new Color(0.1f, 0.1f, 0.1f);
                Terraform.xyObject[x][y].GetComponent<SpriteRenderer>().color = colorSelected;
            }
            t = 0;
        }

        // break...
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt)) && BlockBreakable(x,y)) {

            t += Time.deltaTime;

            // darken block (breaking...)
            float breakTime = blockHardness[Terraform.xyBlock[x][y]] / toolEfficiency;
            float ΔC = -0.004f / breakTime;
            Terraform.xyObject[x][y].GetComponent<SpriteRenderer>().color += new Color(ΔC, ΔC, ΔC);

            // drop & break
            if (t >= breakTime) {

                // drop
                GameObject drop = Instantiate(
                    C.BlockPrefab[blockDrop[Terraform.xyInfo[x][y]]],
                    new Vector2(x, y),
                    Quaternion.identity,
                    itemParent);

                // make drop drop
                drop.name = x + ", " + y + " [" + blockDrop[Terraform.xyInfo[x][y]] + "]";
                drop.transform.localScale = new Vector3(0.5f, 0.5f);
                drop.GetComponent<BoxCollider2D>().enabled = true; // kanske ta bort platform effector & edge collider också?
                drop.GetComponent<Rigidbody2D>().simulated = true;
                drop.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                drop.AddComponent<Item>().id = blockDrop[Terraform.xyInfo[x][y]];
                Destroy(drop.GetComponent<Block>());

                // break
                Terraform.xyInfo[x][y] = 0;
                Terraform.xyBlock[x][y] = 0;
                Destroy(Terraform.xyObject[x][y]);
                t = 0;
            }

        }

    }
    private void PlaceBlock() {

        // place block
        if (Input.GetKey(KeyCode.End) && !BlockExist(x, y) && Player.collectables[C.SpotBlock[GUI.spotSelected]] > 0 && (xLocal != 0 || yLocal != 0)) {

            Terraform.xyObject[x][y] = Instantiate(
                C.BlockPrefab[C.SpotBlock[GUI.spotSelected]],
                new Vector2(x, y),
                Quaternion.identity,
                Terraform.xParent[x].transform);

            Terraform.xyInfo[x][y] = C.SpotBlock[GUI.spotSelected];
            Terraform.xyBlock[x][y] = C.SpotBlock[GUI.spotSelected];
            Terraform.xyObject[x][y].name = x + ", " + y + " [" + Terraform.xyInfo[x][y] + "]";

            Player.collectables[C.SpotBlock[GUI.spotSelected]]--;

        }

        // jetpack
        if (Input.GetKey(KeyCode.PageUp) && !BlockExist(x, y) && (xLocal != 0 || yLocal != 0)) {

            Terraform.xyObject[x][y] = Instantiate(
                C.BlockPrefab[5],
                new Vector2(x, y),
                Quaternion.identity,
                Terraform.xParent[x].transform);

            Terraform.xyInfo[x][y] = 5;
            Terraform.xyBlock[x][y] = 5;
            Terraform.xyObject[x][y].name = x + ", " + y + " [" + Terraform.xyInfo[x][y] + "]";
        }

    }

    // block status
    public static bool BlockBreakable(int x, int y) {

        if (BlockReachable(x, y) && Terraform.xyBlock[x][y] != 4) {
            return true;
        }
        else { return false; }

    }
    public static bool BlockReachable(int x, int y) {

        if (BlockExist(x, y) && !BlockBlocked(x, y)) {
            return true;
        }
        else { return false; }

    }
    public static bool BlockBlocked(int x, int y) {

        if (BnfoExist(x + 1, y) && BnfoExist(x - 1, y) && BnfoExist(x, y + 1) && BnfoExist(x, y - 1)) {
            return true;
        }
        else { return false; }

    }
    public static bool BlockExist(int x, int y) {

        if (Terraform.xyBlock.ContainsKey(x) && Terraform.xyBlock[x].ContainsKey(y) && Terraform.xyBlock[x][y] != 0) {
            return true;
        }
        else { return false; }

    }
    public static bool BnfoExist(int x, int y) {

        if (Terraform.xyInfo.ContainsKey(x) && Terraform.xyInfo[x].ContainsKey(y) && Terraform.xyInfo[x][y] != 0) { // hmmm
            return true;
        }
        else { return false; }

    }
    public static bool ObjectExist(int x, int y) {

        if (Terraform.xyObject.ContainsKey(x) && Terraform.xyObject[x].ContainsKey(y)) {
            return true;
        }
        else { return false; }

    }

}
