namespace WatchtowerGlue.Model;

public class RegistryEvents {
	public RegistryEvent[] events { get; set; }
}

public class RegistryEvent {
	public string action { get; set; }
	public EventTarget target { get; set; }
	public EventRequest request { get; set; }
}

public class EventTarget {
	public string repository { get; set; }
}

public class EventRequest {
	public string host { get; set; }
}
