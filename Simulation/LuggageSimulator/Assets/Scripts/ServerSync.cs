using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServoDto {
    public int id { get; set; }
    public int Angle { get; set; }
}

public class ServerSync : MonoBehaviour
{
    public List<GameObject> servos;
    public GameObject belt;

    public bool simulate;
    void Start()
    {
        Time.timeScale = 1.0f;

        if (simulate)
            InvokeRepeating("SyncServer", 1, 0.2f);
    }

    async void SyncServer()
    {
        int angle0 = (int)((servos[0].transform.position.z + 8.9)/1.53 * 90) + 10;
        int angle1 = (int)((servos[1].transform.position.z + 9)/1.63 * 160) + 10;
        int angle2 = (int)((servos[2].transform.position.z + 9)/1.63 * 160) + 10;
        int angle3 = (int)((servos[3].transform.position.z + 9)/1.63 * 160) + 10;

        string motorSpeedJsonBody = "{\"function\": 2,\"distance\": 2}";
        UnityWebRequest motorSpeedRequest = UnityWebRequest.Post("https://localhost:7196/sync/movebelt", motorSpeedJsonBody, "application/json");
        motorSpeedRequest.SendWebRequest();

        string servoPosJsonBody = "[{\"id\": 0,\"Angle\": " + angle0 + "},{\"id\": 1,\"Angle\": " + angle1 + "},{\"id\": 2,\"Angle\": " + angle2 + "},{\"id\": 3,\"Angle\": " + angle3 + "}]";
        UnityWebRequest servoPositioningRequest = UnityWebRequest.Post("https://localhost:7196/sync/setservopos", servoPosJsonBody, "application/json");
        servoPositioningRequest.SendWebRequest();
    }

    void Update()
    {

    }
}