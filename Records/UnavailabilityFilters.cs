using System;

namespace Root.Records;

/// <summary>
/// Represents a set of filter criteria used to query and filter unavailability records.
/// </summary>
public record struct UnavailabilityFilters {

	/// <summary>
	/// Value used for filtering unavailabilities by year.
	/// </summary>
	public string Year;
}
