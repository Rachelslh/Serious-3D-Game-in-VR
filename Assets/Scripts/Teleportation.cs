using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using TMPro;

public class Teleportation : MonoBehaviour
{
    [Header ("Player")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject navigator;
    [SerializeField] GameObject reticle;
     
    [Header ("Teleport Targets")]
    [SerializeField] GameObject gate;
    [SerializeField] GameObject teleportTarget;
    [SerializeField] ParticleSystem gateFX;
    [SerializeField] ParticleSystem FX1;

    [Header ("Canvas")]
    [SerializeField] Canvas mover;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI TourText;

    private NavMeshAgent agent;
    private Vector3 offset;
    private Vector3 initialPosition;

    IEnumerator Start()
    {
        initialPosition = mover.transform.position;
        offset = mover.transform.position - player.transform.position;
        TourText.enabled = false;
        agent = player.GetComponent<NavMeshAgent>();

        yield return new WaitForSeconds(0.05f);
        
         navigator.GetComponent<Navigator>().enabled = true;
    }

    IEnumerator MyCoroutine()
    {
        navigator.GetComponent<SpawningWire>().spawnWire(0);
        TourText.enabled = false;
        navigator.GetComponent<Navigator>().enabled = false;
        agent.Warp(teleportTarget.transform.position);
        mover.transform.position = initialPosition;
        timerText.enabled = true;
        scoreText.enabled = true;
        FX1.Play();

        yield return new WaitForSeconds(1);
       
        navigator.GetComponent<Navigator>().enabled = true;
    }

    protected bool PathComplete()
    {
        /*if(Vector3.Distance(gate.transform.position, agent.transform.position) <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                return true;
        }*/
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                 return true;
         
        return false;
    }

    void Update()
    {
        mover.transform.position = player.transform.position + offset;
        if (!PathComplete() && (int)Vector3.Distance(gate.transform.position, agent.transform.position) == 35 && !TourText.enabled)
        {
            gateFX.Play();
            TourText.enabled = true;
            timerText.enabled = false;
            scoreText.enabled = false;
        }
        if (Vector3.Distance(gate.transform.position, agent.transform.position) <= 0.9)
            StartCoroutine(MyCoroutine());
    }
    
}
