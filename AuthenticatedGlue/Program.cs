using System.Text;
using AuthenticatedGlue;
using AuthenticatedGlue.Services;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => {
		options.InputFormatters.Add(new PlainTextInputFormatter() {
			SupportedMediaTypes = {
				"application/jwt",
			},
			SupportedEncodings = {
				Encoding.ASCII,
				Encoding.UTF8,
				Encoding.Latin1,
			},
		});
	})
	.AddXmlSerializerFormatters()
	.AddFormatterMappings(mappings => {
		mappings.SetMediaTypeMappingForFormat("string", "application/jwt");
	});
builder.Services.AddHttpLogging();

builder.Services.AddHttpClient();

builder.Services.AddOptions();
builder.Services.Configure<KeySelectingAlgorithmFactory.Options>(builder.Configuration.GetSection("Keys"));
builder.Services.Configure<WatchtowerService.Options>(builder.Configuration.GetSection("Watchtower"));

builder.Services.AddSingleton<IReplayCache, NoopReplayCache>();
builder.Services.AddSingleton<WatchtowerService>();
builder.Services.AddSingleton<IJwtDecoder, JwtDecoder>();
builder.Services.AddSingleton<IJwtValidator, JwtValidator>();
builder.Services.AddSingleton<IBase64UrlEncoder, JwtBase64UrlEncoder>();
builder.Services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
builder.Services.AddSingleton<IJsonSerializer, JsonNetSerializer>();
builder.Services.AddSingleton<IAlgorithmFactory, KeySelectingAlgorithmFactory>();
builder.Services.AddSingleton(_ => ValidationParameters.Default);

var app = builder.Build();
//app.UseHttpLogging();
app.MapControllers();

app.Logger.LogInformation("Deployment OK! :ralArtHapp:");

app.Run();
