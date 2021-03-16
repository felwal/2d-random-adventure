using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraform : MonoBehaviour {

    public string biome = "plains";
    float pFlat, pDouble, pTree, pHouse;
    float pUp = 0.5f;
    //int mountainRadius = 25;

    static bool loadingR, loadingL;

    int xPlayer, xMin, xMax;
    int yPlayer, yMin, yMax;
    int yOrigin;
    int ΔTree = 0;

    public static Color[] color = new Color[C.amount];
    public GameObject[] blockPrefab = new GameObject[C.amount];
    Transform terrainParent;
    public GameObject emptyPrefab;

    static Dictionary<bool, GameObject> xDirectionParent = new Dictionary<bool, GameObject>();
    static Dictionary<int, GameObject> x10Parent = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> xParent = new Dictionary<int, GameObject>();

    public static Dictionary<int, Dictionary<int, int>> xyInfo = new Dictionary<int, Dictionary<int, int>>();
    public static Dictionary<int, Dictionary<int, int>> xyBlock = new Dictionary<int, Dictionary<int, int>>();
    public static Dictionary<int, Dictionary<int, GameObject>> xyObject = new Dictionary<int, Dictionary<int, GameObject>>();

    //_______________________________________________________________________________________________________________________
    //_______________________________________________________________________________________________________________________

    void Start() {

        C.BlockPrefab = blockPrefab;      
        terrainParent = GameObject.Find("Terrain").transform;

        switch (biome) {
            case "flat":
                pFlat = 1f;
                pTree = 0;
                break;

            case "plains":
                pFlat = 0.75f;
                pDouble = 0.1f;
                pTree = 0.1f;
                break;

            case "forest":
                pFlat = 0.75f;
                pDouble = 0.1f;
                pTree = 0.5f;
                break;

            case "mountains":
                pFlat = 0.2f;
                pDouble = 0.5f;
                pUp = 0.6f;
                pTree = 0.1f;
                break;

            case "xmountains":
                pFlat = 0.05f;
                pDouble = 0.8f;
                pTree = 0.1f;
                break;

        }

        // set color
        for (int c = 1; c < color.Length; c++) {
            color[c] = blockPrefab[c].GetComponent<SpriteRenderer>().color;
        }

        yOrigin = (int)transform.position.y - 4;
        generateTerrain(yOrigin);

    }
    private void Update() {

        xPlayer = (int)Mathf.Round(transform.position.x);
        xMin = xPlayer - C.renderDistanceX;
        xMax = xPlayer + C.renderDistanceX;

        yPlayer = (int)Mathf.Round(transform.position.y);
        yMin = yPlayer - C.renderDistanceY - 1; if (yMin < 0) yMin = 0;
        yMax = yPlayer + C.renderDistanceY + 1;

        loadTerrain();

    }

    // generate
    void generateTerrain(int yGrass) {

        // create direction parents
        xDirectionParent[true] =    InstantiateEmpty(terrainParent, "+");
        x10Parent[0] =              InstantiateEmpty(terrainParent, "0");
        xDirectionParent[false] =   InstantiateEmpty(terrainParent, "-");

        // instantiate
        for (int x = -C.worldRadius; x <= C.worldRadius; x++) {

            xyInfo[x] = new Dictionary<int, int>();
            xyBlock[x] = new Dictionary<int, int>();
            xyObject[x] = new Dictionary<int, GameObject>();

        }

        // generate
        int yGrassR = yGrass;
        int yGrassL = yGrass;
        for (int x = 0; x <= C.worldRadius; x++)   { yGrassR = generate(yGrassR, x, true); } // right
        for (int x = -1; x >= -C.worldRadius; x--) { yGrassL = generate(yGrassL, x, false); } // left

    }
    int generate(int yGrass, int x, bool right) {

        // create empty parent per 10x
        int x10 = x / 10; if (x > 0 && x % 10 != 0) { x10++; } else if (x < 0 && x % 10 != 0) { x10--; } // avrunda åt rätt håll
        if (x > 0 && x % 10 == 1)       { 
            x10Parent[x10] = InstantiateEmpty(xDirectionParent[right].transform, (x+"-"+(x+9))); 
            } // +
        else if (x < 0 && x % 10 == -1) { 
            x10Parent[x10] = InstantiateEmpty(xDirectionParent[right].transform, (-x+"-"+-(x-9))); 
            } // -
        // create empty parent per x
        xParent[x] = InstantiateEmpty(x10Parent[x10].transform, x.ToString());

        // generate info
        yGrass = nGrass(yGrass, x);
        for (int y = 0; y <= C.worldCeiling; y++) {

            if (y == 0)                                     { xyInfo[x][y] = 4; } // bedrock
            else if (y > yGrass)                            { xyInfo[x][y] = 0; } // 
            else if (y == yGrass)                           { xyInfo[x][y] = 2; } // grass
            else if (yGrass-y > randomNorm(2, 30, 11, 4))   { xyInfo[x][y] = 1; } // stone
            else                                            { xyInfo[x][y] = 3; } // dirt

        }

        // tree
        if (ΔTree >= 2) {
            if ((Mathf.Abs(Mathf.Abs(x)-C.worldRadius) > 5) && random() < pTree) { 
                tree(x, yGrass); 
                ΔTree = 0;
            }
        }
        else { 
            ΔTree++; 
        }
        
        return yGrass;
    }

    // load
    void loadTerrain() {

        // load
        if (!loadingR) { for (int x = xPlayer; x <= xMax; x++) { load(x, true); } } // right 
        if (!loadingL) { for (int x = xPlayer-1; x >= xMin; x--) { load(x, false); } } // left

        // unload (till binärt?)
        for (int y = yMin; y <= yMax; y++) { unloadX(y); } // x
        for (int x = xMin; x <= xMax; x++) { unloadY(x); } // y
        // remove collider & rb


        }
    void load(int x, bool r) {

        if (r) { loadingR = true; }
        else { loadingL = true; }

        // load block
        for (int y = yMin; y <= yMax; y++) {

            if (xyInfo.ContainsKey(x) && xyInfo[x].ContainsKey(y) && xyInfo[x][y] != 0 && 
                (!xyBlock.ContainsKey(x) || !xyBlock[x].ContainsKey(y) || xyBlock[x][y] == 0)) {

                xyObject[x][y] = Instantiate(
                    blockPrefab[xyInfo[x][y]],
                    new Vector2(x, y),
                    Quaternion.identity,
                    xParent[x].transform);

                xyObject[x][y].name = x + ", " + y + " [" + xyInfo[x][y] + "]";
                xyBlock[x][y] = xyInfo[x][y];
           
            }            
        }

        if (r) { loadingR = false; }
        else { loadingL = false; }
    }
    void unloadX(int y) {

        for (int x = xMax+1; Selector.objectExist(x, y); x++) { 
            if (Selector.blockExist(x, y)) { xyBlock[x][y] = 0; }
            Destroy(xyObject[x][y]);
            } // right

        for (int x = xMin-1; Selector.objectExist(x, y); x--) { 
            if (Selector.blockExist(x, y)) { xyBlock[x][y] = 0; }
            Destroy(xyObject[x][y]);
            } // left

    }
    void unloadY(int x) {

        for (int y = yMax+1; Selector.objectExist(x, y); y++) { 
            if (Selector.blockExist(x, y)) { xyBlock[x][y] = 0; }
            Destroy(xyObject[x][y]);
            } // upper

        for (int y = yMin-1; Selector.objectExist(x, y); y--) { 
            if (Selector.blockExist(x, y)) { xyBlock[x][y] = 0; }
            Destroy(xyObject[x][y]);
            } // lower

    }

    // verktyg
    float random() {

        return UnityEngine.Random.Range(0f, 1f);

    }
    int randomInt(int min, int max) {

        return UnityEngine.Random.Range(min, max + 1);

    }
    int randomNorm(int a, int b, int μ, float σ)  {

        float min = 0;
        float rand = random();
        float[] p = new float[b - a + 1]; // probability
        int result = μ;

        for (int y = a; y <= b; y++) {

            // get possibility
            p[y - a] = ((1 / (σ * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-(Mathf.Pow(y - μ, 2) / (2 * Mathf.Pow(σ, 2)))));

            // choose int with rand
            if (rand >= min && rand < min + p[y - a]) { result = y; } // körs nästan aldrig
            min += p[y - a];

        }

        return result;
    }
    int nGrass(int yGrass, int x) {

        float rFlat = random();
        float rUp = random();
        float rDouble = random();

        if (rFlat > pFlat && (x > 1 || x < -1)) { // om inte platt

            // om up 
            if (rUp < pUp || yGrass < 10) {              
                if (rDouble < pDouble) { yGrass += 2; } // om dubbelt
                else { yGrass++; }
            }
            // om ned
            else {               
                if (rDouble < pDouble) { yGrass -= 2; } // om dubbelt
                else { yGrass--; }
            }
        }

        return yGrass;

    }
    void tree(int x, int yGrass) {

        int height = randomNorm(2, 10, 5, 3);
        bool crooked = false;

        // logs
        xyInfo[x][yGrass+1] = 7;
        for (int h = 2; h <= height; h++) {
            
            if (!crooked && random() < 0.2f) {
                float p = random();

                if (p < 0.25f) {
                    x++;
                } // r
                else if (p < 0.5f) {
                    x++; h--;
                } // r up
                else if (p < 0.75f) {
                    x--;
                } // l
                else {
                    x--; h--;
                } // l up

                crooked = true;
            } // sneväxt?
            
            xyInfo[x][yGrass+h] = 7;
        }

        // leaves
        int xLocal = x;
        int yLocal = yGrass + height+1;
        int maxL = randomInt(0, 7);
        Dictionary<int, Dictionary<int, bool>> adjacent = blockAdjacent(xLocal, yLocal);

        xyInfo[xLocal][yLocal] = 8;
        for (int amount = 1; amount < maxL; amount++) {

            // räkna antal lediga
            int emptyAmount = 0; 
            /*for (int i = 0; i < 8; i++) {
                if (adjacent[i] == true) { emptyAmount++; }
            }*/
            
            // slumpa vilken ledig
            float p = random();
            float pTot = 0;
            int chosenOne;
            for (int c = 0; c < emptyAmount; c++) { 
                pTot += 1 / emptyAmount;
                if (p < pTot) { chosenOne = c; }        
            } 

            // placera leaves


        }
    
        


    }
    Dictionary<int, Dictionary<int, bool>> blockAdjacent(int xLocal, int yLocal) {

        Dictionary<int, Dictionary<int, bool>> blocked = new Dictionary<int, Dictionary<int, bool>>();

        for (int x = xLocal-1; x <= xLocal+1; x++) {
            blocked[x] = new Dictionary<int, bool>();

            for (int y = yLocal-1; y <= yLocal+1; y++) {

                if (x != xLocal && y != yLocal && Selector.blockExist(x,y)) {
                    blocked[x][y] = true;
                }     
            }
        }

        return blocked;
    }

    public GameObject InstantiateEmpty(Transform parent, string name) {

        GameObject obj = Instantiate(
            emptyPrefab,
            new Vector2(0, 0),
            Quaternion.identity,
            parent);
        obj.name = name;

        return obj;
    }



}

// spara block info i instanser av en klass?
// random med standardavvikelse, medelvärde
// biomer & mountains & valleys & caves
