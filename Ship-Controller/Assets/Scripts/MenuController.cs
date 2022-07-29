using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    None,
    Main,
    Option,
}

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void OpenScenePanel()
    {

    }

    public void OpenOptionPanel()
    {

    }

    public void OpenPanel()
    {

    }

    public void QuitGame()
    {
        gameManager.QuitGame();
    }
}
