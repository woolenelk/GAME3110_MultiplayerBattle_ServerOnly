using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetowrkManagerSpawner : MonoBehaviour
{
    public static NetworkClient networkClient;
    public GameObject networkClientPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //make sure we only ever have one network client
        if(networkClient == null)
        {
            networkClient = Instantiate(networkClientPrefab).GetComponent<NetworkClient>();
        }
    }

}
