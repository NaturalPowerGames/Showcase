using System;

public static class TimeEvents
{
	public static Action<int> OnTimeOfDayChanged;
	public static Action<DayPhase> OnDayPhaseChanged;
}
