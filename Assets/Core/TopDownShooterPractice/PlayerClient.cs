using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class PlayerClient : MonoBehaviour
{
    [SerializeField]
    private float SendDataCooldown = 0.05f;
    [SerializeField]
    private float Speed = 5;
    [field:SerializeField]
    public PlayerData playerData { get; private set; } = new();
    private UdpClient playerClient;
    private Thread recieveThread;
    void Start()
    {
        SendConnectionToServer();
    }
    void OnDestroy()
    {
        EndConnectionToServer();   
    }
    void SendConnectionToServer()
    {
        playerClient = new(8888);
        try
        {
            playerClient.Connect("127.0.0.1", 7777);
            recieveThread = new(new ThreadStart(RecievceThread));
            StartCoroutine(SendEnumerator());
            
        }
        catch (Exception e)
        {
            print("Error while connection to server. Error message: " + e.Message);
        }
    }
    void EndConnectionToServer()
    {
        try
        {
            playerClient.Close();
            recieveThread.Abort();
            enabled = false;
        }
        catch (Exception e)
        {
            print("Error while connection to server. Error message: " + e.Message);
        }
    }
    void SendData()
    {
        try
        {
            byte[] toSend = Encoding.ASCII.GetBytes(JsonUtility.ToJson(playerData));
            if (toSend != null)
            {
                playerClient.Send(toSend, toSend.Length);
            }

        } catch (Exception e) { }
    }
    IEnumerator SendEnumerator()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(SendDataCooldown);
            playerData.x = transform.position.x;
            playerData.y = transform.position.y;
            SendData();
        }
    }
    void RecievceThread()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 12345);
        while (true)
        {
            byte[] receivedBytes = playerClient.Receive(ref remoteEndPoint);
            string recieveString = Encoding.ASCII.GetString(receivedBytes);
            object jsonRecieve = JsonUtility.FromJson(recieveString, typeof(PlayerData[]));
            if (jsonRecieve != null && jsonRecieve is PlayerData[] playerDatas)
            {
                foreach (PlayerData playerData in playerDatas)
                {
                    GameObject potentialPlayerObject = GameObject.Find("Player_" + playerData.id.ToString());
                    if (potentialPlayerObject != null)
                    {
                        Vector3 position = new(playerData.x, playerData.y, 0);
                        potentialPlayerObject.transform.position = position;
                        print(position.ToString());
                    }
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            playerData.health -= 1;
        }
    }
}
public class PlayerData
{
    public float x = 0;
    public float y = 0;
    public int health = 3;
    [SerializeField]
    public int id;
}