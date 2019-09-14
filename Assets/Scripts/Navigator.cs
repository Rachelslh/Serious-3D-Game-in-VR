using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

//[ExecuteInEditMode]
public class Navigator : MonoBehaviour
{
    [Header ("RayCasting")]
    [SerializeField] float wallSearchDistance = 5;
    [SerializeField] float offWallDistance = .5f;
    [SerializeField] float groundSearchDistance = 5f;
    [SerializeField] float navMeshSearchDistance = .5f;

    [SerializeField] LayerMask cullingMask = -5;
    [SerializeField] QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Reticle")]
    [SerializeField] GameObject reticle;

    [Header("Agent")]
    [SerializeField] NavMeshAgent agent;

    [Header("Debug")]
    [SerializeField] bool drawLines = true;

    bool _wallHit;
    RaycastHit _wallHitInfo;
    Vector3 _wallHitPosition;
    Vector3 _wallHitNormal;
    Vector3 _wallBouncedPosition;

    bool _groundHit;
    RaycastHit _groundHitInfo;
    Vector3 _groundHitPosition;
    
    bool navMeshHit;
    NavMeshHit navMeshHitInfo;

    Vector3 _targetLocation;

    bool scoreDown = true; 

    private bool _hasTargetLocation;
    public bool hasTargetLocation
    {
        get { return _hasTargetLocation; }
        private set
        {
            if(value != _hasTargetLocation)
            {
                _hasTargetLocation = value;

                if (reticle)
                    reticle.SetActive(_hasTargetLocation);
            }
        }
    }

    [ContextMenu ("Set Agent Destination")]
    public void SetAgentDestination()
    {
        if(agent && hasTargetLocation)
        {
            agent.SetDestination(_targetLocation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _wallHit = Physics.Raycast(transform.position, transform.forward, out _wallHitInfo, wallSearchDistance + offWallDistance, cullingMask, queryTriggerInteraction);
        if (_wallHit)
        {
            _wallHitPosition = _wallHitInfo.point;
            _wallHitNormal = _wallHitInfo.normal;
            _wallBouncedPosition = _wallHitPosition + _wallHitNormal * offWallDistance;
        }
        else
        {
            _wallHitPosition = transform.position + transform.forward * wallSearchDistance;
            //_wallHitNormal = -transform.forward;
            _wallBouncedPosition = _wallHitPosition;
        }
        _groundHit = Physics.Raycast(_wallBouncedPosition, Vector3.down, out _groundHitInfo, groundSearchDistance, cullingMask, queryTriggerInteraction);
        if (_groundHit)
        {
            _groundHitPosition = _groundHitInfo.point;

            hasTargetLocation = navMeshHit = NavMesh.SamplePosition(_groundHitPosition, out navMeshHitInfo, navMeshSearchDistance, NavMesh.AllAreas);
            if (navMeshHit)
            {
                _targetLocation = navMeshHitInfo.position;
                if (reticle)
                    reticle.transform.SetPositionAndRotation(_targetLocation, Quaternion.identity);
            }
        }

        else
        {
            hasTargetLocation = false;
        }
        
        SetAgentDestination();
        if (Camera.main.transform.localRotation.y <= -0.3 || (!reticle.activeInHierarchy && Camera.main.transform.localRotation.y < -0.1))
        {
            GetComponent<SpawningWire>().spawnWire(-1);
            reticle.SetActive(false);
            agent.isStopped = true;
            if(scoreDown)
            {
                if (Scoring.score - 3 >= 0)
                    Scoring.score -= 3;
                else
                    Scoring.score = 0;
                scoreDown = false;
            }
        }
        else if (Camera.main.transform.localRotation.y >= 0.3 || (!reticle.activeInHierarchy && Camera.main.transform.localRotation.y > 0.1))
        {
            GetComponent<SpawningWire>().spawnWire(1);
            reticle.SetActive(false);
            agent.isStopped = true;
            if (scoreDown)
            {
                if (Scoring.score - 3 >= 0)
                    Scoring.score -= 3;
                else
                    Scoring.score = 0;
                scoreDown = false;
            }
        }
        else
        {
            GetComponent<SpawningWire>().spawnWire(0);
            reticle.SetActive(true);
            agent.isStopped = false;
            scoreDown = true;
        }

#if UNITY_EDITOR
        if (drawLines)
        {
            Debug.DrawLine(transform.position, _wallHitPosition, _wallHit ? Color.green : Color.yellow);
            Debug.DrawLine(_wallHitPosition, _wallBouncedPosition, Color.blue);
            if (_groundHit)
                Debug.DrawLine(_wallBouncedPosition, _groundHitPosition, Color.cyan);
            if (_groundHit)
                Debug.DrawLine(_groundHitPosition, navMeshHitInfo.position, Color.white);
        }
#endif

    }

    void OnDrawGizmosSelected()
    {
        if (!_groundHit && !drawLines)
            return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_groundHitPosition, navMeshSearchDistance);
    }

}
