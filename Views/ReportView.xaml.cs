using System.Windows;
using DenDen.ViewModels;

namespace DenDen.Views;

/// <summary>
/// ReportView.xaml の相互作用ロジック
/// </summary>
public partial class ReportView : Window
{
    public ReportView(ReportViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.CloseRequested += (s, e) => Close();
    }
}
