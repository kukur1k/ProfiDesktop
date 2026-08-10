using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProfiDesktop.Services;

namespace ProfiDesktop.ViewModels;

public partial class SearchViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private string? _technology = null;
    [ObservableProperty] private List<string>? _suggestions = null;
    [ObservableProperty] private int _minLevel = 0;
    [ObservableProperty] private int _maxLevel = 10;
    [ObservableProperty] private int _minRating = 0;
    [ObservableProperty] private int _minExp = 0;
    [ObservableProperty] private int _pageSize = 25;

    [ObservableProperty] private List<int> _pageSizes = new() { 25, 50, 100 };

    [ObservableProperty] private int    _currentPage  = 1;

    [ObservableProperty] private bool _showSuggest = false;

    // строка для поиска технологий с подсказками
    [ObservableProperty] private string _suggestQuery;
     

   

    [ObservableProperty] private List<SearchUserResultItem?> _totalUsersItems;
    [ObservableProperty] private List<SearchUserResultItem?> _usersItems;
    

    public SearchViewModel(MainWindowViewModel main)
    {
        _main = main;
        _ = LoadUsers();
    }

    public async Task<string> LoadUsers()
    {
        var responce = await ApiService.Instance.GetUsersSearchAsync(Technology, MinLevel, MaxLevel, MinRating, MinExp, PageSize);
        TotalUsersItems = responce?.data?.Users;
        UsersItems = responce?.data?.Users;
        return "";
    }

    partial void OnPageSizeChanged(int oldValue, int newValue)
    {
        UsersItems = TotalUsersItems.Take(newValue).ToList();
    }

    [RelayCommand]
    private async Task Search()
    {
        CurrentPage = 1;
        await LoadUsers();
    }


    partial void OnSuggestQueryChanged(string value)
    {
        if (value.Length >= 1)
        {
            _ = LoagSuggestAsync(value);
        }
        else
        {
            Suggestions.Clear();
        }
    }

    // загрузка подсказки
    private async Task LoagSuggestAsync(string query)
    {
        var result = await ApiService.Instance.GetSuggestSearchAsync(query);
        if (result is not null && result.Count > 0)
        {
            var selected = Technology?.Split(',').ToList();
            Suggestions = result.Where(x => !selected.Contains(x)).ToList();
            ShowSuggest = Suggestions.Count > 0;
        }
        else
        {
            Suggestions.Clear();
            ShowSuggest = false;
        }
    }
    

    // команда при выборе (добавлении чипа технологии из списка)
    [RelayCommand]
    private void SelectTechOnSuggest(string technology)
    {
        var exist = !string.IsNullOrEmpty(Technology)
            ? Technology.Split(',').ToList()
            : new List<string>();

        if (!exist.Contains(technology))
        {
            exist.Add(technology);
            Technology = string.Join(",", exist);
        }
    }


}