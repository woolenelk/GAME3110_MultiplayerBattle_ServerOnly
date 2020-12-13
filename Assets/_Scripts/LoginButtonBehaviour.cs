using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class LoginButtonBehaviour : MonoBehaviour
{
    public GameObject errorMessage;
    public float errorMessageDuration;
    IEnumerator errorMessageCoroutine;

    [SerializeField]
    TMP_InputField usernameInput;
    
    [SerializeField]
    TMP_InputField passwordInput;

    private void Start()
    {
        errorMessage.SetActive(false);
    }

    public void OnLoginButtonPressed()
    {

        NetworkClient networkManager = FindObjectOfType<NetworkClient>();
        networkManager.Login(usernameInput.text, passwordInput.text);


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
