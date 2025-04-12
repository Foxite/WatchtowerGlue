using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

var httpClient = new HttpClient();

const string apiUrl = "http://localhost:5134/notify";
const string privateKeyPem =
	"""
    -----BEGIN PRIVATE KEY-----
    ME4CAQAwEAYHKoZIzj0CAQYFK4EEACIENzA1AgEBBDCtQRZWzF6QqqqR42As5EfX
    a+EKcyRKzW182MhNT23j5guQmraLCu618wtOeVNF43w=
    -----END PRIVATE KEY-----
    """;
const string certPem =
	"""
    -----BEGIN CERTIFICATE-----
    MIIBYTCB56ADAgECAgYBlipi680wCgYIKoZIzj0EAwIwGTEXMBUGA1UEAwwOc2ln
    LTE3NDQ0Njc4NDgwHhcNMjUwNDEyMTQyNDA4WhcNMjYwMjA2MTQyNDA4WjAZMRcw
    FQYDVQQDDA5zaWctMTc0NDQ2Nzg0ODB2MBAGByqGSM49AgEGBSuBBAAiA2IABK6p
    DgS82tf7EG+pXb/Kmbs7j4lpGvPKB2fvYWsEaI1DoeK+vnPG7qczG5UtRxoKEpW+
    1t15q7TfnnxpI9/ldDxmfY0n6nPYRrmxtOwnkmg7Wc4goP6A3to6nCQ4gFybXzAK
    BggqhkjOPQQDAgNpADBmAjEA0hHtdnRYpGnLdDP/Uz1RsCMXVuzaaLOE3kSRcFrH
    Zz00rYSv8hyA3yORxnBekplYAjEA9A6ZElGVP8S8J6dsLYu8EUe2/DsHnOQnsHuM
    6aFH6zgc9O8vmVZ6PME78IJ0+ZRz
    -----END CERTIFICATE-----
    """;

var rs256 = new ES384Algorithm(X509Certificate2.CreateFromPem(certPem, privateKeyPem));
var jwtEncoder = new JwtEncoder(rs256, new JsonNetSerializer(), new JwtBase64UrlEncoder());

static long EpochTimestamp(DateTime dateTime) {
	return (int) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
}

string jwt = jwtEncoder.Encode(new Dictionary<string, object>() {
	{ "kid", "sig-1744467848" },
}, new {
	jti = Guid.NewGuid(),
	iat = EpochTimestamp(DateTime.UtcNow),
	nbf = EpochTimestamp(DateTime.UtcNow),
	exp = EpochTimestamp(DateTime.UtcNow + TimeSpan.FromMinutes(5)),
	images = (string[]) ["testimage"],
}, null);

var result = await httpClient.SendAsync(new HttpRequestMessage() {
	Method = HttpMethod.Post,
	RequestUri = new Uri(apiUrl),
	Content = new StringContent(jwt, new MediaTypeHeaderValue("application/jwt")),
});

Console.WriteLine(result.StatusCode);
Console.WriteLine(await result.Content.ReadAsStringAsync());
