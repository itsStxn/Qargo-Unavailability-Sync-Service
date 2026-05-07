using System;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Services.Interfaces;

public interface IQargoService {
	public Task<Dictionary<string, List<Unavailability>>> MapResources();
}
