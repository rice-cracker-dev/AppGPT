using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AppGPT.Models;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppGPT.Services;

public interface IChatService
{
    bool IsAuthenticated { get; }
    Exception? Exception { get; }
    ObservableCollection<Conversation> Conversations { get; }
    void Authenticate(string apiKey);
    Task Prompt(Conversation conversation, string prompt);
}

public partial class ChatService : ObservableObject, IChatService
{
    private OpenAIClient? _client;

    [ObservableProperty] private bool _isAuthenticated;
    [ObservableProperty] private Exception? _exception;

    public ObservableCollection<Conversation> Conversations { get; }

    public ChatService()
    {
        Conversations = new ObservableCollection<Conversation>(new [] { new Conversation() });
    }

    public void Authenticate(string apiKey)
    {
        _client = new OpenAIClient(apiKey);
        IsAuthenticated = true;
    }

    public async Task Prompt(Conversation conversation, string prompt)
    {
        try
        {
            if (!IsAuthenticated || _client == null || conversation.IsAnswering)
                return;

            conversation.IsAnswering = true;
            conversation.Messages.Add(new ChatMessage(ChatRole.User, prompt));

            var options = new ChatCompletionsOptions();
            conversation.Messages.ToList().ForEach(value => options.Messages.Add(value));

            var response = await _client.GetChatCompletionsAsync("gpt-3.5-turbo", options);
            response.Value.Choices.ToList().ForEach(value => conversation.Messages.Add(value.Message));
            conversation.IsAnswering = false;
        }
        catch (Exception ex)
        {
            Exception = ex;
        }
        finally
        {
            conversation.IsAnswering = false;
        }
    }

    [RelayCommand]
    private void CloseException() => Exception = null;
}