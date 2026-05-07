using System;
using Serilog;

namespace Root.Core;

public class Base {
	private readonly ILogger Logger;
	protected string _name;

	public Base() {
		Logger = Log.ForContext(GetType());
		_name = "None";
	}

	protected Base(string name) {
		Logger = Log.ForContext(GetType());
		_name = name;
	}

	public string Msg(string text) {
		return $"(service: {_name}) >> {text}";
	}

	public void Echo(string text) {
		Logger.Information(Msg(text));
	}

	public void Warn(string text) {
		Logger.Warning(Msg(text));
	}
}
