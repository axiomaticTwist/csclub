using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeekAtGlance {
	static DateTime testWeek = new DateTime(2019, 9, 30);

	static DateTime dt = testWeek.StartOfWeek(DayOfWeek.Sunday);

	public static DateTime GetStart() {
		return dt;
	}
}


public static class DateTimeExtensions {
	public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {
		// Get the amount of days we are ahead of the start of the week
		int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
		// Subtract the amount of days we are ahead to get the start of the week
		return dt.AddDays(-diff).Date;
	}
}