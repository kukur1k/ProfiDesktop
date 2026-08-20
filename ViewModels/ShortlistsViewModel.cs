using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ProfiDesktop.Models;
using ProfiDesktop.Services;

namespace ProfiDesktop.ViewModels;

public partial class ShortlistsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    

    [ObservableProperty] private List<string> _shortlistsTitles = new(); // выбор по имени
    [ObservableProperty] private string _selectedShortlist;
    private bool HasSelectedShortlist => !string.IsNullOrEmpty(SelectedShortlist);

    // для каждой подборки
    [ObservableProperty] private DateTime _createdAt = DateTime.Now;
    [ObservableProperty] public List<SLCandidate>? _candidates;
    [ObservableProperty] private int _candCount = 0;



    [ObservableProperty] public List<ShortList>? _allShortLists;



    public ShortlistsViewModel(MainWindowViewModel main)
    {
        _main = main;
        _ = GetShortLists();
    }

    public async Task GetShortLists()
    {
        var res = await ApiService.Instance.GetShortListsAsync();
        if (res is null) return;
        AllShortLists = res.data;
        ShortlistsTitles = res.data.Select(r => r.Name).ToList();
    }

    partial void OnSelectedShortlistChanged(string value)
    {
        OnPropertyChanged(nameof(HasSelectedShortlist));
        var shortList = _allShortLists?.FirstOrDefault(s => s.Name == value);
        if (shortList is null) return;

        Candidates = shortList.Candidates.ToList() ?? [];
        CandCount = shortList.CandidatesCount;
        CreatedAt = shortList.CreatedAt;
    }



}
