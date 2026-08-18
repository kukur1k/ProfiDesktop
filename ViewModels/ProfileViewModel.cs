using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProfiDesktop.Services;
namespace ProfiDesktop.ViewModels;

public partial class ProfileViewModel : ViewModelBase
{
    
    private readonly MainWindowViewModel _main;
    private readonly int _userId;

    // данные юзера
    [ObservableProperty] private string _publicId      = "";
    [ObservableProperty] private string _displayName   = "";
    [ObservableProperty] private bool   _isActive      = false;

    [ObservableProperty] private double _competencyIndex = 0;
    [ObservableProperty] private double _trustLevel      = 0;
    [ObservableProperty] private int    _confirmsCount   = 0;
    [ObservableProperty] private int    _skillsCount     = 0;

    // скилы и опыт

    public ObservableCollection<SkillItem> Skills { get; } = new();
    public ObservableCollection<ExperienceItem> Experiences { get; } = new();


    public double CompetencyBarWidth => CompetencyIndex * 3;
    public double TrustBarWidth      => TrustLevel * 3;

    
    
    
    [ObservableProperty] private bool   _isLoading = false;
    [ObservableProperty] private string _errorText = "";
    [ObservableProperty] private bool   _hasError  = false;

    public ProfileViewModel(MainWindowViewModel main, int userId)
    {
        _main = main;
        _userId = userId;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var data = await ApiService.Instance.GetUserByIdAsync(_userId);
            if (data is null)
            {
                ErrorText = "Профиль не наайден";
                HasError = true;
                return;
            }

            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                DisplayName = data.DisplayName;
                IsActive = data.IsActive;
                CompetencyIndex = data.CompetencyIndex;
                TrustLevel      = data.TrustLevel;
                ConfirmsCount   = data.ConfirmsCount;
                SkillsCount     = data.SkillsCount;

                OnPropertyChanged(nameof(CompetencyBarWidth));
                OnPropertyChanged(nameof(TrustBarWidth));

                Skills.Clear();
                foreach (var s in data.Skills)
                {
                    Skills.Add(new SkillItem{
                        Technology = s.Technology,
                        Level = s.Level,
                        HasConfirms = s.HasConfirms,
                        ConfirmsCount = s.ConfirmsCount
                    });
                }

                Experiences.Clear();
                foreach (var e in data.Experiences)
                {
                    Experiences.Add(new ExperienceItem
                    {
                        DateStart = e.DateStart,
                        DateEnd = e.DateEnd,
                        IsCurrent = e.IsCurrent,
                        EmpType = e.EmpType,
                        Position = e.Position
                    });
                }
            });
        }
        catch
        {
            ErrorText = "Ошибка загрузки профиля";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

}



public class ProfileData
{
    public string PublicId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public double CompetencyIndex { get; set; }
    public double TrustLevel { get; set; }
    public int ConfirmsCount { get; set; }
    public int SkillsCount { get; set; }
    public List<SkillItem> Skills { get; set; } = new();
    public List<ExperienceItem> Experiences { get; set; } = new();
}

public class SkillItem
{
    public string Technology { get; set; } = string.Empty;
    public int Level { get; set; }
    public int ConfirmsCount { get; set; }
    public bool HasConfirms {get; set;}
}

public class ExperienceItem
{
    public string DateStart { get; set; } = string.Empty;
    public string DateEnd   { get; set; } = string.Empty;
    public bool   IsCurrent { get; set; }
    public string EmpType   { get; set; } = string.Empty;
    public string Position  { get; set; } = string.Empty;
}