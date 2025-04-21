using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PasswordManagerMobileApp.Models;

public partial class PasswordAccountDTO : ObservableObject
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string? Title { get; set; }
    public string? Username { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    // or use font icons if needed
    public string EyeIcon => IsPasswordVisible ? "🐵" : "🙈";

    partial void OnIsPasswordVisibleChanged(bool oldValue, bool newValue)
    {
        OnPropertyChanged(nameof(EyeIcon)); // Notify UI to update icon
    }

    [ObservableProperty]
    private bool isPasswordVisible;

    public string DisplayedPassword => IsPasswordVisible ? Password : "••••••••";

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
        OnPropertyChanged(nameof(DisplayedPassword)); // Needed because DisplayedPassword is derived
    }

    public override string ToString()
    {
        return $"{nameof(Id)}:{Id}, {nameof(Title)}:{Title}, {nameof(Username)}:{Username}, {nameof(Password)}:{Password}, {nameof(UserId)}:{UserId}";
    }
}
