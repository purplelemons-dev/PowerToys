﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using ManagedCommon;
using ProjectsEditor.Models;

namespace ProjectsEditor.Utils
{
    public class DrawHelper
    {
        private const int IconSize = 128;
        private static Font font = new("Tahoma", 24);
        private static List<Brush> iconBrushes = new List<Brush>
        {
            ////Brushes.Gold,
            ////Brushes.SteelBlue,
            ////Brushes.SkyBlue,
            ////Brushes.DarkGoldenrod,
            ////Brushes.ForestGreen,
            ////Brushes.Peru,
            ////Brushes.Chartreuse,
            ////Brushes.LightPink,
            ////Brushes.CadetBlue,
            ////Brushes.DarkSalmon,
            ////Brushes.Orange,
            ////Brushes.DarkSeaGreen,
            ////Brushes.Yellow,
            ////Brushes.Green,
            ////Brushes.Orange,
            ////Brushes.White,
            new SolidBrush(Color.FromArgb(255, 40, 101, 120)),
            new SolidBrush(Color.FromArgb(255, 58, 91, 153)),
            new SolidBrush(Color.FromArgb(255, 87, 88, 163)),
            new SolidBrush(Color.FromArgb(255, 116, 87, 160)),
            new SolidBrush(Color.FromArgb(255, 139, 82, 145)),
        };

        private static int iconBrushIndex;

