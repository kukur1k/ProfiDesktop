using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProfiDesktop.Services;


namespace ProfiDesktop.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    // Главный ВМ со всеми основными полями -- передаем для доступа к ним и управления навигацией через главный ВМ 
    private readonly MainWindowViewModel _main;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _rememberMe;
    [ObservableProperty] private string _errorText = "";  // -- текст ошибки
    [ObservableProperty] private bool _hasError;  // -- наличие ошибки
    [ObservableProperty] private bool _isLoading;  // -- состояние загрузки


    // При вызове ВМ-логин передаем ему главный ВМ для управления 
    public LoginViewModel(MainWindowViewModel main)
    {
        _main = main;
    }


    [RelayCommand]
    private async Task Login()
    {
        HasError = false;
        ErrorText = "";

        if (Password.Length < 6)
        {
            HasError = true;
            ErrorText = "Пароль минимум 6 символов";
            return;
        }

        IsLoading = true;
        var error = await ApiService.Instance.LoginAsync(Email, Password);
        IsLoading = false;

        if (error is not null)
        {
            HasError = true;
            ErrorText = error;
            return;
        }

        // получили данные о юзере при успешном входе
        var me = await ApiService.Instance.GetMe();

        // Обратились к главному ВМ и у него заполнели поля о пользователе
        _main.OnLoggedIn(me?.LastName + " " + me?.FirstName, me?.Role ?? "User");
        // Обратились к главному ВМ и в нем указали главную страницу текущей, и ей передали сам главный ВМ для управления
        _main.CurrentPage = new DashBoardViewModel(_main);
    }

    [RelayCommand]
    private void Cancel()
    {
        Email = "";
        Password = "";
        HasError = false;
    }
}