using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;

	public class VeloController : MonoBehaviour {

    [Header ("Physical Components")]

	[SerializeField] private bool frontWheelDrive = true;
	[SerializeField] private bool rearWheelDrive = true;

	//Assign front wheels to 0, rear wheels to 1 and 2
	[SerializeField] private GameObject[] wheelMeshes = new GameObject[2];
	[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[2];

	[SerializeField] private GameObject steeringWheelMesh;

	//Assign gameobject here to easily adjust the center of mass
	[SerializeField] private Transform centerOfMass;

    [Header ("Torque And Steering Values")]

	//Values for power and steering
	[SerializeField] private float maxSteer = 40f;
	[SerializeField] private float maxTorque = 100f;

    public Canvas canvas1;

    #region Values to work with

    private float value = 0;
    private float velocity = 0.0f;

    //private Vector3 offset;
    //public Canvas mover;
    //private Vector3 initialPosition;
    

    private float noMovementThreshold = 0.00001f;
    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    private bool isMoving;

    #endregion

    [Header ("VR Tracking")]

    [SerializeField] bool disablePositionalTrackingOnAwake = true;

    
    [Header ("RigidBody")]
    private Rigidbody rigidbody;

    [Header("Reticle")]
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject reticle2;
    [SerializeField] List<GameObject> chemin1 = new List<GameObject>();
    [SerializeField] List<GameObject> chemin2 = new List<GameObject>();
    [SerializeField] List<GameObject> chemin3 = new List<GameObject>();
    
    private int index = 1;
    private int lastIndex = 0;
    private GameObject g;
    private Vector3 lastPos;

    private bool InChemin1 = true;
    private bool InChemin2 = false;
    private bool InChemin3 = false;

    //Let other scripts see if the object is moving
    public bool IsMoving
    {
        get { return isMoving; }
    }


    private void Awake()
    {

        lastPos = chemin1[0].transform.position;
        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }

    private void Start()
    {

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
            rigidbody.centerOfMass = centerOfMass.localPosition;
        
    }
    

    private void LateUpdate()
    {
        if (Camera.main.transform.localRotation.y <= -0.05f)
        {
                value = Mathf.SmoothDamp(value, -1, ref velocity, 0.1f);
        }
        else if (Camera.main.transform.localRotation.y >= 0.05f)
        {
                value = Mathf.SmoothDamp(value, 1, ref velocity, 0.1f);
        }
        else 
            value = Mathf.SmoothDamp(value, 0, ref velocity, 0.1f);


        Steer(value);

        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        
        Thrust(1);
    }
    
    void Steer(float value)	{
	    //Collider angle
	    float currentSteer = value * maxSteer;
        if (currentSteer > maxSteer)
            currentSteer = maxSteer;
	    wheelColliders[0].steerAngle = currentSteer;
        
	    //Wheel meshes update
	    for(int i = 0; i < wheelColliders.Length; i++) {
	    	Quaternion rot;
	    	Vector3 pos;
	    	wheelColliders[i].GetWorldPose(out pos, out rot);
	    	wheelMeshes[i].transform.position = pos;
	    	wheelMeshes[i].transform.rotation = rot;
        
              }
        
	    if(steeringWheelMesh) {
	    	steeringWheelMesh.transform.localEulerAngles = new Vector3(steeringWheelMesh.transform.localEulerAngles.x, currentSteer, steeringWheelMesh.transform.localEulerAngles.z);
	    }
		}

	void Thrust(float value) {
		float torque = value * maxTorque;

		if(frontWheelDrive) {
			wheelColliders[0].motorTorque = torque;
		}

		if(rearWheelDrive) 
			wheelColliders [1].motorTorque = torque;
	}
    
   private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject == reticle || collision.gameObject == reticle2)
        {

            Scoring.score += 2;

            g = getCurrentPosReticle(collision.gameObject);
            lastPos = g.transform.position;
            //First Turn
            if (g.Equals(chemin1[9]))
            {
                if (lastIndex == 0 || lastIndex == 10)
                {
                    reticle2.SetActive(false);
                    lastIndex = index;
                    reticle.transform.SetParent(chemin1[index - 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin2[0].transform, false);
                    reticle.transform.SetParent(chemin1[10].transform, false);
                }
                lastIndex = 9;
            }
            else if (g.Equals(chemin2[0])) {
                if (lastIndex == 9 || lastIndex == 10)
                {
                    reticle2.SetActive(false);
                    lastIndex = index;
                    reticle.transform.SetParent(chemin2[index + 1].transform, false);
                    InChemin1 = false;
                    InChemin2 = true;
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin1[9].transform, false);
                    reticle.transform.SetParent(chemin1[10].transform, false);
                    InChemin2 = false;
                    InChemin1 = true;
                }
                lastIndex = 0;
            }
            else if (g.Equals(chemin1[10])) {
                if (lastIndex == 0 || lastIndex == 9)
                {
                    reticle2.SetActive(false);
                    lastIndex = index;
                    reticle.transform.SetParent(chemin1[index + 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin2[0].transform, false);
                    reticle.transform.SetParent(chemin1[9].transform, false);
                }
                lastIndex = 10;
            }

            //Second Turn
            else if (g.Equals(chemin1[12])) {
                if (lastIndex == 0 || lastIndex == 13)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin1[index - 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin2[0].transform, false);
                    reticle.transform.SetParent(chemin1[13].transform, false);
                }
                lastIndex = 12;
            }
            else if (g.Equals(chemin1[13])) {
                if (lastIndex == 0 || lastIndex == 12)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin1[index + 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin2[0].transform, false);
                    reticle.transform.SetParent(chemin1[12].transform, false);
                }
                lastIndex = 13;
            }
            else if (g.Equals(chemin3[0])) {
                if (lastIndex == 12 || lastIndex == 13)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin3[index + 1].transform, false);
                    InChemin3 = true;
                    InChemin1 = false;
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin1[12].transform, false);
                    reticle.transform.SetParent(chemin1[13].transform, false);
                    InChemin3 = false;
                    InChemin1 = true;
                }
                lastIndex = 0;
            }

            //Third Tour
            else if (g.Equals(chemin1[15])) {
                if (lastIndex == 7 || lastIndex == 16)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin1[index - 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin3[7].transform, false);
                    reticle.transform.SetParent(chemin1[16].transform, false);
                }
                lastIndex = 15;
            }
            else if (g.Equals(chemin1[16])) {
                if (lastIndex == 7 || lastIndex == 15)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin1[index + 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin3[7].transform, false);
                    reticle.transform.SetParent(chemin1[15].transform, false);
                }
                lastIndex = 16;
            }
            else if (g.Equals(chemin3[7])) {
                if (lastIndex == 15 || lastIndex == 16)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin3[index - 1].transform, false);
                    InChemin3 = true;
                    InChemin1 = false;
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin1[15].transform, false);
                    reticle.transform.SetParent(chemin1[16].transform, false);
                    InChemin3 = false;
                    InChemin1 = true;
                }
                lastIndex = 7;
            }

            //Fourth Tour
            else if (g.Equals(chemin1[17])) {
                if (lastIndex == 18 || lastIndex == 7)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin1[index - 1].transform, false);
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin2[7].transform, false);
                    reticle.transform.SetParent(chemin1[18].transform, false);
                }
                lastIndex = 17;
            }
            else if (g.Equals(chemin1[18])) {
                if (lastIndex == 17 || lastIndex == 7)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin1[0].transform, false);
                    lastIndex = -1;
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin2[7].transform, false);
                    reticle.transform.SetParent(chemin1[17].transform, false);
                    lastIndex = 18;
                }
            }
            else if (g.Equals(chemin2[7])) {
                if (lastIndex == 17 || lastIndex == 18)
                {
                    reticle2.SetActive(false);
                    reticle.transform.SetParent(chemin2[index - 1].transform, false);
                    InChemin2 = true;
                    InChemin1 = false;
                }
                else
                {
                    reticle2.SetActive(true);
                    reticle2.transform.SetParent(chemin1[17].transform, false);
                    reticle.transform.SetParent(chemin1[18].transform, false);
                    InChemin2 = false;
                    InChemin1 = true;
                }
                lastIndex = 7;
            }
            else
            {
                reticle2.SetActive(false); 
                if (lastIndex > index)
                {
                    if (InChemin1) {
                        if (lastIndex == 1)
                        {
                            reticle.transform.SetParent(chemin1[18].transform, false);
                            index = 19;
                        }
                        else
                            reticle.transform.SetParent(chemin1[index - 1].transform, false);
                    }
                    else if (InChemin2)
                        reticle.transform.SetParent(chemin2[index - 1].transform, false);
                    else if (InChemin3)
                        reticle.transform.SetParent(chemin3[index - 1].transform, false);
                }
                else
                {
                    if (InChemin1)
                        reticle.transform.SetParent(chemin1[index + 1].transform, false);
                    else if (InChemin2)
                        reticle.transform.SetParent(chemin2[index + 1].transform, false);
                    else if (InChemin3)
                        reticle.transform.SetParent(chemin3[index + 1].transform, false);
                }
                lastIndex = index;
            }
        }
        else
        {
            rigidbody.isKinematic = true;
            transform.position = lastPos;
            Steer(0);
            rigidbody.isKinematic = false;
        }
    }

    private GameObject getCurrentPosReticle(GameObject reticle)
    {
        foreach (GameObject g in chemin1)
        {
            if (reticle.transform.position == g.transform.position)
            {
                index = chemin1.IndexOf(g);
                return (g);
            }
        }
        foreach (GameObject g in chemin2)
        {
            if (reticle.transform.position == g.transform.position)
            {
                index = chemin2.IndexOf(g);
                return (g);
            }
        }
        foreach (GameObject g in chemin3)
        {
            if (reticle.transform.position == g.transform.position)
            {
                index = chemin3.IndexOf(g);
                return (g);
            }
        }
        return null;
    }
}
