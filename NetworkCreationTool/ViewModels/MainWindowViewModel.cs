﻿using System.IO;
using System.Windows.Forms;
using Aptacode.StateNet.Interfaces;
using Aptacode.StateNet.Network;
using Aptacode.StateNet.Wpf.ViewModels;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Aptacode.StateNet.NetworkCreationTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            NetworkViewModel = new NetworkViewModel();
        }

        #region EventHandlers

        private void OnStateSelected(object sender, StateViewModel e)
        {
            ConnectionEditorViewModel.SelectedState = e;
        }

        #endregion


        private void New()
        {
            _selectedFilePath = string.Empty;
            _network = new StateNetwork();
            var startState = _network.CreateState("Start");
            _network.StartState = startState;
            StateNetworkViewModel = new StateNetworkViewModel(_network);
        }

        private void Load()
        {
            _selectedFilePath = SelectFile();
            if (string.IsNullOrEmpty(_selectedFilePath)) return;

            var jsonString = File.ReadAllText(_selectedFilePath);
            _network = JsonConvert.DeserializeObject<StateNetwork>(jsonString);
            StateNetworkViewModel = new StateNetworkViewModel(_network);
        }

        private void Save()
        {
            while (string.IsNullOrEmpty(_selectedFilePath)) _selectedFilePath = SaveNew();

            var jsonString = JsonConvert.SerializeObject(_network);
            File.WriteAllText(_selectedFilePath, jsonString);
        }

        private string SaveNew()
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = "NewNetwork"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Json Files (*.json) |*.json;"; // Filter files by extension

            // Show save file dialog box
            var result = dlg.ShowDialog();

            // Process save file dialog box results
            return result == true ? dlg.FileName : string.Empty;
        }

        private string SelectFile()
        {
            var fileDialog = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "Json Files (*.json) |*.json;"
            };

            return fileDialog.ShowDialog() == DialogResult.OK ? fileDialog.FileName : string.Empty;
        }

        #region Properties

        private IStateNetwork _network;
        private string _selectedFilePath;

        private StateNetworkViewModel _stateNetworkViewModel;

        public StateNetworkViewModel StateNetworkViewModel
        {
            get => _stateNetworkViewModel;
            set
            {
                SetProperty(ref _stateNetworkViewModel, value);
                StateSelectorViewModel = new StateSelectorViewModel(StateNetworkViewModel);
                InputSelectorViewModel = new InputSelectorViewModel(StateNetworkViewModel);
                ConnectionEditorViewModel = new ConnectionEditorViewModel(StateNetworkViewModel);
                NetworkViewModel.StateNetwork = StateNetworkViewModel;
                StateSelectorViewModel.OnStateSelected += OnStateSelected;
            }
        }

        private StateSelectorViewModel _stateSelectorViewModel;

        public StateSelectorViewModel StateSelectorViewModel
        {
            get => _stateSelectorViewModel;
            set => SetProperty(ref _stateSelectorViewModel, value);
        }

        private InputSelectorViewModel _inputSelectorViewModel;

        public InputSelectorViewModel InputSelectorViewModel
        {
            get => _inputSelectorViewModel;
            set => SetProperty(ref _inputSelectorViewModel, value);
        }

        private ConnectionEditorViewModel _connectionEditorViewModel;

        public ConnectionEditorViewModel ConnectionEditorViewModel
        {
            get => _connectionEditorViewModel;
            set => SetProperty(ref _connectionEditorViewModel, value);
        }

        private NetworkViewModel _networkViewModel;

        public NetworkViewModel NetworkViewModel
        {
            get => _networkViewModel;
            set => SetProperty(ref _networkViewModel, value);
        }

        #endregion


        #region Commands

        private DelegateCommand _loadButtonCommand;

        public DelegateCommand LoadButtonCommand =>
            _loadButtonCommand ?? (_loadButtonCommand = new DelegateCommand(Load));

        private DelegateCommand _saveButtonCommand;

        public DelegateCommand SaveButtonCommand =>
            _saveButtonCommand ?? (_saveButtonCommand = new DelegateCommand(Save));

        private DelegateCommand _newButtonCommand;

        public DelegateCommand NewButtonCommand =>
            _newButtonCommand ?? (_newButtonCommand = new DelegateCommand(New));

        #endregion
    }
}