using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class DisplayIPAddress : MonoBehaviour
{
    public Text ipAddressText; // Assurez-vous d'ajouter un objet Text à votre scène

    void Start()
    {
        // Récupérer l'adresse IP
        string ipAddress = GetLocalIPAddress();
        if (ipAddress != null)
        {
            // Afficher l'adresse IP dans l'interface utilisateur
            ipAddressText.text = "Votre adresse IP locale : " + ipAddress;
        }
        else
        {
            ipAddressText.text = "Adresse IP introuvable";
        }
    }

    // Méthode pour obtenir l'adresse IP locale
    private string GetLocalIPAddress()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }
}
