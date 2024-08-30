using UnityEngine;
using PolyverseSkiesAsset;
using System;

public class DayNightCycleController : MonoBehaviour
{
	[Header("Cycle Settings")]
	[Range(0, 1)]
	public float timeOfDay = 0f;
	public float dayLength = 120f;
	public Color nightColor, dayColor;
	private int lastHour;
	private DayPhase dayPhase;
	[Header("Additional Lights")]
	public Light[] additionalLights;
	public ParticleSystem[] additionalParticleSystems;
	public float additionalLightMinIntensity = 0.5f;
	public float additionalLightMaxIntensity = 1f;
	public float particleMinEmissionRate = 0f;
	public float particleMaxEmissionRate = 10f;

	[Header("Time Settings")]
	[Range(0, 24)]
	public float nightTimeStart = 21f; // 9 PM
	[Range(0, 24)]
	public float morningTimeStart = 5f; // 5 AM
	[Range(0, 24)]
	public float noonTimeStart = 12f; // 12 PM
	[Range(0, 24)]
	public float midnight = 24f; // 12 PM
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
		lastHour = Mathf.FloorToInt(timeOfDay * 24f);
	}

	private void Start()
	{
		timeSpeed = 1f / dayLength;
		StartFirstDay();
	}

	private void StartFirstDay()
	{
		timeOfDay = normalizedMorningStart;
		TimeEvents.OnDayPhaseChanged?.Invoke(dayPhase);
	}

	private void Update()
	{
		IncreaseDayTime();
		CheckDayHour();
		directionalLight.intensity = InterpolateLightIntensity();
		SetAdditionalLights();
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

	private void CheckDayHour()
	{
		int currentHour = Mathf.FloorToInt(timeOfDay * 24f);

		if (currentHour != lastHour)
		{
			TimeEvents.OnTimeOfDayChanged?.Invoke(currentHour);
			lastHour = currentHour;
			Debug.Log($"It's {currentHour}");
		}
	}

	private void IncreaseDayTime()
	{
		timeOfDay += timeSpeed * Time.deltaTime;
		if (timeOfDay >= 1f)
		{
			timeOfDay = 0;
			if (dayPhase != DayPhase.Midnight)
			{
				dayPhase = DayPhase.Midnight;
				Debug.Log($"It is: {dayPhase}");
				TimeEvents.OnDayPhaseChanged?.Invoke(DayPhase.Midnight);
			}
		}
		if (timeOfDay >= normalizedNightStart && timeOfDay < normalizedMidnight)
		{
			if (dayPhase != DayPhase.Evening)
			{
				dayPhase = DayPhase.Evening;
				Debug.Log($"It is: {dayPhase}");
				TimeEvents.OnDayPhaseChanged?.Invoke(DayPhase.Evening);
			}
		}
		if (timeOfDay >= normalizedMorningStart && timeOfDay < normalizedNoonStart)
		{
			if (dayPhase != DayPhase.Morning)
			{
				dayPhase = DayPhase.Morning;
				Debug.Log($"It is: {dayPhase}");
				TimeEvents.OnDayPhaseChanged?.Invoke(DayPhase.Morning);
			}
		}
		if (timeOfDay >= normalizedNoonStart && timeOfDay < normalizedNightStart)
		{
			if (dayPhase != DayPhase.Noon)
			{
				dayPhase = DayPhase.Noon;
				Debug.Log($"It is: {dayPhase}");
				TimeEvents.OnDayPhaseChanged?.Invoke(DayPhase.Noon);
			}
		}
	}

	private void SetAdditionalLights()
	{
		foreach (var light in additionalLights)
		{
			float additionalLightIntensity = 0f;
			if (timeOfDay >= normalizedNightStart && timeOfDay < normalizedMidnight)
			{
				float lerpFactor = Mathf.InverseLerp(normalizedNightStart, normalizedMidnight, timeOfDay);
				additionalLightIntensity = Mathf.Lerp(additionalLightMinIntensity, additionalLightMaxIntensity, lerpFactor);
			}
			else if (timeOfDay < normalizedMidnight && timeOfDay < normalizedMorningStart)
			{
				float lerpFactor = Mathf.InverseLerp(0, normalizedMorningStart, timeOfDay);
				additionalLightIntensity = Mathf.Lerp(additionalLightMaxIntensity, additionalLightMinIntensity, lerpFactor);
			}
			light.intensity = additionalLightIntensity;
			light.enabled = additionalLightIntensity > 0f; // Turn off the light if intensity is 0
		}

		foreach (var particleSystem in additionalParticleSystems)
		{
			var emission = particleSystem.emission;
			float particleEmissionRate = 0f;
			if (timeOfDay >= normalizedNightStart && timeOfDay < normalizedMidnight)
			{
				float lerpFactor = Mathf.InverseLerp(normalizedNightStart, normalizedMidnight, timeOfDay);
				particleEmissionRate = Mathf.Lerp(particleMinEmissionRate, particleMaxEmissionRate, lerpFactor);
			}
			else if (timeOfDay < normalizedMidnight && timeOfDay < normalizedMorningStart)
			{
				float lerpFactor = Mathf.InverseLerp(0, normalizedMorningStart, timeOfDay);
				particleEmissionRate = Mathf.Lerp(particleMaxEmissionRate, particleMinEmissionRate, lerpFactor);
			}
			emission.rateOverTime = particleEmissionRate;
			particleSystem.gameObject.SetActive(particleEmissionRate > 0f); // Activate/deactivate the particle system
		}
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
