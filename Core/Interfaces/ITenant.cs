using System;
using Root.DTOs.ResourceListComponents;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Core.Interfaces;

public interface ITenant {
	public Task<List<Resource>> GetResourcesAsync();
	public Task<List<Unavailability>> GetUnavailabilitiesAsync(string resourceId);
	public Task<HttpResponseMessage> UpdateUnavailabilitiesAsync(string resourceId, UActions actions);
	public Task<HttpResponseMessage> CreateUnavailabilitiesAsync(string resourceId, UActions actions);
}
