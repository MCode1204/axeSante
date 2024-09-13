using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wss;

    // Démarre le serveur WebSocket
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // Rend cet objet persistant entre les scènes

        // Démarre le serveur WebSocket sur localhost, port 8080
        wss = new WebSocketServer("wss://172.20.10.4:443");
        wss.AddWebSocketService<SceneHandler>("/SceneChange");
        wss.Start();
        Debug.Log("Serveur WebSocket démarré sur wss://172.20.10.4:443/SceneChange");
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
}
