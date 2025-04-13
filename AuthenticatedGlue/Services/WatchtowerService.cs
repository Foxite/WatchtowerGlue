using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace AuthenticatedGlue.Services;

public class WatchtowerService(HttpClient httpClient, IOptions<WatchtowerService.Options> options) {
	public async Task TriggerUpdate(IEnumerable<string> images) {
		await httpClient.SendAsync(new HttpRequestMessage() {
			Method = HttpMethod.Get,
			RequestUri = new Uri($"{options.Value.Url}/v1/update?image={UrlEncoder.Default.Encode(string.Join(",", images))}"),
			Headers = {
				Authorization = new AuthenticationHeaderValue("Bearer", options.Value.Token),
			},
		});
	}

	public class Options {
		public string Token { get; set; }
		public string Url { get; set; }
	}
}
