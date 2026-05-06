using System;
using Root.Source.Interfaces;

namespace Root.Source;

public class EnvSource : IEnvSource {
	public EnvSource() {
		DotNetEnv.Env.Load();
	}
	
	public string Load(string varName) {
		return Environment.GetEnvironmentVariable(varName)
			?? throw new Exception($"\"{varName}\" environment variable is not set");
	}
}
