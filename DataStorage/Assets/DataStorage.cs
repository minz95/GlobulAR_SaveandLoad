﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataStorage : MonoBehaviour {
    string persist_path;
    string directory_path;
    public bool isSphere = true;    //whether the object is sphere or plane(2D rectangle)

    // dealing with save/gravity mode
    bool save_mode = false;
    bool gravity_mode = false;
    bool is_colliding = false;

    //string path;
    GameObject obj;
    GameObject save_obj = null;
    Renderer rend;
    Collider colid;
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    private Color TriggerColor = Color.red;
    private bool dragging = false;
    private float distance;
    DimBoxes.BoundBox boundbox;

    // dealing with raycast-object selection
    LayerMask layer_mask;
    RaycastHit hit_info;

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

        if(is_colliding)
        {
            boundbox = save_obj.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
            if (boundbox != null)
            {
                boundbox.EnableBox();
            }
        }

        //DEBUG
        if(colid.isTrigger)
        {
            Debug.Log("isTrigger ON now");
        }
    }

    private void OnMouseDrag()
    {
        if (is_colliding)
        {
            boundbox = save_obj.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
            if (boundbox != null)
            {
                boundbox.EnableBox();
            }
        }
    }

    void OnMouseUp()
    {
        if (save_mode && (save_obj != null))
        {
            Debug.Log("store data!");
            StoreData(save_obj);
            
        }
        else if(gravity_mode && (save_obj != null))
        {
            Debug.Log("save mode set for container");
            save_mode = true;
            colid.isTrigger = false;
            colid.attachedRigidbody.useGravity = true;
        }
        dragging = false;
        boundbox = save_obj.GetComponentInParent<DimBoxes.BoundBox>();
        if (boundbox != null)
        {
            boundbox.UnableBox();
        }
        colid.attachedRigidbody.velocity = Vector3.zero;
        Debug.Log(colid.attachedRigidbody.velocity);
        Debug.Log("mouse up!");
    }

    // Use this for initialization
    // set the storage path
    void Start () {
        persist_path = Application.persistentDataPath;
        directory_path = persist_path + "/mapping_data";
        obj = this.gameObject;
        rend = this.gameObject.GetComponent<Renderer>();
        colid = this.gameObject.GetComponent<Collider>();
        if (!Directory.Exists(directory_path))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(directory_path);
        }

        // occluded by the invisible walls
        rend.material.renderQueue = 2002;
        boundbox = FindObjectOfType<DimBoxes.BoundBox>();
        layer_mask = ~1 << 2;   // ignore the layer 2 ("Ignore Raycast" layer - gravity field)
    }
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragging)
        {
            //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }

        // if clicked some object
        if(Physics.Raycast(ray, out hit_info, layer_mask))
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;

            if (is_colliding)
            {
                boundbox = save_obj.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
                if (boundbox != null)
                {
                    boundbox.EnableBox();
                }
            }
        }

    }
    
    /**
     * StoreData
     * in: GameObject
     * data format: "id(name),position(x,y,z),rotation(w,x,y,z),scale(x,y,z)"
     */
    void StoreData(GameObject gameObject)
    {
        string path = directory_path + "/" + gameObject.name + ".csv";
        string content = "";
        content += obj.name + ",";

        // position inverse transform point
        Vector3 pos = obj.transform.position;
        //Vector3 local_pos = obj.transform.InverseTransformVector(pos - gameObject.transform.position);
        Vector3 local_pos = pos - gameObject.transform.position;
        content += local_pos.x + ",";
        content += local_pos.y + ",";
        content += local_pos.z + ",";

        // rotation
        content += obj.transform.rotation.w + ",";
        content += obj.transform.rotation.x + ",";
        content += obj.transform.rotation.y + ",";
        content += obj.transform.rotation.z + ",";

        // scale
        content += obj.transform.localScale.x + ",";
        content += obj.transform.localScale.y + ",";
        content += obj.transform.localScale.z + ",";

        // primitive type : true: sphere, false: rectangular
        if (isSphere) content += 1;
        else content += 0;

        using (TextWriter writer = File.AppendText(path))
        {
            writer.WriteLine(content);
        }
    }

    void ClearData(string path)
    {
        File.WriteAllText(path, "");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter");
        // if not container, trigger save mode
        if (other.gameObject.tag != "Container" && other.gameObject.tag != "photo_object")
        {
            Transform p = other.gameObject.transform.parent;
            if (p == null || p.gameObject.tag != "Container")
            {
                save_mode = true;
                save_obj = other.gameObject;
                is_colliding = true;
                Debug.Log("save mode set");
            }
        }

        // if container's gravity field, trigger gravity mode
        // object should not be in a save mode (don't touch anything else!)
        if(other.gameObject.name == "gravity_field" && save_mode == false && gravity_mode == false)
        {
            gravity_mode = true;
            save_obj = other.gameObject;
            is_colliding = true;
            Debug.Log("gravity mode set");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger exit");
        if(other.gameObject.tag != "Container" && other.gameObject.tag != "photo_object")
        {
            Transform p = other.gameObject.transform.parent;
            if (p == null || p.gameObject.tag != "Container")
            {
                save_mode = false;
                is_colliding = false;
                Debug.Log("save mode exit");
                if(dragging)
                {
                    boundbox = other.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
                    if (boundbox != null)
                    {
                        boundbox.UnableBox();
                    }
                }
            }
        }
        if(other.gameObject.name == "gravity_field" && save_mode == false && gravity_mode == true)
        {
            Debug.Log("gravity mode exit");
            gravity_mode = false;
            colid.isTrigger = true;
            colid.attachedRigidbody.useGravity = false;
            is_colliding = false;
            if(dragging)
            {
                boundbox = other.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
                if(boundbox != null)
                {
                    boundbox.UnableBox();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision enter, save!");
        if(save_mode && collision.collider.gameObject.tag != "photo_object")
        {
            StoreData(save_obj);
            save_mode = false;
        }
    }
}