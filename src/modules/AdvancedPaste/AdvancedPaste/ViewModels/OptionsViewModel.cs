﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AdvancedPaste.Helpers;
using AdvancedPaste.Models;
using AdvancedPaste.Services;
using AdvancedPaste.Settings;
using Common.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using WinUIEx;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

namespace AdvancedPaste.ViewModels
{
    public sealed partial class OptionsViewModel : ObservableObject, IDisposable
    {
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly DispatcherTimer _clipboardTimer;
        private readonly IUserSettings _userSettings;
        private readonly IPasteFormatExecutor _pasteFormatExecutor;
        private readonly AICompletionsHelper _aiHelper;
        private readonly App app = App.Current as App;

        public DataPackageView ClipboardData { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCustomAIEnabled))]
        [NotifyPropertyChangedFor(nameof(ClipboardHasData))]
        [NotifyPropertyChangedFor(nameof(InputTxtBoxPlaceholderText))]
        [NotifyPropertyChangedFor(nameof(AIDisabledErrorText))]
        private ClipboardFormat _availableClipboardFormats;

        [ObservableProperty]
        private bool _clipboardHistoryEnabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AIDisabledErrorText))]
        [NotifyPropertyChangedFor(nameof(IsCustomAIEnabled))]
        private bool _isAllowedByGPO;

        [ObservableProperty]
        private string _apiErrorText;

        [ObservableProperty]
        private string _query = string.Empty;

        private bool _pasteFormatsDirty;

        [ObservableProperty]
        private bool _busy;

        public ObservableCollection<PasteFormat> StandardPasteFormats { get; } = [];

        public ObservableCollection<PasteFormat> CustomActionPasteFormats { get; } = [];

        public bool IsCustomAIEnabled => IsAllowedByGPO && _aiHelper.IsAIEnabled && ClipboardHasText;

        public bool ClipboardHasData => AvailableClipboardFormats != ClipboardFormat.None;

        private bool ClipboardHasText => AvailableClipboardFormats.HasFlag(ClipboardFormat.Text);

        private bool Visible => app?.GetMainWindow()?.Visible is true;

        public event EventHandler<CustomActionActivatedEventArgs> CustomActionActivated;

        public OptionsViewModel(AICompletionsHelper aiHelper, IUserSettings userSettings, IPasteFormatExecutor pasteFormatExecutor)
        {
            _aiHelper = aiHelper;
            _userSettings = userSettings;
            _pasteFormatExecutor = pasteFormatExecutor;

            GeneratedResponses = [];
            GeneratedResponses.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasMultipleResponses));
                OnPropertyChanged(nameof(CurrentIndexDisplay));
            };

            ClipboardHistoryEnabled = IsClipboardHistoryEnabled();
            _clipboardTimer = new() { Interval = TimeSpan.FromSeconds(1) };
            _clipboardTimer.Tick += ClipboardTimer_Tick;
            _clipboardTimer.Start();

            RefreshPasteFormats();
            _userSettings.Changed += (_, _) => EnqueueRefreshPasteFormats();
            PropertyChanged += (_, e) =>
            {
                string[] dirtyingProperties = [nameof(Query), nameof(IsCustomAIEnabled), nameof(AvailableClipboardFormats)];

                if (dirtyingProperties.Contains(e.PropertyName))
                {
                    EnqueueRefreshPasteFormats();
                }
            };
        }

        private async void ClipboardTimer_Tick(object sender, object e)
        {
            if (Visible)
            {
                await ReadClipboard();
                UpdateAllowedByGPO();
            }
        }

        private void EnqueueRefreshPasteFormats()
        {
            if (_pasteFormatsDirty)
            {
                return;
            }

            _pasteFormatsDirty = true;
            _dispatcherQueue.TryEnqueue(() =>
            {
                RefreshPasteFormats();
                _pasteFormatsDirty = false;
            });
        }

        private PasteFormat CreatePasteFormat(PasteFormats format) => new(format, AvailableClipboardFormats, IsCustomAIEnabled, ResourceLoaderInstance.ResourceLoader.GetString);

        private PasteFormat CreatePasteFormat(AdvancedPasteCustomAction customAction) => new(customAction, AvailableClipboardFormats, IsCustomAIEnabled);

        private void RefreshPasteFormats()
        {
            var ctrlString = ResourceLoaderInstance.ResourceLoader.GetString("CtrlKey");
            int shortcutNum = 0;

            string GetNextShortcutText()
            {
                shortcutNum++;
                return shortcutNum <= 9 ? $"{ctrlString}+{shortcutNum}" : string.Empty;
            }

            IEnumerable<PasteFormat> FilterAndSort(IEnumerable<PasteFormat> pasteFormats) =>
                from pasteFormat in pasteFormats
                let comparison = StringComparison.CurrentCultureIgnoreCase
                where pasteFormat.Name.Contains(Query, comparison) || pasteFormat.Prompt.Contains(Query, comparison)
                orderby pasteFormat.IsEnabled descending
                select pasteFormat;

            void UpdateFormats(ObservableCollection<PasteFormat> collection, IEnumerable<PasteFormat> pasteFormats)
            {
                collection.Clear();

                foreach (var format in FilterAndSort(pasteFormats))
                {
                    if (format.IsEnabled)
                    {
                        format.ShortcutText = GetNextShortcutText();
                    }

                    collection.Add(format);
                }
            }

            UpdateFormats(StandardPasteFormats, Enum.GetValues<PasteFormats>()
                                                    .Where(format => PasteFormat.MetadataDict[format].IsCoreAction || _userSettings.AdditionalActions.Contains(format))
                                                    .Select(CreatePasteFormat));

            UpdateFormats(CustomActionPasteFormats, _userSettings.CustomActions.Select(CreatePasteFormat));
        }

        public void Dispose()
        {
            _clipboardTimer.Stop();
            GC.SuppressFinalize(this);
        }

        public async Task ReadClipboard()
        {
            if (Busy)
            {
                return;
            }

            ClipboardData = Clipboard.GetContent();
            AvailableClipboardFormats = await ClipboardHelper.GetAvailableClipboardFormats(ClipboardData);
        }

        public async Task OnShow()
        {
            ApiErrorText = string.Empty;
            Query = string.Empty;

            await ReadClipboard();
            UpdateAllowedByGPO();

            if (IsAllowedByGPO)
            {
                var openAIKey = AICompletionsHelper.LoadOpenAIKey();
                var currentKey = _aiHelper.GetKey();
                bool keyChanged = openAIKey != currentKey;

                if (keyChanged)
                {
                    app.GetMainWindow().StartLoading();

                    await Task.Run(() =>
                    {
                        _aiHelper.SetOpenAIKey(openAIKey);
                    }).ContinueWith(
                        (t) =>
                        {
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                app.GetMainWindow().FinishLoading(_aiHelper.IsAIEnabled);
                                OnPropertyChanged(nameof(InputTxtBoxPlaceholderText));
                                OnPropertyChanged(nameof(AIDisabledErrorText));
                                OnPropertyChanged(nameof(IsCustomAIEnabled));
                            });
                        },
                        TaskScheduler.Default);
                }
            }

            ClipboardHistoryEnabled = IsClipboardHistoryEnabled();
            GeneratedResponses.Clear();
        }

        // List to store generated responses
        public ObservableCollection<string> GeneratedResponses { get; set; } = [];

        // Index to keep track of the current response
        private int _currentResponseIndex;

        public int CurrentResponseIndex
        {
            get => _currentResponseIndex;
            set
            {
                if (value >= 0 && value < GeneratedResponses.Count)
                {
                    SetProperty(ref _currentResponseIndex, value);
                    CustomFormatResult = GeneratedResponses[_currentResponseIndex];
                    OnPropertyChanged(nameof(CurrentIndexDisplay));
                }
            }
        }

        public bool HasMultipleResponses => GeneratedResponses.Count > 1;

        public string CurrentIndexDisplay => $"{CurrentResponseIndex + 1}/{GeneratedResponses.Count}";

        public string InputTxtBoxPlaceholderText
            => ResourceLoaderInstance.ResourceLoader.GetString(ClipboardHasData ? "CustomFormatTextBox/PlaceholderText" : "ClipboardEmptyWarning");

        public string AIDisabledErrorText
        {
            get
            {
                if (!ClipboardHasText)
                {
                    return ResourceLoaderInstance.ResourceLoader.GetString("ClipboardDataNotTextWarning");
                }

                if (!IsAllowedByGPO)
                {
                    return ResourceLoaderInstance.ResourceLoader.GetString("OpenAIGpoDisabled");
                }

                if (!_aiHelper.IsAIEnabled)
                {
                    return ResourceLoaderInstance.ResourceLoader.GetString("OpenAINotConfigured");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public ObservableCollection<Tuple<string, string>> AvailableFormatsText
        {
            get
            {
                List<Tuple<ClipboardFormat, string>> formatQueryList = new()
                {
                    new Tuple<ClipboardFormat, string>(ClipboardFormat.Text, "Text "),
                    new Tuple<ClipboardFormat, string>(ClipboardFormat.Html, "Html "),
                    new Tuple<ClipboardFormat, string>(ClipboardFormat.Audio, "Audio "),
                    new Tuple<ClipboardFormat, string>(ClipboardFormat.Image, "Image "),
                    new Tuple<ClipboardFormat, string>(ClipboardFormat.ImageFile, "ImageFile "),
                };

                ObservableCollection<Tuple<string, string>> returnList = new();

                foreach (var formatQuery in formatQueryList)
                {
                    if (AvailableClipboardFormats.HasFlag(formatQuery.Item1))
                    {
                        returnList.Add(new Tuple<string, string>(formatQuery.Item2, "Hello world"));
                    }
                }

                return returnList;
            }
        }

        [ObservableProperty]
        private string _customFormatResult;

        [RelayCommand]
        public void PasteCustom()
        {
            var text = GeneratedResponses.ElementAtOrDefault(CurrentResponseIndex);

            if (!string.IsNullOrEmpty(text))
            {
                ClipboardHelper.SetClipboardTextContent(text);
                HideWindow();

                if (_userSettings.SendPasteKeyCombination)
                {
                    ClipboardHelper.SendPasteKeyCombination();
                }

                Query = string.Empty;
            }
        }

        // Command to select the previous custom format
        [RelayCommand]
        public void PreviousCustomFormat()
        {
            if (CurrentResponseIndex > 0)
            {
                CurrentResponseIndex--;
            }
        }

        // Command to select the next custom format
        [RelayCommand]
        public void NextCustomFormat()
        {
            if (CurrentResponseIndex < GeneratedResponses.Count - 1)
            {
                CurrentResponseIndex++;
            }
        }

        // Command to open the Settings window.
        [RelayCommand]
        public void OpenSettings()
        {
            SettingsDeepLink.OpenSettings(SettingsDeepLink.SettingsWindow.AdvancedPaste, true);
            (App.Current as App).GetMainWindow().Close();
        }

        internal async Task ExecutePasteFormatAsync(PasteFormats format, PasteActionSource source)
        {
            await ReadClipboard();
            await ExecutePasteFormatAsync(CreatePasteFormat(format), source);
        }

        internal async Task ExecutePasteFormatAsync(PasteFormat pasteFormat, PasteActionSource source)
        {
            if (Busy)
            {
                Logger.LogWarning($"Execution of {pasteFormat.Name} from {source} suppressed as busy");
                return;
            }

            if (!pasteFormat.IsEnabled)
            {
                return;
            }

            Busy = true;
            ApiErrorText = string.Empty;
            Query = pasteFormat.Query;

            if (pasteFormat.Format == PasteFormats.Custom)
            {
                SaveQuery(Query);
            }

            try
            {
                // Minimum time to show busy spinner for AI actions when triggered by global keyboard shortcut.
                var aiActionMinTaskTime = TimeSpan.FromSeconds(2);
                var delayTask = (Visible && source == PasteActionSource.GlobalKeyboardShortcut) ? Task.Delay(aiActionMinTaskTime) : Task.CompletedTask;
                var aiOutput = await _pasteFormatExecutor.ExecutePasteFormatAsync(pasteFormat, source);

                await delayTask;

                if (pasteFormat.Format != PasteFormats.Custom)
                {
                    HideWindow();

                    if (source == PasteActionSource.GlobalKeyboardShortcut || _userSettings.SendPasteKeyCombination)
                    {
                        ClipboardHelper.SendPasteKeyCombination();
                    }
                }
                else
                {
                    var pasteResult = source == PasteActionSource.GlobalKeyboardShortcut || !_userSettings.ShowCustomPreview;

                    GeneratedResponses.Add(aiOutput);
                    CurrentResponseIndex = GeneratedResponses.Count - 1;
                    CustomActionActivated?.Invoke(this, new CustomActionActivatedEventArgs(pasteFormat.Prompt, pasteResult));

                    if (pasteResult)
                    {
                        PasteCustom();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error executing paste format", ex);
                ApiErrorText = ex is PasteActionException ? ex.Message : ResourceLoaderInstance.ResourceLoader.GetString("PasteError");
            }

            Busy = false;
        }

        internal async Task ExecutePasteFormat(VirtualKey key)
        {
            var pasteFormat = StandardPasteFormats.Concat(CustomActionPasteFormats)
                                                  .Where(pasteFormat => pasteFormat.IsEnabled)
                                                  .ElementAtOrDefault(key - VirtualKey.Number1);

            if (pasteFormat != null)
            {
                await ExecutePasteFormatAsync(pasteFormat, PasteActionSource.InAppKeyboardShortcut);
            }
        }

        internal async Task ExecuteCustomAction(int customActionId, PasteActionSource source)
        {
            Logger.LogTrace();

            await ReadClipboard();

            var customAction = _userSettings.CustomActions.FirstOrDefault(customAction => customAction.Id == customActionId);

            if (customAction != null)
            {
                await ExecutePasteFormatAsync(CreatePasteFormat(customAction), source);
            }
        }

        internal async Task GenerateCustomFunction(PasteActionSource triggerSource)
        {
            AdvancedPasteCustomAction customAction = new() { Name = "Default", Prompt = Query };
            await ExecutePasteFormatAsync(CreatePasteFormat(customAction), triggerSource);
        }

        private void HideWindow()
        {
            var mainWindow = app.GetMainWindow();

            if (mainWindow != null)
            {
                Windows.Win32.Foundation.HWND hwnd = (Windows.Win32.Foundation.HWND)mainWindow.GetWindowHandle();
                Windows.Win32.PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_HIDE);
            }
        }

        internal CustomQuery RecallPreviousCustomQuery()
        {
            return LoadPreviousQuery();
        }

        internal void SaveQuery(string inputQuery)
        {
            Logger.LogTrace();

            DataPackageView clipboardData = Clipboard.GetContent();

            if (clipboardData == null || !clipboardData.Contains(StandardDataFormats.Text))
            {
                Logger.LogWarning("Clipboard does not contain text data");
                return;
            }

            var currentClipboardText = Task.Run(async () => await clipboardData.GetTextAsync()).Result;

            var queryData = new CustomQuery
            {
                Query = inputQuery,
                ClipboardData = currentClipboardText,
            };

            SettingsUtils utils = new();
            utils.SaveSettings(queryData.ToString(), Constants.AdvancedPasteModuleName, Constants.LastQueryJsonFileName);
        }

        internal CustomQuery LoadPreviousQuery()
        {
            SettingsUtils utils = new();
            var query = utils.GetSettings<CustomQuery>(Constants.AdvancedPasteModuleName, Constants.LastQueryJsonFileName);
            return query;
        }

        private bool IsClipboardHistoryEnabled()
        {
            string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Clipboard\";
            try
            {
                int enableClipboardHistory = (int)Registry.GetValue(registryKey, "EnableClipboardHistory", false);
                return enableClipboardHistory != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void UpdateAllowedByGPO()
        {
            IsAllowedByGPO = PowerToys.GPOWrapper.GPOWrapper.GetAllowedAdvancedPasteOnlineAIModelsValue() != PowerToys.GPOWrapper.GpoRuleConfigured.Disabled;
        }
    }
}
