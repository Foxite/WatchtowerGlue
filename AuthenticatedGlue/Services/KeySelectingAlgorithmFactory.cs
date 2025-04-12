using System.Security.Cryptography.X509Certificates;
using JWT;
using JWT.Algorithms;
using Microsoft.Extensions.Options;

namespace AuthenticatedGlue.Services;

public class KeySelectingAlgorithmFactory(IOptionsMonitor<KeySelectingAlgorithmFactory.Options> options) : IAlgorithmFactory {
	public IJwtAlgorithm Create(JwtDecoderContext context) {
		if (!options.CurrentValue.Keys.TryGetValue(context.Header.KeyId, out string? publicKey)) {
			throw new Exception($"Cannot find key id {context.Header.KeyId}");
		}

		var cert = X509Certificate2.CreateFromPem(publicKey);

		return context.Header.Algorithm switch {
			"ES256" => new ES256Algorithm(cert),
			"ES384" => new ES384Algorithm(cert),
			"ES512" => new ES512Algorithm(cert),
			"RS256" => new RS256Algorithm(cert),
			"RS384" => new RS384Algorithm(cert),
			"RS512" => new RS512Algorithm(cert),
			_       => throw new NotSupportedException($"Key type not supported: {context.Header.Algorithm}"),
		};
	}

	public class Options {
		public Dictionary<string, string> Keys { get; set; }
	}
}
