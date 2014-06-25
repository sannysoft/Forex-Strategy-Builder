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
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;
using ForexStrategyBuilder.Properties;
using ForexStrategyBuilder.Utils;
using ProgReporter;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Base class containing the data.
    /// </summary>
    public class Data
    {
        private static string[] asStrategyIndicators;

        /// <summary>
        ///     The default constructor.
        /// </summary>
        static Data()
        {
            Icon = Resources.Icon;
            PointChar = '.';
            Dfs = "dd.MM";
            Df = "dd.MM.yy";
            AutoUsePrvBarValue = true;
            FirstBar = 40;
            PeriodColor = new Dictionary<DataPeriod, Color>();
            AdditionalFolder = "Additional" + Path.DirectorySeparatorChar;
            SourceFolder = "Indicators" + Path.DirectorySeparatorChar;
            DefaultStrategyDir = "Strategies" + Path.DirectorySeparatorChar;
            ColorDir = "Colors" + Path.DirectorySeparatorChar;
            LanguageDir = "Languages" + Path.DirectorySeparatorChar;
            SystemDir = "System" + Path.DirectorySeparatorChar;
            LibraryDir = "Libraries" + Path.DirectorySeparatorChar;
            UserFilesDir = "User Files";
            ProgramName = "Forex Strategy Builder";
            IsProgramReleaseCandidate = false;
            IsProgramBeta = false;
            LoadedSavedStrategy = "";
            StrategyName = "New.xml";
            StrategyDir = "Strategies" + Path.DirectorySeparatorChar;
            OfflineDocsDir = "Docs" + Path.DirectorySeparatorChar;
            DefaultOfflineDataDir = "Data" + Path.DirectorySeparatorChar;
            OfflineDataDir = "Data" + Path.DirectorySeparatorChar;
            Debug = false;
            IsData = false;
            IsResult = false;
            IsStrategyChanged = false;
            StackStrategy = new Stack<Strategy>();
            GeneratorHistory = new List<Strategy>();
            ProgStats = new ProgStats {AppLicenseType = LicenseType.Valid};
            ProgStats.AppStart("980834a958e961563091a670660243e7dd17d543");
            IsIntrabarData = false;
            SingleOrder = false;
            WorkingMinutes = 0;
            MaxDD = 100;
            MM = 0;
            AutoSave = false;
            AutoMM = false;

            // Program's Major, Minor, Version and Build numbers must be <= 99.
            ProgramVersion = Application.ProductVersion;
            string[] version = ProgramVersion.Split('.');
            ProgramId = 1000000*int.Parse(version[0]) +
                        10000*int.Parse(version[1]) +
                        100*int.Parse(version[2]) +
                        1*int.Parse(version[3]);
            Strategy.GenerateNew();
        }

        /// <summary>
        ///     Gets the program name.
        /// </summary>
        public static string ProgramName { get; private set; }

        /// <summary>
        ///     Programs icon.
        /// </summary>
        public static Icon Icon { get; private set; }

        /// <summary>
        ///     Gets the program version.
        /// </summary>
        public static string ProgramVersion { get; private set; }

        /// <summary>
        ///     Gets the program Beta state.
        /// </summary>
        public static bool IsProgramBeta { get; private set; }

        /// <summary>
        ///     Gets the program Release Candidate.
        /// </summary>
        public static bool IsProgramReleaseCandidate { get; private set; }

        /// <summary>
        ///     Gets the program Id
        /// </summary>
        public static int ProgramId { get; private set; }

        /// <summary>
        ///     Gets the program directory.
        /// </summary>
        public static string ProgramsDir { get; private set; }

        /// <summary>
        ///     Gets the Users Files directory.
        /// </summary>
        public static string UserFilesDir { get; private set; }

        /// <summary>
        ///     Gets the path to System Dir.
        /// </summary>
        public static string SystemDir { get; private set; }

        /// <summary>
        ///     Gets the path to LanguageDir Dir.
        /// </summary>
        public static string LanguageDir { get; private set; }

        /// <summary>
        ///     Gets the path to Color Scheme Dir.
        /// </summary>
        public static string ColorDir { get; private set; }

        /// <summary>
        ///     Gets or sets the data directory.
        /// </summary>
        public static string OfflineDataDir { get; set; }

        /// <summary>
        ///     Gets the default data directory.
        /// </summary>
        public static string DefaultOfflineDataDir { get; private set; }

        /// <summary>
        ///     Gets or sets the docs directory.
        /// </summary>
        private static string OfflineDocsDir { get; set; }

        /// <summary>
        ///     If set to true - strategy generator will always use 1 order at a time
        /// </summary>
        public static bool SingleOrder { get; set; }

        /// <summary>
        /// Will save strategy automatically in case of autogeneration
        /// </summary>
        public static bool AutoSave { get; set; }

        /// <summary>
        /// Change lots automatically
        /// </summary>
        public static bool AutoMM { get; set; }

        /// <summary>
        ///     Gets the path to Library Dir.
        /// </summary>
        public static string LibraryDir { get; private set; }

        /// <summary>
        ///     Gets the path to Default Strategy Dir.
        /// </summary>
        public static string DefaultStrategyDir { get; private set; }

        /// <summary>
        ///     Gets or sets the path to dir Strategy.
        /// </summary>
        public static string StrategyDir { get; set; }

        /// <summary>
        ///     Gets or sets the strategy name with extension.
        /// </summary>
        public static string StrategyName { get; set; }

        /// <summary>
        /// Number of minutes to generate strategy, 0 - not set (default)
        /// </summary>
        public static int WorkingMinutes { get; set; }

        /// <summary>
        /// % lots by default
        /// </summary>
        public static decimal MM { get; set; }

        /// <summary>
        /// Maximum drawdown
        /// </summary>
        public static int MaxDD { get; set; }

        /// <summary>
        ///     Gets the current strategy full path.
        /// </summary>
        public static string StrategyPath
        {
            get { return Path.Combine(StrategyDir, StrategyName); }
        }

        /// <summary>
        ///     Gets or sets the custom indicators folder
        /// </summary>
        public static string SourceFolder { get; private set; }

        /// <summary>
        ///     Gets or sets the Additional  folder
        /// </summary>
        public static string AdditionalFolder { get; private set; }

        /// <summary>
        ///     Gets or sets the strategy name for Configs.LastStrategy
        /// </summary>
        public static string LoadedSavedStrategy { get; set; }

        /// <summary>
        ///     User Experience Module
        /// </summary>
        public static IProgStats ProgStats { get; set; }

        /// <summary>
        ///     The current strategy.
        /// </summary>
        public static Strategy Strategy { get; set; }

        /// <summary>
        ///     The current instrument.
        /// </summary>
        public static InstrumentProperties InstrProperties { get; set; }

        /// <summary>
        ///     The current strategy undo
        /// </summary>
        public static Stack<Strategy> StackStrategy { get; private set; }

        /// <summary>
        ///     The Generator History
        /// </summary>
        public static List<Strategy> GeneratorHistory { get; private set; }

        /// <summary>
        ///     The Generator History current strategy
        /// </summary>
        public static int GenHistoryIndex { get; set; }

        /// <summary>
        ///     The scanner colors
        /// </summary>
        public static Dictionary<DataPeriod, Color> PeriodColor { get; private set; }

        /// <summary>
        ///     Debug mode
        /// </summary>
        public static bool Debug { get; set; }

        public static string Symbol
        {
            get { return InstrProperties.Symbol; }
        }

        public static DataPeriod Period { get; set; }

        public static string PeriodString
        {
            get { return DataPeriodToString(Period); }
        }

        public static DateTime Update { get; set; }
        public static int Bars { get; set; }

        public static bool IsData { get; set; }
        public static bool IsResult { get; set; }
        public static bool IsStrategyChanged { get; set; }
        public static int FirstBar { get; set; }

        /// <summary>
        ///     Sets or gets value of the AutoUsePrvBarValue
        /// </summary>
        public static bool AutoUsePrvBarValue { get; set; }

        /// <summary>
        ///     Gets the number format.
        /// </summary>
        public static string Ff
        {
            get { return "F" + InstrProperties.Digits; }
        }

        /// <summary>
        ///     Gets the date format.
        /// </summary>
        public static string Df { get; private set; }

        /// <summary>
        ///     Gets the short date format.
        /// </summary>
        public static string Dfs { get; private set; }

        /// <summary>
        ///     Gets the point character
        /// </summary>
        public static char PointChar { get; private set; }

        /// <summary>
        ///     Relative font height
        /// </summary>
        public static float VerticalDlu { get; set; }

        public static double VDpiScale { get; set; }
        public static double HDpiScale { get; set; }

        /// <summary>
        ///     Relative font width
        /// </summary>
        public static float HorizontalDlu { get; set; }

        /// <summary>
        ///     Initial settings.
        /// </summary>
        public static void Start()
        {
            // Sets the date format.
            if (DateTimeFormatInfo.CurrentInfo != null)
                Df = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            if (Df == "dd/MM yyyy") Df = "dd/MM/yyyy"; // Fixes the Uzbek (Latin) issue
            Df = Df.Replace(" ", ""); // Fixes the Slovenian issue
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                char[] acDs = DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray();
                string[] asSs = Df.Split(acDs, 3);
                asSs[0] = asSs[0].Substring(0, 1) + asSs[0].Substring(0, 1);
                asSs[1] = asSs[1].Substring(0, 1) + asSs[1].Substring(0, 1);
                asSs[2] = asSs[2].Substring(0, 1) + asSs[2].Substring(0, 1);
                Df = asSs[0] + acDs[0] + asSs[1] + acDs[0] + asSs[2];

                if (asSs[0].ToUpper() == "YY")
                    Dfs = asSs[1] + acDs[0] + asSs[2];
                else if (asSs[1].ToUpper() == "YY")
                    Dfs = asSs[0] + acDs[0] + asSs[2];
                else
                    Dfs = asSs[0] + acDs[0] + asSs[1];
            }

            var presentationUtils = new PresentationUtils();
            Size dpi = presentationUtils.GetScreenDpi();
            VDpiScale = dpi.Height/96.0;
            HDpiScale = dpi.Width/96.0;

            // Point character
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            PointChar = cultureInfo.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];

            // Set the working directories
            ProgramsDir = Directory.GetCurrentDirectory();
            UserFilesDir = Path.Combine(ProgramsDir, UserFilesDir);

            DefaultOfflineDataDir = Path.Combine(UserFilesDir, OfflineDataDir);
            OfflineDataDir = DefaultOfflineDataDir;
            OfflineDocsDir = Path.Combine(UserFilesDir, OfflineDocsDir);
            StrategyDir = Path.Combine(UserFilesDir, DefaultStrategyDir);
            SourceFolder = Path.Combine(UserFilesDir, SourceFolder);
            SystemDir = Path.Combine(UserFilesDir, SystemDir);
            LibraryDir = Path.Combine(UserFilesDir, LibraryDir);
            LanguageDir = Path.Combine(SystemDir, LanguageDir);
            ColorDir = Path.Combine(SystemDir, ColorDir);

            // Scanner colors
            PeriodColor.Add(DataPeriod.M1, ColorTranslator.FromHtml("#04FF14"));
            PeriodColor.Add(DataPeriod.M5, ColorTranslator.FromHtml("#87E800"));
            PeriodColor.Add(DataPeriod.M15, ColorTranslator.FromHtml("#FFED00"));
            PeriodColor.Add(DataPeriod.M30, ColorTranslator.FromHtml("#E8C400"));
            PeriodColor.Add(DataPeriod.H1, ColorTranslator.FromHtml("#E8AB00"));
            PeriodColor.Add(DataPeriod.H4, ColorTranslator.FromHtml("#FF8B07"));
            PeriodColor.Add(DataPeriod.D1, ColorTranslator.FromHtml("#E84006"));
            PeriodColor.Add(DataPeriod.W1, ColorTranslator.FromHtml("#FF0E1F"));
        }

        /// <summary>
        ///     Sets the indicator names
        /// </summary>
        public static void SetStrategyIndicators()
        {
            asStrategyIndicators = new string[Strategy.Slots];
            for (int i = 0; i < Strategy.Slots; i++)
                asStrategyIndicators[i] = Strategy.Slot[i].IndicatorName;
        }

        /// <summary>
        ///     It tells if the strategy description is relevant.
        /// </summary>
        public static bool IsStrDescriptionRelevant()
        {
            bool isStrategyIndicatorsChanged = Strategy.Slots != asStrategyIndicators.Length;

            if (isStrategyIndicatorsChanged == false)
            {
                for (int i = 0; i < Strategy.Slots; i++)
                    if (asStrategyIndicators[i] != Strategy.Slot[i].IndicatorName)
                        isStrategyIndicatorsChanged = true;
            }

            return !isStrategyIndicatorsChanged;
        }

        /// <summary>
        ///     Converts a data period from DataPeriods type to string.
        /// </summary>
        public static string DataPeriodToString(DataPeriod dataPeriod)
        {
            switch (dataPeriod)
            {
                case DataPeriod.M1:
                    return "1 " + Language.T("Minute");
                case DataPeriod.M5:
                    return "5 " + Language.T("Minutes");
                case DataPeriod.M15:
                    return "15 " + Language.T("Minutes");
                case DataPeriod.M30:
                    return "30 " + Language.T("Minutes");
                case DataPeriod.H1:
                    return "1 " + Language.T("Hour");
                case DataPeriod.H4:
                    return "4 " + Language.T("Hours");
                case DataPeriod.D1:
                    return "1 " + Language.T("Day");
                case DataPeriod.W1:
                    return "1 " + Language.T("Week");
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        ///     Paints a rectangle with gradient
        /// </summary>
        public static void GradientPaint(Graphics g, RectangleF rect, Color color, int depth)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (depth > 0 && Configs.GradientView)
            {
                Color color1 = GetGradientColor(color, +depth);
                Color color2 = GetGradientColor(color, -depth);
                var rect1 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
                var linearGradientBrush = new LinearGradientBrush(rect1, color1, color2, 90);
                g.FillRectangle(linearGradientBrush, rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), rect);
            }
        }

        /// <summary>
        ///     Color change
        /// </summary>
        public static Color GetGradientColor(Color baseColor, int depth)
        {
            if (!Configs.GradientView)
                return baseColor;

            int r = Math.Max(Math.Min(baseColor.R + depth, 255), 0);
            int g = Math.Max(Math.Min(baseColor.G + depth, 255), 0);
            int b = Math.Max(Math.Min(baseColor.B + depth, 255), 0);

            return Color.FromArgb(r, g, b);
        }

        public static void DrawCheckerBoard(Graphics g, Color color, Rectangle rectangle)
        {
            int x1 = rectangle.X;
            int x2 = rectangle.X + rectangle.Width;
            int y1 = rectangle.Y;
            int y2 = rectangle.Y + rectangle.Height;

            using (var penTick = new Pen(color) { DashPattern = new float[] { 1, 1 } })
            {
                for (int y = y1; y < y2; y += 2)
                    g.DrawLine(penTick, x1, y, x2, y);
                for (int y = y1 + 1; y < y2; y += 2)
                    g.DrawLine(penTick, x1 + 1, y, x2, y);
            }
        }

        #region Market data arrays

        public static DateTime[] Time { get; set; }
        public static double[] Open { get; set; }
        public static double[] High { get; set; }
        public static double[] Low { get; set; }
        public static double[] Close { get; set; }
        public static int[] Volume { get; set; }

        #endregion

        #region Intrabar Scanner

        public static Bar[][] IntraBarData { get; set; }
        public static int[] IntraBarBars { get; set; }
        public static bool IsIntrabarData { get; set; }
        public static int[] IntraBars { get; set; }

        /// <summary>
        ///     Number of intrabar periods that have been loaded
        /// </summary>
        public static int LoadedIntraBarPeriods { get; set; }

        public static DataPeriod[] IntraBarsPeriods { get; set; }

        // Tick data
        //static Dictionary<DateTime, double[]> tickData;
        //public static Dictionary<DateTime, double[]> TickData { get { return tickData; } set { tickData = value; } }
        public static double[][] TickData { get; set; }
        public static bool IsTickData { get; set; }
        public static long Ticks { get; set; }


        /// <summary>
        ///     Calculates the Modelling Quality
        /// </summary>
        public static double ModellingQuality
        {
            get
            {
                if (!Backtester.IsScanPerformed)
                    return 0;

                int startGen = 0;

                for (int i = 0; i < Bars; i++)
                    if (IntraBarsPeriods[i] < Period)
                    {
                        startGen = i;
                        break;
                    }

                int startGenM1 = Bars - 1;

                for (int i = 0; i < Bars; i++)
                    if (IntraBarsPeriods[i] == DataPeriod.M1)
                    {
                        startGenM1 = i;
                        break;
                    }

                double modellingQuality = (0.25*(startGen - FirstBar) + 0.5*(startGenM1 - startGen) +
                                           0.9*(Bars - startGenM1))/(Bars - FirstBar)*100;

                return modellingQuality;
            }
        }

        #endregion

        #region Market statistics

        // Statistical information for the instrument data
        public static double MinPrice { get; set; }
        public static double MaxPrice { get; set; }
        public static int DaysOff { get; set; }
        public static int AverageGap { private get; set; }
        public static int MaxGap { private get; set; }
        public static int AverageHighLow { private get; set; }
        public static int MaxHighLow { private get; set; }
        public static int AverageCloseOpen { private get; set; }
        public static int MaxCloseOpen { private get; set; }
        public static bool DataCut { get; set; }

        /// <summary>
        ///     Gets the market stats parameters
        /// </summary>
        public static string[] MarketStatsParam { get; private set; }

        /// <summary>
        ///     Gets the market stats values
        /// </summary>
        public static string[] MarketStatsValue { get; private set; }

        /// <summary>
        ///     Gets the market stats flags
        /// </summary>
        public static bool[] MarketStatsFlag { get; private set; }

        /// <summary>
        ///     Initializes the stats names
        /// </summary>
        public static void InitMarketStatistic()
        {
            MarketStatsParam = new[]
                {
                    Language.T("Symbol"),
                    Language.T("Period"),
                    Language.T("Number of bars"),
                    Language.T("Date of updating"),
                    Language.T("Time of updating"),
                    Language.T("Date of beginning"),
                    Language.T("Time of beginning"),
                    Language.T("Minimum price"),
                    Language.T("Maximum price"),
                    Language.T("Average Gap"),
                    Language.T("Maximum Gap"),
                    Language.T("Average High-Low"),
                    Language.T("Maximum High-Low"),
                    Language.T("Average Close-Open"),
                    Language.T("Maximum Close-Open"),
                    Language.T("Maximum days off"),
                    Language.T("Maximum data bars"),
                    Language.T("No data older than"),
                    Language.T("No data newer than"),
                    Language.T("Fill In Data Gaps"),
                    Language.T("Cut Off Bad Data")
                };

            MarketStatsValue = new string[21];
            MarketStatsFlag = new bool[21];
        }

        /// <summary>
        ///     Generate the Market Statistics
        /// </summary>
        public static void GenerateMarketStats()
        {
            MarketStatsValue[0] = Symbol;
            MarketStatsValue[1] = DataPeriodToString(Period);
            MarketStatsValue[2] = Bars.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[3] = Update.ToString(Df);
            MarketStatsValue[4] = Update.ToString("HH:mm");
            MarketStatsValue[5] = Time[0].ToString(Df);
            MarketStatsValue[6] = Time[0].ToString("HH:mm");
            MarketStatsValue[7] = MinPrice.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[8] = MaxPrice.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[9] = AverageGap + " " + Language.T("points");
            MarketStatsValue[10] = MaxGap + " " + Language.T("points");
            MarketStatsValue[11] = AverageHighLow + " " + Language.T("points");
            MarketStatsValue[12] = MaxHighLow + " " + Language.T("points");
            MarketStatsValue[13] = AverageCloseOpen + " " + Language.T("points");
            MarketStatsValue[14] = MaxCloseOpen + " " + Language.T("points");
            MarketStatsValue[15] = DaysOff.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[16] = Configs.MaxBars.ToString(CultureInfo.InvariantCulture);
            MarketStatsValue[17] = Configs.UseStartTime
                                       ? Configs.DataStartTime.ToShortDateString()
                                       : Language.T("No limits");
            MarketStatsValue[18] = Configs.UseEndTime
                                       ? Configs.DataEndTime.ToShortDateString()
                                       : Language.T("No limits");
            MarketStatsValue[19] = Configs.FillInDataGaps ? Language.T("Accomplished") : Language.T("Switched off");
            MarketStatsValue[20] = Configs.CutBadData ? Language.T("Accomplished") : Language.T("Switched off");
        }

        #endregion

        public static bool AutostartGenerator { get; set; }

        public static IDataSet DataSet { get; set; }
    }
}