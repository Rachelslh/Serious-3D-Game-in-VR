
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UserConnection : MonoBehaviour
{
    public TextMeshProUGUI status;
    public TMP_InputField inputUser, inputPass, regUsername, regPassword, regEmail, confirmPass;

    private string giantString;
    public string[] registeredUsers;

    public List<string> usernames = new List<string>();
    public List<string> passwords = new List<string>();
    private int currentID;
    private bool takenUsername;

    public static string username = "";

    LoadScenes load;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        load = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LoadScenes>();

        WWW users = new WWW("http://chachaser.000webhostapp.com/ReadUser.php");
        yield return users;

        giantString = users.text;
        Debug.Log(giantString);
        registeredUsers = giantString.Split(';');

        string pattern = @"Username = (.*)\|Password = (.*)";

        for(int i = 0; i < registeredUsers.Length; i++)
        {
            Match m = Regex.Match(registeredUsers[i],pattern);
            usernames.Add(m.Groups[1].Value);
            passwords.Add(m.Groups[2].Value);
        }
     
    }


    public void TryToLogin()
    {
        status.text = "";
        currentID = -1;

        if (inputUser.text == "" || inputPass.text == "")
            status.text = "Veuillez Entrer Votre Pseudo Et Mot De Passe";
        else
        {
            for (int i = 0; i < registeredUsers.Length; i++)
            {
                if (inputUser.text == usernames[i])
                    currentID = i;
            }
            if (currentID == -1)
                status.text = "Utilisateur Inexistant";
            else
            {
                if (inputPass.text == passwords[currentID])
                {
                    username = inputUser.text;
                    load.PlayGame(2);
                }
                else
                    status.text = "Mot De Passe Erroné";
            }
        }
    }

    public void TryToRegister()
    {
        status.text = "";
        takenUsername = false;

        if(regUsername.text == "" || regPassword.text == "" || regEmail.text == "" || confirmPass.text == "")
            status.text = "Veuillez Remplir Les Champs Indiqués";
        else if( regPassword.text != confirmPass.text)
            status.text = "Veuillez Confirmer Votre Mot De Passe Correctement";
        else
        {
            for (int i = 0; i < registeredUsers.Length; i++)
            {
                if (regUsername.text == usernames[i])
                    takenUsername = true;
            }
            if (takenUsername == false)
            {
                Register(regUsername.text, regPassword.text, regEmail.text);
                username = regUsername.text;
                load.PlayGame(2);
            }
            else
                status.text = "Pseudo Déja Existant";
        }

    }

    public void Register(string username, string password, string email)
    {
        WWWForm form = new WWWForm();

        form.AddField("usernamePost", username);
        form.AddField("emailPost", email);
        form.AddField("passwordPost", password);

        WWW register = new WWW("http://chachaser.000webhostapp.com/InsertUser.php", form);
        
    }

    public void OpenRegistration()
    {
        SceneManager.LoadScene(1,LoadSceneMode.Single);
        DestroyImmediate(Camera.main.gameObject);
    }

     public void OpenAuth()
    {
        SceneManager.LoadScene(0,LoadSceneMode.Single);
        DestroyImmediate(Camera.main.gameObject);
    }

}
