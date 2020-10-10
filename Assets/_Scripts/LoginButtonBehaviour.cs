using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginButtonBehaviour : MonoBehaviour
{
    public void OnLoginButtonPressed()
    {
        Debug.Log("Login Button Pressed");
        SceneManager.LoadScene("Play");
    }
}
