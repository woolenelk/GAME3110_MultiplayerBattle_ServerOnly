using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollFiller : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject scrollContent;
    public GameObject scrollItemprefab;
    public NetworkClient networkClient;
    

    // Start is called before the first frame update
    void Start()
    {
        networkClient = FindObjectOfType<NetworkClient>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ClearLobbies()
    {
        var lobbies = FindObjectsOfType<JoinablePlayer>();
        foreach (var lobby in lobbies)
        {
            Destroy(lobby.gameObject);
        }
    }

    public void GenerateItem(NetworkObjects.Lobby lobby, int Wins = -1)
    {
        GameObject scrollItemObj = Instantiate(scrollItemprefab);
        scrollItemObj.transform.SetParent(scrollContent.transform, false);
        scrollItemObj.transform.Find("PlayerLevel").gameObject.GetComponent<TMP_Text>().text = ((int)(Wins / 10)).ToString();
        scrollItemObj.transform.Find("PlayerName").gameObject.GetComponent<TMP_Text>().text = lobby.Player1;
        Color newColor = Color.white;
        if(Wins - (int.Parse(networkClient.Player.Wins) /3) > 5 ) //5 level or more greater
        {
            newColor = Color.red;
        }
        else if(Wins - (int.Parse(networkClient.Player.Wins) / 3) > 3) //more than 3 levels 
        {
            newColor = Color.yellow;
        }
        else if (Wins - (int.Parse(networkClient.Player.Wins) / 3) > -2) //no more than 2 levels below us
        {
            newColor = Color.green;
        }
        scrollItemObj.transform.Find("Background").gameObject.GetComponent<Image>().color = newColor;

        scrollItemObj.transform.GetComponent<JoinablePlayer>().AvailableLobby = lobby;

        scrollView.verticalNormalizedPosition = 1;
    }
}
