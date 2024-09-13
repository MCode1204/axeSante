using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wss;

    //public string ipaddress= "213.55.220.45"; + ipaddress+

    // Démarre le serveur WebSocket
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // Rend cet objet persistant entre les scènes

        // Démarre le serveur WebSocket sur localhost, port 8080
        wss = new WebSocketServer("ws://213.55.223.205:8080");
        wss.AddWebSocketService<SceneHandler>("/SceneChange");
        wss.Start();
        Debug.Log("Serveur WebSocket démarré sur ws://213.55.220.45:8080/SceneChange");
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
