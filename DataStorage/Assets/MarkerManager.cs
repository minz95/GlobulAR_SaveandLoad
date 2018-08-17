using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MarkerManager : MonoBehaviour {
    Renderer[] rend;
    //private bool dragging = false;
    //private float distance;

    /*
    // mouse event will be replaced with hololens gesture interactions
    void OnMouseEnter()
    {
        //rend.material.color = mouseOverColor;

    }

    void OnMouseExit()
    {
        //rend.material.color = originalColor;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    private void OnMouseDrag()
    {

    }

    void OnMouseUp()
    {
        dragging = false;
    }
    */

    // Use this for initialization
    void Start () {
        rend = gameObject.GetComponentsInChildren<Renderer>();
        //var currentAlpha = rend[0].material.color.a;
        /*
        for(int i = 0; i < rend.Length; i++)
        {
            Color color = rend[i].material.color;
            color.a = 0.0f;
            rend[i].material.color = color;
            rend[i].material.shader = Shader.Find("Transparent/Diffuse");
            rend[i].material.renderQueue = 2001;
        }
        */
        //Debug.Log("color alpha: " + rend[0].material.color.a);
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
        */
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
