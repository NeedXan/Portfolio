using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boundsObjects;

    public static RoomManager instance;

    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
}
