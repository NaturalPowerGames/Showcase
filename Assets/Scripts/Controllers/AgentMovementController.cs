using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Animancer;

public class AgentMovementController : MonoBehaviour, IAnimalInteractor
{
	public Transform walkPointsParent;
	private Transform[] walkPoints;
	private NavMeshAgent agent;
	private int currentWalkIndex;
	private bool isWalking = false;
	[Header("Settings")]
	[SerializeField] private bool wait;
	[SerializeField] private bool continuousWalking, interactable, automatic;
	[Header("Animations")]
	private AnimancerComponent animancer;
	[SerializeField] private AnimationClip walkClip;
	[SerializeField] private AnimationClip idleClip;
	[SerializeField] private float maxWaitBetweenMove;
	[SerializeField] private float interactionTime;
	[SerializeField] private AnimalType animalType;
	private void Awake()
	{
		animancer = GetComponent<AnimancerComponent>();
	}

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
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
		bool infinite = true;
		while (infinite)
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
					currentWalkIndex = Random.Range(0, walkPoints.Length);
				}

				agent.SetDestination(walkPoints[currentWalkIndex].position);
				PlayWalkAnimation();

				yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending);
				PlayIdleAnimation();
				float waitTime = Random.Range(0f, maxWaitBetweenMove);
				yield return new WaitForSeconds(wait ? waitTime : 0);
				isWalking = false;
				interactionTime -= waitTime;
				infinite = interactionTime >= 0 || automatic;
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

	public void OnInteract()
	{
		if (interactable)
		{
			AnimalEvents.OnAnimalSoundRequested?.Invoke(AnimalType());
			StartCoroutine(WalkToRandomPoint());
			interactionTime += 5f;
		}
	}

	public AnimalType AnimalType()
	{
		return animalType;
	}
}
