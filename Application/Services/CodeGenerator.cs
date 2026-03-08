using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Application.Services;

public class CodeGenerator: ICodeGenerator
{
    public string GenerateSessionCode()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "")
            .Replace("+", "")[..6];
    }
}