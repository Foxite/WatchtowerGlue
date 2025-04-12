using AuthenticatedGlue.Services;
using Microsoft.AspNetCore.Mvc;
using JWT;
using JWT.Exceptions;

namespace AuthenticatedGlue.Controllers;

[ApiController]
[Route("/notify")]
public class NotificationController(ILogger<NotificationController> logger, IJwtDecoder jwtDecoder, WatchtowerService watchtowerService, IReplayCache replayCache) : ControllerBase {
	[HttpPost]
	[Consumes("application/jwt")]
	public async Task<IActionResult> TakeEvent([FromBody] string jwtBody) {
		Console.WriteLine("AAAAAAAA");
		//HttpContext.Request.EnableBuffering();
		//HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
		
		/*
		using (var sr = new StreamReader(HttpContext.Request.Body, leaveOpen: true)) {
			jwtBody = await sr.ReadToEndAsync();
		}
		*/
		if (!replayCache.Check(jwtBody)) {
			logger.LogWarning("Detected replay from {RemoteIp}", HttpContext.Connection.RemoteIpAddress);
			return BadRequest(new {
				success = false,
				error = "jwt-replay",
				message = "Potential replay detected. Generate a new JWT.",
			});
		}
		
		AuthenticatedNotification? notification;
		try {
			notification = jwtDecoder.DecodeToObject<AuthenticatedNotification>(jwtBody);
		} catch (FormatException ex) {
			logger.LogWarning(ex, "FormatException when decoding JWT");
			return BadRequest(new {
				success = false,
				error = "jwt-format-error",
				message = ex.Message,
			});
		} catch (SignatureVerificationException ex) {
			logger.LogWarning(ex, "SignatureVerificationException when decoding JWT");
			return Unauthorized(new {
				success = false,
				error = "jwt-signature-error",
				message = ex.Message,
			});
		}

		logger.LogInformation("Triggering update for: {Images}", string.Join(", ", notification.Images));
		await watchtowerService.TriggerUpdate(notification.Images);

		return Ok();
	}
}
