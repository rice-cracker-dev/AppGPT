using AppGPT.Models;
using AppGPT.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppGPT.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string _apiKeyInput = string.Empty;
    [ObservableProperty] private string _input = string.Empty;
    [ObservableProperty] private Conversation? _selectedConversation;

    public IChatService ChatService { get; }

    public MainWindowViewModel(IChatService chatService)
    {
        ChatService = chatService;
    }

    // Design-time ViewModel 
    public MainWindowViewModel()
    {
        ChatService = new ChatService();
    }

    [RelayCommand]
    private void SubmitKeyInput()
    {
        if (string.IsNullOrWhiteSpace(ApiKeyInput))
            return;
        
        ChatService.Authenticate(ApiKeyInput);
    }

    [RelayCommand]
    private void SubmitInput()
    {
        if (SelectedConversation != null && !string.IsNullOrWhiteSpace(Input) && !SelectedConversation.IsAnswering)
            ChatService.Prompt(SelectedConversation, Input);

        Input = string.Empty;
    }

    [RelayCommand]
    private void AddNewConversation() => ChatService.Conversations.Add(new Conversation());
}