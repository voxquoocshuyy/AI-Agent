namespace AI.Agent.Infrastructure.AI;

public interface IAzureOpenAIService
{
    // TODO: Định nghĩa các method cần thiết dựa trên usage thực tế
    Task<bool> TestConnectionAsync();
}

public class AzureOpenAIService : IAzureOpenAIService
{
    // TODO: Implement các method của IAzureOpenAIService
    public async Task<bool> TestConnectionAsync()
    {
        // TODO: Implement logic kiểm tra kết nối Azure OpenAI
        return true;
    }
} 