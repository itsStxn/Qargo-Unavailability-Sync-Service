using System;
using Root.Core;
using Root.Services.Interfaces;

namespace Root.Services;

public class InteractService : Base, IInteractService {
	private readonly QargoService _qargo;
	private readonly MasterService _master;

	public InteractService(QargoService qargo, MasterService master) {
		_qargo = qargo;
		_master = master;
	}

	private async Task QargoToMaster() {
		Echo("Linking unavailability between qargo and master...");

		// ? Servive level resouce mapping
		await Task.WhenAll( // ? Parallel execution
			_qargo.MapResources(),
			_master.MapResources()
		);

		// ? Explore shared resources
		foreach (var (resourceId, unavailList) in _qargo.ResourceMap) {
			if (_master.ResourceMap.TryGetValue(resourceId, out var unavailAction)) {
				// ? Link qargo resource's unavailabilities to master
				foreach (var u in unavailList) {
					unavailAction.Assign(u);
				}
			}
		}

		Echo("Linked unavailability between qargo and master");
	}

	public async Task SyncUnavailabilities() {
		await QargoToMaster();

		Echo("Syncing qargo and master...");

		// ? Explore master resources and execute unavailAction on qargo
		foreach (var (resourceId, unavailAction) in _master.ResourceMap) {
			var create = _qargo.CreateUnavailabilitiesAsync(resourceId, unavailAction);
			var update = _qargo.UpdateUnavailabilitiesAsync(resourceId, unavailAction);

			// ? Parallel execution
			await Task.WhenAll(create, update);
		}

		Echo("Finished syncing qargo and master");
	}
}
