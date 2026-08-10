using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProfiDesktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentPage;
    [ObservableProperty] private bool _isSideBarExpanded = true;
    [ObservableProperty] private string _userName = "Иванов И.И.";
    [ObservableProperty] private string _userRole = "HR";
    [ObservableProperty] private string _lastUpdated = "";

    public bool IsLoginPage => CurrentPage is LoginViewModel;

    // вызываем каждый раз при смене страницы и проверяем не страница входа ли это
    partial void OnCurrentPageChanged(ViewModelBase newValue)
    {
        OnPropertyChanged(nameof(IsLoginPage));
    }

    public MainWindowViewModel()
    {
        _currentPage = new LoginViewModel(this);
    }

    public void OnLoggedIn(string userName, string role)
    {
        UserName = userName;
        UserRole = $"({role})";
        CurrentPage = new DashBoardViewModel(this);
    }

    [RelayCommand]
    public void Logout()
    {
        UserName = "";
        UserRole = "";
        CurrentPage = new LoginViewModel(this);
    }

    [RelayCommand]
    public async Task Refresh()
    {
        if (CurrentPage is DashBoardViewModel dashBoard)
            await dashBoard.LoadDataAsync();
    }

    [RelayCommand] public void ToggleSidebar()   => IsSideBarExpanded = !IsSideBarExpanded;
    [RelayCommand] public void GoDashboard()     => CurrentPage = new DashBoardViewModel(this);
    [RelayCommand] public void GoSearch()     => CurrentPage = new SearchViewModel(this);
}

