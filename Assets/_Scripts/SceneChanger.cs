/**
    SceneChanger.cs
    Nabil Babu
    101214336
    Dec 13th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneDestination; 
    

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneDestination);
    }

    public void QuitApplication()
    {
        Application.Quit(); 
    }
}