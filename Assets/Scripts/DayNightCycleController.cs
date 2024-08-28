using UnityEngine;
using PolyverseSkiesAsset;

public class DayNightCycleController : MonoBehaviour
{
    [Header("Cycle Settings")]
    [Range(0, 1)]
    public float timeOfDay = 0f; // Time of day, normalized (0 to 1)
    public float dayLength = 120f; // Length of a full day in seconds

    [Header("Sun and Moon")]
    public Transform sunTransform; // The Sun object
    public Transform moonTransform; // The Moon object

    [Header("Rotation Settings")]
    public Vector3 dayRotationAxis = new Vector3(1, 0, 0); // Axis for Sun rotation
    public Vector3 nightRotationAxis = new Vector3(-1, 0, 0); // Axis for Moon rotation

    private float timeSpeed; // Speed at which time progresses
    private PolyverseSkies polyverseSkies;
    private bool isReversing = false; // Whether the time is reversing

    private void Awake()
	{
        polyverseSkies = GetComponent<PolyverseSkies>();
	}

	void Start()
    {
        timeSpeed = 1f / dayLength; // Calculate the speed of time progression
    }

    void Update()
    {
        if (!isReversing)
        {
            timeOfDay += timeSpeed * Time.deltaTime;
            if (timeOfDay >= 1f)
            {
                timeOfDay = 1f;
                isReversing = true;
            }
        }
        else
        {
            timeOfDay -= timeSpeed * Time.deltaTime;
            if (timeOfDay <= 0f)
            {
                timeOfDay = 0f;
                isReversing = false;
            }
        }

        // Rotate the Sun
        float sunAngle = timeOfDay * 360f;
        sunTransform.rotation = Quaternion.Euler(dayRotationAxis * sunAngle);

        // Rotate the Moon
        float moonAngle = (timeOfDay + 0.5f) * 360f; // Moon is opposite to the Sun
        moonTransform.rotation = Quaternion.Euler(nightRotationAxis * moonAngle);
        polyverseSkies.timeOfDay = timeOfDay;
    }
}
