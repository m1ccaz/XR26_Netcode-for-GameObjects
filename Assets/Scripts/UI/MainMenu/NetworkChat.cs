using UnityEngine;
using TMPro;
using Unity.Netcode;

public class NetworkChat : NetworkBehaviour
{
    public TMP_InputField chatInput;
    public TMP_Text chatDisplay;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && chatInput != null)
        {
            chatInput.onEndEdit.AddListener(SendMessageToServer);
        }
    }

    private void SendMessageToServer(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            SendChatMessageServerRpc(System.Environment.UserName + ": " + message);
            chatInput.text = "";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string message)
    {
        chatDisplay.text += message + "\n";
    }
}