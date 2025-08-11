using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Name Tag")]
    public TMP_Text nameTag;   // Dra in din NameTag (TMP Text) här

    private Rigidbody rb;

    // Använd FixedString för NetworkVariable (string stöds inte)
    private NetworkVariable<FixedString64Bytes> playerName =
        new NetworkVariable<FixedString64Bytes>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        // Uppdatera skylten när namnet ändras
        playerName.OnValueChanged += OnNameChanged;

        // Ägare får sätta sitt namn
        if (IsOwner)
        {
            var fs = new FixedString64Bytes(System.Environment.UserName);
            playerName.Value = fs; // owner-write är tillåtet
        }

        // Sätt direkt ifall värdet redan finns
        OnNameChanged(default, playerName.Value);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Bara ägaren styr sin egen spelare
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (Input.GetKeyDown(KeyCode.Space) && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnNameChanged(FixedString64Bytes oldVal, FixedString64Bytes newVal)
    {
        if (nameTag != null)
            nameTag.text = newVal.ToString();
    }
}