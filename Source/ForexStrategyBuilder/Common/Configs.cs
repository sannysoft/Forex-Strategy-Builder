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
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Win32;

namespace ForexStrategyBuilder
{
    public static class Configs
    {
        static XmlDocument xmlConfig;
        static bool isConfigLoaded;
        static bool isResetActivated;
        private const string DateStringFormat = "yyyy-MM-dd HH:mm:ss";

        private const int MinBarsDefault = 300;
        private const int MaxBarsLimitDefault = 50000;
        private const int MaxIntraBarsDefault = 50000;
        private const int MaxEntryFiltersDefault = 4;
        private const int MaxExitFiltersDefault = 2;
        private const int SigmaModeMainChartDefault = 1;
        private const int SigmaModeSeparatedChartDefault = 5;

        private const string LanguageDefault = "English";
        private const bool ShowStartingTipDefault = true;
        private const int CurrentTipNumberDefault = -1;
        private const bool IsGradientViewDefault = true;
        private const string DataDirectoryDefault = "";
        private const string ColorSchemeDefault = "Light Blue";
        private const bool IsRememberLastStrDefault = true;
        private const string LastStrategyDefault = "";
        private const bool IsCheckForUpdatesDefault = true;
        private const bool IsCheckForNewBetaDefault = false;
        private const bool IsCheckDataDefault = true;
        private const bool IsFillDataGapsDefault = false;
        private const bool IsCutBadDataDefault = false;
        private const bool IsCutSatSunDataDefault = false;
        private const bool IsLoadCustIndDefault = true;
        private const bool IsShowCustIndDefault = false;
        private const int MaxBarsDefault = 20000;
        private static readonly DateTime DataStartTimeDefault = new DateTime(1990, 1, 1, 0, 0, 0);
        private static readonly DateTime DataEndTimeDefault = new DateTime(2020, 1, 1, 0, 0, 0);
        private const bool IsUseEndTimeDefault = false;
        private const bool IsUseStartTimeDefault = false;
        private const bool IsAccountInMoneyDefault = true;
        private const string AccountCurrencyDefault = "USD";
        private const int InitialAccountDefault = 10000;
        private const int LeverageDefault = 100;
        private const bool IsShowJournalDefault = true;
        private const bool IsJournalByBarsDefault = false;
        private const bool IsJournalShowTransfersDefault = false;
        private const bool IsAutoscanDefault = false;
        private const bool IsTradeUntilMarginCallDefault = true;
        private const bool IsAdditionalStatisticsDefault = false;
        private const bool IsUseLogicalGroupsDefault = false;
        private const bool IsPlaySoundsDefault = true;
        private const string CriteriaSettingsDefault = "";
        private const string GeneratorOptionsDefault = "";
        private const string OptimizerOptionsDefault = "";
        private const string ColumnSeparatorDefault = ",";
        private const string DecimalSeparatorDefault = ".";
        private const bool UseTickDataDefault = false;
        private const string JforexDataPathDefault = "";
        private const string JForexImportDestFolderDefault = "";
        private const int MarketClosingHourDefault = 21;
        private const int MarketOpeningHourDefault = 21;
        private const string Metatrader4DataPathDefault = "";
        private const string ImportStartingDateDefault = "";
        private const string ImportEndingDateDefault = "";
        private const string MT4ImportDestFolderDefault = "";
        private const int MinBarsInBarFileDefault = 3000;
        private const string OandaImportDestFolderDefault = "";
        private const string BannedIndicatorsDefault = "";
        private const bool ShowPriceChartOnAccountChartDefault = false;
        private const bool AnalyzerHideFSBDefault = true;
        private const bool SendUsageStatsDefault = true;
        private const int MainScreenWidthDefault = 790;
        private const int MainScreenHeightDefault = 590;
        private const bool ShowStatusBarDefault = true;
        private const bool StrategyDirWatchDefault = false;
        private const string TrueFxImportDestFolderDefault = "";

        // Indicator Chart
        private const int IndicatorChartZoomDefault = 8;
        private const bool IsIndicatorChartInfoPanelDefault = true;
        private const bool IsIndicatorChartDynamicInfoDefault = true;
        private const bool IsIndicatorChartGridDefault = true;
        private const bool IsIndicatorChartCrossDefault = false;
        private const bool IsIndicatorChartVolumeDefault = false;
        private const bool IsIndicatorChartLotsDefault = true;
        private const bool IsIndicatorChartEntryExitPointsDefault = true;
        private const bool IsIndicatorChartCorrectedPositionPriceDefault = false;
        private const bool IsIndicatorChartBalanceEquityChartDefault = false;
        private const bool IsIndicatorChartFloatingPLChartDefault = false;
        private const bool IsIndicatorChartIndicatorsDefault = true;
        private const bool IsIndicatorChartAmbiguousMarkDefault = true;
        private const bool IsIndicatorChartTrueChartsDefault = false;
        private const bool IsIndicatorChartProtectionsDefault = false;

        // Balance Chart
        private const int BalanceChartZoomDefault = 8;
        private const bool IsBalanceChartInfoPanelDefault = true;
        private const bool IsBalanceChartDynamicInfoDefault = true;
        private const bool IsBalanceChartGridDefault = true;
        private const bool IsBalanceChartCrossDefault = false;
        private const bool IsBalanceChartVolumeDefault = false;
        private const bool IsBalanceChartLotsDefault = true;
        private const bool IsBalanceChartEntryExitPointsDefault = true;
        private const bool IsBalanceChartCorrectedPositionPriceDefault = true;
        private const bool IsBalanceChartBalanceEquityChartDefault = true;
        private const bool IsBalanceChartFloatingPLChartDefault = true;
        private const bool IsBalanceChartIndicatorsDefault = false;
        private const bool IsBalanceChartAmbiguousMarkDefault = true;
        private const bool IsBalanceChartTrueChartsDefault = false;
        private const bool IsBalanceChartProtectionsDefault = false;

        public static string PathToConfigFile { get; private set; }

        // ------------------------------------------------------------
        static int minBars = MinBarsDefault;
        /// <summary>
        /// Minimum data bars
        /// </summary>
        public static int MinBars
        {
            get { return minBars; }
            private set
            {
                minBars = value;
                SetNode("config/MIN_BARS", value);
            }
        }
        // ------------------------------------------------------------
        static int maxBarsLimit = MaxBarsLimitDefault;
        /// <summary>
        /// Maximum data bars
        /// </summary>
        public static int MaxBarsLimit
        {
            get { return maxBarsLimit; }
            private set
            {
                maxBarsLimit = value;
                SetNode("config/MAX_BARS", value);
            }
        }
        // ------------------------------------------------------------
        static int maxIntraBars = MaxIntraBarsDefault;
        /// <summary>
        /// Maximum data intra bars
        /// </summary>
        public static int MaxIntraBars
        {
            get { return maxIntraBars; }
            private set
            {
                maxIntraBars = value;
                SetNode("config/MAX_INTRA_BARS", value);
            }
        }

        static int maxEntryFilters = MaxEntryFiltersDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int MaxEntryFilters
        {
            get { return maxEntryFilters; }
            set
            {
                maxEntryFilters = value;
                SetNode("config/MAX_ENTRY_FILTERS", value);
            }
        }

