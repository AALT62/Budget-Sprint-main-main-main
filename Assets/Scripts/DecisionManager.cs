using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    public GameObject decisionPanel;
    public GameObject player;

    public void MakeChoice()
    {
        // Called by both buttons
        decisionPanel.SetActive(false);
        player.GetComponent<LaneMover>().isPaused = false;
    }
}
