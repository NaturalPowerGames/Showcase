using UnityEngine;
using DG.Tweening;

public class RandomRotationController : MonoBehaviour
{
	public float maxRotationAngle = 45f;
	public float maxInterval = 2f;
	private float nextRotationTime;

	void Start()
	{
		nextRotationTime = Time.time + maxInterval;
	}

	void Update()
	{
		if (Time.time >= nextRotationTime)
		{			
			transform.DORotate(new Vector3(0f, UnityEngine.Random.Range(0, maxRotationAngle), 0f), 0.5f);
			nextRotationTime = Time.time + UnityEngine.Random.Range(0, maxInterval);
		}
	}
}