        static int maxExitFilters = MaxExitFiltersDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int MaxExitFilters
        {
            get { return maxExitFilters; }
            set
            {
                maxExitFilters = value;
                SetNode("config/MAX_EXIT_FILTERS", value);
            }
        }

        static int sigmaModeMainChart = SigmaModeMainChartDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int SigmaModeMainChart
        {
            get { return sigmaModeMainChart; }
            private set
            {
                sigmaModeMainChart = value;
                SetNode("config/SIGMA_MODE_MAIN_CHART", value);
            }
        }

        static int sigmaModeSeparatedChart = SigmaModeSeparatedChartDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int SigmaModeSeparatedChart
        {
            get { return sigmaModeSeparatedChart; }
            private set
            {
                sigmaModeSeparatedChart = value;
                SetNode("config/SIGMA_MODE_SEPARATED_CHART", value);
            }
        }

        // -------------------------------------------------------------
        static bool isInstalled = true;
        private static bool IsInstalled
        {
            set
            {
                isInstalled = value;
                SetNode("config/installed", value);
            }
        }
        // ------------------------------------------------------------
        // Program's Language
        static string language = LanguageDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string Language
        {
            get { return language; }
            set
            {
                language = value;
                SetNode("config/language", value);
            }
        }
        // -------------------------------------------------------------
        // Show starting Tips
        static bool isShowStartingTip = ShowStartingTipDefault;
        /// <summary>
        /// Whether to show starting tips
        /// </summary>
        public static bool ShowStartingTip
        {
            get { return isShowStartingTip; }
            set
            {
                isShowStartingTip = value;
                SetNode("config/showStartingTip", value);
            }
        }
        // -------------------------------------------------------------
        // Current tip number
        static int currentTipNumber = CurrentTipNumberDefault;
        /// <summary>
        /// Gets or sets the current starting tip number
        /// </summary>
        public static int CurrentTipNumber
        {
            get { return currentTipNumber; }
            set
            {
                currentTipNumber = value;
                SetNode("config/currentTipNumber", value);
            }
        }
        // -------------------------------------------------------------
        // Show Gradient View
        static bool isGradientView = IsGradientViewDefault;
        /// <summary>
        /// Whether to show Gradient View
        /// </summary>
        public static bool GradientView
        {
            get { return isGradientView; }
            set
            {
                isGradientView = value;
                SetNode("config/gradientView", value);
            }
        }
        // ------------------------------------------------------------
        // Data directory
        static string dataDirectory = DataDirectoryDefault;
        /// <summary>
        /// Data Directory
        /// </summary>
        public static string DataDirectory
        {
            set
            {
                dataDirectory = value;
                SetNode("config/dataDirectory", value);
            }
        }
        // ------------------------------------------------------------
        // ColorScheme
        static string colorScheme = ColorSchemeDefault;
        /// <summary>
        /// ColorScheme
        /// </summary>
        public static string ColorScheme
        {
            get { return colorScheme; }
            set
            {
                colorScheme = value;
                SetNode("config/colorScheme", value);
            }
        }

        // ------------------------------------------------------------
        // Remember the Last Strategy
        static bool isRememberLastStr = IsRememberLastStrDefault;
        /// <summary>
        /// Remember the Last Strategy
        /// </summary>
        public static bool RememberLastStr
        {
            get { return isRememberLastStr; }
            set
            {
                isRememberLastStr = value;
                SetNode("config/rememberLastStrategy", value);
            }
        }
        // ------------------------------------------------------------
        // Last Strategy
        static string lastStrategy = LastStrategyDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string LastStrategy
        {
            get { return lastStrategy; }
            set
            {
                lastStrategy = value;
                SetNode("config/lastStrategy", value);
            }
        }
        // ------------------------------------------------------------
        // Check for new versions
        static bool isCheckForUpdates = IsCheckForUpdatesDefault;
        /// <summary>
        /// Check for new versions at startup.
        /// </summary>
        public static bool CheckForUpdates
        {
            get { return isCheckForUpdates; }
            set
            {
                isCheckForUpdates = value;
                SetNode("config/checkForUpdates", value);
            }
        }
        // ------------------------------------------------------------
        // Check for new beta
        static bool isCheckForNewBeta = IsCheckForNewBetaDefault;
        /// <summary>
        /// Check for new new beta at startup.
        /// </summary>
        public static bool CheckForNewBeta
        {
            get { return isCheckForNewBeta; }
            set
            {
                isCheckForNewBeta = value;
                SetNode("config/checkForNewBeta", value);
            }
        }
        // ------------------------------------------------------------
        // Check the Data
        static bool isCheckData = IsCheckDataDefault;
        /// <summary>
        /// Whether to Check the Data
        /// </summary>
        public static bool CheckData
        {
            get { return isCheckData; }
            set
            {
                isCheckData = value;
                SetNode("config/checkData", value);
            }
        }
        // ------------------------------------------------------------
        // Fill In Data Gaps
        static bool isFillDataGaps = IsFillDataGapsDefault;
        /// <summary>
        /// Whether to fill the data gaps in
        /// </summary>
        public static bool FillInDataGaps
        {
            get { return isFillDataGaps; }
            set
            {
                isFillDataGaps = value;
                SetNode("config/fillDataGaps", value);
            }
        }
        // ------------------------------------------------------------
        // Fill In Data Gaps
        static bool isCutBadData = IsCutBadDataDefault;
        /// <summary>
        /// Whether to cut off bed data
        /// </summary>
        public static bool CutBadData
        {
            get { return isCutBadData; }
            set
            {
                isCutBadData = value;
                SetNode("config/cutBadData", value);
            }
        }
        // -------------------------------------------------------------
        // Cuts Saturday and Sunday data
        static bool isCutSatSunData = IsCutSatSunDataDefault;
        /// <summary>
        /// Whether to cut off Sat and Sun data
        /// </summary>
        public static bool CutSatSunData
        {
            get { return isCutSatSunData; }
            set
            {
                isCutSatSunData = value;
                SetNode("config/cutSatSunData", value);
            }
        }
        // -------------------------------------------------------------
        // Whether to load custom indicators
        static bool isLoadCustomIndicators = IsLoadCustIndDefault;
        /// <summary>
        /// Whether to load custom indicators at startup.
        /// </summary>
        public static bool LoadCustomIndicators
        {
            get { return isLoadCustomIndicators; }
            set
            {
                isLoadCustomIndicators = value;
                SetNode("config/loadCustomIndicators", value);
            }
        }
        // -------------------------------------------------------------
        // Whether to Show custom indicators
        static bool isShowCustomIndicators = IsShowCustIndDefault;
        /// <summary>
        /// Whether to Show custom indicators at startup.
        /// </summary>
        public static bool ShowCustomIndicators
        {
            get { return isShowCustomIndicators; }
            set
            {
                isShowCustomIndicators = value;
                SetNode("config/showCustomIndicators", value);
            }
        }
        // -------------------------------------------------------------
        // Maximum data bars
        static int maxBars = MaxBarsDefault;
        /// <summary>
        /// Maximum data bars
        /// </summary>
        public static int MaxBars
        {
            get { return maxBars; }
            set
            {
                maxBars = value;
                SetNode("config/dataMaxBars", value);
            }
        }

