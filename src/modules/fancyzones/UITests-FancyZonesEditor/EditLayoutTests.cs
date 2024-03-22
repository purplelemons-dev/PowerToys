﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FancyZonesEditorCommon.Data;
using Microsoft.FancyZonesEditor.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using static FancyZonesEditorCommon.Data.Constants;
using static FancyZonesEditorCommon.Data.CustomLayouts;
using static Microsoft.FancyZonesEditor.UnitTests.Utils.FancyZonesEditorSession;

namespace Microsoft.FancyZonesEditor.UITests
{
    [TestClass]
    public class EditLayoutTests
    {
        private static readonly CustomLayouts.CustomLayoutListWrapper Layouts = new CustomLayouts.CustomLayoutListWrapper
        {
            CustomLayouts = new List<CustomLayouts.CustomLayoutWrapper>
            {
                new CustomLayoutWrapper
                {
                    Uuid = "{0D6D2F58-9184-4804-81E4-4E4CC3476DC1}",
                    Type = Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid],
                    Name = "Grid custom layout",
                    Info = new CustomLayouts().ToJsonElement(new GridInfoWrapper
                    {
                        Rows = 2,
                        Columns = 2,
                        RowsPercentage = new List<int> { 5000, 5000 },
                        ColumnsPercentage = new List<int> { 5000, 5000 },
                        CellChildMap = new int[][] { [0, 1], [2, 3] },
                        SensitivityRadius = 30,
                        Spacing = 26,
                        ShowSpacing = false,
                    }),
                },
                new CustomLayoutWrapper
                {
                    Uuid = "{0EB9BF3E-010E-46D7-8681-1879D1E111E1}",
                    Type = Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid],
                    Name = "Grid-9",
                    Info = new CustomLayouts().ToJsonElement(new GridInfoWrapper
                    {
                        Rows = 3,
                        Columns = 3,
                        RowsPercentage = new List<int> { 3333, 3333, 3334 },
                        ColumnsPercentage = new List<int> { 3333, 3333, 3334 },
                        CellChildMap = new int[][] { [0, 1, 2], [3, 4, 5], [6, 7, 8] },
                        SensitivityRadius = 20,
                        Spacing = 3,
                        ShowSpacing = false,
                    }),
                },
                new CustomLayoutWrapper
                {
                    Uuid = "{E7807D0D-6223-4883-B15B-1F3883944C09}",
                    Type = Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas],
                    Name = "Canvas custom layout",
                    Info = new CustomLayouts().ToJsonElement(new CanvasInfoWrapper
                    {
                        RefHeight = 952,
                        RefWidth = 1500,
                        SensitivityRadius = 10,
                        Zones = new List<CanvasInfoWrapper.CanvasZoneWrapper>
                        {
                            new CanvasInfoWrapper.CanvasZoneWrapper
                            {
                                X = 0,
                                Y = 0,
                                Width = 900,
                                Height = 522,
                            },
                            new CanvasInfoWrapper.CanvasZoneWrapper
                            {
                                X = 900,
                                Y = 0,
                                Width = 600,
                                Height = 750,
                            },
                            new CanvasInfoWrapper.CanvasZoneWrapper
                            {
                                X = 0,
                                Y = 522,
                                Width = 1500,
                                Height = 430,
                            },
                        },
                    }),
                },
            },
        };

        private static TestContext? _context;
        private static FancyZonesEditorSession? _session;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _context = testContext;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _context = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LayoutTemplates layoutTemplates = new LayoutTemplates();
            LayoutTemplates.TemplateLayoutsListWrapper templateLayoutsListWrapper = new LayoutTemplates.TemplateLayoutsListWrapper
            {
                LayoutTemplates = new List<LayoutTemplates.TemplateLayoutWrapper>
                {
                    new LayoutTemplates.TemplateLayoutWrapper
                    {
                        Type = LayoutType.Blank.TypeToString(),
                    },
                    new LayoutTemplates.TemplateLayoutWrapper
                    {
                        Type = LayoutType.Focus.TypeToString(),
                        ZoneCount = 10,
                    },
                    new LayoutTemplates.TemplateLayoutWrapper
                    {
                        Type = LayoutType.Rows.TypeToString(),
                        ZoneCount = 2,
                        ShowSpacing = true,
                        Spacing = 10,
                        SensitivityRadius = 10,
                    },
                    new LayoutTemplates.TemplateLayoutWrapper
                    {
                        Type = LayoutType.Columns.TypeToString(),
                        ZoneCount = 2,
                        ShowSpacing = true,
                        Spacing = 20,
                        SensitivityRadius = 20,
                    },
                    new LayoutTemplates.TemplateLayoutWrapper
                    {
                        Type = LayoutType.Grid.TypeToString(),
                        ZoneCount = 4,
                        ShowSpacing = false,
                        Spacing = 10,
                        SensitivityRadius = 30,
                    },
                    new LayoutTemplates.TemplateLayoutWrapper
                    {
                        Type = LayoutType.PriorityGrid.TypeToString(),
                        ZoneCount = 3,
                        ShowSpacing = true,
                        Spacing = 1,
                        SensitivityRadius = 40,
                    },
                },
            };
            FancyZonesEditorSession.Files.LayoutTemplatesIOHelper.WriteData(layoutTemplates.Serialize(templateLayoutsListWrapper));

            CustomLayouts customLayouts = new CustomLayouts();
            FancyZonesEditorSession.Files.CustomLayoutsIOHelper.WriteData(customLayouts.Serialize(Layouts));

            DefaultLayouts defaultLayouts = new DefaultLayouts();
            DefaultLayouts.DefaultLayoutsListWrapper defaultLayoutsListWrapper = new DefaultLayouts.DefaultLayoutsListWrapper
            {
                DefaultLayouts = new List<DefaultLayouts.DefaultLayoutWrapper> { },
            };
            FancyZonesEditorSession.Files.DefaultLayoutsIOHelper.WriteData(defaultLayouts.Serialize(defaultLayoutsListWrapper));

            LayoutHotkeys layoutHotkeys = new LayoutHotkeys();
            LayoutHotkeys.LayoutHotkeysWrapper layoutHotkeysWrapper = new LayoutHotkeys.LayoutHotkeysWrapper
            {
                LayoutHotkeys = new List<LayoutHotkeys.LayoutHotkeyWrapper> { },
            };
            FancyZonesEditorSession.Files.LayoutHotkeysIOHelper.WriteData(layoutHotkeys.Serialize(layoutHotkeysWrapper));

            AppliedLayouts appliedLayouts = new AppliedLayouts();
            AppliedLayouts.AppliedLayoutsListWrapper appliedLayoutsWrapper = new AppliedLayouts.AppliedLayoutsListWrapper
            {
                AppliedLayouts = new List<AppliedLayouts.AppliedLayoutWrapper> { },
            };
            FancyZonesEditorSession.Files.AppliedLayoutsIOHelper.WriteData(appliedLayouts.Serialize(appliedLayoutsWrapper));

            _session = new FancyZonesEditorSession(_context!);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _session?.Close();
            FancyZonesEditorSession.Files.Restore();
        }

        [TestMethod]
        public void OpenEditMode()
        {
            _session?.ClickEditLayout(Layouts.CustomLayouts[0].Name);
            _session?.Click(_session?.FindByAccessibilityId(AccessibilityId.EditZonesButton));
            Assert.IsNotNull(_session?.FindByName(ElementName.GridLayoutEditor));
            _session?.Click(ElementName.Cancel);
        }

        [TestMethod]
        public void OpenEditModeFromContextMenu()
        {
            _session?.ClickContextMenuItem(Layouts.CustomLayouts[0].Name, FancyZonesEditorSession.ElementName.EditZones);
            Assert.IsNotNull(_session?.FindByName(ElementName.GridLayoutEditor));
            _session?.Click(ElementName.Cancel);
        }

        [TestMethod]
        public void Canvas_AddZone_Save()
        {
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);
            _session?.Click(_session?.FindByAccessibilityId(AccessibilityId.NewZoneButton));
            _session?.Click(ElementName.Save);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());
            Assert.AreEqual(expected.Zones.Count + 1, actual.Zones.Count);
        }

        [TestMethod]
        public void Canvas_AddZone_Cancel()
        {
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);
            _session?.Click(_session?.FindByAccessibilityId(AccessibilityId.NewZoneButton));
            _session?.Click(ElementName.Cancel);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());
            Assert.AreEqual(expected.Zones.Count, actual.Zones.Count);
        }

        [TestMethod]
        public void Canvas_DeleteZone_Save()
        {
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);
            _session?.WaitElementDisplayedByName(FancyZonesEditorSession.ElementName.CanvasEditorWindow);
            _session?.ClickDeleteZone(1);
            _session?.Click(ElementName.Save);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());
            Assert.AreEqual(expected.Zones.Count - 1, actual.Zones.Count);
        }

        [TestMethod]
        public void Canvas_DeleteZone_Cancel()
        {
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);
            _session?.ClickDeleteZone(1);
            _session?.Click(ElementName.Cancel);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());
            Assert.AreEqual(expected.Zones.Count, actual.Zones.Count);
        }

        [TestMethod]
        public void Canvas_MoveZone_Save()
        {
            int zoneNumber = 1;
            int xOffset = 100;
            int yOffset = 100;
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.Drag(_session.GetZone(zoneNumber, FancyZonesEditorSession.ClassName.CanvasZone)!, xOffset, yOffset);
            _session?.Click(ElementName.Save);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());

            // changed zone, exact offset may vary depending on screen resolution
            Assert.IsTrue(expected.Zones[zoneNumber - 1].X < actual.Zones[zoneNumber - 1].X, $"X: {expected.Zones[zoneNumber - 1].X} > {actual.Zones[zoneNumber - 1].X}");
            Assert.IsTrue(expected.Zones[zoneNumber - 1].Y < actual.Zones[zoneNumber - 1].Y, $"Y: {expected.Zones[zoneNumber - 1].Y} > {actual.Zones[zoneNumber - 1].Y}");
            Assert.AreEqual(expected.Zones[zoneNumber - 1].Width, actual.Zones[zoneNumber - 1].Width);
            Assert.AreEqual(expected.Zones[zoneNumber - 1].Height, actual.Zones[zoneNumber - 1].Height);

            // other zones
            for (int i = 0; i < expected.Zones.Count; i++)
            {
                if (i != zoneNumber - 1)
                {
                    Assert.AreEqual(expected.Zones[i].X, actual.Zones[i].X);
                    Assert.AreEqual(expected.Zones[i].Y, actual.Zones[i].Y);
                    Assert.AreEqual(expected.Zones[i].Width, actual.Zones[i].Width);
                    Assert.AreEqual(expected.Zones[i].Height, actual.Zones[i].Height);
                }
            }
        }

        [TestMethod]
        public void Canvas_MoveZone_Cancel()
        {
            int zoneNumber = 1;
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.Drag(_session.GetZone(zoneNumber, FancyZonesEditorSession.ClassName.CanvasZone)!, 100, 100);
            _session?.Click(ElementName.Cancel);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());
            for (int i = 0; i < expected.Zones.Count; i++)
            {
                Assert.AreEqual(expected.Zones[i].X, actual.Zones[i].X);
                Assert.AreEqual(expected.Zones[i].Y, actual.Zones[i].Y);
                Assert.AreEqual(expected.Zones[i].Width, actual.Zones[i].Width);
                Assert.AreEqual(expected.Zones[i].Height, actual.Zones[i].Height);
            }
        }

        [TestMethod]
        public void Canvas_ResizeZone_Save()
        {
            int zoneNumber = 1;
            int xOffset = 100;
            int yOffset = 100;
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.Drag((WindowsElement)_session.GetZone(zoneNumber, FancyZonesEditorSession.ClassName.CanvasZone)?.FindElementByAccessibilityId(FancyZonesEditorSession.AccessibilityId.TopRightCorner)!, xOffset, yOffset);
            _session?.Click(ElementName.Save);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());

            // changed zone, exact offset may vary depending on screen resolution
            Assert.AreEqual(expected.Zones[zoneNumber - 1].X, actual.Zones[zoneNumber - 1].X);
            Assert.IsTrue(expected.Zones[zoneNumber - 1].Y < actual.Zones[zoneNumber - 1].Y, $"Y: {expected.Zones[zoneNumber - 1].Y} > {actual.Zones[zoneNumber - 1].Y}");
            Assert.IsTrue(expected.Zones[zoneNumber - 1].Width < actual.Zones[zoneNumber - 1].Width, $"Width: {expected.Zones[zoneNumber - 1].Width} < {actual.Zones[zoneNumber - 1].Width}");
            Assert.IsTrue(expected.Zones[zoneNumber - 1].Height > actual.Zones[zoneNumber - 1].Height, $"Height: {expected.Zones[zoneNumber - 1].Height} < {actual.Zones[zoneNumber - 1].Height}");

            // other zones
            for (int i = 0; i < expected.Zones.Count; i++)
            {
                if (i != zoneNumber - 1)
                {
                    Assert.AreEqual(expected.Zones[i].X, actual.Zones[i].X);
                    Assert.AreEqual(expected.Zones[i].Y, actual.Zones[i].Y);
                    Assert.AreEqual(expected.Zones[i].Width, actual.Zones[i].Width);
                    Assert.AreEqual(expected.Zones[i].Height, actual.Zones[i].Height);
                }
            }
        }

        [TestMethod]
        public void Canvas_ResizeZone_Cancel()
        {
            int zoneNumber = 1;
            int xOffset = 100;
            int yOffset = 100;
            var canvas = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Canvas]);
            _session?.ClickContextMenuItem(canvas.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.Drag((WindowsElement)_session.GetZone(zoneNumber, FancyZonesEditorSession.ClassName.CanvasZone)?.FindElementByAccessibilityId(FancyZonesEditorSession.AccessibilityId.TopRightCorner)!, xOffset, yOffset);
            _session?.Click(ElementName.Cancel);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.CanvasFromJsonElement(canvas.Info.ToString());
            var actual = customLayouts.CanvasFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == canvas.Uuid).Info.GetRawText());

            for (int i = 0; i < expected.Zones.Count; i++)
            {
                Assert.AreEqual(expected.Zones[i].X, actual.Zones[i].X);
                Assert.AreEqual(expected.Zones[i].Y, actual.Zones[i].Y);
                Assert.AreEqual(expected.Zones[i].Width, actual.Zones[i].Width);
                Assert.AreEqual(expected.Zones[i].Height, actual.Zones[i].Height);
            }
        }

        [TestMethod]
        public void Grid_SplitZone_Save()
        {
            int zoneNumber = 1;
            var grid = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid]);
            _session?.ClickContextMenuItem(grid.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.GetZone(zoneNumber, FancyZonesEditorSession.ClassName.GridZone)!.Click(); // horizontal split in the middle of the zone
            _session?.Click(ElementName.Save);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.GridFromJsonElement(grid.Info.ToString());
            var actual = customLayouts.GridFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == grid.Uuid).Info.GetRawText());

            // new column added
            Assert.AreEqual(expected.Columns + 1, actual.Columns);
            Assert.AreEqual(expected.ColumnsPercentage[0], actual.ColumnsPercentage[0] + actual.ColumnsPercentage[1]);
            Assert.AreEqual(expected.ColumnsPercentage[1], actual.ColumnsPercentage[2]);

            // rows are not changed
            Assert.AreEqual(expected.Rows, actual.Rows);
            for (int i = 0; i < expected.Rows; i++)
            {
                Assert.AreEqual(expected.RowsPercentage[i], actual.RowsPercentage[i]);
            }
        }

        [TestMethod]
        public void Grid_SplitZone_Cancel()
        {
            int zoneNumber = 1;
            var grid = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid]);
            _session?.ClickContextMenuItem(grid.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.GetZone(zoneNumber, FancyZonesEditorSession.ClassName.GridZone)!.Click(); // horizontal split in the middle of the zone
            _session?.Click(ElementName.Cancel);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.GridFromJsonElement(grid.Info.ToString());
            var actual = customLayouts.GridFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == grid.Uuid).Info.GetRawText());

            // columns are not changed
            Assert.AreEqual(expected.Columns, actual.Columns);
            for (int i = 0; i < expected.Columns; i++)
            {
                Assert.AreEqual(expected.ColumnsPercentage[i], actual.ColumnsPercentage[i]);
            }

            // rows are not changed
            Assert.AreEqual(expected.Rows, actual.Rows);
            for (int i = 0; i < expected.Rows; i++)
            {
                Assert.AreEqual(expected.RowsPercentage[i], actual.RowsPercentage[i]);
            }
        }

        [TestMethod]
        public void Grid_MergeZones_Save()
        {
            var grid = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid]);
            _session?.ClickContextMenuItem(grid.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.MergeGridZones(1, 2);
            _session?.Click(ElementName.Save);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.GridFromJsonElement(grid.Info.ToString());
            var actual = customLayouts.GridFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == grid.Uuid).Info.GetRawText());

            // columns are not changed
            Assert.AreEqual(expected.Columns, actual.Columns);
            for (int i = 0; i < expected.Columns; i++)
            {
                Assert.AreEqual(expected.ColumnsPercentage[i], actual.ColumnsPercentage[i]);
            }

            // rows are not changed
            Assert.AreEqual(expected.Rows, actual.Rows);
            for (int i = 0; i < expected.Rows; i++)
            {
                Assert.AreEqual(expected.RowsPercentage[i], actual.RowsPercentage[i]);
            }

            // cells are updated to [0,0][1,2]
            Assert.IsTrue(actual.CellChildMap[0].SequenceEqual([0, 0]));
            Assert.IsTrue(actual.CellChildMap[1].SequenceEqual([1, 2]));
        }

        [TestMethod]
        public void Grid_MergeZones_Cancel()
        {
            var grid = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid]);
            _session?.ClickContextMenuItem(grid.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.MergeGridZones(1, 2);
            _session?.Click(ElementName.Cancel);

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.GridFromJsonElement(grid.Info.ToString());
            var actual = customLayouts.GridFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == grid.Uuid).Info.GetRawText());

            // columns are not changed
            Assert.AreEqual(expected.Columns, actual.Columns);
            for (int i = 0; i < expected.Columns; i++)
            {
                Assert.AreEqual(expected.ColumnsPercentage[i], actual.ColumnsPercentage[i]);
            }

            // rows are not changed
            Assert.AreEqual(expected.Rows, actual.Rows);
            for (int i = 0; i < expected.Rows; i++)
            {
                Assert.AreEqual(expected.RowsPercentage[i], actual.RowsPercentage[i]);
            }

            // cells are not changed
            for (int i = 0; i < expected.CellChildMap.Length; i++)
            {
                Assert.IsTrue(actual.CellChildMap[i].SequenceEqual(expected.CellChildMap[i]));
            }
        }

        [TestMethod]
        public void Grid_MoveSplitter_Save()
        {
            var grid = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid] && x.Name == "Grid-9");
            _session?.ClickContextMenuItem(grid.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.MoveSplitter(0, -100);
            _session?.Click(ElementName.Save);
            _session?.Click(ElementName.Save); // single click doesn't work after moving a splitter

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.GridFromJsonElement(grid.Info.ToString());
            var actual = customLayouts.GridFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == grid.Uuid).Info.GetRawText());

            // columns are not changed
            Assert.AreEqual(expected.Columns, actual.Columns);
            for (int i = 0; i < expected.Columns; i++)
            {
                Assert.AreEqual(expected.ColumnsPercentage[i], actual.ColumnsPercentage[i]);
            }

            // rows are changed
            Assert.AreEqual(expected.Rows, actual.Rows);
            Assert.IsTrue(expected.RowsPercentage[0] > actual.RowsPercentage[0]);
            Assert.IsTrue(expected.RowsPercentage[1] < actual.RowsPercentage[1]);
            Assert.AreEqual(expected.RowsPercentage[2], actual.RowsPercentage[2]);

            // cells are not changed
            for (int i = 0; i < expected.CellChildMap.Length; i++)
            {
                Assert.IsTrue(actual.CellChildMap[i].SequenceEqual(expected.CellChildMap[i]));
            }
        }

        [TestMethod]
        public void Grid_MoveSplitter_Cancel()
        {
            var grid = Layouts.CustomLayouts.Find(x => x.Type == Constants.CustomLayoutTypeNames[Constants.CustomLayoutType.Grid] && x.Name == "Grid-9");
            _session?.ClickContextMenuItem(grid.Name, FancyZonesEditorSession.ElementName.EditZones);

            _session?.MoveSplitter(0, -100);
            _session?.Click(ElementName.Cancel);
            _session?.Click(ElementName.Cancel); // single click doesn't work after moving a splitter

            // check the file
            var customLayouts = new CustomLayouts();
            var data = customLayouts.Read(customLayouts.File);
            var expected = customLayouts.GridFromJsonElement(grid.Info.ToString());
            var actual = customLayouts.GridFromJsonElement(data.CustomLayouts.Find(x => x.Uuid == grid.Uuid).Info.GetRawText());

            // columns are not changed
            Assert.AreEqual(expected.Columns, actual.Columns);
            for (int i = 0; i < expected.Columns; i++)
            {
                Assert.AreEqual(expected.ColumnsPercentage[i], actual.ColumnsPercentage[i]);
            }

            // rows are not changed
            Assert.AreEqual(expected.Rows, actual.Rows);
            for (int i = 0; i < expected.Rows; i++)
            {
                Assert.AreEqual(expected.RowsPercentage[i], actual.RowsPercentage[i]);
            }

            // cells are not changed
            for (int i = 0; i < expected.CellChildMap.Length; i++)
            {
                Assert.IsTrue(actual.CellChildMap[i].SequenceEqual(expected.CellChildMap[i]));
            }
        }
    }
}
