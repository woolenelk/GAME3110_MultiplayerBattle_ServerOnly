using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetworkMessages;

public class StartgameButtonBehaviour : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;

    public GameObject errorMessage;
    public float errorMessageDuration;
    IEnumerator errorMessageCoroutine;


    private void Start()
    {
        errorMessage.SetActive(false);
    }

    public void OnStartGameButtonPressed()
    {
        if (FindObjectOfType<NetworkClient>().isHostingPlayer())
        {
            if (Player1.activeInHierarchy && Player2.activeInHierarchy) //make sure we have both player 1 and 2 in the lobby
            {

                FindObjectOfType<NetworkClient>().SendServerStartSignal();

                Debug.Log("Play Button Pressed");
                //SceneManager.LoadScene("Play");


            }
            else
            {
                if (errorMessageCoroutine != null)
                {
                    StopCoroutine(errorMessageCoroutine);

                }
                errorMessageCoroutine = DisplayErrorMessage();
                StartCoroutine(errorMessageCoroutine);
            }
        }
    }

    IEnumerator DisplayErrorMessage()
    {
        errorMessage.SetActive(true);
        yield return new WaitForSeconds(errorMessageDuration);
        errorMessage.SetActive(false);
    }
}
