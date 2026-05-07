using System;
using Root.Errors.Interfaces;

namespace Root.Errors;

public class AppException : Exception, IAppException {
	public AppException(string message) : base(message) { }
	public AppException(string message, Exception? inner) : base(message, inner) { }

	public static T Wrap<T>(Exception ex) where T : AppException {
		if (ex is T already) return already;
		return (T)Activator.CreateInstance(typeof(T), ex.Message, ex)!;
	}

	public static Exception Label<T>(Exception ex, string msg) where T: AppException {
		if (ex is T) return ex;
		return new Exception(msg, ex);
	}
}
