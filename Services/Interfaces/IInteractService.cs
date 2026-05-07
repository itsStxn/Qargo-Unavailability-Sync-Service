using System;

namespace Root.Services.Interfaces;

public interface IInteractService {
	public Task SyncUnavailabilities();
}
