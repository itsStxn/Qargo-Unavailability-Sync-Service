using System;
using Root.Errors.Interfaces;

namespace Root.Errors;

/// <summary>
/// Represents the base application exception type used throughout the system.
/// Provides helper utilities for wrapping and labeling exceptions consistently.
/// </summary>
public class AppException : Exception, IAppException {

	/// <summary>
	/// Initializes a new instance of <see cref="AppException"/>
	/// with the specified error message.
	/// </summary>
	/// <param name="message">
	/// The error message describing the exception.
	/// </param>
	public AppException(string message) : base(message) { }


	/// <summary>
	/// Initializes a new instance of <see cref="AppException"/>
	/// with the specified error message and inner exception.
	/// </summary>
	/// <param name="message">
	/// The error message describing the exception.
	/// </param>
	/// <param name="inner">
	/// The exception that caused the current exception.
	/// </param>
	public AppException(string message, Exception? inner) : base(message, inner) { }


	/// <summary>
	/// Wraps the provided exception into the specified application exception type.
	/// If the exception is already of type <typeparamref name="T"/>,
	/// the original instance is returned unchanged.
	/// </summary>
	/// <typeparam name="T">
	/// The target <see cref="AppException"/> type to wrap into.
	/// </typeparam>
	/// <param name="ex">
	/// The exception instance to wrap.
	/// </param>
	/// <returns>
	/// An instance of <typeparamref name="T"/> representing the wrapped exception.
	/// </returns>
	public static T Wrap<T>(Exception ex) where T : AppException {
		if (ex is T already) return already;

		return (T)Activator.CreateInstance(typeof(T), ex.Message, ex)!;
	}


	/// <summary>
	/// Labels an exception with an outer message while preserving the original exception as the inner exception.
	/// If the exception is already of type <typeparamref name="T"/>,
	/// the original instance is returned unchanged.
	/// </summary>
	/// <typeparam name="T">
	/// The application exception type used to determine whether wrapping is required.
	/// </typeparam>
	/// <param name="ex">
	/// The exception instance to label.
	/// </param>
	/// <param name="msg">
	/// The outer message to associate with the new exception.
	/// </param>
	/// <returns>
	/// The original exception if already of type <typeparamref name="T"/>;
	/// otherwise a new <see cref="Exception"/> wrapping the original exception.
	/// </returns>
	public static Exception Label<T>(Exception ex, string msg) where T : AppException {
		if (ex is T) return ex;

		return new Exception(msg, ex);
	}
}
