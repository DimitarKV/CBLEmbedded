using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerSync : MonoBehaviour
{
    public GameObject cube;
    void Start()
    {
        Time.timeScale = 1.0f;
        InvokeRepeating("SyncServer", 1, 0.2f);
    }

    async void SyncServer() {
        Debug.Log(cube.GetComponent<Transform>().rotation.eulerAngles.x);
        WWWForm form = new WWWForm();
        form.AddField("swing1Rotation", cube.transform.rotation.y.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://localhost:7196/sync/all", form);
        www.SendWebRequest();
    }

    void Update()
    {
        
    }
}
