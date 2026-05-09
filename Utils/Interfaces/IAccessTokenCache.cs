using System;

namespace Root.Utils.Interfaces;

public interface IAccessTokenUtil {
	public string ReadCache();
	public void CreateCache(string token);
	public static abstract string ToPrettyJson<T>(T data, bool print = false);
}
