using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using WebApiMorning.Dtos;
using WebApiMorning.Entities;

public class VcardInputFormatter : TextInputFormatter
{
    public VcardInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/vcard"));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanReadType(Type type)
        => type == typeof(StudentDto);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context, Encoding effectiveEncoding)
    {
        var httpContext = context.HttpContext;
        var serviceProvider = httpContext.RequestServices;

        var logger = serviceProvider.GetRequiredService<ILogger<VcardInputFormatter>>();

        using var reader = new StreamReader(httpContext.Request.Body, effectiveEncoding);
        string? nameLine = null;

        try
        {
            await ReadLineAsync("BEGIN:VCARD", reader, context, logger);
            await ReadLineAsync("VERSION:", reader, context, logger);

            nameLine = await ReadLineAsync("N:", reader, context, logger);

            var split = nameLine.Split(":".ToCharArray());
            var student = new StudentDto{
                Fullname = await ReadLineAsync("FN:", reader, context, logger),
                SeriaNo = await ReadLineAsync("SND:", reader, context, logger),
                Age = int.Parse(await ReadLineAsync("AGE:", reader, context, logger)),
                Score = int.Parse(await ReadLineAsync("SCORE:", reader, context, logger)),
                Id = int.Parse(await ReadLineAsync("UID:", reader, context, logger)),
            };
            
            //student.Fullname = await ReadLineAsync("FN:", reader, context, logger);
            //student.SeriaNo = await ReadLineAsync("SND:", reader, context, logger);
            //student.Age = int.Parse(await ReadLineAsync("AGE:", reader, context, logger));
            //student.Score = int.Parse(await ReadLineAsync("SCORE:", reader, context, logger));
            //student.Id = int.Parse(await ReadLineAsync("UID:", reader, context, logger));

            await ReadLineAsync("END:VCARD", reader, context, logger);

            logger.LogInformation("nameLine = {nameLine}", nameLine);

            return await InputFormatterResult.SuccessAsync(student);
        }
        catch
        {
            logger.LogError("Read failed: nameLine = {nameLine}", nameLine);
            return await InputFormatterResult.FailureAsync();
        }
    }

    private static async Task<string> ReadLineAsync(
        string expectedText, StreamReader reader, InputFormatterContext context,
        ILogger logger)
    {
        var line = await reader.ReadLineAsync();

        if (line is null || !line.StartsWith(expectedText))
        {
            var errorMessage = $"Looked for '{expectedText}' and got '{line}'";

            context.ModelState.TryAddModelError(context.ModelName, errorMessage);
            logger.LogError(errorMessage);

            throw new Exception(errorMessage);
        }

        return line;
    }
}