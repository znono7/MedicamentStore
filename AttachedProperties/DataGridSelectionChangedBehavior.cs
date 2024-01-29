using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MedicamentStore
{
    public static class DataGridSelectionChangedBehavior
    {
        public static ICommand GetSelectionChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SelectionChangedCommandProperty);
        }

        public static void SetSelectionChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SelectionChangedCommandProperty, value);
        }
        #region for Client

        public static readonly DependencyProperty SelectionChangedCommandProperty =
        DependencyProperty.RegisterAttached(
            "SelectionChangedCommand",
            typeof(ICommand),
            typeof(DataGridSelectionChangedBehavior),
            new PropertyMetadata(null, OnSelectionChangedCommandChanged));


        private static void OnSelectionChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                dataGrid.SelectionChanged += (sender, args) =>
                {
                    if (e.NewValue is ICommand command && command.CanExecute(dataGrid.SelectedItem))
                    {
                        command.Execute(dataGrid.SelectedItem);

                        if (dataGrid.SelectedItem is Client client && dataGrid.DataContext is CustomerCmbViewModel viewModel)
                        {
                            viewModel.HandleSelectedItem(client);
                        }

                    }
                };
            }
        }

        #endregion

        #region for Items
        public static ICommand GetSelectionSuppChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SelectionSuppChangedCommandProperty);
        }

        public static void SetSelectionSuppChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SelectionSuppChangedCommandProperty, value);
        }
        public static readonly DependencyProperty SelectionSuppChangedCommandProperty =
        DependencyProperty.RegisterAttached(
            "SelectionSuppChangedCommand",
            typeof(ICommand),
            typeof(DataGridSelectionChangedBehavior),
            new PropertyMetadata(null, OnSelectionSuppChangedCommandChanged));


        private static void OnSelectionSuppChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                dataGrid.SelectionChanged += (sender, args) =>
                {
                    if (e.NewValue is ICommand command && command.CanExecute(dataGrid.SelectedItem))
                    {
                        command.Execute(dataGrid.SelectedItem);

                        if (dataGrid.SelectedItem is Supplies item && dataGrid.DataContext is CustomerCmbSuppViewModel viewModel)
                        {
                            viewModel.HandleSelectedItem(item);
                        }

                    }
                };
            }
        }

        #endregion
    }



    //public static class DataGridSelectionChangedBehavior
    //{
    //    public static ICommand GetSelectionChangedCommand(DependencyObject obj)
    //    {
    //        return (ICommand)obj.GetValue(SelectionChangedCommandProperty);
    //    }

    //    public static void SetSelectionChangedCommand(DependencyObject obj, ICommand value)
    //    {
    //        obj.SetValue(SelectionChangedCommandProperty, value);
    //    }
    //    public static readonly DependencyProperty SelectionChangedCommandProperty =
    //    DependencyProperty.RegisterAttached(
    //        "SelectionChangedCommand",
    //        typeof(ICommand),
    //        typeof(DataGridSelectionChangedBehavior),
    //        new PropertyMetadata(null, OnSelectionChangedCommandChanged));

    //    private static void OnSelectionChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is DataGrid dataGrid)
    //        {
    //            dataGrid.SelectionChanged += (sender, args) =>
    //            {
    //                if (e.NewValue is ICommand command && command.CanExecute(dataGrid.SelectedItem))
    //                {
    //                    command.Execute(dataGrid.SelectedItem);

    //                    if (dataGrid.SelectedItem is Client selectedItem)
    //                    {
    //                        CustomerCmbViewModel viewModel = dataGrid.DataContext as CustomerCmbViewModel;
    //                        if (viewModel != null)
    //                        {
    //                            viewModel.SelectedId = selectedItem.Id;
    //                            viewModel.SelectedName = selectedItem.Name;
    //                        }
    //                    }
    //                }
    //            };
    //        }
    //    }
    //}
}
