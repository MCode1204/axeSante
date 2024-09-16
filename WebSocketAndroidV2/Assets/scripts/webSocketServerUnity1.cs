using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wss;
   [SerializeField] private string baseAddress = "ws://192.168.1.";  // Base de l'IP
    [SerializeField] private int port = 8080;  // Le port sur lequel ton serveur �coute.
    private List<string> availableHosts = new List<string>();

    // D�marre le serveur WebSocket
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // Rend cet objet persistant entre les sc�nes

        // D�marre le serveur WebSocket sur localhost, port 8080,
        // wss = new WebSocketServer("ws://192.168.1.106:8080");
        wss = new WebSocketServer(port);
        wss.AddWebSocketService<SceneHandler>("/SceneChange");
        wss.Start();
        Debug.Log("Serveur WebSocket d�marr� sur ws://0.0.0.0:8080/SceneChange");

        // Scanner les IPs de 1 � 255 (modifiez selon les besoins).
        StartCoroutine(ScanNetworkForWebSocketHosts());
    }

    // Ferme le serveur WebSocket lorsque l'application est ferm�e
    void OnDestroy()
    {
        if (wss != null)
        {
            wss.Stop();
            wss = null;
            Debug.Log("Serveur WebSocket arr�t�.");
        }
    }

    // Classe pour g�rer les messages WebSocket
    public class SceneHandler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            // Message re�u contenant le nom de la sc�ne
            string sceneName = e.Data;

            // Charger la sc�ne dans Unity
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log("Chargement de la sc�ne :" + sceneName+"|");
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
                ws.Close();  // Fermez la connexion apr�s le succ�s.
            };

            ws.OnError += (sender, e) =>
            {
                Debug.Log("Connection failed: " + wsAddress + " - " + e.Message);
                connectionFailed = true;
            };

            ws.ConnectAsync();

            // Attendre jusqu'� 5 secondes pour la connexion
            float timeout = 5.0f;
            while (!connectionSuccessful && !connectionFailed && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;  // Attendre la frame suivante.
            }

            // Si la connexion a �chou� apr�s le timeout, loguez une erreur.
            if (!connectionSuccessful && timeout <= 0)
            {
                Debug.Log("Connection timed out: " + wsAddress);
                connectionFailed = true;
            }

            yield return new WaitForSeconds(1.0f);  // Attendre 1 seconde avant de tester l�IP suivante.
        }

        Debug.Log("Scan termin�. H�tes disponibles :");
        foreach (string host in availableHosts)
        {
            Debug.Log(host);
        }

    }
}