        static DateTime dataStartTime = DataStartTimeDefault;
        /// <summary>
        /// Start time of market data.
        /// </summary>
        public static DateTime DataStartTime
        {
            get { return dataStartTime; }
            set
            {
                dataStartTime = value;
                SetNode("config/dataStartTime", dataStartTime.ToString(DateStringFormat));
            }
        }

        static DateTime dataEndTime = DataEndTimeDefault;
        /// <summary>
        /// End time of market data.
        /// </summary>
        public static DateTime DataEndTime
        {
            get { return dataEndTime; }
            set
            {
                dataEndTime = value;
                SetNode("config/dataEndTime", dataEndTime.ToString(DateStringFormat));
            }
        }


        static bool isUseStartTime = IsUseStartTimeDefault;
        /// <summary>
        /// Use start time
        /// </summary>
        public static bool UseStartTime
        {
            get { return isUseStartTime; }
            set
            {
                isUseStartTime = value;
                SetNode("config/dataUseStartTime", value);
            }
        }

        static bool isUseEndTime = IsUseEndTimeDefault;
        /// <summary>
        /// Use ending time
        /// </summary>
        public static bool UseEndTime
        {
            get { return isUseEndTime; }
            set
            {
                isUseEndTime = value;
                SetNode("config/dataUseEndTime", value);
            }
        }

        static bool isAccountInMoney = IsAccountInMoneyDefault;
        /// <summary>
        /// Whether to express the account in currency or in points
        /// </summary>
        public static bool AccountInMoney
        {
            get { return isAccountInMoney; }
            set
            {
                isAccountInMoney = value;
                SetNode("config/account/accountInMoney", value);
            }
        }

        static string accountCurrency = AccountCurrencyDefault;
        /// <summary>
        /// Account Currency
        /// </summary>
        public static string AccountCurrency
        {
            get { return accountCurrency; }
            set
            {
                accountCurrency = value;
                SetNode("config/account/accountCurrency", value);
            }
        }

        static int initialAccount = InitialAccountDefault;
        /// <summary>
        /// Initial Account
        /// </summary>
        public static int InitialAccount
        {
            get { return initialAccount; }
            set
            {
                initialAccount = value;
                SetNode("config/account/initialAccount", value);
            }
        }

        static int leverage = LeverageDefault;
        /// <summary>
        /// Leverage
        /// </summary>
        public static int Leverage
        {
            get { return leverage; }
            set
            {
                leverage = value;
                SetNode("config/account/leverage", value);
            }
        }

        static bool isShowJournal = IsShowJournalDefault;
        /// <summary>
        /// Whether to show the journal
        /// </summary>
        public static bool ShowJournal
        {
            get { return isShowJournal; }
            set
            {
                isShowJournal = value;
                SetNode("config/showJournal", value);
            }
        }

        static bool isJournalByBars = IsJournalByBarsDefault;
        /// <summary>
        /// Arrange the journal by bars
        /// </summary>
        public static bool JournalByBars
        {
            get { return isJournalByBars; }
            set
            {
                isJournalByBars = value;
                SetNode("config/journalByBars", value);
            }
        }

        static bool isJournalShowTransfers = IsJournalShowTransfersDefault;
        /// <summary>
        /// Sets if journal shows transfers
        /// </summary>
        public static bool JournalShowTransfers
        {
            get { return isJournalShowTransfers; }
            set
            {
                isJournalShowTransfers = value;
                SetNode("config/journalShowTransfers", value);
            }
        }

        static bool isAutoscan = IsAutoscanDefault;
        /// <summary>
        /// Perform auto scanning
        /// </summary>
        public static bool Autoscan
        {
            get { return isAutoscan; }
            set
            {
                isAutoscan = value;
                SetNode("config/autoscan", value);
            }
        }

        static bool isTradeUntilMarginCall = IsTradeUntilMarginCallDefault;
        /// <summary>
        /// Close all trades after a Margin Call
        /// </summary>
        public static bool TradeUntilMarginCall
        {
            get { return isTradeUntilMarginCall; }
            set
            {
                isTradeUntilMarginCall = value;
                SetNode("config/tradeUntilMarginCall", value);
            }
        }

        static bool isAdditionalStatistics = IsAdditionalStatisticsDefault;
        /// <summary>
        /// Calculates additional stats
        /// </summary>
        public static bool AdditionalStatistics
        {
            get { return isAdditionalStatistics; }
            set
            {
                isAdditionalStatistics = value;
                SetNode("config/additionalStatistics", value);
            }
        }

        static bool isUseLogicalGroups = IsUseLogicalGroupsDefault;
        /// <summary>
        /// Logical groups for the entry / exit filters.
        /// </summary>
        public static bool UseLogicalGroups
        {
            get { return isUseLogicalGroups; }
            set
            {
                isUseLogicalGroups = value;
                SetNode("config/useLogicalGroups", value);
            }
        }

        static bool isPlaySounds = IsPlaySoundsDefault;
        /// <summary>
        /// Sets if the program plays sounds on events.
        /// </summary>
        public static bool PlaySounds
        {
            get { return isPlaySounds; }
            set
            {
                isPlaySounds = value;
                SetNode("config/playSounds", value);
            }
        }

        static string criteriaSettings = CriteriaSettingsDefault;
        public static string CriteriaSettings
        {
            get { return criteriaSettings; }
            set
            {
                criteriaSettings = value;
                SetNode("config/criteriaSettings", value);
            }
        }

        static string generatorOptions = GeneratorOptionsDefault;
        /// <summary>
        /// Generator options
        /// </summary>
        public static string GeneratorOptions
        {
            get { return generatorOptions; }
            set
            {
                generatorOptions = value;
                SetNode("config/generatorOptions", value);
            }
        }

        static string optimizerOptions = OptimizerOptionsDefault;
        /// <summary>
        /// Optimizer options
        /// </summary>
        public static string OptimizerOptions
        {
            get { return optimizerOptions; }
            set
            {
                optimizerOptions = value;
                SetNode("config/optimizerOptions", value);
            }
        }

        static string columnSeparator = ColumnSeparatorDefault;
        public static string ColumnSeparator
        {
            get { return columnSeparator; }
            set
            {
                columnSeparator = value;
                SetNode("config/columnSeparator", value);
            }
        }

        static string decimalSeparator = DecimalSeparatorDefault;
        public static string DecimalSeparator
        {
            get { return decimalSeparator; }
            set
            {
                decimalSeparator = value;
                SetNode("config/decimalSeparator", value);
            }
        }

        static bool useTickData = UseTickDataDefault;
        public static bool UseTickData
        {
            get { return useTickData; }
            set
            {
                useTickData = value;
                SetNode("config/useTickData", value);
            }
        }


        static string jforexDataPath = JforexDataPathDefault;
        public static string JForexDataPath
        {
            get { return jforexDataPath; }
            set
            {
                jforexDataPath = value;
                SetNode("config/jforexDataPath", value);
            }
        }

        static string metatrader4DataPath = Metatrader4DataPathDefault;
        public static string MetaTrader4DataPath
        {
            get { return metatrader4DataPath; }
            set
            {
                metatrader4DataPath = value;
                SetNode("config/metatrader4DataPath", value);
            }
        }

        static int marketClosingHour = MarketClosingHourDefault;
        public static int MarketClosingHour
        {
            get { return marketClosingHour; }
            set
            {
                marketClosingHour = value;
                SetNode("config/marketClosingHour", value);
            }
        }

