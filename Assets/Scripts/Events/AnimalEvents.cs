using System;

public static class AnimalEvents
{
	public static Action<AnimalType> OnAnimalInteractionRequested;
	public static Action<AnimalType> OnAnimalSoundRequested;
}
