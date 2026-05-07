using System;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Core.Interfaces;

public interface IUActions {
	public void Assign(string unavailId, Unavailability unavail);
}
