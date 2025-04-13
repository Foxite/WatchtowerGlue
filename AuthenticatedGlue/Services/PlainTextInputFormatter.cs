using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AuthenticatedGlue;

public class PlainTextInputFormatter : TextInputFormatter {
	public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding) {
		using TextReader reader = context.ReaderFactory(context.HttpContext.Request.Body, encoding);
		string text = await reader.ReadToEndAsync();
		return InputFormatterResult.Success(text);
	}
}
