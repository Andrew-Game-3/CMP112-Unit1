using UnityEngine;

public class CameraController : MonoBehaviour
{

    //intialise variables
    public Transform player;
    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Z STILL IMPORTANT IN 2D - need to set Z to -10 or something caus eotherwise camera is on the same layer as player and you just see background
        //code copies players position, but lets you edit how offset the camera should be from the players position
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, player.position.z + offset.z);
    }
}
