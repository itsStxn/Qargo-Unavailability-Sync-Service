using System;
using Root.Core.Interfaces;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Core;

public class UActions : IUActions {
	public readonly Dictionary<string, Unavailability> ToCreate;
	public readonly Dictionary<string, Unavailability> ToUpdate;

	public UActions() {
		ToCreate = [];
		ToUpdate = [];
	}

	public void Assign(string unavailId, Unavailability unavail) {
		// TODO: code...
	}
}
