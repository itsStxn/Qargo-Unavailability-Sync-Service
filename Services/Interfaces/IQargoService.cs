using System;
using Root.Core;

namespace Root.Services.Interfaces;

public interface IQargoService {
	public Task UpdateUnavailabilitiesAsync(string resourceId, MUActions actions);
	public Task CreateUnavailabilitiesAsync(string resourceId, MUActions actions);
}
