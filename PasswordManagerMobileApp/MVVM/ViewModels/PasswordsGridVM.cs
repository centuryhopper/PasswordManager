using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.MVVM;

public partial class PasswordsGridVM : ObservableObject
{
    private const int PageSize = 10;
    private readonly IPasswordManagerService passwordManagerService;

    public ObservableCollection<PasswordAccountDTO> PagedItems { get; } = [];

    private List<PasswordAccountDTO> _allItems = [];

    public PasswordsGridVM(IPasswordManagerService passwordManagerService)
    {
        this.passwordManagerService = passwordManagerService;
    }

    public async Task InitializeAsync()
    {
        _allItems = (await passwordManagerService.GetPasswordAccountsAsync()).ToList();
        LoadPage();
    }

    [ObservableProperty]
    private int currentPage = 1;

    public int TotalPages => (_allItems.Count + PageSize - 1) / PageSize;

    public bool CanGoNext => CurrentPage < TotalPages;
    public bool CanGoPrevious => CurrentPage > 1;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void NextPage()
    {
        if (CanGoNext)
            CurrentPage++;
    }

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private void PreviousPage()
    {
        if (CanGoPrevious)
            CurrentPage--;
    }

    partial void OnCurrentPageChanged(int oldValue, int newValue)
    {
        LoadPage();
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrevious));
        NextPageCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
    }

    private void LoadPage()
    {
        PagedItems.Clear();

        var items = _allItems
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        foreach (var item in items)
        {
            PagedItems.Add(item);
        }
    }
}
