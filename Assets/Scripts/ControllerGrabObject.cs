using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;

    private void lg(string l)
    {
        Debug.Log(l);
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        lg("SetCol");
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    private void GrabObject()
    {
        lg("grab");
        objectInHand = collidingObject;
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        lg("Release");
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        objectInHand = null;
    }


    void Update ()
    {
		if (Controller.GetHairTriggerDown())
        {
            lg("trigD");
            if (collidingObject)
            {
                GrabObject();
            }
        }

        if (Controller.GetHairTriggerUp())
        {
            lg("trigU");
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        lg("enter");
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        lg("stay");
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        lg("exit");
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }
}
