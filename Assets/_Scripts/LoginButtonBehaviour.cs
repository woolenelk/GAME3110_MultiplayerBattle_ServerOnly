using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class LoginButtonBehaviour : MonoBehaviour
{
    public GameObject errorMessage;
    public float errorMessageDuration;
    IEnumerator errorMessageCoroutine;    

    private void Start()
    {
        errorMessage.SetActive(false);
    }

    public void OnLoginButtonPressed()
    {

        NetworkClient networkManager = FindObjectOfType<NetworkClient>();
        networkManager.Login("kevin", "test");

        //if credentials valid move to lobbies scene
        Debug.Log("Login Button Pressed");
        //SceneManager.LoadScene("Lobbies");

        //if credentials fail display error notification of incorrect credentials
        if(errorMessageCoroutine != null)
        {
            StopCoroutine(errorMessageCoroutine);

        }
        errorMessageCoroutine = DisplayErrorMessage();
        StartCoroutine(errorMessageCoroutine);
    }


    IEnumerator DisplayErrorMessage()
    {
        errorMessage.SetActive(true);
        yield return new WaitForSeconds(errorMessageDuration);
        errorMessage.SetActive(false);
    }
}
