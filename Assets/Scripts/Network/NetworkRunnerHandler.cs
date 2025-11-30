using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] private 
    NetworkRunner networkRunnerPrefab;
    NetworkRunner networkRunner;


    private void Awake()
    {
        networkRunner = FindObjectOfType<NetworkRunner>();
    }

     


    void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network Runner";
        }

        var clientTask = InitializedNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), null);

        Utils.DebugLog("InitializeNetworkRunner called");
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        INetworkSceneManager sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        { 
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializedNetworkRunner(NetworkRunner networkRunner, GameMode gameMode, string sessionName, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        INetworkSceneManager sceneManager = GetSceneManager(networkRunner);
        networkRunner.AddCallbacks(FindObjectOfType<Spawner>());

        networkRunner.ProvideInput = true;

        return networkRunner.StartGame(new StartGameArgs
            {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            SceneManager = sceneManager
        });
    }

}
