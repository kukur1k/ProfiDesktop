using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProfiDesktop.Services;

namespace ProfiDesktop.ViewModels;

public partial class ChoiceSlViewModel : ViewModelBase
{
    private readonly int _userId;

    [ObservableProperty] private List<ShortList> _shortlists = new();
    [ObservableProperty] private ShortList? _selectedShortlist;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _noteStr = string.Empty;
    [ObservableProperty] private string _errorText = string.Empty;
    [ObservableProperty] private bool _hasError;

    

    public ChoiceSlViewModel(int userId)
    {
        _userId = userId;

    }

    public async Task GetShortLists()
    {
        var res = await ApiService.Instance.GetShortListsAsync();
        if (res is null) return;
        Shortlists = res.data;
    }


    [RelayCommand]
    private async Task AddToShortList()
    {
        if (SelectedShortlist is null)
        {
            ErrorText = "Выберете подборку";
            HasError = true;
            return;
        }

        IsLoading = true;
        HasError = false;

        try
        {
            var error = await ApiService.Instance.PostUserToShortlist(_userId, SelectedShortlist.Id,
                 NoteStr ?? $"Добавлен {DateTime.Now} пользователем - {_userId}");
            if (error is null)
            {
                CloseWindow(true);
            }
            else
            {
                ErrorText = error;
                HasError = true;
            }
        }
        catch(Exception ex)
        {
            ErrorText = ex.Message;
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
        
        
    }



    [RelayCommand]
    private void Cancel()
    {
        CloseWindow(false);
    }

    private void CloseWindow(bool success)
    {
        // Закрыть модальное окно
        if (Avalonia.Application.Current?.ApplicationLifetime is 
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            
            var windows = desktop.Windows;
            var modal = windows.FirstOrDefault(w => w.DataContext == this);
            if (modal is not null)
            {
                modal.Close(success);
            }
        }
    }

}
