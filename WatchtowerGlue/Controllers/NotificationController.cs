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
	public async Task ForwardNotification(RegistryEvents registryEvents) {
		/*
		RegistryEvents registryEvents;
		using (var sr = new StreamReader(HttpContext.Request.Body, leaveOpen: false)) {
			string body = await sr.ReadToEndAsync();
			Console.WriteLine(body);
			registryEvents = System.Text.Json.JsonSerializer.Deserialize<RegistryEvents>(body)!;
		}//*/

		m_Logger.LogInformation("Received events:");
		foreach (RegistryEvent evt in registryEvents.events) {
			m_Logger.LogInformation("{Action} {Repo} {Target}", evt.action, evt.request.host, evt.target.repository);
		}
		m_Service.Receive(registryEvents);
	}
}

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
