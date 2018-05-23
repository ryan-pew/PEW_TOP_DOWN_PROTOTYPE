//This script is used to make UI elements face the player's camera

using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform mainCamera;   //The camera's transform
    Player ourPlayer;

    void Start()
    {
        //Set the Main Camera as the target
        mainCamera = Camera.main.transform;
        ourPlayer = GetComponentInParent<Player>();
        Debug.Log(Camera.main.name);
    }

    //Update after all other updates have run
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main.transform;
            return;
        }

        //Apply the rotation needed to look at the camera. Note, since pointing a UI text element
        //at the camera makes it appear backwards, we are actually pointing this object
        //directly *away* from the camera.
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);
        Debug.Log(ourPlayer.playerName + " Quaternion.LookRotation( " +transform.position + mainCamera.position + " ) ID = " + ourPlayer.playerControllerId);
        
    }
}