using UnityEngine;

public class LaneMover : MonoBehaviour
{
    public float laneDistance = 5f;
    public float laneSwitchSpeed = 10f;
    public float forwardSpeed = 5f;

    private int currentLane = 1;
    private Vector3 targetPosition;
    [HideInInspector] public bool isPaused = false;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (isPaused) return;

        // Move forward constantly
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Handle input for lane switching
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
        {
            currentLane--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2)
        {
            currentLane++;
        }

        float targetX = (currentLane - 1) * laneDistance;
        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);
    }
}
