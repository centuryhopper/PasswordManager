// using CommunityToolkit.Mvvm.ComponentModel;
// using CommunityToolkit.Mvvm.Input;
// using System.Collections.ObjectModel;
// using PasswordManagerMobileApp.Models;

// namespace PasswordManagerMobileApp.MVVM;

// public partial class PaginatedTableViewModel : ObservableObject
// {
//     private const int PageSize = 10;

//     public ObservableCollection<Person> PagedItems { get; } = new();

//     private List<Person> _allItems;

//     public PaginatedTableViewModel()
//     {
//         _allItems = Enumerable.Range(1, 100).Select(i => new Person
//         {
//             Name = $"Person {i}",
//             Age = 20 + (i % 30)
//         }).ToList();

//         LoadPage();
//     }

//     [ObservableProperty]
//     private int currentPage = 1;

//     public int TotalPages => (_allItems.Count + PageSize - 1) / PageSize;

//     public bool CanGoNext => CurrentPage < TotalPages;
//     public bool CanGoPrevious => CurrentPage > 1;

//     [RelayCommand(CanExecute = nameof(CanGoNext))]
//     private void NextPage()
//     {
//         if (CanGoNext)
//             CurrentPage++;
//     }

//     [RelayCommand(CanExecute = nameof(CanGoPrevious))]
//     private void PreviousPage()
//     {
//         if (CanGoPrevious)
//             CurrentPage--;
//     }

//     partial void OnCurrentPageChanged(int oldValue, int newValue)
//     {
//         LoadPage();
//         OnPropertyChanged(nameof(CanGoNext));
//         OnPropertyChanged(nameof(CanGoPrevious));
//         NextPageCommand.NotifyCanExecuteChanged();
//         PreviousPageCommand.NotifyCanExecuteChanged();
//     }

//     private void LoadPage()
//     {
//         PagedItems.Clear();

//         var items = _allItems
//             .Skip((CurrentPage - 1) * PageSize)
//             .Take(PageSize);

//         foreach (var item in items)
//             PagedItems.Add(item);
//     }
// }
