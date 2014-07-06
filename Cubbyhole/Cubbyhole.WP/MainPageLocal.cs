using Cubbyhole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cubbyhole.WP
{
    public partial class MainPage
    {
        private void Entity_Tap_Local(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _viewModelLocator.MainViewModel.ProceedWithItemLocal(ItemsListBoxLocal.SelectedIndex);
        }

        private void Entity_Hold_Local(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedEntity = (sender as Grid).DataContext as DiskEntity;
            _viewModelLocator.MainViewModel.SelectLocalEntity(selectedEntity);
        }

        private void LocalEntity_Rename_Click(object sender, RoutedEventArgs e)
        {
            var selectedLocalEntity = _viewModelLocator.MainViewModel.SelectedLocalEntity;
            if (selectedLocalEntity.GetType().Equals(typeof(File)))
            {
                var nameParts = selectedLocalEntity.Name.Split('.');
                string extension = nameParts.Last();
                int index = selectedLocalEntity.Name.IndexOf(extension);
                string name = selectedLocalEntity.Name.Substring(0, index - 1);
                _viewModelLocator.MainViewModel.RenameTextBox = name;
            }
            else
            {
                // No folder in local storage, I'm lazy
            }
            localEntity_rename_popup.IsOpen = true;
        }

        private void LocalRename_Ok_Click(object sender, RoutedEventArgs e)
        {
            localEntity_rename_popup.IsOpen = false;
            if (_viewModelLocator.MainViewModel.RenameTextBox.Equals(""))
            {
                _informerManagerLocator.InformerManager.AddMessage("Error", "You have to give a name to the file");
            }
            else
            {
                BindingExpression expression = LocalEntity_rename_textbox.GetBindingExpression(TextBox.TextProperty);
                expression.UpdateSource();
                var selectedLocalEntity = _viewModelLocator.MainViewModel.SelectedLocalEntity;
                if (selectedLocalEntity.GetType().Equals(typeof(File)))
                {
                    var nameParts = selectedLocalEntity.Name.Split('.');
                    string extension = nameParts.Last();
                    _viewModelLocator.MainViewModel.SelectedLocalEntity.Name = _viewModelLocator.MainViewModel.RenameTextBox + "." + extension;
                }
                else
                {
                    // No folder in local storage, I'm lazy
                }
                _viewModelLocator.MainViewModel.UpdateLocalEntity(_viewModelLocator.MainViewModel.SelectedLocalEntity);
            }
        }

        private void LocalRename_Cancel_Click(object sender, RoutedEventArgs e)
        {
            localEntity_rename_popup.IsOpen = false;
        }

        private void LocalEntity_Delete_Click(object sender, RoutedEventArgs e)
        {
            _viewModelLocator.MainViewModel.DeleteLocalEntity(_viewModelLocator.MainViewModel.SelectedLocalEntity);
        }
    }
}
