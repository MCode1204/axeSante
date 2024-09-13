using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wss;

    //public string ipaddress= "213.55.220.45"; + ipaddress+

    // D�marre le serveur WebSocket
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // Rend cet objet persistant entre les sc�nes

        // D�marre le serveur WebSocket sur localhost, port 8080
        wss = new WebSocketServer("ws://213.55.223.205:8080");
        wss.AddWebSocketService<SceneHandler>("/SceneChange");
        wss.Start();
        Debug.Log("Serveur WebSocket d�marr� sur ws://213.55.220.45:8080/SceneChange");
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
