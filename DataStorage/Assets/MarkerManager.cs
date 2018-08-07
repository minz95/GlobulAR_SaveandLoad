using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MarkerManager : MonoBehaviour {
    Renderer[] rend;


	// Use this for initialization
	void Start () {
        rend = gameObject.GetComponentsInChildren<Renderer>();

	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnCollisionExit(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

}
