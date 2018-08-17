# GlobulAR_SaveandLoad

## How to use
* MarkerManager.cs : attached to the marker object (the object that will be mapping other objects)
  * Container marker should contain a **gravity field** as a child object
  * gravity field contains **box collider**, sligthly bigger than the size of the container
  * sample gravity field is in the Prefabs folder
* DataStorage.cs : attached to the photo object (the object that will be mapped to other objects)
  * if the photo object is not a sphere, then uncheck the 'Is Sphere' checkbox in the editor
* StorageManager.cs : attached to the empty object in the scene. Controll the storage procedure
  * Marker objects in the scene should be contained in the StorageManager's **Gameobjs** variable
* CameraManager.cs : attached to the main camera in the scene


* need to set the photo objects' **tag as 'photo_object'**
* need to set the box objects' (container-type objects) **tag as 'Container'**


* Sample Scene 'DataStorage' is in the Scenes folder
* Sample Prefabs for markers and photo objects are in Prefabs folder