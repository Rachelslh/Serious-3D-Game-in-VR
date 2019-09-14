using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.AI;
using System;
using System.Text.RegularExpressions;
using System.Collections;

public class Scoring : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, int>> data1 = new Dictionary<string, Dictionary<string, int>>();

    float euclidean_similarity(string person1, string person2)
    {
        List<string> common_ranked_items = new List<string>();

        foreach (KeyValuePair<string, int> entry in data1[person1])
        {
            // do something with entry.Value or entry.Key
            string itm1 = entry.Key;

            foreach (KeyValuePair<string, int> entry2 in data1[person2])
            {
                string itm2 = entry2.Key;
                if (itm1 == itm2)
                {
                    common_ranked_items.Add(itm1);
                }
            }
        }

        var rankings = new List<(int, int)>();

        foreach (string niv in common_ranked_items)
        {
            var c = (data1[person1][niv], data1[person2][niv]);
            rankings.Add(c);
        }
        var ser = (2, 4);
        int a, b, r;
        float sum = 0;
        for (int i = 0; i < rankings.Count; i++)
        {
            ser = rankings[i];
            a = ser.ToTuple().Item1;
            b = ser.ToTuple().Item2;
            r = a - b;
            float distance = Convert.ToSingle(Math.Pow(r, 2));
            sum = sum + distance;
        }
        float retour = 1 / (1 + sum);
        return retour;
    }



    //****************************************************************************************************************
    Dictionary<string, float> recommend(string person, int bound)
    {

        var scores = new List<(float, string)>();
        foreach (KeyValuePair<string, Dictionary<string, int>> other in data1)
        {
            if (other.Key != person)
            {
                var c = (euclidean_similarity(person, other.Key), other.Key);
                scores.Add(c);
            }
        }

        scores.Sort();
        scores.Reverse();

        var scores2 = new List<(float, string)>();
        if (scores.Count < bound)
            bound = scores.Count;
        for (int i = 0; i < bound; i++)
        {
            //scores2[i] = scores[i];
            scores2.Add(scores[i]);
        }

        Dictionary<string, List<(float, List<float>)>> recomms = new Dictionary<string, List<(float, List<float>)>>();
        for (int i = 0; i < scores2.Count; i++)
        {
            float sim = scores2[i].Item1;
            string other = scores2[i].Item2;

            Dictionary<string, int> ranked = new Dictionary<string, int>();
            ranked = data1[other];

            foreach (KeyValuePair<string, int> itm in ranked)
            {
                if (!data1[person].ContainsKey(itm.Key))
                {
                    float weight = sim * itm.Value;
                    if (recomms.ContainsKey(itm.Key))
                    {
                        var kl = recomms[itm.Key];
                        float s = kl[0].Item1;
                        List<float> weights = new List<float>();
                        weights = kl[0].Item2;

                        weights.Add(weight);
                        List<(float, List<float>)> k = new List<(float, List<float>)>();
                        k.Add((s + sim, weights));
                        recomms.Remove(itm.Key);
                        recomms.Add(itm.Key, k);
                    }
                    else
                    {
                        List<(float, List<float>)> k = new List<(float, List<float>)>();
                        List<float> r = new List<float>();
                        r.Add(weight);
                        k.Add((sim, r));
                        recomms.Remove(itm.Key);
                        recomms.Add(itm.Key, k);
                    }
                }

            }
        }
        Dictionary<string, float> recomms2 = new Dictionary<string, float>();

        foreach (KeyValuePair<string, List<(float, List<float>)>> rec in recomms)
        {
            var kl = recomms[rec.Key];
            float sim = kl[0].Item1;
            List<float> item = new List<float>();
            item = kl[0].Item2;
            if (sim == 0)
                sim = 2;
            float som = 0;
            for (int i = 0; i < item.Count; i++)
                som = som + item[i];

            recomms2.Add(rec.Key, som / sim);
        }


        return recomms2;
    }






    #region Text to display

    [Header ("Text")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI scoreText;

    #endregion

    #region variables to calculate with the timer

    private float mainTimer = 0;
    private float timer = 0;
    private float TimerSpeed = 1f;
    public static int score = 0;
    private float FirstTiming;

    #endregion

    #region bools to use
    
    private bool doOnce = false;

    #endregion

    #region variables to calculate with the recommandation

    private float var;
    public GameObject player;
    private NavMeshAgent me;
    private String minutes = "05";
    private String seconds = "00";

    bool re = true;

    private string giantString;
    public string[] registeredUsers;
    public List<string> scores = new List<string>();
    public List<string> cpts = new List<string>();
    int total;
    int nbr_niveau;
    int nbr_partie;
    int scorePredit;
    string username = UserConnection.username;
    bool niv1=true;
    string table;
    bool rej;

    #endregion

    #region end of game and velo mode

    LoadScenes load;
    public static bool velo = false;

    #endregion


    // Start is called before the first frame update
    IEnumerator Start()
    {

        load = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LoadScenes>();

        if (!velo)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            me = player.GetComponent<NavMeshAgent>();
        }

        username = UserConnection.username;
        if (velo)
            table = "Velo";
        else
            table = "Cardio";

        //recuperer toutes les parties du user
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("tablePost", table);

        WWW users = new WWW("http://chachaser.000webhostapp.com/ReadAllCardio.php", form);
        yield return users;

        giantString = users.text;
        if (giantString == "") // la premeiere partie du user
        {
            niv1 = true;
            timerText.text = "Timer     01 :00";
            mainTimer = 60;
        }
        else
        {
            registeredUsers = giantString.Split(';');

            string pattern = @"score = (.*)\|cpt = (.*)";

            for (int j = 0; j < registeredUsers.Length; j++)
            {
                Match m = Regex.Match(registeredUsers[j], pattern);
                scores.Add(m.Groups[1].Value);
                cpts.Add(m.Groups[2].Value);
            }

            //le nbr de parties jouées
            total = int.Parse(cpts[registeredUsers.Length - 2]);
            if (total < 5)
            {
                niv1 = true;
                switch (total)
                {
                    case 1: //partie 2
                        timerText.text = "Timer     02:00";
                        mainTimer = 120;
                        break;
                    case 2:  //partie 3
                        timerText.text = "Timer     03:30";
                        mainTimer = 210;
                        break;
                    case 3:  //partie 4
                        timerText.text = "Timer     03:00";
                        mainTimer = 180;
                        break;
                    case 4: //partie 5
                        timerText.text = "Timer     04:00";
                        mainTimer = 240;
                        break;
                }
            }
            else
            {
                // total ==> max cpt
                //s'il n'a tjr pas gagné la derniere partie , il doit la rejouer
                WWWForm form2 = new WWWForm();
                form2.AddField("usernamePost", username);
                form2.AddField("cptPost", total.ToString());
                form2.AddField("tablePost", table);

                WWW win = new WWW("http://chachaser.000webhostapp.com/RecupScore.php", form2);
                yield return win;

                giantString = win.text;
                if (giantString == "F")
                {
                    //il rejoue la partie
                    rej = true;
                    int scoreDPNP = int.Parse(scores[registeredUsers.Length - 2]);
                    scorePredit = scoreDPNP;
                    niv1 = false;
                }
                else
                {
                    //predir le score de la partie
                    niv1 = false;
                    // ********************************premiere partie d'un niveau*************************************************
                    if (total % 5 == 0)
                    {
                        int scoreDPNP = int.Parse(scores[registeredUsers.Length - 2]); //score de la derniere partie du niveau precedent
                        scorePredit = scoreDPNP + 10;
                    }
                    else
                    {
                        int i = 1;
                        while (i <= total)
                        {
                            int j = i;

                            Dictionary<string, int> p_s = new Dictionary<string, int>();
                            while (i <= total && (i % 5 > 0))
                            {
                                p_s.Add("partie " + (i % 5), int.Parse(scores[i - 1]));
                                i++;
                            }
                            if (i < total + 1 && i % 5 == 0)
                                p_s.Add("partie 5", int.Parse(scores[i - 1]));

                            data1.Add("Niveau " + (j / 5 + 1), p_s);
                            i++;
                        }

                        var nbr = (total / 5) + 1;

                        string qq = "Niveau " + nbr;
                        Dictionary<string, float> resu = recommend(qq, 5);
                        int nbr_p = 5 - resu.Count + 1;
                        string pp = "partie " + nbr_p;
                        scorePredit = (int)resu[pp];
                    }
                    niv1 = false;
                }
                mainTimer = (scorePredit * 30) / 5;
            }

        }
        print(scorePredit);//****************************************verif*************************************
        
        timer = mainTimer;
        FirstTiming = mainTimer;
        calculateTimer();

        player.GetComponent<Teleportation>().enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        //UnityEngine.Debug.Log(niv1);
        if (re == true)
        {
            if (!velo)
            {
                if (Seconds() == 30)
                {
                    score += 5;
                    mainTimer -= 30;
                }
            }
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime * TimerSpeed;
                calculateTimer();

                if (minutes == "00" && seconds == "00")
                {
                    timerText.color = Color.red;

                    re= false;
                    if (!velo)
                        score += 5;
                    //**********************************
                    Enregitrement(true);
                }
            }
            else if (timer <= 0.0f && !doOnce)
            {
                doOnce = true;
            }
        }
        scoreText.text = "Score     " + score.ToString();
    }

    private int Seconds()
    {
        return  (int)(mainTimer - timer);
    }

    private void calculateTimer()
    {
        minutes = Math.Floor(timer / 60).ToString("00");
        seconds = ((int)timer % 60).ToString("00");
       
        timerText.text = "Timer     " + minutes + " : " + seconds;
    }

    public void Register(string username, string score, string table, string win)
    {
        WWWForm form = new WWWForm();

        form.AddField("usernamePost", username);
        form.AddField("scorePost", score);
        form.AddField("tablePost", table);
        form.AddField("winPost", win);

        WWW register = new WWW("http://chachaser.000webhostapp.com/InsertCardio.php", form);
        
    }

    public void RegisterNiv1(string username, string score, string table)
    {
        WWWForm form = new WWWForm();

        form.AddField("usernamePost", username);
        form.AddField("scorePost", score);
        form.AddField("tablePost", table);


        WWW register = new WWW("http://chachaser.000webhostapp.com/InsertCardioNiv1.php", form);

    }

    public void RegisterModif(string username, string score, string cpt, string table)
    {
        WWWForm form = new WWWForm();

        form.AddField("usernamePost", username);
        form.AddField("scorePost", score);
        form.AddField("cptPost", cpt);
        form.AddField("tablePost", table);

        WWW register = new WWW("http://chachaser.000webhostapp.com/ModifCardio.php", form);

    }

    public void Enregitrement(bool partieFinie)
    {
        if (niv1 == false)
        {
            if (rej == true)
            {
                if ((int)timer!=0)
                {
                    UnityEngine.Debug.Log("game over");
                }
                else
                {
                    UnityEngine.Debug.Log("BRAVO !!");
                    FinDePartie.gagner = true;
                    RegisterModif(username, scorePredit.ToString(), total.ToString(), table);
                    UnityEngine.Debug.Log("hayaaaaaaa");
                }
            }
            else
            {
                if (minutes == "00" || seconds == "00")
                {
                    UnityEngine.Debug.Log("game over");
                    Register(username, FirstTiming.ToString(), table, "F");
                }
                else
                {
                    UnityEngine.Debug.Log("BRAVO !!");
                    FinDePartie.gagner = true;
                    Register(username, FirstTiming.ToString(), table, "T");
                }

            }

        }
        else
        {
            if(partieFinie)
                RegisterNiv1(username, FirstTiming.ToString(), table);
            else
                Register(username, FirstTiming.ToString(), table, "F");
        }

        if(partieFinie)
            FinDePartie.gagner = true;
        else
            FinDePartie.gagner = false;
        FinDePartie.score = (int) FirstTiming ;
        load.PlayGame(6);
    }
    
}
