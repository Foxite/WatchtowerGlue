using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using WatchtowerGlue.Controllers;
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
		m_Timer.Interval = long.Parse(Environment.GetEnvironmentVariable("DEBOUNCE_MILLIS")!);
		m_Timer.Elapsed += (o, e) => Forward();
	}

	public void Receive(RegistryEvents registryEvents) {
		lock (m_Lock) {
			foreach (RegistryEvent @event in registryEvents.Events) {
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
					images = string.Join(",", m_PendingEvents.Where(evt => evt.Action == "push").Select(evt => $"{evt.Request.Host}/{evt.Target.Repository}"));
				}

				await m_Http.SendAsync(new HttpRequestMessage() {
					Method = HttpMethod.Get,
					RequestUri = new Uri(Environment.GetEnvironmentVariable("WATCHTOWER") + "/v1/update?image=" + UrlEncoder.Default.Encode(images)),
					Headers = {
						Authorization = new AuthenticationHeaderValue("Bearer " + Environment.GetEnvironmentVariable("WATCHTOWER_TOKEN"))
					}
				});
			} catch (Exception e) {
				m_Logger.LogError(e, "Error while forwarding events");
			}
		}).GetAwaiter().GetResult();
	}
}
