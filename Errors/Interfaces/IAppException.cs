using System;

namespace Root.Errors.Interfaces;

public interface IAppException {
	public static abstract T Wrap<T>(Exception ex) where T : AppException;
	public static abstract Exception Label<T>(Exception ex, string msg) where T : AppException;
}