        static int marketOpeningHour = MarketOpeningHourDefault;
        public static int MarketOpeningHour
        {
            get { return marketOpeningHour; }
            set
            {
                marketOpeningHour = value;
                SetNode("config/marketOpeningHour", value);
            }
        }

        static string importStartingDate = ImportStartingDateDefault;
        public static string ImportStartingDate
        {
            get { return importStartingDate; }
            set
            {
                importStartingDate = value;
                SetNode("config/importStartingDate", value);
            }
        }

        static string importEndingDate = ImportEndingDateDefault;
        public static string ImportEndingDate
        {
            get { return importEndingDate; }
            set
            {
                importEndingDate = value;
                SetNode("config/importEndingDate", value);
            }
        }

        static string mt4ImportDestFolder = MT4ImportDestFolderDefault;
        public static string MT4ImportDestFolder
        {
            get { return mt4ImportDestFolder; }
            set
            {
                mt4ImportDestFolder = value;
                SetNode("config/mt4ImportDestFolder", value);
            }
        }

        static string jForexImportDestFolder = JForexImportDestFolderDefault;
        public static string JForexImportDestFolder
        {
            get { return jForexImportDestFolder; }
            set
            {
                jForexImportDestFolder = value;
                SetNode("config/jForexImportDestFolder", value);
            }
        }

        static int minBarsInBarFile = MinBarsInBarFileDefault;
        public static int MinBarsInBarFile
        {
            get { return minBarsInBarFile; }
            set
            {
                minBarsInBarFile = value;
                SetNode("config/minBarsInBarFile", value);
            }
        }

        static string oandaImportDestFolder = OandaImportDestFolderDefault;
        public static string OandaImportDestFolder
        {
            get { return oandaImportDestFolder; }
            set
            {
                oandaImportDestFolder = value;
                SetNode("config/oandaImportDestFolder", value);
            }
        }

        static string bannedIndicators = BannedIndicatorsDefault;
        public static string BannedIndicators
        {
            get { return bannedIndicators; }
            set
            {
                bannedIndicators = value;
                SetNode("config/bannedIndicators", value);
            }
        }

        static bool showPriceChartOnAccountChart = ShowPriceChartOnAccountChartDefault;
        public static bool ShowPriceChartOnAccountChart
        {
            get { return showPriceChartOnAccountChart; }
            set
            {
                showPriceChartOnAccountChart = value;
                SetNode("config/showPriceChartOnAccountChart", value);
            }
        }

        static bool analyzerHideFSB = AnalyzerHideFSBDefault;
        public static bool AnalyzerHideFSB
        {
            get { return analyzerHideFSB; }
            set
            {
                analyzerHideFSB = value;
                SetNode("config/analyzerHideFSB", value);
            }
        }

        static bool sendUsageStats = SendUsageStatsDefault;
        public static bool SendUsageStats
        {
            get { return sendUsageStats; }
            set
            {
                sendUsageStats = value;
                SetNode("config/sendUsageStats", value);
            }
        }

        static int mainScreenWidth = MainScreenWidthDefault;
        public static int MainScreenWidth
        {
            get { return mainScreenWidth; }
            set
            {
                mainScreenWidth = value;
                SetNode("config/mainScreenWidth", value);
            }
        }

        static int mainScreenHeight = MainScreenHeightDefault;
        public static int MainScreenHeight
        {
            get { return mainScreenHeight; }
            set
            {
                mainScreenHeight = value;
                SetNode("config/mainScreenHeight", value);
            }
        }

        static bool showStatusBar = ShowStatusBarDefault;
        public static bool ShowStatusBar
        {
            get { return showStatusBar; }
            set
            {
                showStatusBar = value;
                SetNode("config/showStatusBar", value);
            }
        }

	
        static bool strategyDirWatch = StrategyDirWatchDefault;
        public static bool StrategyDirWatch
        {
            get { return strategyDirWatch; }
            set
            {
                strategyDirWatch = value;
                SetNode("config/strategyDirWatch", value);
            }
        }

        static string truefxImportDestFolder = TrueFxImportDestFolderDefault;
        public static string TrueFxImportDestFolder
        {
            get { return truefxImportDestFolder; }
            set
            {
                oandaImportDestFolder = value;
                SetNode("config/truefxImportDestFolder", value);
            }
        }

        // -------------------------------------------------------------


        static int indicatorChartZoom = IndicatorChartZoomDefault;
        public static int IndicatorChartZoom
        {
            get { return indicatorChartZoom; }
            set
            {
                indicatorChartZoom = value;
                SetNode("config/indicatorChart/zoom", value);
            }
        }

        static bool isIndicatorChartInfoPanel = IsIndicatorChartInfoPanelDefault;
        public static bool IndicatorChartInfoPanel
        {
            get { return isIndicatorChartInfoPanel; }
            set
            {
                isIndicatorChartInfoPanel = value;
                SetNode("config/indicatorChart/infoPanel", value);
            }
        }

        static bool isIndicatorChartDynamicInfo = IsIndicatorChartDynamicInfoDefault;
        public static bool IndicatorChartDynamicInfo
        {
            get { return isIndicatorChartDynamicInfo; }
            set
            {
                isIndicatorChartDynamicInfo = value;
                SetNode("config/indicatorChart/dynamicInfo", value);
            }
        }

        static bool isIndicatorChartGrid = IsIndicatorChartGridDefault;
        public static bool IndicatorChartGrid
        {
            get { return isIndicatorChartGrid; }
            set
            {
                isIndicatorChartGrid = value;
                SetNode("config/indicatorChart/grid", value);
            }
        }

        static bool isIndicatorChartCross = IsIndicatorChartCrossDefault;
        public static bool IndicatorChartCross
        {
            get { return isIndicatorChartCross; }
            set
            {
                isIndicatorChartCross = value;

                SetNode("config/indicatorChart/cross", value);
            }
        }

        static bool isIndicatorChartVolume = IsIndicatorChartVolumeDefault;
        public static bool IndicatorChartVolume
        {
            get { return isIndicatorChartVolume; }
            set
            {
                isIndicatorChartVolume = value;
                SetNode("config/indicatorChart/volume", value);
            }
        }

        static bool isIndicatorChartLots = IsIndicatorChartLotsDefault;
        public static bool IndicatorChartLots
        {
            get { return isIndicatorChartLots; }
            set
            {
                isIndicatorChartLots = value;
                SetNode("config/indicatorChart/lots", value);
            }
        }

        static bool isIndicatorChartEntryExitPoints = IsIndicatorChartEntryExitPointsDefault;
        public static bool IndicatorChartEntryExitPoints
        {
            get { return isIndicatorChartEntryExitPoints; }
            set
            {
                isIndicatorChartEntryExitPoints = value;
                SetNode("config/indicatorChart/entryExitPoints", value);
            }
        }

        static bool isIndicatorChartCorrectedPositionPrice = IsIndicatorChartCorrectedPositionPriceDefault;
        public static bool IndicatorChartCorrectedPositionPrice
        {
            get { return isIndicatorChartCorrectedPositionPrice; }
            set
            {
                isIndicatorChartCorrectedPositionPrice = value;
                SetNode("config/indicatorChart/correctedPositionPrice", value);
            }
        }

