using Microsoft.AspNetCore.Mvc;
using WatchtowerGlue.Model;
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
		foreach (RegistryEvent evt in registryEvents.events) {
			m_Logger.LogInformation("{Action} {Repo} {Target}", evt.action, evt.request.host, evt.target.repository);
		}
		m_Service.Receive(registryEvents);
	}
}
