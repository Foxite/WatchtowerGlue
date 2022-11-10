using Microsoft.AspNetCore.Mvc;
using WatchtowerGlue.Services;

namespace WatchtowerGlue.Controllers;

[ApiController]
[Route("/")]
public class NotificationController : ControllerBase {
	private readonly ILogger<NotificationController> m_Logger;
	private readonly NotificationService m_Service;

	public NotificationController(ILogger<NotificationController> logger, NotificationService service) {
		m_Logger = logger;
		m_Service = service;
	}

	[HttpPost]
	public void ForwardNotification(RegistryEvents registryEvents) {
		m_Logger.LogInformation("Received events:");
		foreach (RegistryEvent evt in registryEvents.Events) {
			m_Logger.LogInformation("{Action} {Repo} {Target}", evt.Action, evt.Request.Host, evt.Target.Repository);
		}
		m_Service.Receive(registryEvents);
	}
}

public class RegistryEvents {
	public RegistryEvent[] Events { get; set; }
}

public class RegistryEvent {
	public Guid Id { get; set; }
	public DateTimeOffset Timestamp { get; set; }
	public string Action { get; set; }
	public EventTarget Target { get; set; }
	public EventRequest Request { get; set; }
	public EventActor Actor { get; set; }
	public EventSource Source { get; set; }
}

public class EventTarget {
	public string MediaType { get; set; }
	public string Digest { get; set; }
	public long? Size { get; set; }
	public long Length { get; set; }
	public string Repository { get; set; }
	public string Url { get; set; }
	public string? Tag { get; set; }
}

public class EventRequest {
	public Guid Id { get; set; }
	public string Addr { get; set; }
	public string Host { get; set; }
	public HttpMethod Method { get; set; }
	public string UserAgent { get; set; }
}

public class EventActor {
	public string Name { get; set; }
}

public class EventSource {
	public string Addr { get; set; }
	public Guid? InstanceId { get; set; }
}
