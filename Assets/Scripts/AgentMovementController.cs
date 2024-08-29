using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Animancer;

public class AgentMovementController : MonoBehaviour
{
	public Transform walkPointsParent; // Parent object containing the walk points
	private Transform[] walkPoints; // Array of walk points
	private NavMeshAgent agent; // Reference to the NavMeshAgent component
	private int currentWalkIndex; // Index of the current walk point
	private bool isWalking = false; // Is the agent currently walking
	[Header("Settings")]
	[SerializeField] private bool wait, continuousWalking;
	[Header("Animations")]
	private AnimancerComponent animancer; // Reference to the Animancer component
	[SerializeField] private AnimationClip walkClip; // Walk animation clip
	[SerializeField] private AnimationClip idleClip; // Idle_A animation clip
	[SerializeField] private float maxWaitBetweenMove;

	private void Awake()
	{
		animancer = GetComponent<AnimancerComponent>();
	}

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();

		// Get all walk points from the parent object
		int childCount = walkPointsParent.childCount;
		walkPoints = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			walkPoints[i] = walkPointsParent.GetChild(i);
		}
		StartCoroutine(WalkToRandomPoint());
	}

	private IEnumerator WalkToRandomPoint()
	{
		while (true)
		{
			if (!isWalking)
			{
				isWalking = true;

				if (continuousWalking)
				{
					currentWalkIndex++;
					if (currentWalkIndex >= walkPoints.Length)
					{
						currentWalkIndex = 0;
					}
				}
				else
				{
					// Pick a random point
					currentWalkIndex = Random.Range(0, walkPoints.Length);
				}

				agent.SetDestination(walkPoints[currentWalkIndex].position);
				PlayWalkAnimation();

				// Wait until the agent arrives at the destination
				yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending);
				PlayIdleAnimation();

				// Wait for a random time between 0 and 5 seconds before moving again
				yield return new WaitForSeconds(wait ? Random.Range(0f, maxWaitBetweenMove) : 0);

				isWalking = false;
			}

			if (continuousWalking)
			{
				yield return null;
			}
		}
	}
	private void PlayWalkAnimation()
	{
		animancer.Play(walkClip);
	}

	private void PlayIdleAnimation()
	{
		animancer.Play(idleClip);
	}
}