        public static BitmapImage DrawPreview(Project project, Rectangle bounds)
        {
            double scale = 0.1;
            int Scaled(double value)
            {
                return (int)(value * scale);
            }

            Dictionary<string, int> repeatCounter = new Dictionary<string, int>();

            var selectedApps = project.Applications.Where(x => x.IsSelected);
            foreach (Application app in selectedApps)
            {
                if (repeatCounter.TryGetValue(app.AppPath, out int value))
                {
                    repeatCounter[app.AppPath] = ++value;
                }
                else
                {
                    repeatCounter.Add(app.AppPath, 1);
                }

                app.RepeatIndex = repeatCounter[app.AppPath];
            }

            // remove those repeatIndexes, which are single 1-es (no repetitions) by setting them to 0
            foreach (Application app in selectedApps.Where(x => repeatCounter[x.AppPath] == 1))
            {
                app.RepeatIndex = 0;
            }

            foreach (Application app in project.Applications.Where(x => !x.IsSelected))
            {
                app.RepeatIndex = 0;
            }

            // now that all repeat index values are set, update the repeat index strings on UI
            foreach (Application app in project.Applications)
            {
                app.OnPropertyChanged(new PropertyChangedEventArgs("RepeatIndexString"));
            }

            Bitmap previewBitmap = new Bitmap(Scaled(bounds.Width), Scaled(bounds.Height * 1.2));
            using (Graphics g = Graphics.FromImage(previewBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.Clear(Color.FromArgb(0, 0, 0, 0));
                Brush brush = new SolidBrush(Common.ThemeManager.GetCurrentTheme() == Common.Theme.Dark ? Color.FromArgb(10, 255, 255, 255) : Color.FromArgb(10, 0, 0, 0));
                foreach (Application app in project.Applications.Where(x => x.IsSelected && !x.Minimized))
                {
                    Rectangle rect = new Rectangle(Scaled(app.ScaledPosition.X - bounds.Left), Scaled(app.ScaledPosition.Y - bounds.Top), Scaled(app.ScaledPosition.Width), Scaled(app.ScaledPosition.Height));
                    DrawWindow(g, brush, rect, app);
                }

                Rectangle rectMinimized = new Rectangle(0, Scaled(bounds.Height), Scaled(bounds.Width), Scaled(bounds.Height * 0.2));
                DrawWindow(g, brush, rectMinimized, project.Applications.Where(x => x.IsSelected && x.Minimized));
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

                return bitmapImage;
            }
        }

        public static void DrawWindow(Graphics graphics, Brush brush, Rectangle bounds, Application app)
        {
            if (graphics == null)
            {
                return;
            }

            if (brush == null)
            {
                return;
            }

            using (GraphicsPath path = RoundedRect(bounds))
            {
                if (app.IsHighlighted)
                {
                    graphics.DrawPath(new Pen(Common.ThemeManager.GetCurrentTheme() == Common.Theme.Dark ? Color.White : Color.DarkGray, graphics.VisibleClipBounds.Height / 50), path);
                }
                else
                {
                    graphics.DrawPath(new Pen(Common.ThemeManager.GetCurrentTheme() == Common.Theme.Dark ? Color.FromArgb(128, 82, 82, 82) : Color.FromArgb(128, 160, 160, 160), graphics.VisibleClipBounds.Height / 200), path);
                }

                graphics.FillPath(brush, path);
            }

            double iconSize = Math.Min(bounds.Width, bounds.Height) * 0.3;
            Rectangle iconBounds = new Rectangle((int)(bounds.Left + (bounds.Width / 2) - (iconSize / 2)), (int)(bounds.Top + (bounds.Height / 2) - (iconSize / 2)), (int)iconSize, (int)iconSize);

            try
            {
                graphics.DrawIcon(app.Icon, iconBounds);
                if (app.RepeatIndex > 0)
                {
                    string indexString = app.RepeatIndex.ToString(CultureInfo.InvariantCulture);
                    int indexSize = (int)(iconBounds.Width * 0.5);
                    Rectangle indexBounds = new Rectangle(iconBounds.Right - indexSize, iconBounds.Bottom - indexSize, indexSize, indexSize);

                    var textSize = graphics.MeasureString(indexString, font);
                    var state = graphics.Save();
                    graphics.TranslateTransform(indexBounds.Left, indexBounds.Top);
                    graphics.ScaleTransform(indexBounds.Width / textSize.Width, indexBounds.Height / textSize.Height);
                    graphics.DrawString(indexString, font, Brushes.Black, PointF.Empty);
                    graphics.Restore(state);
                }
            }
            catch (Exception)
            {
                // sometimes drawing an icon throws an exception despite that the icon seems to be ok
            }
        }

        public static void DrawWindow(Graphics graphics, Brush brush, Rectangle bounds, IEnumerable<Application> apps)
        {
            int appsCount = apps.Count();
            if (appsCount == 0)
            {
                return;
            }

            if (graphics == null)
            {
                return;
            }

            if (brush == null)
            {
                return;
            }

            using (GraphicsPath path = RoundedRect(bounds))
            {
                if (apps.Where(x => x.IsHighlighted).Any())
                {
                    graphics.DrawPath(new Pen(Common.ThemeManager.GetCurrentTheme() == Common.Theme.Dark ? Color.White : Color.DarkGray, graphics.VisibleClipBounds.Height / 50), path);
                }
                else
                {
                    graphics.DrawPath(new Pen(Common.ThemeManager.GetCurrentTheme() == Common.Theme.Dark ? Color.FromArgb(128, 82, 82, 82) : Color.FromArgb(128, 160, 160, 160), graphics.VisibleClipBounds.Height / 200), path);
                }

                graphics.FillPath(brush, path);
            }

            double iconSize = Math.Min(bounds.Width, bounds.Height) * 0.5;
            for (int iconCounter = 0; iconCounter < appsCount; iconCounter++)
            {
                Application app = apps.ElementAt(iconCounter);
                Rectangle iconBounds = new Rectangle((int)(bounds.Left + (bounds.Width / 2) - (iconSize * ((appsCount / 2) - iconCounter))), (int)(bounds.Top + (bounds.Height / 2) - (iconSize / 2)), (int)iconSize, (int)iconSize);

                try
                {
                    graphics.DrawIcon(app.Icon, iconBounds);
                    if (app.RepeatIndex > 0)
                    {
                        string indexString = app.RepeatIndex.ToString(CultureInfo.InvariantCulture);
                        int indexSize = (int)(iconBounds.Width * 0.5);
                        Rectangle indexBounds = new Rectangle(iconBounds.Right - indexSize, iconBounds.Bottom - indexSize, indexSize, indexSize);

                        var textSize = graphics.MeasureString(indexString, font);
                        var state = graphics.Save();
                        graphics.TranslateTransform(indexBounds.Left, indexBounds.Top);
                        graphics.ScaleTransform(indexBounds.Width / textSize.Width, indexBounds.Height / textSize.Height);
                        graphics.DrawString(indexString, font, Brushes.Black, PointF.Empty);
                        graphics.Restore(state);
                    }
                }
                catch (Exception)
                {
                    // sometimes drawing an icon throws an exception despite that the icon seems to be ok
                }
            }
        }

        public static GraphicsPath RoundedRect(Rectangle bounds)
        {
            int minorSize = Math.Min(bounds.Width, bounds.Height);
            int radius = (int)(minorSize / 8);

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc
            path.AddArc(arc, 180, 90);

            // top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        internal static string CreateShortcutIcon(Project project, out Bitmap bitmap)
        {
            object shDesktop = (object)"Desktop";
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            string shortcutIconFilename = (string)shell.SpecialFolders.Item(ref shDesktop) + $"\\{project.Name}.ico";
            bitmap = new Bitmap(IconSize, IconSize);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // if (project != null)
                // {
                //    List<Application> selectedApps = project.Applications.Where(x => x.IsSelected).ToList();
                //    if (selectedApps.Count > 0)
                //    {
                //        graphics.DrawIcon(selectedApps[0].Icon, new Rectangle(0, 0, IconSize / 2, IconSize / 2));
                //    }
                //    if (selectedApps.Count > 1)
                //    {
                //        graphics.DrawIcon(selectedApps[1].Icon, new Rectangle(IconSize / 2, 0, IconSize / 2, IconSize / 2));
                //    }
                //    if (selectedApps.Count > 2)
                //    {
                //        graphics.DrawIcon(selectedApps[2].Icon, new Rectangle(0, IconSize / 2, IconSize / 2, IconSize / 2));
                //    }
                //    if (selectedApps.Count > 3)
                //    {
                //        graphics.DrawIcon(selectedApps[3].Icon, new Rectangle(IconSize / 2, IconSize / 2, IconSize / 2, IconSize / 2));
                //    }
                // }
                // graphics.FillRectangle(new System.Drawing.SolidBrush(Color.FromArgb(128, 32, 32, 32)), 0, 0, IconSize, IconSize);
                graphics.FillEllipse(iconBrushes[iconBrushIndex], 0, 0, IconSize, IconSize);

                string shortcutChars = "PR";

                if (project != null)
                {
                    shortcutChars = project.GetShortcutChars();
                }

                Rectangle indexBounds;
                if (shortcutChars.Length > 1)
                {
                    indexBounds = new Rectangle(0, 0, IconSize, IconSize);
                }
                else
                {
                    indexBounds = new Rectangle(IconSize / 4, 0, IconSize / 2, IconSize);
                }

                var textSize = graphics.MeasureString(shortcutChars, font);
                var state = graphics.Save();
                graphics.TranslateTransform(indexBounds.Left, indexBounds.Top);
                graphics.ScaleTransform(indexBounds.Width / textSize.Width, indexBounds.Height / textSize.Height);
                graphics.DrawString(shortcutChars, font, Brushes.White, PointF.Empty);
                graphics.Restore(state);
                iconBrushIndex++;
                if (iconBrushIndex >= iconBrushes.Count)
                {
                    iconBrushIndex = 0;
                }
            }

            FileStream fileStream = new FileStream(shortcutIconFilename, FileMode.OpenOrCreate);

            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);

                BinaryWriter iconWriter = new BinaryWriter(fileStream);
                if (fileStream != null && iconWriter != null)
                {
                    // 0-1 reserved, 0
                    iconWriter.Write((byte)0);
                    iconWriter.Write((byte)0);

                    // 2-3 image type, 1 = icon, 2 = cursor
                    iconWriter.Write((short)1);

                    // 4-5 number of images
                    iconWriter.Write((short)1);

                    // image entry 1
                    // 0 image width
                    iconWriter.Write((byte)IconSize);

                    // 1 image height
                    iconWriter.Write((byte)IconSize);

                    // 2 number of colors
                    iconWriter.Write((byte)0);

                    // 3 reserved
                    iconWriter.Write((byte)0);

                    // 4-5 color planes
                    iconWriter.Write((short)0);

                    // 6-7 bits per pixel
                    iconWriter.Write((short)32);

                    // 8-11 size of image data
                    iconWriter.Write((int)memoryStream.Length);

                    // 12-15 offset of image data
                    iconWriter.Write((int)(6 + 16));

                    // write image data
                    // png data must contain the whole png data file
                    iconWriter.Write(memoryStream.ToArray());

                    iconWriter.Flush();
                }
            }

