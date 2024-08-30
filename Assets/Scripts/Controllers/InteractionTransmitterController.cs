using UnityEngine;

public class InteractionTransmitterController : MonoBehaviour, IAnimalInteractor
{
	[SerializeField]
	private GameObject[] targets;
	private AnimalType animalType;
	public AnimalType AnimalType()
	{
		return animalType; //not needed?
	}

	public void OnInteract()
	{
		for (int i = 0; i < targets.Length; i++)
		{
			targets[i].GetComponent<IAnimalInteractor>().OnInteract();
		}
	}

	private void OnMouseDown()
	{
		OnInteract();
	}
}
