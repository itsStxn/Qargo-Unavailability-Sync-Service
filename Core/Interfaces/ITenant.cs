using System;
using Root.DTOs;
using Root.DTOs.ResourceListComponents;

namespace Root.Core.Interfaces;

public interface ITenant {
	public Task<List<Resource>> GetResourcesAsync();
	public Task<T> GetUnavailabilitiesAsync<T>(string resourceId);
}
