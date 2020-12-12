using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //public bool CloseHand = false;

    public Material NormalMaterial;
    public Material HightlightMaterial;

    public Transform HandTransform;
    public Transform NormalHand;
    public Transform ClosedHand;

    public bool HandState = false;

    private Transform _objectTransform;
    private Transform _objectParantTransform;


    private void Start()
    {
        //Debug.Log(CloseHand+" - "+HandState+" - "+(CloseHand != HandState));
        //Debug.Log(HandState);
        //Debug.Log();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _objectTransform != null)
        {
            _objectTransform.GetComponent<Rigidbody>().isKinematic = true;
            _objectTransform.parent = transform;
        }

        if (Input.GetKeyUp(KeyCode.Space) && _objectTransform != null)
        {
            _objectTransform.parent = _objectParantTransform;
            _objectTransform.GetComponent<Rigidbody>().isKinematic = false;
        }

        
         //DoHand(CloseHand);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Movable" && _objectTransform == null)
        {
            if (_objectTransform != null)
            {
                _objectTransform.GetComponent<MeshRenderer>().material = NormalMaterial;
            }

            _objectTransform = other.transform;
            if (!_objectTransform.GetComponent<Rigidbody>().isKinematic)
                _objectParantTransform = _objectTransform.parent;

            _objectTransform.GetComponent<MeshRenderer>().material = HightlightMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == _objectTransform)
        {
            _objectTransform.GetComponent<MeshRenderer>().material = NormalMaterial;
            _objectTransform = null;
        }
    }

    public void DoHand(bool t)
    {
        //Start();
        //Debug.Log("t: "+t);
        if (t != HandState)
        {
            //Start();
            //Debug.Log("t: " + t);
            HandState = t;
            //Start();
            if (t)
            {
                Transform c, c1;
                for (int i = 0; i < 5; i++)
                {
                    c = HandTransform.GetChild(i);
                    c1 = ClosedHand.GetChild(i);

                    c.localRotation = c1.localRotation;
                    c.GetChild(0).localRotation = c1.GetChild(0).localRotation;
                    c.GetChild(0).GetChild(0).localRotation = c1.GetChild(0).GetChild(0).localRotation;
                }

                if (_objectTransform != null)
                {
                    _objectTransform.GetComponent<Rigidbody>().isKinematic = true;
                    _objectTransform.parent = transform;
                }

            }
            else
            {
                Transform c, c1;
                for (int i = 0; i < 5; i++)
                {
                    c = HandTransform.GetChild(i);
                    c1 = NormalHand.GetChild(i);

                    c.localRotation = c1.localRotation;
                    c.GetChild(0).localRotation = c1.GetChild(0).localRotation;
                    c.GetChild(0).GetChild(0).localRotation = c1.GetChild(0).GetChild(0).localRotation;
                }

                if (_objectTransform != null)
                {
                    _objectTransform.parent = _objectParantTransform;
                    _objectTransform.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }
    
}
