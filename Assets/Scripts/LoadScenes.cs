
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR;

public class LoadScenes : MonoBehaviour
{
    private bool veloMode = false;

    public LoadScenes()
    {

    }

    
    public void enableVelo()
    {
        veloMode = true;
    }
    
    public void PlayGame(int i)
    {
       switch (i)
        {
            case 2:
            case 5:
                QualitySettings.antiAliasing = 4;
                break;
            case 4:
                if (veloMode)
                    Scoring.velo = true;
                QualitySettings.antiAliasing = 0;
                break;
            case 3:
                QualitySettings.antiAliasing = 2;
                break;
        }
        SceneManager.LoadScene(i);
        /*GameObject[] g = SceneManager.GetActiveScene().GetRootGameObjects();
        SceneManager.LoadSceneAsync(i,LoadSceneMode.Single);
        for (int j = 0; j < g.Length; j++)
        {
            DestroyImmediate(g[j]);
        }*/
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
