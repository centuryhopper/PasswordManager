using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using PasswordManagerMobileApp.Models;
using PasswordManagerMobileApp.Services;

namespace PasswordManagerMobileApp.MVVM;


/*
TotalPages is now re-notified when _allItems changes

CanGoNext and CanGoPrevious are properly synced

CurrentPage triggers LoadPage() and state updates automatically

Everything that relies on _allItems gets notified after InitializeAsync()
*/

public partial class PasswordsGridVM : ObservableObject
{
    private const int PageSize = 10;
    private readonly IPasswordManagerService passwordManagerService;

    public ObservableCollection<PasswordAccountDTO> PagedItems { get; } = [];

    private List<PasswordAccountDTO> _allItems = [];

    [ObservableProperty]
    private int currentPage = 1;
    [ObservableProperty]
    private bool isPasswordVisible;

    public PasswordsGridVM(IPasswordManagerService passwordManagerService)
    {
        this.passwordManagerService = passwordManagerService;

        Task.Run(async () => {
            _allItems = (await passwordManagerService.GetPasswordAccountsAsync()).ToList();
            CurrentPage = 1; // Reset to first page
            LoadPage();
            UpdatePaginationState();
        });
    }

    public int TotalPages => (_allItems.Count + PageSize - 1) / PageSize;

    public bool CanGoNext => CurrentPage < TotalPages;
    public bool CanGoPrevious => CurrentPage > 1;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void NextPage()
    {
        if (CanGoNext)
        {
            CurrentPage++;
            UpdatePaginationState();
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private void PreviousPage()
    {
        if (CanGoPrevious)
        {
            CurrentPage--;
            UpdatePaginationState();
        }
    }

    partial void OnCurrentPageChanged(int oldValue, int newValue)
    {
        LoadPage();
        UpdatePaginationState();
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

    private void UpdatePaginationState()
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(CanGoPrevious));

        NextPageCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
    }
}
