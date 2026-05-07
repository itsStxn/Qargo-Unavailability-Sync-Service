using System;

namespace Root.Errors.Interfaces;

public interface IAppException {
	public static abstract T Wrap<T>(Exception ex) where T : AppException;
}
