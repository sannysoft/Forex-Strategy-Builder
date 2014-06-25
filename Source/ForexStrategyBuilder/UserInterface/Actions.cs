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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ForexStrategyBuilder.Common;
using ForexStrategyBuilder.Dialogs.Generator;
using ForexStrategyBuilder.Dialogs.Optimizer;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Properties;
using ForexStrategyBuilder.Utils;
using System.Timers;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Actions : Controls
    /// </summary>
    public sealed partial class Actions : Controls
    {
        private static int splashScreenId;

        /// <summary>
        ///     The default constructor.
        /// </summary>
        private Actions()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            StartPosition = FormStartPosition.CenterScreen;
            Size = GetFormSize();
            MinimumSize = new Size(500, 375);
            Icon = Data.Icon;
            Text = Data.ProgramName;
            splashScreenId = WinApi.GetWindowId(null, "FSB Launcher");

            FormClosing += ActionsFormClosing;
            Application.Idle += ApplicationIdle;

            DataPeriod p;
            String symbol;
            bool changeMarket = ReadCommandLineOptions(out p, out symbol);

            PrepareInstruments();
            LoadCustomIndicators();
            ProvideStrategy();
            Calculate(false);
            CheckUpdate.CheckForUpdate(Data.SystemDir, MiLiveContent, MiForex);
            ShowStartingTips();
            UpdateStatusLabel("- loading user interface...");
            SetStrategyDirWatcher();            

            if (changeMarket && Instruments.InstrumentList.ContainsKey(symbol))
            {
                SetMarket(symbol, p);
                if (LoadInstrument(false) == 0)
                    Calculate(true);
            }

            if (Data.AutostartGenerator)
                ShowGenerator();
        }

        private bool ReadCommandLineOptions(out DataPeriod p, out String symbol)
        {
            p = Data.Period;
            symbol = Data.Symbol;

            bool changeMarket = true;

            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-autostartgenerator"))
                {
                    Data.AutostartGenerator = true;                    
                }
                else if (arg.StartsWith("-autosave"))
                {
                    Data.AutoSave = true;
                }
                else if (arg.StartsWith("-automm"))
                {
                    Data.AutoMM = true;
                }
                else if (arg.StartsWith("-singleorder"))
                {
                    Data.SingleOrder = true;
                }
                else if (arg.StartsWith("-i"))
                {
                    symbol = arg.Substring(2).Trim();
                    changeMarket = true;
                }
                else if (arg.StartsWith("-tf"))
                {
                    try
                    {
                        p = (DataPeriod)Enum.Parse(typeof(DataPeriod), arg.Substring(3).Trim());
                        changeMarket = true;
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (arg.StartsWith("-time"))
                {
                    try
                    {
                        Data.WorkingMinutes = int.Parse(arg.Substring(5).Trim());
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (arg.StartsWith("-mm"))
                {
                    try
                    {
                        Data.MM = decimal.Parse(arg.Substring(3).Trim());
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (arg.StartsWith("-maxdd"))
                {
                    Data.MaxDD = int.Parse(arg.Substring(6).Trim());
                }

            return changeMarket;
        }

        private bool IsDiscardSelectedIndexChange { get; set; }

        /// <summary>
        ///     The starting point of the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Data.Start();
            Instruments.LoadInstruments();
            Configs.LoadConfigs();
            Language.InitLanguages();
            LayoutColors.InitColorSchemes();
            Data.InitMarketStatistic();
            Data.InstrProperties = Instruments.InstrumentList[Data.Strategy.Symbol].Clone();

            Application.Run(new Actions());
        }

        private void PrepareInstruments()
        {
            UpdateStatusLabel("- loading historical data...");
            if (LoadInstrument(false) != 0)
            {
                LoadInstrument(true);
                string message =
                    Language.T(
                        "Forex Strategy Builder cannot load a historical data file and is going to use integrated data!");
                MessageBox.Show(message, Language.T("Data File Loading"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        private void LoadCustomIndicators()
        {
            if (!Configs.LoadCustomIndicators)
            {
                IndicatorManager.CombineAllIndicators();
                return;
            }

            UpdateStatusLabel("- loading custom indicators...");

            try
            {
                CustomIndicators.LoadCustomIndicators();
            }
            catch (Exception e)
            {
                var checker = new DotNetVersionChecker();
                bool isNet35 = checker.IsDonNet35Installed();
                string msg;

                if (isNet35)
                {
                    msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != "")
                        msg += Environment.NewLine + e.InnerException.Message;
                }
                else
                {
                    msg = "FSB cannot compile the custom indicators." + Environment.NewLine +
                          "Please install .NET 3.5 or newer and try again.";
                }

                MessageBox.Show(msg, "Loading Custom Indicators",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            if (Configs.ShowCustomIndicators)
                CustomIndicators.ShowLoadedCustomIndicators();
        }

        private void ProvideStrategy()
        {
            UpdateStatusLabel("- loading strategy...");
            string strategyPath = Data.StrategyPath;

            if (Configs.RememberLastStr && Configs.LastStrategy != "")
            {
                string lastStrategy = Path.GetDirectoryName(Configs.LastStrategy);
                if (lastStrategy != "")
                    lastStrategy = Configs.LastStrategy;
                else
                {
                    string sPath = Path.Combine(Data.UserFilesDir, Data.DefaultStrategyDir);
                    lastStrategy = Path.Combine(sPath, Configs.LastStrategy);
                }
                if (File.Exists(lastStrategy))
                    strategyPath = lastStrategy;
            }

            if (OpenStrategy(strategyPath) == 0)
            {
                AfterStrategyOpening(false);
            }
        }

        private static void ShowStartingTips()
        {
            if (!Configs.ShowStartingTip) return;
            var startingTips = new StartingTips();
            if (startingTips.TipsCount > 0)
                startingTips.Show();
        }

        /// <summary>
        ///     Gets the starting size of the main screen.
        /// </summary>
        private Size GetFormSize()
        {
            int width = Math.Min(Configs.MainScreenWidth, SystemInformation.MaxWindowTrackSize.Width);
            int height = Math.Min(Configs.MainScreenHeight, SystemInformation.MaxWindowTrackSize.Height);

            return new Size(width, height);
        }

        /// <summary>
        ///     Application idle
        /// </summary>
        private void ApplicationIdle(object sender, EventArgs e)
        {
            Application.Idle -= ApplicationIdle;
            WinApi.CloseWindow(splashScreenId);
        }

        /// <summary>
        ///     Updates the splash screen label.
        /// </summary>
        private static void UpdateStatusLabel(string comment)
        {
            WinApi.SendWindowsStringMessage(splashScreenId, 0, comment);
        }

        /// <summary>
        ///     Checks whether the strategy have been saved or not
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            if (Data.AutoSave)
                return;

            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                e.Cancel = true;

            if (e.Cancel) return;

            // Remember the last used strategy
            if (Configs.RememberLastStr)
            {
                if (Data.LoadedSavedStrategy != "")
                {
                    string strategyPath = Path.GetDirectoryName(Data.LoadedSavedStrategy) +
                                          Path.DirectorySeparatorChar;
                    string defaultPath = Path.Combine(Data.UserFilesDir, Data.DefaultStrategyDir);
                    if (strategyPath == defaultPath)
                        Data.LoadedSavedStrategy = Path.GetFileName(Data.LoadedSavedStrategy);
                }
                Configs.LastStrategy = Data.LoadedSavedStrategy;
            }

            WindowState = FormWindowState.Normal;
            Configs.MainScreenWidth = Width;
            Configs.MainScreenHeight = Height;

            Configs.SaveConfigs();
            Instruments.SaveInstruments();
            Data.ProgStats.AppStop();
        }

// ---------------------------------------------------------- //

        /// <summary>
        ///     Edits the Strategy Properties Slot
        /// </summary>
        private void EditStrategyProperties()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());

            var strategyProperties = new StrategyProperties();
            strategyProperties.ShowDialog();

            if (strategyProperties.DialogResult == DialogResult.OK)
            {
                StatsBuffer.UpdateStatsBuffer();

                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                BalanceChart.SetChartData();
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
                HistogramChart.SetChartData();
                HistogramChart.InitChart();
                HistogramChart.Invalidate();
                SetupJournal();
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else
            {
                UndoStrategy();
            }
        }

        /// <summary>
        ///     Edits the Strategy Slot
        /// </summary>
        private void EditSlot(int slot)
        {
            SlotTypes slotType = Data.Strategy.Slot[slot].SlotType;
            bool isSlotExist = Data.Strategy.Slot[slot].IsDefined;
            if (isSlotExist)
                Data.StackStrategy.Push(Data.Strategy.Clone());

            var indicatorDialog = new IndicatorDialog(slot, slotType, isSlotExist);
            indicatorDialog.ShowDialog();

            if (indicatorDialog.DialogResult == DialogResult.OK)
            {
                StatsBuffer.UpdateStatsBuffer();

                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                IndicatorChart.InitChart();
                IndicatorChart.Invalidate();
                RebuildStrategyLayout();
                BalanceChart.SetChartData();
                BalanceChart.InitChart();
                BalanceChart.Invalidate();
                HistogramChart.SetChartData();
                HistogramChart.InitChart();
                HistogramChart.Invalidate();
                SetupJournal();
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else
            {
                // Cancel was pressed
                UndoStrategy();
            }
        }

        /// <summary>
        ///     Moves a Slot Upwards
        /// </summary>
        private void MoveSlotUpwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterUpwards(iSlotToMove);

            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        ///     Moves a Slot Downwards
        /// </summary>
        private void MoveSlotDownwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterDownwards(iSlotToMove);

            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        ///     Duplicates a Slot
        /// </summary>
        private void DuplicateSlot(int slotToDuplicate)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.DuplicateFilter(slotToDuplicate);

            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);
        }

        /// <summary>
        ///     Adds a new Open filter
        /// </summary>
        private void AddOpenFilter()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddOpenFilter();
            EditSlot(Data.Strategy.OpenFilters);
        }

        /// <summary>
        ///     Adds a new Close filter
        /// </summary>
        private void AddCloseFilter()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddCloseFilter();
            EditSlot(Data.Strategy.Slots - 1);
        }

        /// <summary>
        ///     Removes a strategy slot.
        /// </summary>
        /// <param name="slotNumber">Slot to remove</param>
        private void RemoveSlot(int slotNumber)
        {
            Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.RemoveFilter(slotNumber);
            RebuildStrategyLayout();
            Calculate(false);
        }

        /// <summary>
        ///     Undoes the strategy
        /// </summary>
        private void UndoStrategy()
        {
            if (Data.StackStrategy.Count <= 1)
            {
                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
            }

            if (Data.StackStrategy.Count > 0)
            {
                Data.Strategy = Data.StackStrategy.Pop();
                RebuildStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        ///     Performs actions when UPBV has been changed
        /// </summary>
        private void UsePreviousBarValueChange()
        {
            if (MiStrategyAUPBV.Checked == false)
            {
                // Confirmation Message
                string message = Language.T("Are you sure you want to control \"Use previous bar value\" manually?");
                DialogResult dialogResult = MessageBox.Show(message, Language.T("Use previous bar value"),
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // OK, we are sure
                    Data.AutoUsePrvBarValue = false;

                    foreach (IndicatorSlot indicatorSlot in Data.Strategy.Slot)
                        foreach (CheckParam checkParam in indicatorSlot.IndParam.CheckParam)
                            if (checkParam.Caption == "Use previous bar value")
                                checkParam.Enabled = true;
                }
                else
                {
                    // Not just now
                    MiStrategyAUPBV.Checked = true;
                }
            }
            else
            {
                Data.AutoUsePrvBarValue = true;
                Data.Strategy.AdjustUsePreviousBarValue();
                RepaintStrategyLayout();
                Calculate(true);
            }
        }

        /// <summary>
        ///     Ask for saving the changed strategy
        /// </summary>
        private DialogResult WhetherSaveChangedStrategy()
        {
            var dr = DialogResult.No;
            if (Data.IsStrategyChanged)
            {
                string message = Language.T("Do you want to save the current strategy?") + Environment.NewLine +
                                 Data.StrategyName;
                dr = MessageBox.Show(message, Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            return dr;
        }

        /// <summary>
        ///     LoadInstrument
        /// </summary>
        private int LoadInstrument(bool useResource)
        {
            Cursor = Cursors.WaitCursor;

            //  Takes the instrument symbol and period
            string symbol = ComboBoxSymbol.Text;
            var dataPeriod = (DataPeriod) Enum.GetValues(typeof (DataPeriod)).GetValue(ComboBoxPeriod.SelectedIndex);
            InstrumentProperties instrProperties = Instruments.InstrumentList[symbol].Clone();

            //  Makes an instance of class Instrument
            var instrument = new Instrument(instrProperties, (int) dataPeriod)
                {
                    DataDir = Data.OfflineDataDir,
                    MaxBars = Configs.MaxBars,
                    StartTime = Configs.DataStartTime,
                    EndTime = Configs.DataEndTime,
                    UseStartTime = Configs.UseStartTime,
                    UseEndTime = Configs.UseEndTime
                };

            // Loads the data
            int loadDataResult = useResource
                                     ? instrument.LoadResourceData(Resources.EURUSD1440, DataPeriod.D1)
                                     : instrument.LoadData();

            if (instrument.Bars > 0 && loadDataResult == 0)
            {
                Data.InstrProperties = instrProperties.Clone();

                Data.Bars = instrument.Bars;
                Data.Period = dataPeriod;
                Data.Update = instrument.Update;

                Data.Time = new DateTime[Data.Bars];
                Data.Open = new double[Data.Bars];
                Data.High = new double[Data.Bars];
                Data.Low = new double[Data.Bars];
                Data.Close = new double[Data.Bars];
                Data.Volume = new int[Data.Bars];

                for (int bar = 0; bar < Data.Bars; bar++)
                {
                    Data.Open[bar] = instrument.Open(bar);
                    Data.High[bar] = instrument.High(bar);
                    Data.Low[bar] = instrument.Low(bar);
                    Data.Close[bar] = instrument.Close(bar);
                    Data.Time[bar] = instrument.Time(bar);
                    Data.Volume[bar] = instrument.Volume(bar);
                }

                Data.MinPrice = instrument.MinPrice;
                Data.MaxPrice = instrument.MaxPrice;
                Data.DaysOff = instrument.DaysOff;
                Data.AverageGap = instrument.AverageGap;
                Data.MaxGap = instrument.MaxGap;
                Data.AverageHighLow = instrument.AverageHighLow;
                Data.MaxHighLow = instrument.MaxHighLow;
                Data.AverageCloseOpen = instrument.AverageCloseOpen;
                Data.MaxCloseOpen = instrument.MaxCloseOpen;
                Data.DataCut = instrument.Cut;
                Data.IsIntrabarData = false;
                Data.IsTickData = false;
                Data.IsData = true;
                Data.IsResult = false;

                // Configs.SetAccountExchangeRate();

                CheckLoadedData();
                Data.GenerateMarketStats();
                InfoPanelMarketStatistics.Update(Data.MarketStatsParam, Data.MarketStatsValue,
                                                 Data.MarketStatsFlag, Language.T("Market Statistics"));
                InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                                  Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else if (loadDataResult == -1)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(Language.T("Error in the data file!"), Language.T("Data file loading"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                return 1;
            }
            else
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(
                    Language.T("There is no data for") + " " + symbol + " " + Data.DataPeriodToString(dataPeriod) + " " +
                    Language.T("in folder") + " " + Data.OfflineDataDir + Environment.NewLine + Environment.NewLine +
                    Language.T("Check the offline data directory path (Menu Market -> Data Directory)"),
                    Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor = Cursors.Default;
                return 1;
            }

            Cursor = Cursors.Default;

            return 0;
        }

        /// <summary>
        ///     Checks the loaded data
        /// </summary>
        private void CheckLoadedData()
        {
            SetInstrumentDataStatusBar();

            if (!Configs.CheckData)
                return;

            string errorMessage = "";

            // Check for defective data
            int maxConsecutiveBars = 0;
            int maxConsecutiveBar = 0;
            int consecutiveBars = 0;
            int lastBar = 0;
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                if (Math.Abs(Data.Open[bar] - Data.Close[bar]) < Data.InstrProperties.Point)
                {
                    if (lastBar == bar - 1 || lastBar == 0)
                    {
                        consecutiveBars++;
                        lastBar = bar;

                        if (consecutiveBars > maxConsecutiveBars)
                        {
                            maxConsecutiveBars = consecutiveBars;
                            maxConsecutiveBar = bar;
                        }
                    }
                }
                else
                {
                    consecutiveBars = 0;
                }
            }

            if (maxConsecutiveBars > 10 && !Data.AutostartGenerator)
            {
                errorMessage += Language.T("Defective till bar number:") + " " + (maxConsecutiveBar + 1) + " - " +
                                Data.Time[maxConsecutiveBar].ToString(CultureInfo.InvariantCulture) +
                                Environment.NewLine +
                                Language.T("You can try to cut it using \"Data Horizon\".") + Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (Data.Bars < 300 && !Data.AutostartGenerator)
            {
                errorMessage += Language.T("Contains less than 300 bars!") + Environment.NewLine +
                                Language.T("Check your data file or the limits in \"Data Horizon\".");
            }

            if (Data.DaysOff > 5 && Data.Period != DataPeriod.W1 && !Data.AutostartGenerator)
            {
                errorMessage += Language.T("Maximum days off") + " " + Data.DaysOff + Environment.NewLine +
                                Language.T("The data is probably incomplete!") + Environment.NewLine +
                                Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (errorMessage != "" && !Data.AutostartGenerator)
            {
                errorMessage = Language.T("Market") + " " + Data.Symbol + " " + Data.DataPeriodToString(Data.Period) +
                               Environment.NewLine + errorMessage;
                MessageBox.Show(errorMessage, Language.T("Data File Loading"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        ///     Open a strategy file
        /// </summary>
        private void OpenFile()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            var opendlg = new OpenFileDialog
                {
                    InitialDirectory = Data.StrategyDir,
                    Filter = Language.T("Strategy file") + " (*.xml)|*.xml",
                    Title = Language.T("Open Strategy")
                };

            if (opendlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                OpenStrategy(opendlg.FileName);
                AfterStrategyOpening(true);
                Calculate(false);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        ///     New Strategy
        /// </summary>
        private void NewStrategy()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            Data.StrategyDir = Path.Combine(Data.UserFilesDir, Data.DefaultStrategyDir);

            if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
            {
                AfterStrategyOpening(false);
                Calculate(false);
            }
        }

        /// <summary>
        ///     Reloads the Custom Indicators.
        /// </summary>
        private void ReloadCustomIndicators()
        {
            // Check if the strategy contains custom indicators
            bool strategyHasCustomIndicator = false;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                // Searching the strategy slots for a custom indicator
                if (IndicatorManager.CustomIndicatorNames.Contains(slot.IndicatorName))
                {
                    strategyHasCustomIndicator = true;
                    break;
                }
            }

            if (strategyHasCustomIndicator)
            {
                // Save the current strategy
                DialogResult dialogResult = WhetherSaveChangedStrategy();

                if (dialogResult == DialogResult.Yes)
                    SaveStrategy();
                else if (dialogResult == DialogResult.Cancel)
                    return;
            }

            LoadCustomIndicators();

            if (strategyHasCustomIndicator)
            {
                // Load and calculate a new strategy
                Data.StrategyDir = Path.Combine(Data.UserFilesDir, Data.DefaultStrategyDir);

                if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
                {
                    AfterStrategyOpening(false);
                    Calculate(false);
                }
            }
        }

        /// <summary>
        ///     Reads the strategy from a file.
        /// </summary>
        /// <returns>0 - success.</returns>
        private int OpenStrategy(string strategyFilePath)
        {
            try
            {
                if (File.Exists(strategyFilePath) && Strategy.Load(strategyFilePath))
                {
                    // Successfully opening
                    Data.Strategy.StrategyName = Path.GetFileNameWithoutExtension(strategyFilePath);
                    Data.StrategyDir = Path.GetDirectoryName(strategyFilePath);
                    Data.StrategyName = Path.GetFileName(strategyFilePath);
                    if (Data.Strategy.OpenFilters > Configs.MaxEntryFilters)
                        Configs.MaxEntryFilters = Data.Strategy.OpenFilters;
                    if (Data.Strategy.CloseFilters > Configs.MaxExitFilters)
                        Configs.MaxExitFilters = Data.Strategy.CloseFilters;
                }
                else
                {
                    Strategy.GenerateNew();
                    Data.LoadedSavedStrategy = "";
                    Text = Data.ProgramName;
                }

                Data.SetStrategyIndicators();
                RebuildStrategyLayout();

                Text = Data.Strategy.StrategyName + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;

                Data.StackStrategy.Clear();
            }
            catch
            {
                Strategy.GenerateNew();
                string message = Language.T("The strategy could not be loaded correctly!");
                MessageBox.Show(message, Language.T("Strategy Loading"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                Data.LoadedSavedStrategy = "";
                Text = Data.ProgramName;
                RebuildStrategyLayout();
                return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Save the current strategy
        /// </summary>
        private void SaveStrategy()
        {
            if (Data.StrategyName == "New.xml")
            {
                SaveAsStrategy();
            }
            else
            {
                try
                {
                    Data.Strategy.Save(Data.StrategyPath);
                    Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                    Data.ProgStats.FeatureClick(2);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                }
            }
        }

        /// <summary>
        ///     Save the current strategy
        /// </summary>
        private void SaveAsStrategy()
        {
            //Creates a dialog form SaveFileDialog
            var savedlg = new SaveFileDialog
                {
                    InitialDirectory = Data.StrategyDir,
                    FileName = Path.GetFileName(Data.StrategyName),
                    AddExtension = true,
                    Title = Language.T("Save the Strategy As"),
                    Filter = Language.T("Strategy file") + " (*.xml)|*.xml"
                };


            if (savedlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                Data.StrategyName = Path.GetFileName(savedlg.FileName);
                Data.StrategyDir = Path.GetDirectoryName(savedlg.FileName);
                Data.Strategy.Save(savedlg.FileName);
                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;
                Data.ProgStats.FeatureClick(2);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        ///     Calculates the strategy.
        /// </summary>
        /// <param name="recalcIndicators">true - to recalculate all the indicators.</param>
        private void Calculate(bool recalcIndicators)
        {
            bool isUpbvChanged = Data.Strategy.AdjustUsePreviousBarValue();

            if (recalcIndicators)
                foreach (IndicatorSlot indSlot in Data.Strategy.Slot)
                {
                    string indicatorName = indSlot.IndicatorName;
                    SlotTypes slotType = indSlot.SlotType;
                    Indicator indicator = IndicatorManager.ConstructIndicator(indicatorName);
                    indicator.Initialize(slotType);
                    indicator.IndParam = indSlot.IndParam;
                    indicator.Calculate(Data.DataSet);

                    indSlot.IndicatorName = indicator.IndicatorName;
                    indSlot.IndParam = indicator.IndParam;
                    indSlot.Component = indicator.Component;
                    indSlot.SeparatedChart = indicator.SeparatedChart;
                    indSlot.SpecValue = indicator.SpecialValues;
                    indSlot.MinValue = indicator.SeparatedChartMinValue;
                    indSlot.MaxValue = indicator.SeparatedChartMaxValue;
                    indSlot.IsDefined = true;
                }

            // Searches the indicators' components to determine the Data.FirstBar
            Data.FirstBar = Data.Strategy.SetFirstBar();

            // Calculates the backtest
            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            Data.IsResult = true;
            StatsBuffer.UpdateStatsBuffer();

            if (isUpbvChanged) RebuildStrategyLayout();
            IndicatorChart.InitChart();
            IndicatorChart.Invalidate();
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
        }

        /// <summary>
        ///     Sets the market according to the strategy
        /// </summary>
        private void SetMarket(string symbol, DataPeriod dataPeriod)
        {
            Data.InstrProperties = Instruments.InstrumentList[symbol].Clone();
            Data.Period = dataPeriod;

            IsDiscardSelectedIndexChange = true;
            ComboBoxSymbol.SelectedIndex = ComboBoxSymbol.Items.IndexOf(symbol);

            switch (dataPeriod)
            {
                case DataPeriod.M1:
                    ComboBoxPeriod.SelectedIndex = 0;
                    break;
                case DataPeriod.M5:
                    ComboBoxPeriod.SelectedIndex = 1;
                    break;
                case DataPeriod.M15:
                    ComboBoxPeriod.SelectedIndex = 2;
                    break;
                case DataPeriod.M30:
                    ComboBoxPeriod.SelectedIndex = 3;
                    break;
                case DataPeriod.H1:
                    ComboBoxPeriod.SelectedIndex = 4;
                    break;
                case DataPeriod.H4:
                    ComboBoxPeriod.SelectedIndex = 5;
                    break;
                case DataPeriod.D1:
                    ComboBoxPeriod.SelectedIndex = 6;
                    break;
                case DataPeriod.W1:
                    ComboBoxPeriod.SelectedIndex = 7;
                    break;
            }

            IsDiscardSelectedIndexChange = false;
        }

        /// <summary>
        ///     Edit the Trading Charges
        /// </summary>
        private void EditTradingCharges()
        {
            var tradingCharges = new TradingCharges
                {
                    Spread = Data.InstrProperties.Spread,
                    SwapLong = Data.InstrProperties.SwapLong,
                    SwapShort = Data.InstrProperties.SwapShort,
                    Commission = Data.InstrProperties.Commission,
                    Slippage = Data.InstrProperties.Slippage
                };

            tradingCharges.ShowDialog();

            if (tradingCharges.DialogResult == DialogResult.OK)
            {
                Data.InstrProperties.Spread = tradingCharges.Spread;
                Data.InstrProperties.SwapLong = tradingCharges.SwapLong;
                Data.InstrProperties.SwapShort = tradingCharges.SwapShort;
                Data.InstrProperties.Commission = tradingCharges.Commission;
                Data.InstrProperties.Slippage = tradingCharges.Slippage;

                Instruments.InstrumentList[Data.InstrProperties.Symbol] = Data.InstrProperties.Clone();

                Calculate(false);

                SetInstrumentDataStatusBar();
            }
            else if (tradingCharges.EditInstrument)
                ShowInstrumentEditor();
        }

        /// <summary>
        ///     Check the needed market conditions
        /// </summary>
        /// <param name="isMessage">To show the message or not</param>
        private void AfterStrategyOpening(bool isMessage)
        {
            if (Data.Strategy.Symbol != Data.Symbol || Data.Strategy.DataPeriod != Data.Period)
            {
                bool toReload = true;

                if (isMessage)
                {
                    DialogResult result = MessageBox.Show(
                        Language.T("The loaded strategy has been designed for a different market!") +
                        Environment.NewLine + Environment.NewLine +
                        Data.Strategy.Symbol + " " + Data.DataPeriodToString(Data.Strategy.DataPeriod) +
                        Environment.NewLine + Environment.NewLine +
                        Language.T("Do you want to load this market data?"),
                        Data.Strategy.StrategyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    toReload = (result == DialogResult.Yes);
                }

                if (toReload)
                {
                    if (!Instruments.InstrumentList.ContainsKey(Data.Strategy.Symbol))
                    {
                        MessageBox.Show(
                            Language.T("There is no information for this market!") +
                            Environment.NewLine + Environment.NewLine +
                            Data.Strategy.Symbol + " " + Data.DataPeriodToString(Data.Strategy.DataPeriod),
                            Data.Strategy.StrategyName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }

                    string symbol = Data.Symbol;
                    DataPeriod dataPeriod = Data.Period;

                    SetMarket(Data.Strategy.Symbol, Data.Strategy.DataPeriod);

                    if (LoadInstrument(false) == 0)
                    {
                        Calculate(true);
                        PrepareScannerCompactMode();
                    }
                    else
                    {
                        SetMarket(symbol, dataPeriod);
                    }
                }
            }
            else if (!Data.IsIntrabarData)
            {
                PrepareScannerCompactMode();
            }
        }

        /// <summary>
        ///     Load intrabar data by using scanner.
        /// </summary>
        private void PrepareScannerCompactMode()
        {
            if (!Configs.Autoscan || (Data.Period == DataPeriod.M1 && !Configs.UseTickData)) return;
            ComboBoxSymbol.Enabled = false;
            ComboBoxPeriod.Enabled = false;

            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += LoadIntrabarData;
            bgWorker.RunWorkerCompleted += IntrabarDataLoaded;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Starts scanner and loads intrabar data.
        /// </summary>
        private void LoadIntrabarData(object sender, DoWorkEventArgs e)
        {
            var scanner = new Scanner {CompactMode = true};
            scanner.ShowDialog();
        }

        /// <summary>
        ///     The intrabar data is loaded. Refresh the program.
        /// </summary>
        private void IntrabarDataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            Calculate(true);

            ComboBoxSymbol.Enabled = true;
            ComboBoxPeriod.Enabled = true;
        }

        /// <summary>
        ///     Generate BBCode for the forum
        /// </summary>
        private void PublishStrategy()
        {
            var publisher = new StrategyPublish();
            publisher.Show();
        }

        /// <summary>
        ///     Shows the Account Settings dialog.
        /// </summary>
        private void ShowAccountSettings()
        {
            var accountSettings = new AccountSettings
                {
                    AccountCurrency = Configs.AccountCurrency,
                    InitialAccount = Configs.InitialAccount,
                    Leverage = Configs.Leverage,
                    RateToUSD = Data.InstrProperties.RateToUSD,
                    RateToEur = Data.InstrProperties.RateToEUR
                };

            accountSettings.SetParams();

            if (accountSettings.ShowDialog() != DialogResult.OK) return;
            Configs.AccountCurrency = accountSettings.AccountCurrency;
            Configs.InitialAccount = accountSettings.InitialAccount;
            Configs.Leverage = accountSettings.Leverage;
            Data.InstrProperties.RateToUSD = accountSettings.RateToUSD;
            Data.InstrProperties.RateToEUR = accountSettings.RateToEur;

            Instruments.InstrumentList[Data.InstrProperties.Symbol] = Data.InstrProperties.Clone();
            Calculate(false);
        }

        /// <summary>
        ///     Shows Scanner.
        /// </summary>
        private void ShowScanner()
        {
            var scanner = new Scanner();
            scanner.ShowDialog();

            StatsBuffer.UpdateStatsBuffer();

            MiStrategyAutoscan.Checked = Configs.Autoscan;

            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        ///     Perform intrabar scanning.
        /// </summary>
        private void Scan()
        {
            if (!Data.IsIntrabarData)
                ShowScanner();
            else
                Backtester.Scan();

            StatsBuffer.UpdateStatsBuffer();

            InfoPanelAccountStatistics.Update(Backtester.AccountStatsParam,
                                              Backtester.AccountStatsValue,
                                              Backtester.AccountStatsFlags,
                                              Language.T("Account Statistics"));
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            BalanceChart.Invalidate();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            HistogramChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        ///     Starts Generator.
        /// </summary>
        private void ShowGenerator()
        {
            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Data.Strategy.Clone());

            string orginalDescription = Data.Strategy.Description;

            var generator = new Generator {ParrentForm = this};
            generator.ShowDialog();
            Data.ProgStats.FeatureClick(0);

            if (generator.DialogResult == DialogResult.OK)
            {
                // We accept the generated strategy
                Data.StrategyName = Data.Strategy.StrategyName + ".xml";
                Text = Data.Strategy.StrategyName + "* - " + Data.ProgramName;

                if (generator.IsStrategyModified)
                {
                    Data.Strategy.Description = (orginalDescription != string.Empty
                                                     ? orginalDescription + Environment.NewLine + Environment.NewLine +
                                                       "-----------" + Environment.NewLine +
                                                       generator.GeneratedDescription
                                                     : generator.GeneratedDescription);
                }
                else
                {
                    Data.SetStrategyIndicators();
                    Data.Strategy.Description = generator.GeneratedDescription;
                }
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                Calculate(true);
            }
            else
            {
                // When we cancel the Generating, we return the original strategy.
                UndoStrategy();
            }

            //Automatically close program
            if (Data.AutoSave) {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Elapsed += new ElapsedEventHandler(OnCloseTimer);
                timer.Interval = 1000;
                timer.AutoReset = true;
                timer.Start();
            }        
        }

        //Closes app for AutoSave param
        void OnCloseTimer(Object sender, ElapsedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        ///     Starts the generator
        /// </summary>
        private void ShowOverview()
        {
            var browser = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHtmlOverview());
            browser.Show();
        }

        /// <summary>
        ///     Call the Optimizer
        /// </summary>
        private void ShowOptimizer()
        {
            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Data.Strategy.Clone());

            var optimizer = new Optimizer {SetParrentForm = this};
            optimizer.ShowDialog();
            Data.ProgStats.FeatureClick(1);

            if (optimizer.DialogResult == DialogResult.OK)
            {
                // We accept the optimized strategy
                Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RepaintStrategyLayout();
                Calculate(true);
            }
            else
            {
                // If we cancel the optimizing, we return the original strategy.
                UndoStrategy();
            }
        }

        /// <summary>
        ///     Show the method Comparator
        /// </summary>
        private void ShowComparator()
        {
            // Save the original method to return it later
            InterpolationMethod interpMethodOriginal = Backtester.InterpolationMethod;

            var comparator = new Comparator();
            comparator.ShowDialog();

            // Returns the original method
            Backtester.InterpolationMethod = interpMethodOriginal;
            Calculate(false);
        }

        /// <summary>
        ///     Shows the Bar Explorer tool.
        /// </summary>
        private void ShowBarExplorer()
        {
            var barExplorer = new BarExplorer(Data.FirstBar);
            barExplorer.ShowDialog();
        }

        /// <summary>
        ///     Sets the data starting parameters.
        /// </summary>
        private void DataHorizon()
        {
            var horizon = new DataHorizon(Configs.MaxBars, Configs.DataStartTime, Configs.DataEndTime,
                                          Configs.UseStartTime, Configs.UseEndTime);
            horizon.ShowDialog();

            if (horizon.DialogResult != DialogResult.OK) return;
            Configs.MaxBars = horizon.MaxBars;
            Configs.DataStartTime = horizon.StartTime;
            Configs.DataEndTime = horizon.EndTime;
            Configs.UseStartTime = horizon.UseStartTime;
            Configs.UseEndTime = horizon.UseEndTime;

            if (LoadInstrument(false) != 0) return;
            Calculate(true);
            PrepareScannerCompactMode();
        }

        /// <summary>
        ///     Shows the Instrument Editor dialog.
        /// </summary>
        private void ShowInstrumentEditor()
        {
            var instrEditor = new InstrumentEditor();
            instrEditor.ShowDialog();

            if (instrEditor.NeedReset)
            {
                IsDiscardSelectedIndexChange = true;

                ComboBoxSymbol.Items.Clear();
                foreach (string symbol in Instruments.SymbolList)
                    ComboBoxSymbol.Items.Add(symbol);
                ComboBoxSymbol.SelectedIndex = ComboBoxSymbol.Items.IndexOf(Data.Symbol);

                IsDiscardSelectedIndexChange = false;
            }

            Data.InstrProperties = Instruments.InstrumentList[Data.InstrProperties.Symbol].Clone();
            SetInstrumentDataStatusBar();
            Calculate(false);
        }

        /// <summary>
        ///     Loads a color scheme.
        /// </summary>
        private void LoadColorScheme()
        {
            string colorFile = Path.Combine(Data.ColorDir, Configs.ColorScheme + ".xml");

            if (!File.Exists(colorFile)) return;
            LayoutColors.LoadColorScheme(colorFile);

            PanelWorkspace.BackColor = LayoutColors.ColorFormBack;
            RepaintStrategyLayout();
            InfoPanelAccountStatistics.SetColors();
            InfoPanelMarketStatistics.SetColors();
            IndicatorChart.InitChart();
            BalanceChart.SetChartData();
            BalanceChart.InitChart();
            HistogramChart.SetChartData();
            HistogramChart.InitChart();
            SetupJournal();
            PanelWorkspace.Invalidate(true);
        }

        /// <summary>
        ///     Sets the Status Bar Data Label
        /// </summary>
        private void SetInstrumentDataStatusBar()
        {
            string swapUnit = "p";
            if (Data.InstrProperties.SwapUnit == ChargeUnit.Money)
                swapUnit = "m";
            else if (Data.InstrProperties.SwapUnit == ChargeUnit.Percents)
                swapUnit = "%";

            string commUnit = "p";
            if (Data.InstrProperties.CommissionUnit == ChargeUnit.Money)
                commUnit = "m";
            else if (Data.InstrProperties.CommissionUnit == ChargeUnit.Percents)
                commUnit = "%";

            StatusLabelInstrument =
                Data.Symbol + " " +
                Data.PeriodString + " (" +
                Data.InstrProperties.Spread + ", " +
                Data.InstrProperties.SwapLong.ToString("F2") + swapUnit + ", " +
                Data.InstrProperties.SwapShort.ToString("F2") + swapUnit + ", " +
                Data.InstrProperties.Commission.ToString("F2") + commUnit + ", " +
                Data.InstrProperties.Slippage + ")" +
                (Data.DataCut ? " - " + Language.T("Cut") : "") +
                (Configs.FillInDataGaps ? " - " + Language.T("No Gaps") : "") +
                (Configs.CheckData ? "" : " - " + Language.T("Unchecked"));
        }
    }
}