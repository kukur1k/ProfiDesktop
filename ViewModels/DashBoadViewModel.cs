using System;
using CommunityToolkit.Mvvm.ComponentModel;


namespace ProfiDesktop.ViewModels;

public partial class DashBoardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    public DashBoardViewModel(MainWindowViewModel main)
    {
        _main = main;
    }
}