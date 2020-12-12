using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHandState : MonoBehaviour
{
    public Transform HandTransform;
    public Transform NormalHand;
    public Transform ClosedHand;

    public GameObject[] OpenHandStateObjects;
    public GameObject[] CloseHandStateObjects;

    public bool HandIsClosed { set { _handIsClosed = value; if (_handIsClosed) CloseHand(); else OpenHand(); } get => _handIsClosed; }
    private bool _handIsClosed;

    private void CloseHand()
    {
        foreach (var item in CloseHandStateObjects)
        {
            item.SetActive(true);
        }
        foreach (var item in OpenHandStateObjects)
        {
            item.SetActive(false);
        }

        Transform c, c1;
        for (int i = 0; i < 5; i++)
        {
            c = HandTransform.GetChild(i);
            c1 = ClosedHand.GetChild(i);

            c.localRotation = c1.localRotation;
            c.GetChild(0).localRotation = c1.GetChild(0).localRotation;
            c.GetChild(0).GetChild(0).localRotation = c1.GetChild(0).GetChild(0).localRotation;
        }
    }

    private void OpenHand()
    {
        foreach (var item in OpenHandStateObjects)
        {
            item.SetActive(true);
        }
        foreach (var item in CloseHandStateObjects)
        {
            item.SetActive(false);
        }

        Transform c, c1;
        for (int i = 0; i < 5; i++)
        {
            c = HandTransform.GetChild(i);
            c1 = NormalHand.GetChild(i);

            c.localRotation = c1.localRotation;
            c.GetChild(0).localRotation = c1.GetChild(0).localRotation;
            c.GetChild(0).GetChild(0).localRotation = c1.GetChild(0).GetChild(0).localRotation;
        }
    }

    private void Start()
    {
        HandIsClosed = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) HandIsClosed = !HandIsClosed;
    }
}


