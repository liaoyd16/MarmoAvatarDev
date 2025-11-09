using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IPSelector : MonoBehaviour
{
    [SerializeField] string default_server_ip = "127.0.0.1";
    enum ServerType { WorkStation, Rasp4 }
    [SerializeField] ServerType serverType;

    public static IPEndPoint remoteEP;
    public static IPEndPoint remoteEP_rasp4;
    TMPro.TMP_InputField ip_input, port_input;
    [SerializeField] int loaded_scene;
    // Start is called before the first frame update
    void Start()
    {
        ip_input = transform.Find("IP").GetComponent<TMPro.TMP_InputField>();
        port_input = transform.Find("Port").GetComponent<TMPro.TMP_InputField>();
    }

    // Update is called once per frame
    public void onConfirmClicked()
    {
        int server_port = 11000;
        int.TryParse(port_input.text, out server_port);

        try
        {
            if (serverType == ServerType.WorkStation)
                remoteEP = new IPEndPoint(IPAddress.Parse(ip_input.text), server_port);
            if (serverType == ServerType.Rasp4)
                remoteEP_rasp4 = new IPEndPoint(IPAddress.Parse(ip_input.text), server_port);
        }
        catch
        {
            if (serverType == ServerType.WorkStation)
                remoteEP = new IPEndPoint(IPAddress.Parse(default_server_ip), server_port);
            if (serverType == ServerType.Rasp4)
                remoteEP_rasp4 = new IPEndPoint(IPAddress.Parse(default_server_ip), server_port);
        }

        Canvas[] canvaslist = FindObjectsOfType<Canvas>();
        foreach (Canvas c in canvaslist)
        {
            c.enabled = false;
        }

        SceneManager.LoadScene(loaded_scene, LoadSceneMode.Single);
    }
}
