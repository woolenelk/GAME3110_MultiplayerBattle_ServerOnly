using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileScript : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<NetworkClient>().RequestPlayerInfo();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        NetworkObjects.Item player = FindObjectOfType<NetworkClient>().Player;
        nameText.text = player.UserID + "  |   LEVEL : " + (int)(int.Parse(player.Wins)/10);
    }
}
