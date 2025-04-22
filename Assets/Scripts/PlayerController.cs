using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneSwitchSpeed = 5f;
    private int currentLane = 1;
    private Vector3 targetPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
        {
            currentLane--;
            UpdateTargetPosition();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2)
        {
            currentLane++;
            UpdateTargetPosition();
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);
    }

    private void UpdateTargetPosition()
    {
        float laneWidth = 3f;
        targetPosition = new Vector3((currentLane - 1) * laneWidth, transform.position.y, transform.position.z);
    }
}