        static bool isIndicatorChartBalanceEquityChart = IsIndicatorChartBalanceEquityChartDefault;
        public static bool IndicatorChartBalanceEquityChart
        {
            get { return isIndicatorChartBalanceEquityChart; }
            set
            {
                isIndicatorChartBalanceEquityChart = value;
                SetNode("config/indicatorChart/balanceEquityChart", value);
            }
        }

        static bool isIndicatorChartFloatingPLChart = IsIndicatorChartFloatingPLChartDefault;
        public static bool IndicatorChartFloatingPLChart
        {
            get { return isIndicatorChartFloatingPLChart; }
            set
            {
                isIndicatorChartFloatingPLChart = value;
                SetNode("config/indicatorChart/floatingPLChart", value);
            }
        }

        static bool isIndicatorChartIndicators = IsIndicatorChartIndicatorsDefault;
        public static bool IndicatorChartIndicators
        {
            get { return isIndicatorChartIndicators; }
            set
            {
                isIndicatorChartIndicators = value;

                SetNode("config/indicatorChart/indicators", value);
            }
        }

        static bool isIndicatorChartAmbiguousMark = IsIndicatorChartAmbiguousMarkDefault;
        public static bool IndicatorChartAmbiguousMark
        {
            get { return isIndicatorChartAmbiguousMark; }
            set
            {
                isIndicatorChartAmbiguousMark = value;
                SetNode("config/indicatorChart/ambiguousMark", value);
            }
        }

        static bool isIndicatorChartTrueCharts = IsIndicatorChartTrueChartsDefault;
        public static bool IndicatorChartTrueCharts
        {
            get { return isIndicatorChartTrueCharts; }
            set
            {
                isIndicatorChartTrueCharts = value;

                SetNode("config/indicatorChart/trueCharts", value);
            }
        }

        static bool isIndicatorChartProtections = IsIndicatorChartProtectionsDefault;
        public static bool IndicatorChartProtections
        {
            get { return isIndicatorChartProtections; }
            set
            {
                isIndicatorChartProtections = value;
                SetNode("config/indicatorChart/protections", value);
            }
        }

        // -------------------------------------------------------------
        static int balanceChartZoom = BalanceChartZoomDefault;
        public static int BalanceChartZoom
        {
            get { return balanceChartZoom; }
            set
            {
                balanceChartZoom = value;
                SetNode("config/balanceChart/zoom", value);
            }
        }

        static bool isBalanceChartInfoPanel = IsBalanceChartInfoPanelDefault;
        public static bool BalanceChartInfoPanel
        {
            get { return isBalanceChartInfoPanel; }
            set
            {
                isBalanceChartInfoPanel = value;
                SetNode("config/balanceChart/infoPanel", value);
            }
        }

        static bool isBalanceChartDynamicInfo = IsBalanceChartDynamicInfoDefault;
        public static bool BalanceChartDynamicInfo
        {
            get { return isBalanceChartDynamicInfo; }
            set
            {
                isBalanceChartDynamicInfo = value;
                SetNode("config/balanceChart/dynamicInfo", value);
            }
        }

        static bool isBalanceChartGrid = IsBalanceChartGridDefault;
        public static bool BalanceChartGrid
        {
            get { return isBalanceChartGrid; }
            set
            {
                isBalanceChartGrid = value;
                SetNode("config/balanceChart/grid", value);
            }
        }

        static bool isBalanceChartCross = IsBalanceChartCrossDefault;
        public static bool BalanceChartCross
        {
            get { return isBalanceChartCross; }
            set
            {
                isBalanceChartCross = value;
                SetNode("config/balanceChart/cross", value);
            }
        }

        static bool isBalanceChartVolume = IsBalanceChartVolumeDefault;
        public static bool BalanceChartVolume
        {
            get { return isBalanceChartVolume; }
            set
            {
                isBalanceChartVolume = value;
                SetNode("config/balanceChart/volume", value);
            }
        }

        static bool isBalanceChartLots = IsBalanceChartLotsDefault;
        public static bool BalanceChartLots
        {
            get { return isBalanceChartLots; }
            set
            {
                isBalanceChartLots = value;
                SetNode("config/balanceChart/lots", value);
            }
        }

        static bool isBalanceChartEntryExitPoints = IsBalanceChartEntryExitPointsDefault;
        public static bool BalanceChartEntryExitPoints
        {
            get { return isBalanceChartEntryExitPoints; }
            set
            {
                isBalanceChartEntryExitPoints = value;
                SetNode("config/balanceChart/entryExitPoints", value);
            }
        }

        static bool isBalanceChartCorrectedPositionPrice = IsBalanceChartCorrectedPositionPriceDefault;
        public static bool BalanceChartCorrectedPositionPrice
        {
            get { return isBalanceChartCorrectedPositionPrice; }
            set
            {
                isBalanceChartCorrectedPositionPrice = value;
                SetNode("config/balanceChart/correctedPositionPrice", value);
            }
        }

        static bool isBalanceChartBalanceEquityChart = IsBalanceChartBalanceEquityChartDefault;
        public static bool BalanceChartBalanceEquityChart
        {
            get { return isBalanceChartBalanceEquityChart; }
            set
            {
                isBalanceChartBalanceEquityChart = value;
                SetNode("config/balanceChart/balanceEquityChart", value);
            }
        }

        static bool isBalanceChartFloatingPLChart = IsBalanceChartFloatingPLChartDefault;
        public static bool BalanceChartFloatingPLChart
        {
            get { return isBalanceChartFloatingPLChart; }
            set
            {
                isBalanceChartFloatingPLChart = value;
                SetNode("config/balanceChart/floatingPLChart", value);
            }
        }

        static bool isBalanceChartIndicators = IsBalanceChartIndicatorsDefault;
        public static bool BalanceChartIndicators
        {
            get { return isBalanceChartIndicators; }
            set
            {
                isBalanceChartIndicators = value;
                SetNode("config/balanceChart/indicators", value);
            }
        }

        static bool isBalanceChartAmbiguousMark = IsBalanceChartAmbiguousMarkDefault;
        public static bool BalanceChartAmbiguousMark
        {
            get { return isBalanceChartAmbiguousMark; }
            set
            {
                isBalanceChartAmbiguousMark = value;
                SetNode("config/balanceChart/ambiguousMark", value);
            }
        }

        static bool isBalanceChartTrueCharts = IsBalanceChartTrueChartsDefault;
        public static bool BalanceChartTrueCharts
        {
            get { return isBalanceChartTrueCharts; }
            set
            {
                isBalanceChartTrueCharts = value;
                SetNode("config/balanceChart/trueCharts", value);
            }
        }

        static bool isBalanceChartProtections = IsBalanceChartProtectionsDefault;
        public static bool BalanceChartProtections
        {
            get { return isBalanceChartProtections; }
            set
            {
                isBalanceChartProtections = value;
                SetNode("config/balanceChart/protections", value);
            }
        }

// ----------------------------------------------------------------------

        /// <summary>
        /// Public constructor
        /// </summary>
        static Configs()
        {
            string externalConfigFile = string.Empty;
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-configfile="))
                    externalConfigFile = CommandLineParser.GetValue(arg);

            xmlConfig = new XmlDocument();

            PathToConfigFile = String.IsNullOrEmpty(externalConfigFile)
                                ? Path.Combine(Data.SystemDir, "config.xml")
                                : externalConfigFile;
        }

