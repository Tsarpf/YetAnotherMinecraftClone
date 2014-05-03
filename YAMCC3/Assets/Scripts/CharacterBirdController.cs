using UnityEngine;
using System.Collections;

public class CharacterBirdController : MonoBehaviour {

    Transform player;
    Transform cameraT;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cameraT = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.rigidbody.AddForce(new Vector3(0, 1000, 0));
        }


        Debug.DrawRay(cameraT.position, cameraT.forward);
        if(Input.GetKey(KeyCode.W))
        {
            player.rigidbody.AddForce(cameraT.forward * 10);
        }

    }
}
