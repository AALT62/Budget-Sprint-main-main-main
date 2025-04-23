using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DecisionTrigger : MonoBehaviour
{
    public DecisionData decisionData;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || decisionData == null) return;

        var uiManager = FindAnyObjectByType<GameUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowPopup(decisionData); // Changed to ShowPopup
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            Debug.LogError("GameUIManager not found!");
        }
    }
}