        private static void SetNode(string node, object value)
        {
            if (!isConfigLoaded) return;
            var xmlNodeList = xmlConfig.SelectNodes(node);
            if (xmlNodeList == null) return;
            var xmlNode = xmlNodeList.Item(0);
            if (xmlNode != null)
                xmlNode.InnerText = value.ToString();
        }

        /// <summary>
        /// Sets the parameters to its default value
        /// </summary>
        public static void ResetParams()
        {
            if (!isConfigLoaded)
                return;

            MinBars                  = MinBarsDefault;
            MaxBarsLimit             = MaxBarsLimitDefault;
            MaxIntraBars             = MaxIntraBarsDefault;
            MaxEntryFilters          = MaxEntryFiltersDefault;
            MaxExitFilters           = MaxExitFiltersDefault;
            SigmaModeMainChart       = SigmaModeMainChartDefault;
            SigmaModeSeparatedChart  = SigmaModeSeparatedChartDefault;

            Language                   = LanguageDefault;
            ShowStartingTip            = ShowStartingTipDefault;
            CurrentTipNumber           = CurrentTipNumberDefault;
            DataDirectory              = DataDirectoryDefault;
            ColorScheme                = ColorSchemeDefault;
            RememberLastStr            = IsRememberLastStrDefault;
            LastStrategy               = LastStrategyDefault;
            CheckForUpdates            = IsCheckForUpdatesDefault;
            CheckForNewBeta            = IsCheckForNewBetaDefault;
            CheckData                  = IsCheckDataDefault;
            FillInDataGaps             = IsFillDataGapsDefault;
            CutBadData                 = IsCutBadDataDefault;
            CutSatSunData              = IsCutSatSunDataDefault;
            LoadCustomIndicators       = IsLoadCustIndDefault;
            ShowCustomIndicators       = IsShowCustIndDefault;
            MaxBars                    = MaxBarsDefault;
            DataStartTime              = DataStartTimeDefault;
            DataEndTime                = DataEndTimeDefault;
            UseEndTime                 = IsUseEndTimeDefault;
            UseStartTime               = IsUseStartTimeDefault;
            AccountInMoney             = IsAccountInMoneyDefault;
            AccountCurrency            = AccountCurrencyDefault;
            InitialAccount             = InitialAccountDefault;
            Leverage                   = LeverageDefault;
            ShowJournal                = IsShowJournalDefault;
            JournalByBars              = IsJournalByBarsDefault;
            Autoscan                   = IsAutoscanDefault;
            TradeUntilMarginCall       = IsTradeUntilMarginCallDefault;
            AdditionalStatistics       = IsAdditionalStatisticsDefault;
            UseLogicalGroups           = IsUseLogicalGroupsDefault;
            PlaySounds                 = IsPlaySoundsDefault;
            GeneratorOptions           = GeneratorOptionsDefault;
            CriteriaSettings           = CriteriaSettingsDefault;
            OptimizerOptions           = OptimizerOptionsDefault;
            ColumnSeparator            = ColumnSeparatorDefault;
            DecimalSeparator           = DecimalSeparatorDefault;
            UseTickData                = UseTickDataDefault;
            JForexDataPath             = JforexDataPathDefault;
            JForexImportDestFolder     = JForexImportDestFolderDefault;
            MetaTrader4DataPath        = Metatrader4DataPathDefault;
            MarketClosingHour          = MarketClosingHourDefault;
            MarketOpeningHour          = MarketOpeningHourDefault;
            ImportStartingDate         = ImportStartingDateDefault;
            ImportEndingDate           = ImportEndingDateDefault;
            MT4ImportDestFolder        = MT4ImportDestFolderDefault;
            MinBarsInBarFile           = MinBarsInBarFileDefault;
            OandaImportDestFolder      = OandaImportDestFolderDefault;
            BannedIndicators           = BannedIndicatorsDefault;
            ShowPriceChartOnAccountChart = ShowPriceChartOnAccountChartDefault;
            AnalyzerHideFSB            = AnalyzerHideFSBDefault;
            MainScreenWidth            = MainScreenWidthDefault;
            MainScreenHeight           = MainScreenHeightDefault;
            ShowStatusBar              = ShowStatusBarDefault;
            StrategyDirWatch           = StrategyDirWatchDefault;
            TrueFxImportDestFolder     = TrueFxImportDestFolderDefault;

            // Indicator Chart
            IndicatorChartZoom                   = IndicatorChartZoomDefault;
            IndicatorChartInfoPanel              = IsIndicatorChartInfoPanelDefault;
            IndicatorChartDynamicInfo            = IsIndicatorChartDynamicInfoDefault;
            IndicatorChartGrid                   = IsIndicatorChartGridDefault;
            IndicatorChartCross                  = IsIndicatorChartCrossDefault;
            IndicatorChartVolume                 = IsIndicatorChartVolumeDefault;
            IndicatorChartLots                   = IsIndicatorChartLotsDefault;
            IndicatorChartEntryExitPoints        = IsIndicatorChartEntryExitPointsDefault;
            IndicatorChartCorrectedPositionPrice = IsIndicatorChartCorrectedPositionPriceDefault;
            IndicatorChartBalanceEquityChart     = IsIndicatorChartBalanceEquityChartDefault;
            IndicatorChartFloatingPLChart        = IsIndicatorChartFloatingPLChartDefault;
            IndicatorChartIndicators             = IsIndicatorChartIndicatorsDefault;
            IndicatorChartAmbiguousMark          = IsIndicatorChartAmbiguousMarkDefault;
            IndicatorChartTrueCharts             = IsIndicatorChartTrueChartsDefault;
            IndicatorChartProtections            = IsIndicatorChartProtectionsDefault;

            // Balance Chart
            BalanceChartZoom                   = BalanceChartZoomDefault;
            BalanceChartInfoPanel              = IsBalanceChartInfoPanelDefault;
            BalanceChartDynamicInfo            = IsBalanceChartDynamicInfoDefault;
            BalanceChartGrid                   = IsBalanceChartGridDefault;
            BalanceChartCross                  = IsBalanceChartCrossDefault;
            BalanceChartVolume                 = IsBalanceChartVolumeDefault;
            BalanceChartLots                   = IsBalanceChartLotsDefault;
            BalanceChartEntryExitPoints        = IsBalanceChartEntryExitPointsDefault;
            BalanceChartCorrectedPositionPrice = IsBalanceChartCorrectedPositionPriceDefault;
            BalanceChartBalanceEquityChart     = IsBalanceChartBalanceEquityChartDefault;
            BalanceChartFloatingPLChart        = IsBalanceChartFloatingPLChartDefault;
            BalanceChartIndicators             = IsBalanceChartIndicatorsDefault;
            BalanceChartAmbiguousMark          = IsBalanceChartAmbiguousMarkDefault;
            BalanceChartTrueCharts             = IsBalanceChartTrueChartsDefault;
            BalanceChartProtections            = IsBalanceChartProtectionsDefault;

            SaveConfigs();
            isResetActivated = true;
        }

