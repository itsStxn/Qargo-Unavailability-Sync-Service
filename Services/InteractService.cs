using System;
using Root.Core;
using Root.Services.Interfaces;

namespace Root.Services;

/// <summary>
/// Coordinates synchronization operations between the Qargo and Master tenants.
/// Responsible for mapping, linking, and synchronizing resource unavailabilities.
/// </summary>
public class InteractService : Base, IInteractService {

	/// <summary>
	/// Service instance representing the Qargo tenant.
	/// </summary>
	private readonly QargoService _qargo;

	/// <summary>
	/// Service instance representing the Master tenant.
	/// </summary>
	private readonly MasterService _master;


	/// <summary>
	/// Initializes a new instance of <see cref="InteractService"/>
	/// with the required tenant service dependencies.
	/// </summary>
	/// <param name="qargo">
	/// The <see cref="QargoService"/> responsible for interacting with the Qargo tenant.
	/// </param>
	/// <param name="master">
	/// The <see cref="MasterService"/> responsible for interacting with the Master tenant.
	/// </param>
	public InteractService(QargoService qargo, MasterService master) {
		_qargo = qargo;
		_master = master;
	}


	/// <summary>
	/// Links Qargo unavailabilities to their corresponding Master resource actions
	/// by matching shared resource IDs and assigning synchronization operations.
	/// </summary>
	/// <returns>A <see cref="Task"/> representing the asynchronous linking operation.</returns>
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


	/// <summary>
	/// Synchronizes unavailabilities between the Master and Qargo tenants.
	/// Existing unavailabilities are linked first, after which create and update
	/// operations are executed in parallel for each mapped resource.
	/// </summary>
	/// <returns>A <see cref="Task"/> representing the asynchronous synchronization operation.</returns>
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
