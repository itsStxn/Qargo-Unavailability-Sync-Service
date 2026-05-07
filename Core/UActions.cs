using System;
using Root.Errors;
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

	public void Assign(Unavailability unavail) {
		// ? Validate input
		string unavailId = unavail.ExternalId 
			?? throw new ConfigException("Input's external id must be not null");
		
		// ? Validate uniqueness
		if (ToUpdate.ContainsKey(unavailId))
			throw new ConfigException("Unavailability id must be unique");

		// ? Nothing to change 
		if (!ToCreate.ContainsKey(unavailId)) return;

		// ? If the existing unavailability differs 
		// ? from the incoming one, mark it for update
		var a = unavail;
		var b = ToCreate[unavailId];
		ToCreate.Remove(unavailId); // ? Move unavailability

		if (a.Reason 	   != b.Reason
		||  a.Description != b.Description
		||  a.StartTime   != b.StartTime
		||  a.EndTime     != b.EndTime) {
			b.ExternalId = a.ExternalId; // ? Update external id
			ToUpdate[unavailId] = b;
		}
	}
}
