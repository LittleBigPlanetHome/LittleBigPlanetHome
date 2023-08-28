using UnityEngine;

public class DynamicText : MonoBehaviour
{
    [HideInInspector]
    public string ogValue;
    [HideInInspector]
    public TMPro.TextMeshProUGUI tmp;
    private void Start() {
        tmp = GetComponent<TMPro.TextMeshProUGUI>();
        ogValue = tmp.text;

        if (tmp.text.Contains("{SERVER}")) {
            ServerManager.Instance.onServerConnect.AddListener(serverconnect);
            print("Now listening for server connection.");
        }

        if (!tmp.text.Contains("!NOREPLACE!"))
            tmp.text = StringManager.instance.GetString(tmp.text);
        else {
            tmp.text = tmp.text.Replace("!NOREPLACE!", "");
        }

        tmp.text = tmp.text.Replace("{VERSION}", Application.version);
    }

    private void OnDisable() {
        try {
            ServerManager.Instance.onServerConnect.RemoveListener(serverconnect);
        } catch {}
    }

    private void serverconnect() {
        tmp.text = tmp.text.Replace("{SERVER}", ServerManager.Instance.connectedServer.ServerId);
    }
}
