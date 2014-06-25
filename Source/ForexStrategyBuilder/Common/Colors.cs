//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Colors
    /// </summary>
    public static class LayoutColors
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        static LayoutColors()
        {
            SetColorsDefault();
        }

        public static string[] ColorSchemeList { get; private set; }
        private static Dictionary<string, string> DictColorFiles { get; set; }
        private static XmlDocument XMLColors { get; set; }

        /// <summary>
        ///     Gradient color depth of the caption bars
        /// </summary>
        public static int DepthCaption { get; private set; }

        /// <summary>
        ///     Gradient color depth of the controls
        /// </summary>
        public static int DepthControl { get; private set; }

        // Workspace
        public static Color ColorWarningRowBack { get; private set; }
        public static Color ColorWarningRowText { get; private set; }
        public static Color ColorSelectedRowBack { get; private set; }
        public static Color ColorSelectedRowText { get; private set; }
        public static Color ColorFormBack { get; private set; }
        public static Color ColorControlBack { get; private set; }
        public static Color ColorControlText { get; private set; }
        public static Color ColorCaptionBack { get; private set; }
        public static Color ColorCaptionText { get; private set; }
        public static Color ColorEvenRowBack { get; private set; }

        public static Color ColorOddRowBack
        {
            get { return ColorControlBack; }
        }

        public static Color ColorJournalLines { get; private set; }
        public static Color ColorSignalRed { get; private set; }

        // Slots
        public static Color ColorSlotCaptionBackAveraging { get; private set; }
        public static Color ColorSlotCaptionBackOpen { get; private set; }
        public static Color ColorSlotCaptionBackOpenFilter { get; private set; }
        public static Color ColorSlotCaptionBackClose { get; private set; }
        public static Color ColorSlotCaptionBackCloseFilter { get; private set; }
        public static Color ColorSlotCaptionText { get; private set; }
        public static Color ColorSlotBackground { get; private set; }
        public static Color ColorSlotIndicatorText { get; private set; }
        public static Color ColorSlotLogicText { get; private set; }
        public static Color ColorSlotParamText { get; private set; }
        public static Color ColorSlotValueText { get; private set; }
        public static Color ColorSlotDash { get; private set; }

        // Charts
        public static Color ColorChartBack { get; private set; }
        public static Color ColorChartFore { get; private set; }
        public static Color ColorChartBalanceLine { get; private set; }
        public static Color ColorChartEquityLine { get; private set; }
        public static Color ColorChartGrid { get; private set; }
        public static Color ColorChartCross { get; private set; }
        public static Color ColorLabelBack { get; private set; }
        public static Color ColorLabelText { get; private set; }
        public static Color ColorTradeLong { get; private set; }
        public static Color ColorTradeShort { get; private set; }
        public static Color ColorTradeClose { get; private set; }
        public static Color ColorVolume { get; private set; }
        public static Color ColorBarWhite { get; private set; }
        public static Color ColorBarBlack { get; private set; }
        public static Color ColorBarBorder { get; private set; }

        // Comparator
        public static Color ComparatorChartBalanceLine { get; private set; }
        public static Color ComparatorChartOptimisticLine { get; private set; }
        public static Color ComparatorChartPessimisticLine { get; private set; }
        public static Color ComparatorChartShortestLine { get; private set; }
        public static Color ComparatorChartNearestLine { get; private set; }
        public static Color ComparatorChartRandomLine { get; private set; }
        public static Color ComparatorChartRandomArea { get; private set; }
        public static Color ComparatorChartRandomBands { get; private set; }

        /// <summary>
        ///     Sets the default color scheme
        /// </summary>
        private static void SetColorsDefault()
        {
            DepthCaption = 25;
            DepthControl = 10;

            // Workspace
            ColorFormBack = Color.FromArgb(153, 204, 204);
            ColorControlBack = Color.FromArgb(245, 255, 255);
            ColorControlText = Color.FromArgb(0, 50, 50);
            ColorCaptionBack = Color.FromArgb(102, 153, 204);
            ColorCaptionText = Color.FromArgb(255, 255, 255);
            ColorEvenRowBack = Color.FromArgb(255, 255, 255);
            ColorWarningRowBack = Color.FromArgb(255, 230, 230);
            ColorWarningRowText = Color.FromArgb(0, 50, 50);
            ColorSelectedRowBack = Color.FromArgb(215, 215, 255);
            ColorSelectedRowText = Color.FromArgb(0, 0, 0);
            ColorJournalLines = Color.FromArgb(90, 90, 120);
            ColorSignalRed = Color.FromArgb(255, 0, 0);

            // Slots
            ColorSlotCaptionBackAveraging = Color.FromArgb(150, 100, 100);
            ColorSlotCaptionBackOpen = Color.FromArgb(102, 153, 51);
            ColorSlotCaptionBackOpenFilter = Color.FromArgb(102, 153, 153);
            ColorSlotCaptionBackClose = Color.FromArgb(204, 102, 51);
            ColorSlotCaptionBackCloseFilter = Color.FromArgb(210, 140, 140);
            ColorSlotCaptionText = Color.FromArgb(255, 255, 255);
            ColorSlotBackground = Color.FromArgb(245, 255, 255);
            ColorSlotIndicatorText = Color.FromArgb(102, 153, 204);
            ColorSlotLogicText = Color.FromArgb(0, 51, 51);
            ColorSlotParamText = Color.FromArgb(51, 153, 153);
            ColorSlotValueText = Color.FromArgb(51, 153, 153);
            ColorSlotDash = Color.FromArgb(204, 204, 153);

            // Chart
            ColorChartBack = Color.FromArgb(245, 255, 255);
            ColorChartFore = Color.FromArgb(0, 50, 50);
            ColorBarWhite = Color.FromArgb(225, 225, 225);
            ColorBarBlack = Color.FromArgb(30, 30, 30);
            ColorBarBorder = Color.FromArgb(0, 0, 0);
            ColorTradeLong = Color.FromArgb(30, 160, 30);
            ColorTradeShort = Color.FromArgb(225, 30, 30);
            ColorTradeClose = Color.FromArgb(225, 130, 30);
            ColorVolume = Color.FromArgb(150, 0, 210);
            ColorLabelBack = Color.FromArgb(255, 255, 255);
            ColorLabelText = Color.FromArgb(0, 50, 50);
            ColorChartGrid = Color.FromArgb(204, 204, 204);
            ColorChartCross = Color.FromArgb(153, 163, 204);
            ColorChartBalanceLine = Color.FromArgb(102, 102, 153);
            ColorChartEquityLine = Color.FromArgb(225, 130, 30);

            // Comparator
            ComparatorChartBalanceLine = Color.FromArgb(102, 102, 153);
            ComparatorChartOptimisticLine = Color.FromArgb(255, 153, 153);
            ComparatorChartPessimisticLine = Color.FromArgb(0, 0, 0);
            ComparatorChartShortestLine = Color.FromArgb(102, 102, 0);
            ComparatorChartNearestLine = Color.FromArgb(153, 102, 153);
            ComparatorChartRandomLine = Color.FromArgb(51, 102, 0);
            ComparatorChartRandomArea = Color.FromArgb(153, 255, 153);
            ComparatorChartRandomBands = Color.FromArgb(204, 204, 153);
        }

        /// <summary>
        ///     Loads the color scheme from a file
        /// </summary>
        public static void LoadColorScheme(string sColorScheme)
        {
            try
            {
                XMLColors = new XmlDocument();
                XMLColors.Load(sColorScheme);

                // Workspace
                ColorFormBack = ParseColor("FormBack");
                ColorControlBack = ParseColor("ControlBack");
                ColorControlText = ParseColor("ControlText");
                ColorCaptionBack = ParseColor("CaptionBack");
                ColorCaptionText = ParseColor("CaptionText");
                ColorEvenRowBack = ParseColor("EvenRowBack");
                ColorWarningRowBack = ParseColor("WarningRowBack");
                ColorWarningRowText = ParseColor("WarningRowText");
                ColorSelectedRowBack = ParseColor("SelectedRowBack");
                ColorSelectedRowText = ParseColor("SelectedRowText");
                ColorJournalLines = ParseColor("JournalLines");
                ColorSignalRed = ParseColor("SignalRed");

                // Strategy Slots
                ColorSlotCaptionBackAveraging = ParseColor("SlotCaptionBackProperties");
                ColorSlotCaptionBackOpen = ParseColor("SlotCaptionBackOpen");
                ColorSlotCaptionBackOpenFilter = ParseColor("SlotCaptionBackOpenFilter");
                ColorSlotCaptionBackClose = ParseColor("SlotCaptionBackClose");
                ColorSlotCaptionBackCloseFilter = ParseColor("SlotCaptionBackCloseFilter");
                ColorSlotCaptionText = ParseColor("SlotCaptionText");
                ColorSlotBackground = ParseColor("SlotBack");
                ColorSlotIndicatorText = ParseColor("SlotIndicatorText");
                ColorSlotLogicText = ParseColor("SlotLogicText");
                ColorSlotParamText = ParseColor("SlotParamText");
                ColorSlotValueText = ParseColor("SlotValueText");
                ColorSlotDash = ParseColor("SlotDash");

                // Chart
                ColorChartBack = ParseColor("ChartBack");
                ColorChartFore = ParseColor("ChartFore");
                ColorBarWhite = ParseColor("BarWhite");
                ColorBarBlack = ParseColor("BarBlack");
                ColorBarBorder = ParseColor("BarBorder");
                ColorTradeLong = ParseColor("TradeLong");
                ColorTradeShort = ParseColor("TradeShort");
                ColorTradeClose = ParseColor("TradeClose");
                ColorVolume = ParseColor("Volume");
                ColorChartGrid = ParseColor("ChartGrid");
                ColorChartCross = ParseColor("ChartCross");
                ColorLabelBack = ParseColor("LabelBack");
                ColorLabelText = ParseColor("LabelText");
                ColorChartBalanceLine = ParseColor("ChartBalanceLine");
                ColorChartEquityLine = ParseColor("ChartEquityLine");

                // Comparator
                ComparatorChartBalanceLine = ParseColor("ComparatorChartBalanceLine");
                ComparatorChartOptimisticLine = ParseColor("ComparatorChartOptimisticLine");
                ComparatorChartPessimisticLine = ParseColor("ComparatorChartPessimisticLine");
                ComparatorChartShortestLine = ParseColor("ComparatorChartShortestLine");
                ComparatorChartNearestLine = ParseColor("ComparatorChartNearestLine");
                ComparatorChartRandomLine = ParseColor("ComparatorChartRandomLine");
                ComparatorChartRandomArea = ParseColor("ComparatorChartRandomArea");
                ComparatorChartRandomBands = ParseColor("ComparatorChartRandomBands");
            }
            catch (Exception exc)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(exc.Message, Language.T("Load a color scheme..."));
            }
        }

        /// <summary>
        ///     Parses the color from the given xml node
        /// </summary>
        private static Color ParseColor(string node)
        {
            XmlNodeList nodeList = XMLColors.SelectNodes("color/" + node);
            if (nodeList != null)
            {
                XmlNode item = nodeList.Item(0);
                if (item != null)
                {
                    XmlAttributeCollection attributes = item.Attributes;
                    if (attributes != null)
                    {
                        int r = int.Parse(attributes.Item(0).InnerText);
                        int g = int.Parse(attributes.Item(1).InnerText);
                        int b = int.Parse(attributes.Item(2).InnerText);

                        return Color.FromArgb(r, g, b);
                    }
                }
            }
            return Color.Red;
        }

        /// <summary>
        ///     Initializes the color scheme.
        /// </summary>
        public static void InitColorSchemes()
        {
            DictColorFiles = new Dictionary<string, string>();
            string sColorDirectory = Data.ColorDir;

            if (Directory.Exists(sColorDirectory) && Directory.GetFiles(sColorDirectory).Length > 0)
            {
                string[] colorFiles = Directory.GetFiles(sColorDirectory);

                foreach (string file in colorFiles)
                {
                    if (!file.EndsWith(".xml", true, null)) continue;
                    try
                    {
                        string colorScheme = Path.GetFileNameWithoutExtension(file);
                        if (colorScheme != null)
                        {
                            DictColorFiles.Add(colorScheme, file);

                            if (colorScheme == Configs.ColorScheme)
                            {
                                LoadColorScheme(file);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Color Scheme", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

            ColorSchemeList = new string[DictColorFiles.Count];
            DictColorFiles.Keys.CopyTo(ColorSchemeList, 0);
            Array.Sort(ColorSchemeList);
        }
    }
}