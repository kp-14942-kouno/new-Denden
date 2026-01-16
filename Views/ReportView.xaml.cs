using System.Windows;
using DeDen.ViewModels;

namespace DeDen.Views;

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
