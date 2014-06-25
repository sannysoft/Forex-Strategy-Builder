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
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Windows.Forms;
using ForexStrategyBuilder.CustomAnalytics;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder.Dialogs.Generator
{
    /// <summary>
    ///     Strategy Generator
    /// </summary>
    public sealed partial class Generator
    {
        private readonly List<string> entryFilterIndicators = new List<string>();
        private readonly List<string> entryIndicators = new List<string>();
        private readonly List<string> exitFilterIndicators = new List<string>();
        private readonly List<string> exitIndicators = new List<string>();
        private readonly List<string> exitIndicatorsWithFilters = new List<string>();
        private readonly List<string> indicatorBlackList;
        private IndicatorSlot[] aLockedEntryFilter; // Holds all locked entry filters.
        private IndicatorSlot[] aLockedExitFilter; // Holds all locked exit filters.
        private int barOOS = Data.Bars - 1;
        private double benchmark;
        private float bestValue;
        private TimeSpan currentBenchmarkTime;
        private CustomGeneratorAnalytics customAnalytics;
        private bool customSortingAdvancedEnabled;
        private string customSortingOptionDisplay = String.Empty;
        private bool customSortingSimpleEnabled;
        private int cycles;
        private bool isEntryLocked; // Shows if the entry logic is locked
        private bool isExitLocked; // Shows if the exit logic is locked
        private bool isGenerating;
        private bool isOOS;
        private bool isStartegyChanged;
        private int lockedEntryFilters;
        private IndicatorSlot lockedEntrySlot; // Holds a locked entry slot.
        private int lockedExitFilters;
        private IndicatorSlot lockedExitSlot; // Holds a locked exit slot.
        private int maxClosingLogicSlots;
        private int maxOpeningLogicSlots;
        private int minutes;
        private int progressPercent;
        private Strategy strategyBest;
        private float targetBalanceRatio = 1;

        private long totalCalculations;
        private TimeSpan totalWorkTime;

        /// <summary>
        ///     BtnGenerate_Click
        /// </summary>
        private void BtnGenerateClick(object sender, EventArgs e)
        {
            if (isGenerating)
            {
                // Cancel the asynchronous operation
                bgWorker.CancelAsync();
            }
            else
            {
                // Setup the Custom Sorting Options
                if (rbnCustomSortingSimple.Checked || rbnCustomSortingAdvanced.Checked)
                {
                    customAnalytics.SimpleSortOption = cbxCustomSortingSimple.Text;
                    customAnalytics.AdvancedSortOption = cbxCustomSortingAdvanced.Text;
                    customAnalytics.AdvancedSortOptionCompareTo = cbxCustomSortingAdvancedCompareTo.Text;
                    customAnalytics.PathToConfigFile = Configs.PathToConfigFile;
                    customAnalytics.Template = StrategyXML.CreateStrategyXmlDoc(Data.Strategy);

                    // Provide full bar data to the analytics assembly if requested
                    if (CustomAnalytics.Generator.IsFullBarDataNeeded)
                    {
                        var bars = new List<CustomAnalytics.Bar>();
                        for (int i = 0; i <= Data.Bars - 1; i++)
                        {
                            var bar = new CustomAnalytics.Bar
                                {
                                    Time = Data.Time[i],
                                    Open = Data.Open[i],
                                    High = Data.High[i],
                                    Low = Data.Low[i],
                                    Close = Data.Close[i],
                                    Volume = Data.Volume[i]
                                };
                            bars.Add(bar);
                        }
                        customAnalytics.Bars = bars;
                    }
                }

                // Start the bgWorker
                PrepareStrategyForGenerating();
                CheckForLockedSlots();
                PrepareIndicatorLists();
                bool isEnoughIndicators = CheckAvailableIndicators();

                if (isEntryLocked && isExitLocked || !isEnoughIndicators)
                {
                    SystemSounds.Hand.Play();
                    return;
                }

                Cursor = Cursors.WaitCursor;

                minutes = chbWorkingMinutes.Checked ? (int) nudWorkingMinutes.Value : int.MaxValue;
                progressBar.Style = chbWorkingMinutes.Checked ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;

                GeneratedDescription = String.Empty;

                foreach (Control control in pnlCommon.Controls)
                    control.Enabled = false;
                foreach (Control control in criteriaControls.Controls)
                    control.Enabled = false;
                foreach (Control control in pnlSettings.Controls)
                    control.Enabled = false;
                foreach (Control control in pnlSorting.Controls)
                    control.Enabled = false;

                indicatorsField.BlockIndicatorChange();

                tsbtLockAll.Enabled = false;
                tsbtUnlockAll.Enabled = false;
                tsbtLinkAll.Enabled = false;
                tsbtOverview.Enabled = false;
                tsbtStrategyInfo.Enabled = false;

                lblCalcStrInfo.Enabled = true;
                lblCalcStrNumb.Enabled = true;
                lblBenchmarkInfo.Enabled = true;
                lblBenchmarkNumb.Enabled = true;
                chbHideFsb.Enabled = true;

                btnAccept.Enabled = false;
                btnCancel.Enabled = false;
                btnGenerate.Text = Language.T("Stop");

                isGenerating = true;

                progressBar.Value = 1;
                progressPercent = 0;
                cycles = 0;

                if (chbGenerateNewStrategy.Checked)
                    top10Field.ClearTop10Slots();

                criteriaControls.OOSTesting = chbOutOfSample.Checked;
                criteriaControls.BarOOS = (int) nudOutOfSample.Value;
                criteriaControls.TargetBalanceRatio = targetBalanceRatio;

                bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event
            var worker = sender as BackgroundWorker;

            // Generate a strategy
            Generating(worker, e);
        }

        /// <summary>
        ///     This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            RestoreFromBest();

            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            balanceChart.SetChartData();
            balanceChart.InitChart();
            balanceChart.Invalidate();

            strategyField.Enabled = true;
            RebuildStrategyLayout(strategyBest);

            isGenerating = false;

            btnAccept.Enabled = true;
            btnCancel.Enabled = true;

            foreach (Control control in pnlCommon.Controls)
                control.Enabled = true;
            foreach (Control control in criteriaControls.Controls)
                control.Enabled = true;
            foreach (Control control in pnlSettings.Controls)
                control.Enabled = true;
            foreach (Control control in pnlSorting.Controls)
                control.Enabled = true;

            indicatorsField.UnBlockIndicatorChange();

            tsbtLockAll.Enabled = true;
            tsbtUnlockAll.Enabled = true;
            tsbtLinkAll.Enabled = true;
            tsbtOverview.Enabled = true;
            tsbtStrategyInfo.Enabled = true;

            SetCustomSortingUI();

            btnGenerate.Text = Language.T("Generate");
            progressBar.Style = ProgressBarStyle.Blocks;

            Cursor = Cursors.Default;

            if (Data.AutoSave)
            {
                //Get results
                double profit = Backtester.NetMoneyBalance - Configs.InitialAccount;

                if (profit > 0)
                {
                    String drawdown = Backtester.MoneyEquityPercentDrawdown.ToString("F2");
                    String filename = Data.Symbol + "_" + Data.PeriodString + "_" + (new Random().Next(0, 1000)) + ".xml";

                    //Create dir
                    string subPath = "Strategies";
                    if (profit > 400000)
                        subPath = "GoodStrategies";
                    System.IO.Directory.CreateDirectory(subPath);

                    //Save result to CSV
                    System.IO.StreamWriter file = new System.IO.StreamWriter(subPath + "/results.csv", true);
                    file.WriteLine(Data.Symbol + ";" + Data.PeriodString + ";" + Data.Strategy.EntryLots + ";" + filename + ";" + profit.ToString("F2") + ";" + drawdown);
                    file.Close();

                    //Autosave strategy                
                    strategyBest.Save(subPath + "/" + filename);
                }

                //Exit
                btnAccept.PerformClick();
            }

        }

        /// <summary>
        ///     Prepare the strategy for generating
        /// </summary>
        private void PrepareStrategyForGenerating()
        {
            lockedEntrySlot = null;
            lockedEntryFilters = 0;
            aLockedEntryFilter = new IndicatorSlot[Math.Max(Strategy.MaxOpenFilters, strategyBest.OpenFilters)];
            lockedExitSlot = null;
            lockedExitFilters = 0;
            aLockedExitFilter = new IndicatorSlot[Math.Max(Strategy.MaxCloseFilters, strategyBest.CloseFilters)];

            // Copy the locked slots
            for (int slot = 0; slot < strategyBest.Slots; slot++)
            {
                if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Locked ||
                    strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                {
                    if (strategyBest.Slot[slot].SlotType == SlotTypes.Open)
                        lockedEntrySlot = strategyBest.Slot[slot];
                    else if (strategyBest.Slot[slot].SlotType == SlotTypes.OpenFilter)
                    {
                        aLockedEntryFilter[lockedEntryFilters] = strategyBest.Slot[slot];
                        lockedEntryFilters++;
                    }
                    else if (strategyBest.Slot[slot].SlotType == SlotTypes.Close)
                        lockedExitSlot = strategyBest.Slot[slot];
                    else if (strategyBest.Slot[slot].SlotType == SlotTypes.CloseFilter)
                    {
                        aLockedExitFilter[lockedExitFilters] = strategyBest.Slot[slot];
                        lockedExitFilters++;
                    }
                }
            }

            if (chbGenerateNewStrategy.Checked)
                bestValue = 0;
            else if (rbnCustomSortingNone.Checked)
                bestValue = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);
            else
                bestValue = float.MinValue;

            maxOpeningLogicSlots = chbMaxOpeningLogicSlots.Checked
                                       ? (int) nudMaxOpeningLogicSlots.Value
                                       : Strategy.MaxOpenFilters;
            maxClosingLogicSlots = chbMaxClosingLogicSlots.Checked
                                       ? (int) nudMaxClosingLogicSlots.Value
                                       : Strategy.MaxCloseFilters;
        }

        /// <summary>
        ///     Check if all slots are locked.
        /// </summary>
        private void CheckForLockedSlots()
        {
            isEntryLocked = false;
            isExitLocked = false;

            if (lockedEntrySlot != null && lockedEntryFilters >= maxOpeningLogicSlots)
                isEntryLocked = true;

            if (lockedEntryFilters > maxOpeningLogicSlots)
                maxOpeningLogicSlots = lockedEntryFilters;

            if (lockedExitSlot != null &&
                !IndicatorManager.ClosingIndicatorsWithClosingFilters.Contains(lockedExitSlot.IndicatorName))
                isExitLocked = true;
            else if (lockedExitSlot != null &&
                     IndicatorManager.ClosingIndicatorsWithClosingFilters.Contains(lockedExitSlot.IndicatorName) &&
                     lockedExitFilters >= maxClosingLogicSlots)
                isExitLocked = true;
            else if (lockedExitSlot == null && lockedExitFilters > 0 && lockedExitFilters >= maxClosingLogicSlots)
                isExitLocked = true;

            if (lockedExitFilters > maxClosingLogicSlots)
                maxClosingLogicSlots = lockedExitFilters;

            for (int slot = 0; slot < strategyBest.Slots; slot++)
            {
                if (strategyBest.Slot[slot].SlotStatus != StrategySlotStatus.Linked) continue;
                if (strategyBest.Slot[slot].SlotType == SlotTypes.Open)
                    isEntryLocked = isEntryLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isEntryLocked;
                else if (strategyBest.Slot[slot].SlotType == SlotTypes.OpenFilter)
                    isEntryLocked = isEntryLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isEntryLocked;
                else if (strategyBest.Slot[slot].SlotType == SlotTypes.Close)
                    isExitLocked = isExitLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isExitLocked;
                else if (strategyBest.Slot[slot].SlotType == SlotTypes.CloseFilter)
                    isExitLocked = isExitLocked ? !IsSlotHasParameters(strategyBest.Slot[slot]) : isExitLocked;
            }
        }

        /// <summary>
        ///     Shows if the slot has any parameters to generate.
        /// </summary>
        private bool IsSlotHasParameters(IndicatorSlot slot)
        {
            foreach (ListParam listParam in slot.IndParam.ListParam)
                if (listParam.Enabled && listParam.ItemList.Length > 1)
                    return true;
            foreach (NumericParam numericParam in slot.IndParam.NumParam)
                if (numericParam.Enabled)
                    return true;

            return false;
        }

        /// <summary>
        ///     Prepare available indicators for each slot.
        /// </summary>
        private void PrepareIndicatorLists()
        {
            // Clear lists
            entryIndicators.Clear();
            entryFilterIndicators.Clear();
            exitIndicators.Clear();
            exitIndicatorsWithFilters.Clear();
            exitFilterIndicators.Clear();

            // Copy all no banned indicators
            foreach (string indicatorName in IndicatorManager.OpenPointIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.Open, indicatorName))
                    entryIndicators.Add(indicatorName);
            foreach (string indicatorName in IndicatorManager.OpenFilterIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.OpenFilter, indicatorName))
                    entryFilterIndicators.Add(indicatorName);
            foreach (string indicatorName in IndicatorManager.ClosePointIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.Close, indicatorName))
                    exitIndicators.Add(indicatorName);
            foreach (string indicatorName in IndicatorManager.ClosingIndicatorsWithClosingFilters)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.Close, indicatorName))
                    exitIndicatorsWithFilters.Add(indicatorName);
            foreach (string indicatorName in IndicatorManager.CloseFilterIndicators)
                if (!indicatorsField.IsIndicatorBanned(SlotTypes.CloseFilter, indicatorName))
                    exitFilterIndicators.Add(indicatorName);

            // Remove not generatable indicators
            foreach (string indicatorName in IndicatorManager.AllIndicatorsNames)
            {
                var indicator = IndicatorManager.ConstructIndicator(indicatorName);
                indicator.Initialize(SlotTypes.Open);
                if (!indicator.IsGeneratable && entryIndicators.Contains(indicatorName))
                    entryIndicators.Remove(indicatorName);
                indicator.Initialize(SlotTypes.OpenFilter);
                if (!indicator.IsGeneratable && entryFilterIndicators.Contains(indicatorName))
                    entryFilterIndicators.Remove(indicatorName);
                indicator.Initialize(SlotTypes.Close);
                if (!indicator.IsGeneratable && exitIndicators.Contains(indicatorName))
                    exitIndicators.Remove(indicatorName);
                if (!indicator.IsGeneratable && exitIndicatorsWithFilters.Contains(indicatorName))
                    exitIndicatorsWithFilters.Remove(indicatorName);
                indicator.Initialize(SlotTypes.CloseFilter);
                if (!indicator.IsGeneratable && exitFilterIndicators.Contains(indicatorName))
                    exitFilterIndicators.Remove(indicatorName);
            }

            // Remove special cases
            bool isPeriodDayOrWeek = Data.Period == DataPeriod.D1 || Data.Period == DataPeriod.W1;

            if (entryIndicators.Contains("Day Opening") && isPeriodDayOrWeek)
                entryIndicators.Remove("Day Opening");
            if (entryIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                entryIndicators.Remove("Hourly High Low");
            if (entryIndicators.Contains("Entry Hour") && isPeriodDayOrWeek)
                entryIndicators.Remove("Entry Hour");

            if (entryFilterIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                entryFilterIndicators.Remove("Hourly High Low");

            if (exitIndicators.Contains("Day Closing") && isPeriodDayOrWeek)
                exitIndicators.Remove("Day Closing");
            if (exitIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                exitIndicators.Remove("Hourly High Low");
            if (exitIndicators.Contains("Exit Hour") && isPeriodDayOrWeek)
                exitIndicators.Remove("Exit Hour");
            if (exitIndicators.Contains("Close and Reverse") &&
                strategyBest.OppSignalAction != OppositeDirSignalAction.Reverse &&
                strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                exitIndicators.Remove("Close and Reverse");

            if (exitIndicatorsWithFilters.Contains("Day Closing") && isPeriodDayOrWeek)
                exitIndicatorsWithFilters.Remove("Day Closing");
            if (exitIndicatorsWithFilters.Contains("Close and Reverse") &&
                strategyBest.OppSignalAction != OppositeDirSignalAction.Reverse &&
                strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                exitIndicatorsWithFilters.Remove("Close and Reverse");

            if (exitFilterIndicators.Contains("Hourly High Low") && isPeriodDayOrWeek)
                exitFilterIndicators.Remove("Hourly High Low");
        }

        /// <summary>
        ///     Checks if enough indicators are allowed
        /// </summary>
        private bool CheckAvailableIndicators()
        {
            if (!isEntryLocked && entryIndicators.Count == 0)
                return false;
            if (entryFilterIndicators.Count < maxOpeningLogicSlots - lockedEntryFilters)
                return false;
            if (!isExitLocked && exitIndicators.Count == 0)
                return false;
            if (!isExitLocked && exitIndicatorsWithFilters.Count == 0 && chbMaxClosingLogicSlots.Enabled &&
                nudMaxClosingLogicSlots.Value > 0)
                return false;
            if (lockedExitFilters > 0 && exitIndicatorsWithFilters.Count == 0)
                return false;
            if (chbMaxClosingLogicSlots.Enabled &&
                exitFilterIndicators.Count < nudMaxClosingLogicSlots.Value - lockedExitFilters)
                return false;
            if (!chbMaxClosingLogicSlots.Enabled &&
                exitFilterIndicators.Count < maxClosingLogicSlots - lockedExitFilters)
                return false;

            return true;
        }

        /// <summary>
        ///     Generates a strategy
        /// </summary>
        private void Generating(BackgroundWorker worker, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            var workTime = new TimeSpan(0, minutes, 0);
            DateTime stopTime = startTime + workTime;

            currentBenchmarkTime = totalWorkTime;

            bool isStopGenerating = false;
            do
            {
                // The generating cycle
                if (worker.CancellationPending)
                {
                    // The Generating was stopped by the user
                    e.Cancel = true;
                    isStopGenerating = true;
                }
                else if (minutes > 0 && stopTime < DateTime.Now)
                {
                    // The time finished
                    isStopGenerating = true;
                }
                else
                {
                    // The main job
                    GenerateStrategySlots();
                    GenerateSameOppSignal();
                    GeneratePermanentSL();
                    GeneratePermanentTP();
                    GenerateBreakEven();
                    GenerateMartingale();

                    // Calculates the back test.
                    bool isBetter = CalculateTheResult(false);

                    // Try to change lits
                    if (Backtester.NetMoneyBalance > 0 && Data.AutoMM)
                    {
                        bool isGood = false;
                        Strategy beforeOptimization = Data.Strategy.Clone();
                        for (double i = 0.5; i <= 10; i += 0.5)
                        {
                            //Try different MM
                            Data.Strategy.EntryLots = i;
                            isGood = CalculateTheResult(false) || isGood;
                            isBetter = isBetter && isGood;
                        }

                        if (!isGood)
                            Data.Strategy = beforeOptimization.Clone();
                    }

                    // Initial Optimization
                    if (chbInitialOptimization.Checked)
                        PerformInitialOptimization(worker, isBetter);

                    totalWorkTime = currentBenchmarkTime.Add(DateTime.Now - startTime);
                    benchmark = 0.0001*Data.Bars*totalCalculations/totalWorkTime.TotalSeconds;
                    SetBenchmarkText(benchmark);
                }


                if (minutes > 0)
                {
                    // Report progress as a percentage of the total task.
                    TimeSpan passedTime = DateTime.Now - startTime;
                    var percentComplete = (int) (100*passedTime.TotalSeconds/workTime.TotalSeconds);
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > progressPercent)
                    {
                        progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            } while (!isStopGenerating);
        }

        /// <summary>
        ///     Calculates the generated result
        /// </summary>
        private bool CalculateTheResult(bool isSaveEqualResult)
        {
            bool isBetter = false;
            cycles++;
            totalCalculations++;

            Data.FirstBar = Data.Strategy.SetFirstBar();
            Data.Strategy.AdjustUsePreviousBarValue();

            // Sets default logical group for all slots that are open (not locked or linked).
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
                if (slot.SlotStatus == StrategySlotStatus.Open)
                    slot.LogicalGroup = Data.Strategy.GetDefaultGroup(slot.SlotNumber);

#if !DEBUG
            try
            {
#endif
                Backtester.Calculate();
                Backtester.CalculateAccountStats();

                float value = 0;
                customSortingOptionDisplay = String.Empty;
                const double epsilon = 0.000001;

                bool isCriteriaFulfilled = criteriaControls.IsCriteriaFulfilled();
                bool isBalanceOk = Backtester.NetBalance > 0;

                if (isCriteriaFulfilled && isBalanceOk)
                {
                    if (rbnCustomSortingNone.Checked)
                        value = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);
                    else if (rbnCustomSortingSimple.Checked)
                        GetSimpleCustomSortingValue(out value, out customSortingOptionDisplay);
                    else if (rbnCustomSortingAdvanced.Checked)
                        GetAdvancedCustomSortingValue(out value, out customSortingOptionDisplay);

                    if (bestValue < value ||
                        (Math.Abs(bestValue - value) < epsilon &&
                         (isSaveEqualResult || Data.Strategy.Slots < strategyBest.Slots)))
                    {
                        strategyBest = Data.Strategy.Clone();
                        strategyBest.PropertiesStatus = Data.Strategy.PropertiesStatus;
                        for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                            strategyBest.Slot[slot].SlotStatus = Data.Strategy.Slot[slot].SlotStatus;

                        string description = GenerateDescription();
                        if (value > bestValue)
                            AddStrategyToGeneratorHistory(description);
                        else
                            UpdateStrategyInGeneratorHistory(description);
                        SetStrategyDescriptionButton();

                        bestValue = value;
                        isBetter = true;
                        isStartegyChanged = true;

                        RefreshSmallBalanceChart();
                        RefreshAccountStatistics();
                        RebuildStrategyLayout(strategyBest);
                        Top10AddStrategy();
                    }
                    else if (top10Field.IsNominated(value))
                    {
                        Top10AddStrategy();
                    }
                    else
                        customAnalytics.CriterionFailsNomination++;
                }

                SetLabelCyclesText(cycles.ToString(CultureInfo.InvariantCulture));
#if !DEBUG
            }
            catch (Exception exception)
            {
                string text = GenerateCalculationErrorMessage(exception.Message);
                const string caption = "Strategy Calculation Error";
                ReportIndicatorError(text, caption);

                isBetter = false;
            }
#endif

            return isBetter;
        }

        /// <summary>
        ///     Calculates an indicator and returns OK status.
        /// </summary>
        private bool CalculateIndicator(SlotTypes slotType, Indicator indicator)
        {
#if !DEBUG
            try
            {
#endif
                indicator.Calculate(Data.DataSet);
                totalCalculations++;
                return true;
#if !DEBUG
            }
            catch (Exception exception)
            {
                string message = "Please report this error in the support forum!";
                if (indicator.CustomIndicator)
                    message = "Please report this error to the author of the indicator!<br />" +
                              "You may remove this indicator from the Custom Indicators folder.";

                string text =
                    "<h1>Error: " + exception.Message + "</h1>" +
                    "<p>" +
                    "Slot type: <strong>" + slotType + "</strong><br />" +
                    "Indicator: <strong>" + indicator + "</strong>" +
                    "</p>" +
                    "<p>" +
                    message +
                    "</p>";

                const string caption = "Indicator Calculation Error";
                ReportIndicatorError(text, caption);
                indicatorBlackList.Add(indicator.IndicatorName);
                return false;
            }
#endif
        }

        /// <summary>
        ///     Restores the strategy from the best one
        /// </summary>
        private void RestoreFromBest()
        {
            Data.Strategy = strategyBest.Clone();
            Data.Strategy.PropertiesStatus = strategyBest.PropertiesStatus;
            for (int slot = 0; slot < strategyBest.Slots; slot++)
                Data.Strategy.Slot[slot].SlotStatus = strategyBest.Slot[slot].SlotStatus;

            RecalculateSlots();
        }

        private void GenerateStrategySlots()
        {
            // Determines the number of slots
            int openFilters = random.Next(lockedEntryFilters, maxOpeningLogicSlots + 1);

            int closeFilters = 0;
            if (lockedExitSlot == null ||
                exitIndicatorsWithFilters.Contains(Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName))
                closeFilters = random.Next(lockedExitFilters, maxClosingLogicSlots + 1);

            // Create a strategy
            Data.Strategy = new Strategy(openFilters, closeFilters)
                {
                    StrategyName = "Generated",
                    UseAccountPercentEntry = strategyBest.UseAccountPercentEntry,
                    MaxOpenLots = strategyBest.MaxOpenLots,
                    EntryLots = Data.MM>0 ? Convert.ToDouble(Data.MM) : strategyBest.EntryLots,
                    AddingLots = Data.MM>0 ? Convert.ToDouble(Data.MM) : strategyBest.AddingLots,
                    ReducingLots = Data.MM > 0 ? Convert.ToDouble(Data.MM) : strategyBest.ReducingLots
                };

            // Entry Slot
            int slot = 0;
            if (lockedEntrySlot != null)
            {
                Data.Strategy.Slot[slot] = lockedEntrySlot.Clone();
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            else
            {
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Entry filter slots
            for (int i = 0; i < lockedEntryFilters; i++)
            {
                slot++;
                Data.Strategy.Slot[slot] = aLockedEntryFilter[i].Clone();
                Data.Strategy.Slot[slot].SlotNumber = slot;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            for (int i = lockedEntryFilters; i < openFilters; i++)
            {
                slot++;
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Exit slot
            if (lockedExitSlot != null)
            {
                slot++;
                Data.Strategy.Slot[slot] = lockedExitSlot.Clone();
                Data.Strategy.Slot[slot].SlotNumber = slot;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    GenerateIndicatorParameters(slot);
            }
            else
            {
                slot++;
                GenerateIndicatorName(slot);
                GenerateIndicatorParameters(slot);
            }

            // Exit filter slots
            if (
                IndicatorManager.ClosingIndicatorsWithClosingFilters.Contains(
                    Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName) && closeFilters > 0)
            {
                for (int i = 0; i < lockedExitFilters; i++)
                {
                    slot++;
                    Data.Strategy.Slot[slot] = aLockedExitFilter[i].Clone();
                    Data.Strategy.Slot[slot].SlotNumber = slot;
                    if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                        GenerateIndicatorParameters(slot);
                }
                for (int i = lockedExitFilters; i < closeFilters; i++)
                {
                    slot++;
                    GenerateIndicatorName(slot);
                    GenerateIndicatorParameters(slot);
                }
            }
        }

        private void GenerateIndicatorName(int slot)
        {
            SlotTypes slotType = Data.Strategy.GetSlotType(slot);
            string indicatorName;

            switch (slotType)
            {
                case SlotTypes.Open:
                    do
                    {
                        indicatorName = entryIndicators[random.Next(entryIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.OpenFilter:
                    do
                    {
                        indicatorName = entryFilterIndicators[random.Next(entryFilterIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.Close:
                    do
                    {
                        indicatorName = Data.Strategy.CloseFilters > 0
                                            ? exitIndicatorsWithFilters[random.Next(exitIndicatorsWithFilters.Count)]
                                            : exitIndicators[random.Next(exitIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                case SlotTypes.CloseFilter:
                    do
                    {
                        indicatorName = exitFilterIndicators[random.Next(exitFilterIndicators.Count)];
                    } while (indicatorBlackList.Contains(indicatorName));
                    break;
                default:
                    indicatorName = "Error!";
                    break;
            }

            Data.Strategy.Slot[slot].IndicatorName = indicatorName;
        }

        private void GenerateIndicatorParameters(int slot)
        {
            string indicatorName = Data.Strategy.Slot[slot].IndicatorName;
            SlotTypes slotType = Data.Strategy.GetSlotType(slot);
            Indicator indicator = IndicatorManager.ConstructIndicator(indicatorName);
            indicator.Initialize(slotType);

            // List parameters
            foreach (ListParam list in indicator.IndParam.ListParam)
                if (list.Enabled)
                {
                    do
                    {
                        list.Index = random.Next(list.ItemList.Length);
                        list.Text = list.ItemList[list.Index];
                    } while (list.Caption == "Base price" && (list.Text == "High" || list.Text == "Low"));
                }

            int firstBar;
            do
            {
                // Numeric parameters
                foreach (NumericParam num in indicator.IndParam.NumParam)
                    if (num.Enabled)
                    {
                        if (num.Caption == "Level" && !indicator.IndParam.ListParam[0].Text.Contains("Level"))
                            continue;
                        if (!chbUseDefaultIndicatorValues.Checked)
                        {
                            double step = Math.Pow(10, -num.Point);
                            double minimum = num.Min;
                            double maximum = num.Max;

                            if (maximum > Data.Bars/3.0 && ((num.Caption.ToLower()).Contains("period") ||
                                                            (num.Caption.ToLower()).Contains("shift") ||
                                                            (num.ToolTip.ToLower()).Contains("period")))
                            {
                                maximum = Math.Max(minimum + step, Data.Bars/3.0);
                            }

                            double value = minimum + step*random.Next((int) ((maximum - minimum)/step));
                            num.Value = Math.Round(value, num.Point);
                        }
                    }

                if (!CalculateIndicator(slotType, indicator))
                    return;

                firstBar = 0;
                foreach (IndicatorComp comp in indicator.Component)
                    if (comp.FirstBar > firstBar)
                        firstBar = comp.FirstBar;
            } while (firstBar > Data.Bars - 10);

            //Set the Data.Strategy
            IndicatorSlot indSlot = Data.Strategy.Slot[slot];
            indSlot.IndicatorName = indicator.IndicatorName;
            indSlot.IndParam = indicator.IndParam;
            indSlot.Component = indicator.Component;
            indSlot.SeparatedChart = indicator.SeparatedChart;
            indSlot.SpecValue = indicator.SpecialValues;
            indSlot.MinValue = indicator.SeparatedChartMinValue;
            indSlot.MaxValue = indicator.SeparatedChartMaxValue;
            indSlot.IsDefined = true;
        }

        private void GenerateSameOppSignal()
        {
            Data.Strategy.PropertiesStatus = strategyBest.PropertiesStatus;
            Data.Strategy.SameSignalAction = strategyBest.SameSignalAction;
            Data.Strategy.OppSignalAction = strategyBest.OppSignalAction;

            if (strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                return;

            if (Data.SingleOrder)
                Data.Strategy.SameSignalAction = SameDirSignalAction.Nothing;
            else
            if (!chbPreserveSameDirAction.Checked)
                Data.Strategy.SameSignalAction =
                    (SameDirSignalAction)
                    Enum.GetValues(typeof (SameDirSignalAction)).GetValue(random.Next(3));

            if (!chbPreserveOppDirAction.Checked)
                Data.Strategy.OppSignalAction =
                    (OppositeDirSignalAction)
                    Enum.GetValues(typeof (OppositeDirSignalAction)).GetValue(random.Next(4));

            if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
                Data.Strategy.OppSignalAction = OppositeDirSignalAction.Reverse;
        }

        private void GeneratePermanentSL()
        {
            if (chbPreservePermSL.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UsePermanentSL = strategyBest.UsePermanentSL;
                Data.Strategy.PermanentSLType = strategyBest.PermanentSLType;
                Data.Strategy.PermanentSL = strategyBest.PermanentSL;
            }
            else
            {
                bool usePermSL = random.Next(100) > 30;
                bool changePermSL = random.Next(100) > 50;
                Data.Strategy.UsePermanentSL = usePermSL;
                Data.Strategy.PermanentSLType = PermanentProtectionType.Relative;
                if (usePermSL && changePermSL)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.PermanentSL = multiplier*random.Next(5, 50);
                    //if (random.Next(100) > 80 &&
                    //    (Data.Strategy.SameSignalAction == SameDirSignalAction.Add   || 
                    //    Data.Strategy.SameSignalAction == SameDirSignalAction.Winner ||
                    //    Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce))
                    //    Data.Strategy.PermanentSLType = PermanentProtectionType.Absolute;
                }
            }
        }

        private void GeneratePermanentTP()
        {
            if (chbPreservePermTP.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UsePermanentTP = strategyBest.UsePermanentTP;
                Data.Strategy.PermanentTPType = strategyBest.PermanentTPType;
                Data.Strategy.PermanentTP = strategyBest.PermanentTP;
            }
            else
            {
                bool usePermTP = random.Next(100) > 30;
                bool changePermTP = random.Next(100) > 50;
                Data.Strategy.UsePermanentTP = usePermTP;
                Data.Strategy.PermanentTPType = PermanentProtectionType.Relative;
                if (usePermTP && changePermTP)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.PermanentTP = multiplier*random.Next(5, 50);
                    //if (random.Next(100) > 80 &&
                    //    (Data.Strategy.SameSignalAction == SameDirSignalAction.Add    ||
                    //    Data.Strategy.SameSignalAction  == SameDirSignalAction.Winner ||
                    //    Data.Strategy.OppSignalAction   == OppositeDirSignalAction.Reduce))
                    //    Data.Strategy.PermanentTPType = PermanentProtectionType.Absolute;
                }
            }
        }

        private void GenerateBreakEven()
        {
            if (chbPreserveBreakEven.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UseBreakEven = strategyBest.UseBreakEven;
                Data.Strategy.BreakEven = strategyBest.BreakEven;
            }
            else
            {
                bool useBreakEven = random.Next(100) > 30;
                bool changeBreakEven = random.Next(100) > 50;
                Data.Strategy.UseBreakEven = useBreakEven;
                if (useBreakEven && changeBreakEven)
                {
                    int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                    Data.Strategy.BreakEven = multiplier*random.Next(5, 50);
                }
            }
        }

        private void GenerateMartingale()
        {
            if (strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
            {
                Data.Strategy.UseMartingale = strategyBest.UseMartingale;
                Data.Strategy.MartingaleMultiplier = strategyBest.MartingaleMultiplier;
            }
            else
            {
                Data.Strategy.UseMartingale = false;
                Data.Strategy.MartingaleMultiplier = 2.0;
            }
        }

        /// <summary>
        ///     Recalculate all the indicator slots
        /// </summary>
        private void RecalculateSlots()
        {
            foreach (IndicatorSlot indSlot in Data.Strategy.Slot)
            {
                string indicatorName = indSlot.IndicatorName;
                SlotTypes slotType = indSlot.SlotType;
                Indicator indicator = IndicatorManager.ConstructIndicator(indicatorName);
                indicator.Initialize(slotType);
                indicator.IndParam = indSlot.IndParam;
                indicator.Calculate(Data.DataSet);

                indSlot.Component = indicator.Component;
                indSlot.IsDefined = true;
            }

            // Searches the indicators' components to determine the Data.FirstBar 
            Data.FirstBar = Data.Strategy.SetFirstBar();
        }

        /// <summary>
        ///     Get the list of supported simple custom sorting options
        /// </summary>
        private List<string> GetSimpleCustomSortingOptions()
        {
            var options = new List<string>
                {
                    Language.T("Annualized Profit"),
                    Language.T("Annualized Profit %"),
                    Language.T("Average Holding Period Ret."),
                    Language.T("Geometric Holding Period Ret."),
                    Language.T("Profit Factor"),
                    Language.T("Sharpe Ratio"),
                    Language.T("Win/Loss Ratio")
                };

            // External Simple Sorting Options
            if (CustomAnalytics.Generator.IsAnalyticsEnabled)
                foreach (string option in CustomAnalytics.Generator.GetSimpleCustomSortingOptions())
                    options.Add(option);

            options.Sort();
            return options;
        }

        /// <summary>
        ///     Returns the simple custom sorting value
        /// </summary>
        private void GetSimpleCustomSortingValue(out float value, out string displayName)
        {
            displayName = customAnalytics.SimpleSortOption;

            switch (customAnalytics.SimpleSortOption)
            {
                case "Annualized Profit":
                    value = (float) Backtester.AnnualizedProfit;
                    break;
                case "Annualized Profit %":
                    value = (float) Backtester.AnnualizedProfitPercent;
                    break;
                case "Average Holding Period Ret.":
                    value = (float) Backtester.AvrgHoldingPeriodRet;
                    break;
                case "Geometric Holding Period Ret.":
                    value = (float) Backtester.GeomHoldingPeriodRet;
                    break;
                case "Profit Factor":
                    value = (float) Backtester.ProfitFactor;
                    break;
                case "Sharpe Ratio":
                    value = (float) Backtester.SharpeRatio;
                    break;
                case "Win/Loss Ratio":
                    value = (float) Backtester.WinLossRatio;
                    break;
                default:
                    // External Simple Sorting Options
                    customAnalytics.Strategy = StrategyXML.CreateStrategyXmlDoc(Data.Strategy);
                    customAnalytics.Positions = GetPositionsList();
                    // Retrieve the Custom Filter Value
                    CustomAnalytics.Generator.GetSimpleCustomSortingValue(ref customAnalytics, out value,
                                                                          out displayName);
                    break;
            }
        }

        /// <summary>
        ///     Get the list of supported advanced custom sorting options
        /// </summary>
        private List<string> GetAdvancedCustomSortingOptions()
        {
            var options = new List<string>();

            // External Advanced Sorting Options
            if (CustomAnalytics.Generator.IsAnalyticsEnabled)
                foreach (string option in CustomAnalytics.Generator.GetAdvancedCustomSortingOptions())
                    options.Add(option);

            options.Sort();

            return options;
        }

        /// <summary>
        ///     Returns the advanced custom sorting value
        /// </summary>
        private void GetAdvancedCustomSortingValue(out float value, out string displayName)
        {
            // External Simple Sorting Options
            customAnalytics.Strategy = StrategyXML.CreateStrategyXmlDoc(Data.Strategy);
            customAnalytics.Positions = GetPositionsList();

            // Retrieve the Custom Filter Value
            CustomAnalytics.Generator.GetAdvancedCustomSortingValue(ref customAnalytics, out value, out displayName);
        }

        /// <summary>
        ///     Construct a list of positions for custom analysis
        /// </summary>
        private List<CustomAnalytics.Position> GetPositionsList()
        {
            var positions = new List<CustomAnalytics.Position>();

            for (int iPos = 0; iPos < Backtester.PositionsTotal; iPos++)
            {
                var pos = new CustomAnalytics.Position();
                Position position = Backtester.PosFromNumb(iPos);
                int bar = Backtester.PosCoordinates[iPos].Bar;

                // Position Number
                pos.PositionNumber = position.PosNumb + 1;

                // Bar Number
                pos.BarNumber = bar + 1;

                // Bar Opening Time
                pos.BarOpeningTime = Data.Time[bar];

                // Position Direction
                switch (position.PosDir)
                {
                    case PosDirection.None:
                        pos.Direction = CustomAnalytics.PosDirection.None;
                        break;
                    case PosDirection.Long:
                        pos.Direction = CustomAnalytics.PosDirection.Long;
                        break;
                    case PosDirection.Short:
                        pos.Direction = CustomAnalytics.PosDirection.Short;
                        break;
                    case PosDirection.Closed:
                        pos.Direction = CustomAnalytics.PosDirection.Closed;
                        break;
                }

                // Lots
                pos.Lots = (float) position.PosLots;

                // Transaction
                switch (position.Transaction)
                {
                    case Transaction.None:
                        pos.Transaction = CustomAnalytics.Transaction.None;
                        break;
                    case Transaction.Open:
                        pos.Transaction = CustomAnalytics.Transaction.Open;
                        break;
                    case Transaction.Close:
                        pos.Transaction = CustomAnalytics.Transaction.Close;
                        break;
                    case Transaction.Add:
                        pos.Transaction = CustomAnalytics.Transaction.Add;
                        break;
                    case Transaction.Reduce:
                        pos.Transaction = CustomAnalytics.Transaction.Reduce;
                        break;
                    case Transaction.Reverse:
                        pos.Transaction = CustomAnalytics.Transaction.Reverse;
                        break;
                    case Transaction.Transfer:
                        pos.Transaction = CustomAnalytics.Transaction.Transfer;
                        break;
                }

                // Order Price
                pos.OrderPrice = (float) position.FormOrdPrice;

                // Average Price
                pos.AveragePrice = (float) position.PosPrice;

                // Profit/Loss
                pos.ProfitLoss = (float) position.ProfitLoss;

                // FLoating Profit/Loss
                pos.FloatingProfitLoss = (float) position.FloatingPL;

                // Balance
                pos.Balance = (float) position.Balance;

                // Equity
                pos.Equity = (float) position.Equity;

                // Add to Positions List
                positions.Add(pos);
            }

            return positions;
        }
    }
}