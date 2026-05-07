using System;
using Root.Core;
using Root.Errors;
using Root.Source.Interfaces;

namespace Root.Source;

/// <summary>
/// Provides access to application environment variables loaded from a .env source.
/// Inherits shared logging and message formatting utilities from <see cref="Base"/>.
/// </summary>
public class EnvSource : Base, IEnvSource {

	/// <summary>
	/// Initializes a new instance of <see cref="EnvSource"/>
	/// and loads environment variables from the configured .env file.
	/// </summary>
	public EnvSource() {
		DotNetEnv.Env.Load();
	}


	/// <summary>
	/// Retrieves the value of the specified environment variable.
	/// </summary>
	/// <param name="varName">
	/// The name of the environment variable to retrieve.
	/// </param>
	/// <returns>
	/// The environment variable value associated with <paramref name="varName"/>.
	/// </returns>
	/// <exception cref="ConfigException">
	/// Thrown when the requested environment variable is not set.
	/// </exception>
	public string Load(string varName) {
		return Environment.GetEnvironmentVariable(varName)
			?? throw new ConfigException(
				Msg($"\"{varName}\" environment variable is not set"));
	}
}
