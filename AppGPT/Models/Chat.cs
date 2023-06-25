using System.Collections.ObjectModel;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AppGPT.Models;

public partial class Conversation : ObservableObject
{
    [ObservableProperty] private string _title = "Untitled";
    [ObservableProperty] private bool _isAnswering;
    
    public ObservableCollection<ChatMessage> Messages { get; } = new();
}