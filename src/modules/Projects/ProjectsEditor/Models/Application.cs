﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ManagedCommon;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;

namespace ProjectsEditor.Models
{
    public class Application : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Project Parent { get; set; }

        public struct WindowPosition
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }
        }

        public string AppName { get; set; }

        public string AppPath { get; set; }

        public string AppTitle { get; set; }

        public string PackageFullName { get; set; }

        public string CommandLineArguments { get; set; }

        public bool Minimized { get; set; }

        public bool Maximized { get; set; }

        private bool _isNotFound;

        [JsonIgnore]
        public bool IsNotFound
        {
            get
            {
                return _isNotFound;
            }

            set
            {
                if (_isNotFound != value)
                {
                    _isNotFound = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsNotFound)));
                }
            }
        }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public bool IsHighlighted { get; set; }

        [JsonIgnore]
        public int RepeatIndex { get; set; }

        [JsonIgnore]
        public string RepeatIndexString
        {
            get
            {
                return RepeatIndex == 0 ? string.Empty : RepeatIndex.ToString(CultureInfo.InvariantCulture);
            }
        }

        [JsonIgnore]
        private Icon _icon = null;

        [JsonIgnore]
        public Icon Icon
        {
            get
            {
                if (_icon == null)
                {
                    try
                    {
                        if (!File.Exists(AppPath) && IsPackagedApp)
                        {
                            Task<AppListEntry> task = Task.Run<AppListEntry>(async () => await GetAppByPackageFamilyNameAsync());
                            AppListEntry packApp = task.Result;
                            if (packApp == null)
                            {
                                IsNotFound = true;
                                _icon = new Icon(@"images\DefaultIcon.ico");
                            }
                            else
                            {
                                string filename = Path.GetFileName(AppPath);
                                string newExeLocation = Path.Combine(packApp.AppInfo.Package.InstalledPath, filename);
                                _icon = Icon.ExtractAssociatedIcon(newExeLocation);
                            }
                        }
                        else
                        {
                            _icon = Icon.ExtractAssociatedIcon(AppPath);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Exception while extracting icon from app path: {AppPath}. Exception message: {e.Message}");
                        IsNotFound = true;
                        _icon = new Icon(@"images\DefaultIcon.ico");
                    }
                }

                return _icon;
            }
        }

        public async Task<AppListEntry> GetAppByPackageFamilyNameAsync()
        {
            var pkgManager = new PackageManager();
            var pkg = pkgManager.FindPackagesForUser(string.Empty, PackagedId).FirstOrDefault();

            if (pkg == null)
            {
                return null;
            }

            var apps = await pkg.GetAppListEntriesAsync();
            if (apps == null || apps.Count == 0)
            {
                return null;
            }

            AppListEntry firstApp = apps[0];

            // RandomAccessStreamReference stream = firstApp.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(64, 64));
            // IRandomAccessStreamWithContentType content = await stream.OpenReadAsync();
            // BitmapImage bitmapImage = new BitmapImage();
            // bitmapImage.StreamSource = (Stream)content;
            return firstApp;
        }

        private BitmapImage _iconBitmapImage;

        public BitmapImage IconBitmapImage
        {
            get
            {
                if (_iconBitmapImage == null)
                {
                    try
                    {
                        Bitmap previewBitmap = new Bitmap(32, 32);
                        using (Graphics graphics = Graphics.FromImage(previewBitmap))
                        {
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            graphics.DrawIcon(Icon, new Rectangle(0, 0, 32, 32));
                        }

                        using (var memory = new MemoryStream())
                        {
                            previewBitmap.Save(memory, ImageFormat.Png);
                            memory.Position = 0;

                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = memory;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze();

                            _iconBitmapImage = bitmapImage;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Exception while drawing icon for app with path: {AppPath}. Exception message: {e.Message}");
                    }
                }

                return _iconBitmapImage;
            }
        }

        public WindowPosition Position { get; set; }

        private WindowPosition? _scaledPosition;

        public WindowPosition ScaledPosition
        {
            get
            {
                if (_scaledPosition == null)
                {
                    double scaleFactor = MonitorSetup.Dpi / 96.0;
                    _scaledPosition = new WindowPosition()
                    {
                        X = (int)(scaleFactor * Position.X),
                        Y = (int)(scaleFactor * Position.Y),
                        Height = (int)(scaleFactor * Position.Height),
                        Width = (int)(scaleFactor * Position.Width),
                    };
                }

                return _scaledPosition.Value;
            }
        }

        public int MonitorNumber { get; set; }

        private MonitorSetup _monitorSetup;

        public MonitorSetup MonitorSetup
        {
            get
            {
                if (_monitorSetup == null)
                {
                    _monitorSetup = Parent.Monitors.Where(x => x.MonitorNumber == MonitorNumber).FirstOrDefault();
                }

                return _monitorSetup;
            }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private bool? _isPackagedApp;

        public string PackagedId { get; set; }

        public string PackagedName { get; set; }

        public string PackagedPublisherID { get; set; }

        public string Aumid { get; set; }

        public bool IsPackagedApp
        {
            get
            {
                if (_isPackagedApp == null)
                {
                    if (!AppPath.StartsWith("C:\\Program Files\\WindowsApps\\", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _isPackagedApp = false;
                    }
                    else
                    {
                        string appPath = AppPath.Replace("C:\\Program Files\\WindowsApps\\", string.Empty);
                        Regex packagedAppPathRegex = new Regex(@"(?<APPID>[^_]*)_\d+.\d+.\d+.\d+_x64__(?<PublisherID>[^\\]*)", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                        Match match = packagedAppPathRegex.Match(appPath);
                        _isPackagedApp = match.Success;
                        if (match.Success)
                        {
                            PackagedName = match.Groups["APPID"].Value;
                            PackagedPublisherID = match.Groups["PublisherID"].Value;
                            PackagedId = $"{PackagedName}_{PackagedPublisherID}";
                            Aumid = $"{PackagedId}!App";
                        }
                    }
                }

                return _isPackagedApp.Value;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
