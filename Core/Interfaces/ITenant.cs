using System;
using Root.Records;
using Root.DTOs.ResourceListComponents;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Core.Interfaces;

public interface ITenant {
	public Task<List<Resource>> GetResourcesAsync();
	public Task<List<Unavailability>> GetUnavailabilitiesAsync(string resourceId);
	public Task<BatchActionResult> CreateUnavailabilitiesAsync(string resourceId, UActions actions);
	public Task<BatchActionResult> UpdateUnavailabilitiesAsync(string resourceId, UActions actions);
}
