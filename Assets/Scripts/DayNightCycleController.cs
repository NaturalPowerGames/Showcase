using UnityEngine;
using PolyverseSkiesAsset;

public class DayNightCycleController : MonoBehaviour
{
    [Header("Cycle Settings")]
    [Range(0, 1)]
    public float timeOfDay = 0f; // Time of day, normalized (0 to 1)
    public float dayLength = 120f; // Length of a full day in seconds
    public Color nightColor, dayColor;

	[Header("Additional Lights")]
	public Light[] additionalLights; // Array of additional lights
	public float additionalLightMinIntensity = 0.5f;
	public float additionalLightMaxIntensity = 1f;

	[Header("Time Settings")]
    [Range(0, 24)]
    public float nightTimeStart = 21f; // 9 PM
    [Range(0, 24)]
    public float morningTimeStart = 5f; // 5 AM
    [Range(0, 24)]
    public float noonTimeStart = 12f; // 12 PM
	[Range(0, 24)]
	public float midnight = 0f; // 12 PM
	private float normalizedNightStart, normalizedMorningStart, normalizedNoonStart, normalizedMidnight;

    [Header("Sun and Moon")]
    public Transform sunTransform; // The Sun object
    public Transform moonTransform; // The Moon object
    public Light directionalLight;

    [Header("Rotation Settings")]
    public Vector3 dayRotationAxis = new Vector3(1, 0, 0); // Axis for Sun rotation
    public Vector3 nightRotationAxis = new Vector3(-1, 0, 0); // Axis for Moon rotation

    private float timeSpeed; // Speed at which time progresses
    private PolyverseSkies polyverseSkies;
    private bool isReversing = false; // Whether the time is reversing

    private void Awake()
	{
        polyverseSkies = GetComponent<PolyverseSkies>();
        normalizedNightStart = nightTimeStart / 24f;
        normalizedMorningStart = morningTimeStart / 24f;
        normalizedNoonStart = noonTimeStart / 24f;
		normalizedMidnight = midnight / 24f;
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

		directionalLight.intensity = InterpolateLightIntensity();
		foreach (var light in additionalLights)
		{
			float additionalLightIntensity = 0f;

			if (timeOfDay >= normalizedNightStart || timeOfDay < normalizedMorningStart) // Nighttime
			{
				if (timeOfDay < normalizedMorningStart) // Increasing intensity up to midnight
				{
					additionalLightIntensity = Mathf.Lerp(additionalLightMinIntensity, additionalLightMaxIntensity, (timeOfDay - normalizedNightStart) / (normalizedMidnight - normalizedNightStart));
				}
				else // Decreasing intensity after midnight
				{
					additionalLightIntensity = Mathf.Lerp(additionalLightMaxIntensity, additionalLightMinIntensity, (timeOfDay - normalizedMidnight) / (normalizedMorningStart - normalizedMidnight));
				}
			}

			light.intensity = additionalLightIntensity;
			light.enabled = additionalLightIntensity > 0f; // Turn off the light if intensity is 0
		}
		// Interpolate between the dayColor and nightColor based on the time of day
		Color currentColor = Color.Lerp(nightColor, dayColor, timeOfDay);
		RenderSettings.ambientLight = currentColor;

		// Rotate the Sun
		float sunAngle = timeOfDay * 360f;
		sunTransform.rotation = Quaternion.Euler(dayRotationAxis * sunAngle);

		// Rotate the Moon
		float moonAngle = (timeOfDay + 0.5f) * 360f; // Moon is opposite to the Sun
		moonTransform.rotation = Quaternion.Euler(nightRotationAxis * moonAngle);
		polyverseSkies.timeOfDay = timeOfDay;
	}

	private float InterpolateLightIntensity()
	{
		float lightIntensity;
		if (timeOfDay >= normalizedNightStart || timeOfDay < normalizedMorningStart) // From night start to morning start
		{
			lightIntensity = 0.5f;
		}
		else if (timeOfDay >= normalizedMorningStart && timeOfDay < normalizedNoonStart) // From morning start to noon
		{
			lightIntensity = Mathf.Lerp(0.5f, 1f, (timeOfDay - normalizedMorningStart) / (normalizedNoonStart - normalizedMorningStart));
		}
		else // From noon to night start, decreasing intensity
		{
			lightIntensity = Mathf.Lerp(1f, 0.5f, (timeOfDay - normalizedNoonStart) / (normalizedNightStart - normalizedNoonStart));
		}

		return lightIntensity;
	}
}
