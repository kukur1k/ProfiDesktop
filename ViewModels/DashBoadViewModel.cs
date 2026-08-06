using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProfiDesktop.Services;


namespace ProfiDesktop.ViewModels;

public partial class DashBoardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    private Timer? _autoRefreshTimer;

    [ObservableProperty] private string _userName = "Иванов И.И.";
    [ObservableProperty] private string _userRole  = "HR";

    // ---------------------------Метрики---------------------------
    [ObservableProperty] private string _activeProfiles = "-";
    [ObservableProperty] private string _profilesDelta = "-";
    [ObservableProperty] private string _profilesDeltaWeek = "-";
    [ObservableProperty] private string _avgRating = "-";
    [ObservableProperty] private string _avgRatingDelta = "-";
    [ObservableProperty] private string _avgRatingDeltaWeek = "-";
    [ObservableProperty] private string _vacancyMatch = "-";
    [ObservableProperty] private string _vacancyMatchDelta = "-";
    [ObservableProperty] private string _vacancyMatchDeltaWeek = "-";

    // ---------------------------Состояние---------------------------
    [ObservableProperty] private bool   _isLoading   = false;
    [ObservableProperty] private string _lastUpdated = "";
    [ObservableProperty] private string _errorText   = "";
    [ObservableProperty] private bool   _hasError    = false;

    // ---------------------------Боковая панель---------------------------
    [ObservableProperty] private bool _isSidebarExpanded = true;


    public DashBoardViewModel(MainWindowViewModel main)
    {
        Console.WriteLine($"DashboardViewModel {ApiService.Instance.AccessToken}");
        _main = main;
        _ = LoadDataAsync();
        _autoRefreshTimer = new Timer(
            _ => Avalonia.Threading.Dispatcher.UIThread.Post(
                async () => await LoadDataAsync()),
            null,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(5)
        );
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            await Task.WhenAll(LoadSummaryAsync(), LoadTopTechAsync());
            LastUpdated = $"Обновлено: {DateTime.Now:HH:mm:ss}";
            Console.WriteLine($"Обновлено: {DateTime.Now:HH:mm:ss}");
        }
        catch (HttpRequestException)
        {
            ErrorText = "Сервер недоступен";
            HasError  = true;
        }
        catch (Exception ex)
        {
            ErrorText = $"Ошибка: {ex.Message}";
            HasError  = true;
        }
        finally
        {
            IsLoading = false;
        }
        
    }


    private async Task LoadSummaryAsync()
    {
        var summary = await ApiService.Instance.GetSummary();
        if (summary is null) return;

        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            ActiveProfiles = summary.ActiveProfiles.ToString("NO");
            ProfilesDelta = FormatDelta(summary.ProfilesDelta, "");
            ProfilesDeltaWeek = FormatDelta(summary.ProfilesDeltaWeek, "");
            VacancyMatch = $"{summary.VacancyMatch}%";
            VacancyMatchDelta = FormatDelta(summary.VacancyMatchDelta, "%");
            VacancyMatchDeltaWeek = FormatDelta(summary.VacancyMatchDeltaWeek, "%");
            AvgRating = $"{summary.AvgRating:F1}/10";
            AvgRatingDelta = FormatDelta(summary.AvgRatingDelta, "");
            AvgRatingDeltaWeek = FormatDelta(summary.AvgRatingDeltaWeek, "");
        });
    
    }

    private async Task LoadTopTechAsync()
    {}

    private static string FormatDelta(double delta, string unit) =>
        delta >= 0 ? $"+{delta}{unit}" : $"{delta}{unit}";
}
