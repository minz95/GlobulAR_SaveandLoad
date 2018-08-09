using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StorageManager : MonoBehaviour {
    string persist_path;
    string path;
    public List<GameObject> gameobjs;    // store the instantiated objects here?

    // Use this for initialization
    void Start () {
        //persist_path = Application.persistentDataPath;
        //path = Path.Combine(persist_path, "/mapping_data");
    }

    void Awake()
    {
        persist_path = Application.persistentDataPath;
        path = persist_path + "/mapping_data";

        // load data
        LoadData();
    }

    // Update is called once per frame
    void Update () {
		
	}


    // TODO: attach the photo_object tag when loading
    //       
    void LoadData()
    {
        foreach (string file in System.IO.Directory.GetFiles(path))
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = null;
                    string[] values = null;

                    // parse and instantiate the object
                    while ((line = sr.ReadLine()) != null)
                    {
                        values = line.Split(',');
                        if (values.Length != 12) continue;

                        string file_name = Path.GetFileName(file);
                        file_name = file_name.Split('.')[0];
                        //GameObject temp = gameobjs.Find(x => x.name == file_name);
                        GameObject temp = GameObject.Find(file_name);
                        if (temp == null)
                        {
                            continue;
                        }

                        int is_sphere = int.Parse(values[11]);
                        GameObject obj;
                        if (is_sphere == 1) obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        else obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        obj.name = values[0];

                        Vector3 pos = new Vector3(float.Parse(values[1]), 
                            float.Parse(values[2]), float.Parse(values[3]));
                        // find the mapped object in order to calculate the world position 

                        Vector3 world_pos = pos + temp.transform.position;
                        // calculate world position through marker object's position
                        obj.transform.position = world_pos;
                        Quaternion rot = new Quaternion(float.Parse(values[4]),
                            float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));
                        obj.transform.rotation = rot;
                        Vector3 sc = new Vector3(float.Parse(values[8]),
                            float.Parse(values[9]), float.Parse(values[10]));
                        obj.transform.localScale = sc;

                        // set the tag "photo_object" to the loaded photo object
                        obj.tag = "photo_object";
                        Rigidbody rigid = obj.AddComponent<Rigidbody>();
                        rigid.velocity = Vector3.zero;
                        rigid.drag = 100000;
                        DataStorage ds = obj.AddComponent<DataStorage>();
                        ds.SetObjects(temp);
                        ds.Init();
                        ds.SetMappingDist(pos);

                        if (temp.tag == "Container")
                        {
                            rigid.useGravity = true;
                            obj.GetComponent<Collider>().isTrigger = false;
                            ds.EnableGravity();
                            ds.IsColliding(true);
                        }
                        else if (temp.transform.parent != null && temp.transform.parent.tag == "Container")
                        {
                            rigid.useGravity = true;
                            obj.GetComponent<Collider>().isTrigger = false;
                            ds.EnableGravity();
                            ds.IsColliding(true);
                        }
                        else
                        {
                            rigid.useGravity = false;
                            obj.GetComponent<Collider>().isTrigger = true;
                        }
                    }
                }
            }
        }
    }
}
