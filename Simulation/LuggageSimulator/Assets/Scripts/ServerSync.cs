using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerSync : MonoBehaviour
{
    public GameObject cube;
    public bool simulate;
    void Start()
    {
        Time.timeScale = 1.0f;
        if (simulate)
            InvokeRepeating("SyncServer", 1, 0.2f);
    }

    async void SyncServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("swing1Rotation", cube.transform.position.x.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://localhost:7196/sync/all", form);
        www.SendWebRequest();
    }

    void Update()
    {

    }
}