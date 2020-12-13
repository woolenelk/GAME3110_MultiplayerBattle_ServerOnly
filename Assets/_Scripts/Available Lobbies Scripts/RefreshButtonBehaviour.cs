using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshButtonBehaviour : MonoBehaviour
{
    public void OnRefreshButtonPressed()
    {
        FindObjectOfType<NetworkClient>().RequestNewLobbies();
    }
}
