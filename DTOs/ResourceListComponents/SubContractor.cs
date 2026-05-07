using System;
using System.Text.Json.Serialization;

namespace Root.DTOs.ResourceListComponents;

public class SubContractor {
	[JsonPropertyName("id")]
	public required string RowId { get; set; }
}