        /// <summary>
        /// Parses the config file
        /// </summary>
        static void ParseConfigs()
        {
            // Constants
            minBars                  = ParseNode("config/MIN_BARS", MinBarsDefault);
            maxBarsLimit             = ParseNode("config/MAX_BARS", MaxBarsLimitDefault);
            maxIntraBars             = ParseNode("config/MAX_INTRA_BARS", MaxIntraBarsDefault);
            maxEntryFilters          = ParseNode("config/MAX_ENTRY_FILTERS", MaxEntryFiltersDefault);
            maxExitFilters           = ParseNode("config/MAX_EXIT_FILTERS", MaxExitFiltersDefault);
            sigmaModeMainChart       = ParseNode("config/SIGMA_MODE_MAIN_CHART", SigmaModeMainChartDefault);
            sigmaModeSeparatedChart  = ParseNode("config/SIGMA_MODE_SEPARATED_CHART", SigmaModeSeparatedChartDefault);

            isInstalled                  = ParseNode("config/installed", false);
            language                     = ParseNode("config/language", LanguageDefault);
            isShowStartingTip            = ParseNode("config/showStartingTip", ShowStartingTipDefault);
            currentTipNumber             = ParseNode("config/currentTipNumber", CurrentTipNumberDefault);
            isGradientView               = ParseNode("config/gradientView", IsGradientViewDefault);
            isShowJournal                = ParseNode("config/showJournal", IsShowJournalDefault);
            isJournalByBars              = ParseNode("config/journalByBars", IsJournalByBarsDefault);
            isJournalShowTransfers       = ParseNode("config/journalShowTransfers", IsJournalShowTransfersDefault);
            isAutoscan                   = ParseNode("config/autoscan", IsAutoscanDefault);
            isTradeUntilMarginCall       = ParseNode("config/tradeUntilMarginCall", IsTradeUntilMarginCallDefault);
            isAdditionalStatistics       = ParseNode("config/additionalStatistics", IsAdditionalStatisticsDefault);
            isUseLogicalGroups           = ParseNode("config/useLogicalGroups", IsUseLogicalGroupsDefault);
            dataDirectory                = ParseNode("config/dataDirectory", DataDirectoryDefault);
            colorScheme                  = ParseNode("config/colorScheme", ColorSchemeDefault);
            isRememberLastStr            = ParseNode("config/rememberLastStrategy", IsRememberLastStrDefault);
            lastStrategy                 = ParseNode("config/lastStrategy", LastStrategyDefault);
            isCheckForUpdates            = ParseNode("config/checkForUpdates", IsCheckForUpdatesDefault);
            isCheckForNewBeta            = ParseNode("config/checkForNewBeta", IsCheckForNewBetaDefault);
            isCheckData                  = ParseNode("config/checkData", IsCheckDataDefault);
            isFillDataGaps               = ParseNode("config/fillDataGaps", IsFillDataGapsDefault);
            isCutBadData                 = ParseNode("config/cutBadData", IsCutBadDataDefault);
            isCutSatSunData              = ParseNode("config/cutSatSunData", IsCutSatSunDataDefault);
            isLoadCustomIndicators       = ParseNode("config/loadCustomIndicators", IsLoadCustIndDefault);
            isShowCustomIndicators       = ParseNode("config/showCustomIndicators", IsShowCustIndDefault);
            isPlaySounds                 = ParseNode("config/playSounds", IsPlaySoundsDefault);
            criteriaSettings             = ParseNode("config/criteriaSettings", CriteriaSettingsDefault);
            generatorOptions             = ParseNode("config/generatorOptions", GeneratorOptionsDefault);
            optimizerOptions             = ParseNode("config/optimizerOptions", OptimizerOptionsDefault);
            columnSeparator              = ParseNode("config/columnSeparator", ColumnSeparatorDefault);
            decimalSeparator             = ParseNode("config/decimalSeparator", DecimalSeparatorDefault);
            useTickData                  = ParseNode("config/useTickData",  UseTickDataDefault);
            jforexDataPath               = ParseNode("config/jforexDataPath",  JforexDataPathDefault);
            jForexImportDestFolder       = ParseNode("config/jForexImportDestFolder", JForexImportDestFolderDefault);
            metatrader4DataPath          = ParseNode("config/metatrader4DataPath", Metatrader4DataPathDefault);
            marketClosingHour            = ParseNode("config/marketClosingHour", MarketClosingHourDefault);
            marketOpeningHour            = ParseNode("config/marketOpeningHour", MarketOpeningHourDefault);
            importStartingDate           = ParseNode("config/importStartingDate", ImportStartingDateDefault);
            importEndingDate             = ParseNode("config/importEndingDate", ImportEndingDateDefault);
            mt4ImportDestFolder          = ParseNode("config/mt4ImportDestFolder", MT4ImportDestFolderDefault);
            minBarsInBarFile             = ParseNode("config/minBarsInBarFile", MinBarsInBarFileDefault);
            oandaImportDestFolder        = ParseNode("config/oandaImportDestFolder", OandaImportDestFolderDefault);
            bannedIndicators             = ParseNode("config/bannedIndicators", BannedIndicatorsDefault);
            showPriceChartOnAccountChart = ParseNode("config/showPriceChartOnAccountChart", ShowPriceChartOnAccountChartDefault);
            analyzerHideFSB              = ParseNode("config/analyzerHideFSB", AnalyzerHideFSBDefault);
            sendUsageStats               = ParseNode("config/sendUsageStats", SendUsageStatsDefault);
            mainScreenWidth              = ParseNode("config/mainScreenWidth", MainScreenWidthDefault);
            mainScreenHeight             = ParseNode("config/mainScreenHeight", MainScreenHeightDefault);
            showStatusBar                = ParseNode("config/showStatusBar", ShowStatusBarDefault);
            strategyDirWatch             = ParseNode("config/strategyDirWatch", StrategyDirWatchDefault);
            truefxImportDestFolder       = ParseNode("config/truefxImportDestFolder", TrueFxImportDestFolderDefault);

            // Data Horizon
            maxBars                      = ParseNode("config/dataMaxBars", MaxBarsDefault);
            dataStartTime                = ParseNode("config/dataStartTime", DataStartTimeDefault);
            dataEndTime                  = ParseNode("config/dataEndTime", DataEndTimeDefault);
            isUseStartTime               = ParseNode("config/dataUseStartTime", IsUseStartTimeDefault);
            isUseEndTime                 = ParseNode("config/dataUseEndTime", IsUseEndTimeDefault);

            // Account
            isAccountInMoney             = ParseNode("config/account/accountInMoney", IsAccountInMoneyDefault);
            accountCurrency              = ParseNode("config/account/accountCurrency", AccountCurrencyDefault);
            initialAccount               = ParseNode("config/account/initialAccount", InitialAccountDefault);
            leverage                     = ParseNode("config/account/leverage", LeverageDefault);

            // Indicator Chart
            indicatorChartZoom                     = ParseNode("config/indicatorChart/zoom", IndicatorChartZoomDefault);
            isIndicatorChartInfoPanel              = ParseNode("config/indicatorChart/infoPanel", IsIndicatorChartInfoPanelDefault);
            isIndicatorChartDynamicInfo            = ParseNode("config/indicatorChart/dynamicInfo", IsIndicatorChartDynamicInfoDefault);
            isIndicatorChartGrid                   = ParseNode("config/indicatorChart/grid", IsIndicatorChartGridDefault);
            isIndicatorChartCross                  = ParseNode("config/indicatorChart/cross", IsIndicatorChartCrossDefault);
            isIndicatorChartVolume                 = ParseNode("config/indicatorChart/volume", IsIndicatorChartVolumeDefault);
            isIndicatorChartLots                   = ParseNode("config/indicatorChart/lots", IsIndicatorChartLotsDefault);
            isIndicatorChartEntryExitPoints        = ParseNode("config/indicatorChart/entryExitPoints", IsIndicatorChartEntryExitPointsDefault);
            isIndicatorChartCorrectedPositionPrice = ParseNode("config/indicatorChart/correctedPositionPrice", IsIndicatorChartCorrectedPositionPriceDefault);
            isIndicatorChartBalanceEquityChart     = ParseNode("config/indicatorChart/balanceEquityChart", IsIndicatorChartBalanceEquityChartDefault);
            isIndicatorChartFloatingPLChart        = ParseNode("config/indicatorChart/floatingPLChart", IsIndicatorChartFloatingPLChartDefault);
            isIndicatorChartIndicators             = ParseNode("config/indicatorChart/indicators", IsIndicatorChartIndicatorsDefault);
            isIndicatorChartAmbiguousMark          = ParseNode("config/indicatorChart/ambiguousMark", IsIndicatorChartAmbiguousMarkDefault);
            isIndicatorChartTrueCharts             = ParseNode("config/indicatorChart/trueCharts", IsIndicatorChartTrueChartsDefault);
            isIndicatorChartProtections            = ParseNode("config/indicatorChart/protections", IsIndicatorChartProtectionsDefault);

            // Balance Chart
            balanceChartZoom                     = ParseNode("config/balanceChart/zoom", BalanceChartZoomDefault);
            isBalanceChartInfoPanel              = ParseNode("config/balanceChart/infoPanel", IsBalanceChartInfoPanelDefault);
            isBalanceChartDynamicInfo            = ParseNode("config/balanceChart/dynamicInfo", IsBalanceChartDynamicInfoDefault);
            isBalanceChartGrid                   = ParseNode("config/balanceChart/grid", IsBalanceChartGridDefault);
            isBalanceChartCross                  = ParseNode("config/balanceChart/cross", IsBalanceChartCrossDefault);
            isBalanceChartVolume                 = ParseNode("config/balanceChart/volume", IsBalanceChartVolumeDefault);
            isBalanceChartLots                   = ParseNode("config/balanceChart/lots", IsBalanceChartLotsDefault);
            isBalanceChartEntryExitPoints        = ParseNode("config/balanceChart/entryExitPoints", IsBalanceChartEntryExitPointsDefault);
            isBalanceChartCorrectedPositionPrice = ParseNode("config/balanceChart/correctedPositionPrice", IsBalanceChartCorrectedPositionPriceDefault);
            isBalanceChartBalanceEquityChart     = ParseNode("config/balanceChart/balanceEquityChart", IsBalanceChartBalanceEquityChartDefault);
            isBalanceChartFloatingPLChart        = ParseNode("config/balanceChart/floatingPLChart", IsBalanceChartFloatingPLChartDefault);
            isBalanceChartIndicators             = ParseNode("config/balanceChart/indicators", IsBalanceChartIndicatorsDefault);
            isBalanceChartAmbiguousMark          = ParseNode("config/balanceChart/ambiguousMark", IsBalanceChartAmbiguousMarkDefault);
            isBalanceChartTrueCharts             = ParseNode("config/balanceChart/trueCharts", IsBalanceChartTrueChartsDefault);
            isBalanceChartProtections            = ParseNode("config/balanceChart/protections", IsBalanceChartProtectionsDefault);
        }

