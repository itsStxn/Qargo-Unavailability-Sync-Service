using System;

namespace Root.Utils.Interfaces;

public interface IAccessTokenCache {
	public string Read();
	public void Create(string token);
}
