using UnityEngine;

public class DecisionZone : MonoBehaviour
{
    public DecisionData decisionToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LaneMover laneMover = other.GetComponent<LaneMover>();
            if (laneMover != null)
            {
                laneMover.isPaused = true;
            }

            GameUIManager.Instance.ShowPopup(decisionToLoad);
            gameObject.SetActive(false);
        }
    }
}