        /// <summary>
        /// Sets parameters after loading config file.
        /// </summary>
        static void ConfigAfterLoading()
        {
            if (maxBars > maxBarsLimit)
                maxBars = maxBarsLimit;

            if (!isInstalled)
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.CreateSubKey("Software\\Forex Software\\Forex Strategy Builder");
                if (regKey != null)
                    SendUsageStats = (regKey.GetValue("UsageStats") == null || regKey.GetValue("UsageStats").ToString() == "0");
                IsInstalled = true;
            }

            if (dataDirectory != "" && Directory.Exists(dataDirectory))
                Data.OfflineDataDir = dataDirectory;

            Data.ProgStats.SendUsageStatistics = SendUsageStats;
        }

        /// <summary>
        /// Parses an integer parameter.
        /// </summary>
        static int ParseNode(string node, int defaultValue)
        {
            int value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null) value = int.Parse(xmlNode.InnerText);
                }
                else
                    CreateElement(node, defaultValue.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                if (!Data.AutostartGenerator)
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Parses a string parameter.
        /// </summary>
        static string ParseNode(string node, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null) value = xmlNode.InnerText;
                }
                else
                    CreateElement(node, defaultValue);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Parses a boolean parameter.
        /// </summary>
        static bool ParseNode(string node, bool defaultValue)
        {
            bool value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null) value = bool.Parse(xmlNode.InnerText);
                }
                else
                    CreateElement(node, defaultValue.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Parses a DateTime parameter.
        /// </summary>
        static DateTime ParseNode(string node, DateTime defaultValue)
        {
            DateTime value = defaultValue;

            try
            {
                XmlNodeList list = xmlConfig.SelectNodes(node);
                if (list != null && list.Count > 0)
                {
                    var xmlNode = list.Item(0);
                    if (xmlNode != null)
                        value = DateTime.ParseExact(xmlNode.InnerText, DateStringFormat, new DateTimeFormatInfo());
                }
                else
                    CreateElement(node, defaultValue.ToString(DateStringFormat));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }

            return value;
        }

        /// <summary>
        /// Creates a XML element.
        /// </summary>
        static void CreateElement(string node, string value)
        {
            string nodeToAdd = node.Replace("config/", "");
            if (nodeToAdd.Contains("/"))
            {
                CreateSubElement(node, value);
                return;
            }
            XmlElement newElem = xmlConfig.CreateElement(nodeToAdd);
            newElem.InnerText = value;
            if (xmlConfig.DocumentElement != null)
                xmlConfig.DocumentElement.AppendChild(newElem);
        }

        private static void CreateSubElement(string node, string value)
        {
            string lastNode = node.Substring(node.LastIndexOf("/", StringComparison.Ordinal) + 1);
            XmlNode newElement = xmlConfig.CreateElement(lastNode);
            string path = node.Substring(0, node.LastIndexOf("/", StringComparison.Ordinal));
            XmlNodeList list = xmlConfig.SelectNodes(path);
            if (list == null || list.Count <= 0) return;
            var xmlNode = list.Item(0);
            if (xmlNode == null) return;
            xmlNode.AppendChild(newElement);
            newElement.InnerText = value;
        }

        /// <summary>
        /// Loads the config file
        /// </summary>
        public static void LoadConfigs()
        {
            try
            {
                if (!File.Exists(PathToConfigFile))
                {
                    xmlConfig = new XmlDocument {InnerXml = Properties.Resources.config};
                }
                else
                {
                    xmlConfig.Load(PathToConfigFile);
                }
                ParseConfigs();
                isConfigLoaded = true;
                ConfigAfterLoading();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error in the configuration file");
            }
        }

        /// <summary>
        /// Saves the config file
        /// </summary>
        public static void SaveConfigs()
        {
            if (isResetActivated || !isConfigLoaded) return;

            try
            {
                xmlConfig.Save(PathToConfigFile);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Configs");
            }
        }
    }
}
