﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectsEditor.Data;
using ProjectsEditor.Models;
using ProjectsEditor.ViewModels;

namespace ProjectsEditor
{
    /// <summary>
    /// Interaction logic for ProjectEditor.xaml
    /// </summary>
    public partial class ProjectEditor : Page
    {
        private MainViewModel _mainViewModel;

        public ProjectEditor(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeComponent();
        }

        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            Project projectToSave = this.DataContext as Project;
            if (projectToSave.EditorWindowTitle == Properties.Resources.CreateProject)
            {
                _mainViewModel.AddNewProject(projectToSave);
            }
            else
            {
                _mainViewModel.SaveProject(projectToSave);
            }

            _mainViewModel.SwitchToMainView();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            // delete the temp file created by the snapshot tool
            TempProjectData parser = new TempProjectData();
            parser.DeleteTempFile();

            _mainViewModel.SwitchToMainView();
        }

        private void EditNameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Project project = this.DataContext as Project;
                project.Name = EditNameTextBox.Text;
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Project project = this.DataContext as Project;
                _mainViewModel.CancelProjectName(project);
            }
        }

        private void EditNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _mainViewModel.SaveProjectName(DataContext as Project);
        }

        private void AppBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Models.Application app = border.DataContext as Models.Application;
            app.IsHighlighted = true;
            Project project = app.Parent;
            project.Initialize();
        }

        private void AppBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Models.Application app = border.DataContext as Models.Application;
            app.IsHighlighted = false;
            Project project = app.Parent;
            project.Initialize();
        }

        private void EditNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Project project = this.DataContext as Project;
            project.Name = EditNameTextBox.Text;
            project.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Project.CanBeSaved)));
        }
    }
}
