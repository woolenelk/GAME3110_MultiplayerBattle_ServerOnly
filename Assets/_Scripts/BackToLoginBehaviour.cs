using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLoginBehaviour : MonoBehaviour
{
    public void OnBackToLoginButtonPressed()
    {
        Debug.Log("Back To Login Button Pressed");
        SceneManager.LoadScene("Login");
    }
}
