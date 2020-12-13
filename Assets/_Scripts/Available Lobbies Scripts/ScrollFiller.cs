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
    

    // Start is called before the first frame update
    void Start()
    {
        
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
            Destroy(lobby);
        }
    }

    public void GenerateItem(string username, string level = "0")
    {
        GameObject scrollItemObj = Instantiate(scrollItemprefab);
        scrollItemObj.transform.SetParent(scrollContent.transform, false);
        scrollItemObj.transform.Find("PlayerLevel").gameObject.GetComponent<TMP_Text>().text = level;
        scrollItemObj.transform.Find("PlayerName").gameObject.GetComponent<TMP_Text>().text = username;

        scrollView.verticalNormalizedPosition = 1;
    }
}
