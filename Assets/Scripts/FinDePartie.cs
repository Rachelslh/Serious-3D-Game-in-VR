using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class FinDePartie : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI BravoText;

    public static int score = 0;
    public static bool gagner = false;

    LoadScenes load;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        load = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LoadScenes>();

        scoreText.SetText("<sup> <#50aaff>Durée "+ score +"</color></sup>");

        if (!gagner)
            BravoText.text = "<size=100%>V</size >ous <size=100%>A</size>vez <size=100%>E</size>choué !";
        

        yield return new WaitForSeconds(10.0f);

        load.PlayGame(2);
    }

}
