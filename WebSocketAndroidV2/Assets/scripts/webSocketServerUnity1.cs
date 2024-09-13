using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wss;

    // D�marre le serveur WebSocket
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // Rend cet objet persistant entre les sc�nes

        // D�marre le serveur WebSocket sur localhost, port 8080
        wss = new WebSocketServer("wss://172.20.10.4:443");
        wss.AddWebSocketService<SceneHandler>("/SceneChange");
        wss.Start();
        Debug.Log("Serveur WebSocket d�marr� sur wss://172.20.10.4:443/SceneChange");
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
}
