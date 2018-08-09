using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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
    GameObject mapped_obj = null;
    Renderer rend;
    Collider colid;
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    private Color TriggerColor = Color.red;
    private bool dragging = false;
    private float distance;
    private Vector3 mapping_vec;    // store the subjective vector between mapped and mapping object
    DimBoxes.BoundBox boundbox;

    // dealing with raycast-object selection
    LayerMask layer_mask;
    RaycastHit hit_info;

    public void EnableGravity()
    {
        gravity_mode = true;
    }

    public void DisableGravity()
    {
        gravity_mode = false;
    }

    public void IsColliding(bool b)
    {
        is_colliding = b;
    }

    public void SetObjects(GameObject gameObject)
    {
        //save_obj = gameObject;
        mapped_obj = gameObject;
        //Debug.Log("initialized the objects~");
    }

    public void SetMappingDist(Vector3 dist)
    {
        mapping_vec = dist;
    }

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
            if (save_obj == null) save_obj = mapped_obj;
            boundbox = save_obj.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
            if (boundbox != null)
            {
                boundbox.EnableBox();
            }
        }
        
    }

    private void OnMouseDrag()
    {
        if (is_colliding)
        {
            if (save_obj == null) save_obj = mapped_obj;
            boundbox = save_obj.gameObject.GetComponentInParent<DimBoxes.BoundBox>();
            if (boundbox != null)
            {
                boundbox.EnableBox();
            }
        }
    }

    void OnMouseUp()
    {
        if (!gravity_mode && save_mode && (save_obj != null))
        {
            Debug.Log("store data!");
            StoreData(save_obj);

            // Delete object data from the previously mapped object
            if (mapped_obj != null)
            {
                DeleteData(mapped_obj);
            }
            mapped_obj = save_obj;
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
        //Debug.Log(colid.attachedRigidbody.velocity);
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

    public void Init()
    {
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

        // follow the mapped obj (calculate the coordinates here)
        if(mapped_obj != null && !dragging)
        {
            Vector3 n_vec = obj.transform.position - mapped_obj.transform.position;
            if (n_vec != mapping_vec)
            {
                Debug.Log("come in mapping distance management");
                // move the object toward the mapping object
                obj.transform.position = mapped_obj.transform.position + mapping_vec;
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
        mapping_vec = local_pos;
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

    void DeleteData(GameObject gameObject)
    {
        string path = directory_path + "/" + gameObject.name + ".csv";
        string n_path = directory_path + "/n_" + gameObject.name + ".csv";

        // Find the content from the csv file
        string line = null;
        //string line_to_delete = "the line i want to delete";
        using (StreamReader reader = new StreamReader(path))
        {
            using (StreamWriter writer = new StreamWriter(n_path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (String.Compare(line.Split(',')[0], obj.name) == 0)
                        continue;
                     
                    writer.WriteLine(line);
                }
            }
        }
        File.Delete(path);
        File.Move(n_path, path);
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
                DeleteData(mapped_obj);
                mapped_obj = null;
            }
        }
        if(other.gameObject.name == "gravity_field" && gravity_mode == true)
        {
            Debug.Log("gravity mode exit");
            gravity_mode = false;
            save_mode = false;
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
            DeleteData(mapped_obj);
            mapped_obj = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (save_mode && collision.collider.gameObject.tag != "photo_object")
        {
            Debug.Log("collision enter, save!");
            StoreData(save_obj);
            save_mode = false;

            // Delete object data from the previously mapped object
            if (mapped_obj != null)
            {
                DeleteData(mapped_obj);
            }
            mapped_obj = save_obj;
        }
    }
}
