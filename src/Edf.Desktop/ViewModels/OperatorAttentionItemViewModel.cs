namespace Edf.Desktop.ViewModels;

using Edf.Application.Operator;

public sealed class OperatorAttentionItemViewModel
{
    public OperatorAttentionItemViewModel(OperatorAttentionItem item)
    {
        Code = item.Code;
        DisplayLine = $"{item.Code}: {item.Message}";
        Detail = item.Detail;
    }

    public string Code { get; }

    public string DisplayLine { get; }

    public string? Detail { get; }
}
