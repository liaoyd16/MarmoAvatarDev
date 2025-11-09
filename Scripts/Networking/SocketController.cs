using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;


public class SocketController : MonoBehaviour
{

    Socket client, client_rasp4;
    ManualResetEvent connectDone1 = new ManualResetEvent(false);
    ManualResetEvent connectDone2 = new ManualResetEvent(false);

    const int BufferSize = 64;

    void Start()
    {
        StartClient();
    }

    void OnDestroy()
    {
        client.Shutdown(SocketShutdown.Both);
        client_rasp4.Shutdown(SocketShutdown.Both);
    }

    // ! Will freeze Start() function if not connected to server !
    void StartClient()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client_rasp4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            client.BeginConnect(IPSelector.remoteEP, ConnectCallback, new ConnectionState(client, connectDone1));
            connectDone1.WaitOne();

            client_rasp4.BeginConnect(IPSelector.remoteEP_rasp4, ConnectCallback, new ConnectionState(client_rasp4, connectDone2));
            connectDone2.WaitOne();

            client.Blocking = false;
            client_rasp4.Blocking = false;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    class ConnectionState
    {
        public Socket Socket;
        public ManualResetEvent Done;

        public ConnectionState(Socket socket, ManualResetEvent done)
        {
            Socket = socket;
            Done = done;
        }
    }

    void ConnectCallback(IAsyncResult ar)
    {
        var state = (ConnectionState)ar.AsyncState;
        try
        {
            state.Socket.EndConnect(ar);
        }
        catch (Exception ex)
        {
            Debug.LogError("Connection failed: " + ex);
        }
        finally
        {
            state.Done.Set();
        }
    }

    public void SendWorkstation(string msg)
    {
        int sentlen = 0;
        try
        {
            Debug.LogFormat("bytes sent: {0}", msg);
            //bytes_sent = client.Send();
            byte[] data = Encoding.ASCII.GetBytes(msg);
            byte[] len_bytes = BitConverter.GetBytes(
                IPAddress.HostToNetworkOrder(data.Length));
            sentlen = client.Send(len_bytes);
            Debug.Assert(sentlen == 4);
            sentlen = client.Send(data);
            Debug.Assert(sentlen == data.Length);
        }
        catch (Exception)
        {
            Debug.LogError("bytes sent = " + sentlen.ToString() + "\n");
        }
    }

    public void SendRasp4(string msg)
    {
        int sentlen = 0;
        try
        {
            Debug.LogFormat("bytes sent to rasp4: {0}", msg);
            byte[] data = Encoding.ASCII.GetBytes(msg);
            sentlen = client_rasp4.Send(data);
        }
        catch (Exception)
        {
            Debug.LogErrorFormat("bytes send = {0}", sentlen);
        }
    }
}
