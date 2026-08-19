using Avalonia.Controls;
using System;  
using Avalonia.Interactivity;
using ProfiDesktop.ViewModels;
using ProfiDesktop.Services;

namespace ProfiDesktop.Views;

public partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
    }

    // Обработчик клика по подсказке
    private void OnSuggestionClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string technology)
        {
            if (DataContext is SearchViewModel vm)
            {
                vm.SelectTechOnSuggest(technology);
            }
        }
    }

    // Обработчик удаления чипа
    private void OnRemoveChipClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string technology)
        {
            if (DataContext is SearchViewModel vm)
            {
                vm.RemoveTechnology(technology);
            }
        }
    }

    private void OnUserDoubleClick(object sender, RoutedEventArgs e)
{
    if (sender is DataGrid grid && grid.SelectedItem is SearchUserResultItem user)
    {
        if (DataContext is SearchViewModel vm)
        {
            vm.GoToUserProfile(user.Id);
        }
    }
}
}