using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public float health;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public bool isFacingLeft;

    public GameData()
    {
        health = 100;
        playerPosition = new Vector3(0, 0, 0);
        playerRotation = Quaternion.identity;
        isFacingLeft = false;
    }
}
