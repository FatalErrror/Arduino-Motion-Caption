using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Placer : MonoBehaviour
{
    public Transform Container;

    // Start is called before the first frame update
    void Start()
    {
        Container = GameObject.Find("VFX").transform;


        for (int i = 0; i < Container.childCount; i++)
        {
            Container.GetChild(i).Translate(Vector3.right*i, Space.World); ;
        }
    }
}
