using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    [SerializeField] private string N1;
    [SerializeField] private string N2;
    [SerializeField] private string N3;
    public void jugar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(N1);


    }
    public void nivel2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(N2);


    }
    public void nivel3()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(N3);


    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();


    }
}
