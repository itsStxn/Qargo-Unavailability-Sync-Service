using System;

namespace Root.Records;

/// <summary>
/// Represents the result of a batch unavailability operation
/// for a single resource.
/// </summary>
public record struct BatchActionResult {

	/// <summary>
	/// Gets the identifier of the resource associated with the batch operation.
	/// </summary>
	public string ResourceId { get; init; }

	/// <summary>
	/// Gets the total number of attempted operations in the batch.
	/// </summary>
	public int Total { get; init; }

	/// <summary>
	/// Gets the number of successfully completed operations.
	/// </summary>
	public int Succeeded { get; init; }

	/// <summary>
	/// Gets the collection of unavailability identifiers that failed
	/// during processing.
	/// </summary>
	public List<string> Failed { get; init; }

	/// <summary>
	/// Gets a value indicating whether the batch operation
	/// contains one or more failures.
	/// </summary>
	public bool HasFailures => Failed.Count > 0;
}
