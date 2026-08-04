using CommunityToolkit.Mvvm.ComponentModel;

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
}

