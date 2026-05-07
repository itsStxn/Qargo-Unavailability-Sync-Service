using System;
using Root.Core;
using Root.Errors;
using Root.Source.Interfaces;

namespace Root.Source;

public class EnvSource : Base, IEnvSource {
	public EnvSource() {
		DotNetEnv.Env.Load();
	}
	
	public string Load(string varName) {
		return Environment.GetEnvironmentVariable(varName)
			?? throw new ConfigException(
				Msg($"\"{varName}\" environment variable is not set"));
	}
}
