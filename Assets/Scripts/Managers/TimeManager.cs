using UnityEngine;

public class TimeManager : MonoBehaviour
{
	[Header("Cycle Settings")]
	[Range(0, 1)]
	public float timeOfDay = 0f;
	public float dayLength = 120f;
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

	private void Update()
	{
		
	}
}
