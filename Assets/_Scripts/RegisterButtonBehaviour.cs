using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class RegisterButtonBehaviour : MonoBehaviour
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

    public void OnRegisterButtonPressed()
    {

        FindObjectOfType<NetworkClient>().Register(usernameInput.text, passwordInput.text);

        //if credentials valid move to lobbies scene
        Debug.Log("Register Button Pressed");
        //SceneManager.LoadScene("Lobbies");

        
    }


    public void DisplayError()
    {
        //if credentials fail display error notification of incorrect credentials
        if (errorMessageCoroutine != null)
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
