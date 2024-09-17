using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;



public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wss;
   [SerializeField] private string baseAddress = "ws://192.168.1.";  // Base de l'IP
    [SerializeField] private int port = 8080;  // Le port sur lequel ton serveur écoute.
    private List<string> availableHosts = new List<string>();

    // WebSocket pour le client Unity
    private WebSocket wsClient;

    // Démarre le serveur WebSocket
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // Rend cet objet persistant entre les scènes

        // Démarre le serveur WebSocket sur localhost, port 8080,
        // wss = new WebSocketServer("ws://192.168.1.106:8080");
        wss = new WebSocketServer("ws://0.0.0.0:" + port);
        wss.AddWebSocketService<SceneHandler>("/SceneChange");
        wss.Start();
        Debug.Log("Serveur WebSocket démarré sur ws://0.0.0.0:8080/SceneChange");

        // Scanner les IPs de 1 à 255 (modifiez selon les besoins).
        StartCoroutine(ScanNetworkForWebSocketHosts());

        // Initialiser et se connecter en tant que client WebSocket
        wsClient = new WebSocket("ws://192.168.1.106:8080"); // Remplace par l'adresse de ton serveur WebSocket

        wsClient.OnOpen += (sender, e) =>
        {
            Debug.Log("Connexion établie avec le serveur WebSocket.");
            wsClient.Send("Client Unity connecté");  // Message pour identifier le client
        };

        wsClient.OnMessage += (sender, e) =>
        {
            Debug.Log("Message reçu du serveur : " + e.Data);
        };

        wsClient.OnError += (sender, e) =>
        {
            Debug.LogError("Erreur WebSocket : " + e.Message);
        };

        wsClient.OnClose += (sender, e) =>
        {
            Debug.Log("Connexion fermée avec le serveur WebSocket.");
        };

        wsClient.Connect();
    }

    // Ferme le serveur WebSocket lorsque l'application est fermée
    void OnDestroy()
    {
        if (wss != null)
        {
            wss.Stop();
            wss = null;
            Debug.Log("Serveur WebSocket arrêté.");
        }
    }

    // Classe pour gérer les messages WebSocket
    public class SceneHandler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            // Message reçu contenant le nom de la scène
            string sceneName = e.Data;

            // Charger la scène dans Unity
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log("Chargement de la scène :" + sceneName+"|");
                SceneManager.LoadScene(sceneName);
            }
        }
    }
    public IEnumerator ScanNetworkForWebSocketHosts()
    {
        for (int i = 1; i < 255; i++)
        {
            string ip = baseAddress + i.ToString();
            string wsAddress = ip + ":" + port;

            Debug.Log("Testing " + wsAddress);

            // Tester la connexion WebSocket.
            WebSocket ws = new WebSocket("ws://" + wsAddress);
            bool connectionSuccessful = false;
            bool connectionFailed = false;

            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Connection success: " + wsAddress);
                availableHosts.Add(wsAddress);
                connectionSuccessful = true;
                ws.Close();  // Fermez la connexion après le succès.
            };

            ws.OnError += (sender, e) =>
            {
                Debug.Log("Connection failed: " + wsAddress + " - " + e.Message);
                connectionFailed = true;
            };

            ws.ConnectAsync();

            // Attendre jusqu'à 5 secondes pour la connexion
            float timeout = 5.0f;
            while (!connectionSuccessful && !connectionFailed && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;  // Attendre la frame suivante.
            }

            // Si la connexion a échoué après le timeout, loguez une erreur.
            if (!connectionSuccessful && timeout <= 0)
            {
                Debug.Log("Connection timed out: " + wsAddress);
                connectionFailed = true;
            }

            yield return new WaitForSeconds(1.0f);  // Attendre 1 seconde avant de tester l’IP suivante.
        }

        Debug.Log("Scan terminé. Hôtes disponibles :");
        foreach (string host in availableHosts)
        {
            Debug.Log(host);
        }

    }
    private async Task<bool> TestWebSocketConnection(string wsAddress)
    {
        using (var ws = new WebSocket(wsAddress))
        {
            bool connectionSuccessful = false;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            ws.OnOpen += (sender, e) =>
            {
                connectionSuccessful = true;
                tcs.SetResult(true);
                ws.Close();
            };

            ws.OnError += (sender, e) =>
            {
                tcs.SetResult(false);
            };

            ws.OnClose += (sender, e) =>
            {
                tcs.TrySetResult(connectionSuccessful);
            };

            ws.ConnectAsync();
            return await tcs.Task;
        }
    }
}