            fileStream.Flush();
            fileStream.Close();
            return shortcutIconFilename;
        }

        internal static BitmapImage DrawPreviewIcons(Project project)
        {
            var selectedApps = project.Applications.Where(x => x.IsSelected);
            int appsCount = selectedApps.Count();
            if (appsCount == 0)
            {
                return null;
            }

            Bitmap previewBitmap = new Bitmap(32 * appsCount, 24);
            using (Graphics graphics = Graphics.FromImage(previewBitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                int appIndex = 0;
                foreach (var app in selectedApps)
                {
                    try
                    {
                        graphics.DrawIcon(app.Icon, new Rectangle(32 * appIndex, 0, 24, 24));
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Exception while drawing the icon for app {app.AppName}. Exception message: {e.Message}");
                    }

                    appIndex++;
                }
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

                return bitmapImage;
            }
        }

        private static void CreateExamples(Project project)
        {
            Bitmap bitmap = new Bitmap(IconSize + 1000, IconSize * iconBrushes.Count);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int brushIndex = 0; brushIndex < iconBrushes.Count; brushIndex++)
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.FillEllipse(iconBrushes[brushIndex], 0, IconSize * brushIndex, IconSize, IconSize);

                    string shortcutChars = "PR";

                    Rectangle indexBounds;
                    indexBounds = new Rectangle(0, IconSize * brushIndex, IconSize, IconSize);

                    var textSize = graphics.MeasureString(shortcutChars, font);
                    var state = graphics.Save();
                    graphics.TranslateTransform(indexBounds.Left, indexBounds.Top);
                    graphics.ScaleTransform(indexBounds.Width / textSize.Width, indexBounds.Height / textSize.Height);
                    graphics.DrawString(shortcutChars, font, Brushes.Black, 0, 0);
                    graphics.Restore(state);

                    var b = (SolidBrush)iconBrushes[brushIndex];
                    var colorName = (from p in typeof(System.Drawing.Color).GetProperties()
                                     where p.PropertyType.Equals(typeof(System.Drawing.Color))
                                     let value = (System.Drawing.Color)p.GetValue(null, null)
                                     where value.R == b.Color.R &&
                                           value.G == b.Color.G &&
                                           value.B == b.Color.B &&
                                           value.A == b.Color.A
                                     select p.Name).DefaultIfEmpty("unknown").First();

                    graphics.DrawString(colorName, font, Brushes.White, IconSize, IconSize * brushIndex);
                }
            }

            bitmap.Save(@"C:\temp\shortcutIcons.png");
        }
    }
}
