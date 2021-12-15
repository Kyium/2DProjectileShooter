using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public struct PlayerMessage : NetworkMessage
{
    public String team;
}
public class MyNetworkManager : NetworkManager
{
    private Dictionary<NetworkConnection, int> playerConns = new Dictionary<NetworkConnection, int>();
    [SerializeField] private int currentTeamNumber = 1;
    [SerializeField] private string startLevelName = "Level_Test";
    public override void OnStartServer()
    {
        Debug.Log("Server Started: " + networkAddress);
        base.OnStartServer();
        NetworkServer.RegisterHandler<PlayerMessage>(onCreatePlayer);
        LoadLevel();
    }
    public override void OnStopServer()
    {
        Debug.Log("Server Stopped");
        base.OnStopServer();
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Application.targetFrameRate = 60;
        Debug.Log("Connected to server");
        base.OnClientConnect(conn);
        PlayerMessage message = new PlayerMessage() 
        {
            team = currentTeamNumber.ToString()
        };
        conn.Send(message);
        LoadLevel();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        currentTeamNumber++;
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("Client connected");
        base.OnServerConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Disconnected from server");
        playerConns.Remove(conn);
        base.OnClientDisconnect(conn);
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(startLevelName, LoadSceneMode.Additive);
    }
    public void onCreatePlayer(NetworkConnection conn, PlayerMessage message)
    {
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<Player>().team = message.team;
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}