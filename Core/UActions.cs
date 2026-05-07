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
		string externalId = unavail.ExternalId 
			?? throw new ConfigException("Input unavailability external id must be not null");
		
		// ? Validate uniqueness
		string unavailId = unavail.Id; 
		if (ToUpdate.ContainsKey(unavailId))
			throw new ConfigException("Input unavailability id must be unique");

		// ? Nothing to change 
		if (!ToCreate.ContainsKey(externalId)) return;

		// ? If the existing unavailability differs 
		// ? from the incoming one, mark it for update
		var a = unavail;
		var b = ToCreate[externalId];
		ToCreate.Remove(externalId); // ? Move unavailability

		if (a.Reason 	   != b.Reason
		||  a.Description != b.Description
		||  a.StartTime   != b.StartTime
		||  a.EndTime     != b.EndTime) {
			b.ExternalId = a.ExternalId; // ? Ensure they have same external id
			ToUpdate.Add(unavailId, b); // ? Use unavail id of qargo => 
												 // ? Update request done in the qargo tenant by that key
		}
	}
}
