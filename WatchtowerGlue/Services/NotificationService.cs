using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using WatchtowerGlue.Controllers;
using WatchtowerGlue.Model;
using Timer = System.Timers.Timer;

namespace WatchtowerGlue.Services; 

public class NotificationService {
	private readonly HttpClient m_Http;
	private readonly Timer m_Timer = new();
	private readonly List<RegistryEvent> m_PendingEvents = new();
	private readonly object m_Lock = new();
	private readonly ILogger<NotificationService> m_Logger;

	public NotificationService(HttpClient http, ILogger<NotificationService> logger) {
		m_Http = http;
		m_Logger = logger;

		m_Timer.AutoReset = false;
		m_Timer.Enabled = false;
		m_Timer.Interval = long.Parse(Environment.GetEnvironmentVariable("DEBOUNCE_MILLIS") ?? "5000");
		m_Timer.Elapsed += (o, e) => Forward();
	}

	public void Receive(RegistryEvents registryEvents) {
		lock (m_Lock) {
			foreach (RegistryEvent @event in registryEvents.events) {
				m_PendingEvents.Add(@event);
			}

			if (m_Timer.Enabled) {
				m_Timer.Stop();
			}
			m_Timer.Start();
		}
	}

	private void Forward() {
		Task.Run(async () => {
			try {
				string images;
				lock (m_Lock) {
					images = string.Join(",", m_PendingEvents.Where(evt => evt.action == "push").Select(evt => $"{evt.request.host}/{evt.target.repository}").Distinct());
				}
				
				m_Logger.LogInformation("Forwarding notifications: {Images}", images);

				string? watchtowerToken = Environment.GetEnvironmentVariable("WATCHTOWER_TOKEN");
				AuthenticationHeaderValue? authorization;

				if (watchtowerToken != null) {
					authorization = new AuthenticationHeaderValue("Bearer", watchtowerToken);
				} else {
					authorization = null;
				}

				await m_Http.SendAsync(new HttpRequestMessage() {
					Method = HttpMethod.Get,
					RequestUri = new Uri(Environment.GetEnvironmentVariable("WATCHTOWER") + "/v1/update?image=" + UrlEncoder.Default.Encode(images)),
					Headers = {
						Authorization = authorization
					}
				});
			} catch (Exception e) {
				m_Logger.LogError(e, "Error while forwarding events");
			}
		}).GetAwaiter().GetResult();
	}
}
