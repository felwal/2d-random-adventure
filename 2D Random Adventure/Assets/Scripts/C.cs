using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class C {

    static float g = -0.5f;
    public static int renderDistanceX = 100; // 34
    public static int renderDistanceY = 25;
    public static int worldRadius = 100;
    public static int worldCeiling = 100;

    public static int amount = 10; // ...of blocks
    public static GameObject[] BlockPrefab = new GameObject[amount];
    public static int[] SpotBlock = new int[8];


    //______________________________________________________
    //______________________________________________________

    // physics
    public static Vector3 GravityPull(float t, float yMax)
    {
        yMax = yMax / 60;// + 0.005f; // till rätt proportioner + marginal
        float v0 = Mathf.Sqrt( (-2)*g*yMax ); // det v0 är när t är det den är när v = 0, dvs vid yMax

        float verticalVelocity = v0 + g*t;

        return new Vector2(0f, verticalVelocity);

    }


}

// saveData