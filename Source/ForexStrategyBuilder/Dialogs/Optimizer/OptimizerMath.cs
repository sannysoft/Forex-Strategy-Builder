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
using System.Media;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;

namespace ForexStrategyBuilder.Dialogs.Optimizer
{
    /// <summary>
    ///     The Optimizer - Math
    /// </summary>
    public sealed partial class Optimizer
    {
        private double[] initialValues;
        private float targetBalanceRatio = 1;

        /// <summary>
        ///     Sets the parameters.
        /// </summary>
        private void SetIndicatorParams()
        {
            protections = 0;
            parameters = 0;

            CountParameters();
            CountPermanentProtections();
            CreateControls();
            CreateProtectionParameters();

            for (int prot = 0; prot < protections; prot++)
            {
                SetParametersValues(prot);
            }

            int param = protections;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                for (int numParam = 0; numParam < 6; numParam++)
                {
                    if (!Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled) continue;
                    aParameter[param] = new Parameter(OptimizerParameterType.Indicator, slot, numParam);
                    SetParametersValues(param);
                    param++;
                }
            }

            int totalHeight = ArrangeControls();

            if (parameters == 0)
            {
                pnlParams.Height = 50;
                lblNoParams.Visible = true;
            }
            else
            {
                pnlParams.Height = totalHeight;
            }

            btnOptimize.Focus();
        }

        /// <summary>
        ///     Counts the strategy's numeric parameters and Indicators.
        /// </summary>
        private void CountParameters()
        {
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        parameters++;
        }

        /// <summary>
        ///     Counts Permanent SL, TP and Breakeven
        /// </summary>
        private void CountPermanentProtections()
        {
            if (Data.Strategy.UsePermanentSL)
            {
                parameters++;
                protections++;
            }
            if (Data.Strategy.UsePermanentTP)
            {
                parameters++;
                protections++;
            }
            if (Data.Strategy.UseBreakEven)
            {
                parameters++;
                protections++;
            }
        }

        /// <summary>
        ///     Creates control arrays.
        /// </summary>
        private void CreateControls()
        {
            aParameter = new Parameter[parameters];
            achbxParameterName = new CheckBox[parameters];
            alblInitialValue = new Label[parameters];
            alblParameterValue = new Label[parameters];
            anudParameterMin = new NumericUpDown[parameters];
            anudParameterMax = new NumericUpDown[parameters];
            anudParameterStep = new NumericUpDown[parameters];
            alblIndicatorName = new Label[parameters];

            for (int param = 0; param < parameters; param++)
            {
                alblIndicatorName[param] = new Label();
                achbxParameterName[param] = new CheckBox();
                alblInitialValue[param] = new Label();
                alblParameterValue[param] = new Label();
                anudParameterMin[param] = new NumericUpDown();
                anudParameterMax[param] = new NumericUpDown();
                anudParameterStep[param] = new NumericUpDown();
            }

            initialValues = new double[parameters];
        }

        /// <summary>
        ///     Initializes protection parameter.
        /// </summary>
        private void CreateProtectionParameters()
        {
            int par = 0;
            if (Data.Strategy.UsePermanentSL)
            {
                aParameter[par] = new Parameter(OptimizerParameterType.PermanentSL, -1, 0);
                par++;
            }
            if (Data.Strategy.UsePermanentTP)
            {
                aParameter[par] = new Parameter(OptimizerParameterType.PermanentTP, -1, 0);
                par++;
            }
            if (Data.Strategy.UseBreakEven)
            {
                aParameter[par] = new Parameter(OptimizerParameterType.BreakEven, -1, 0);
            }
        }

        /// <summary>
        ///     Sets values of optimizer controls.
        /// </summary>
        private void SetParametersValues(int param)
        {
            alblIndicatorName[param].Parent = pnlParams;
            alblIndicatorName[param].Text = aParameter[param].GroupName;
            alblIndicatorName[param].Font = fontIndicator;
            alblIndicatorName[param].ForeColor = LayoutColors.ColorSlotIndicatorText;
            alblIndicatorName[param].BackColor = Color.Transparent;
            alblIndicatorName[param].Height = (int) (20 * Data.VDpiScale);
            alblIndicatorName[param].Width = 200;

            achbxParameterName[param].ForeColor = colorText;
            achbxParameterName[param].BackColor = Color.Transparent;
            achbxParameterName[param].Text = aParameter[param].ParameterName;
            achbxParameterName[param].CheckedChanged += OptimizerCheckedChanged;
            achbxParameterName[param].Parent = pnlParams;
            achbxParameterName[param].Width = 155;
            achbxParameterName[param].TextAlign = ContentAlignment.MiddleLeft;

            int point = aParameter[param].Point;
            double value = aParameter[param].Value;
            double min = aParameter[param].Minimum;
            double max = aParameter[param].Maximum;
            double step = aParameter[param].Step;
            string stringFormat = "{0:F" + point + "}";

            alblInitialValue[param].ForeColor = colorText;
            alblInitialValue[param].BackColor = Color.Transparent;
            alblInitialValue[param].Text = string.Format(stringFormat, value);
            alblInitialValue[param].Parent = pnlParams;
            alblInitialValue[param].Width = 55;
            alblInitialValue[param].TextAlign = ContentAlignment.MiddleCenter;

            alblParameterValue[param].ForeColor = colorText;
            alblParameterValue[param].BackColor = Color.Transparent;
            alblParameterValue[param].Text = string.Format(stringFormat, value);
            alblParameterValue[param].Parent = pnlParams;
            alblParameterValue[param].Width = 55;
            alblParameterValue[param].TextAlign = ContentAlignment.MiddleCenter;

            fontParamValueRegular = alblParameterValue[param].Font;
            fontParamValueBold = new Font(fontParamValueRegular, FontStyle.Bold);

            anudParameterMin[param].BeginInit();
            anudParameterMin[param].Minimum = (decimal) min;
            anudParameterMin[param].Maximum = (decimal) max;
            anudParameterMin[param].Value =
                (decimal) Math.Round(Math.Max(value - lastSetStepButtonValue*step, min), point);
            anudParameterMin[param].DecimalPlaces = point;
            anudParameterMin[param].Increment = (decimal) step;
            anudParameterMin[param].EndInit();
            anudParameterMin[param].Parent = pnlParams;
            anudParameterMin[param].Width = 70;
            anudParameterMin[param].TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(anudParameterMin[param], Language.T("Minimum value."));

            anudParameterMax[param].BeginInit();
            anudParameterMax[param].Minimum = (decimal) min;
            anudParameterMax[param].Maximum = (decimal) max;
            anudParameterMax[param].Value =
                (decimal) Math.Round(Math.Min(value + lastSetStepButtonValue*step, max), point);
            anudParameterMax[param].DecimalPlaces = point;
            anudParameterMax[param].Increment = (decimal) step;
            anudParameterMax[param].EndInit();
            anudParameterMax[param].Parent = pnlParams;
            anudParameterMax[param].Width = 70;
            anudParameterMax[param].TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(anudParameterMax[param], Language.T("Maximum value."));

            anudParameterStep[param].BeginInit();
            anudParameterStep[param].Minimum = (decimal) step;
            anudParameterStep[param].Maximum = (decimal) Math.Max(step, Math.Abs(max - min));
            anudParameterStep[param].Value = (decimal) step;
            anudParameterStep[param].DecimalPlaces = point;
            anudParameterStep[param].Increment = (decimal) step;
            anudParameterStep[param].EndInit();
            anudParameterStep[param].Parent = pnlParams;
            anudParameterStep[param].Width = 70;
            anudParameterStep[param].TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(anudParameterStep[param], Language.T("Step of change."));

            initialValues[param] = value;
        }

        /// <summary>
        ///     Arranges the controls
        /// </summary>
        private int ArrangeControls()
        {
            const int vertMargin = 3;
            const int horizMargin = 5;
            int vertPosition = vertMargin - scrollBar.Value;
            int totalHeight = vertMargin;

            int slotNumber = int.MinValue;
            for (int param = 0; param < parameters; param++)
            {
                if (aParameter[param].SlotNumber != slotNumber)
                {
                    slotNumber = aParameter[param].SlotNumber;
                    alblIndicatorName[param].Location = new Point(horizMargin, vertPosition);
                    vertPosition += alblIndicatorName[param].Height + vertMargin;
                    totalHeight += alblIndicatorName[param].Height + vertMargin;
                }
                else
                {
                    alblIndicatorName[param].Visible = false;
                }
                achbxParameterName[param].Location = new Point(horizMargin, vertPosition);
                alblInitialValue[param].Location = new Point(achbxParameterName[param].Right + horizMargin,
                                                             vertPosition - 1);
                alblParameterValue[param].Location = new Point(alblInitialValue[param].Right + horizMargin,
                                                               vertPosition - 1);
                anudParameterMin[param].Location = new Point(alblParameterValue[param].Right + horizMargin,
                                                             vertPosition + 2);
                anudParameterMax[param].Location = new Point(anudParameterMin[param].Right + horizMargin,
                                                             vertPosition + 2);
                anudParameterStep[param].Location = new Point(anudParameterMax[param].Right + horizMargin,
                                                              vertPosition + 2);
                vertPosition += achbxParameterName[param].Height + vertMargin;
                totalHeight += achbxParameterName[param].Height + vertMargin;
            }

            return totalHeight;
        }

        /// <summary>
        ///     Sets parameters' Min / Max values
        /// </summary>
        private void SetParamsMinMax(int deltaStep)
        {
            for (int param = 0; param < parameters; param++)
            {
                int point = aParameter[param].Point;
                double value = aParameter[param].Value;
                double min = aParameter[param].Minimum;
                double max = aParameter[param].Maximum;
                double step = aParameter[param].Step;

                int deltaStepInner = deltaStep;

                if (aParameter[param].GroupName == "HourOfDay")
                {
                    anudParameterMin[param].Value = 0;
                    anudParameterMax[param].Value = 23;
                }
                else
                {
                    anudParameterMin[param].Value = (decimal)Math.Round(Math.Max(value - deltaStep * step, min), point);
                    anudParameterMax[param].Value = (decimal)Math.Round(Math.Min(value + deltaStep * step, max), point);
                }
                alblParameterValue[param].Text = GetParameterText(param);
            }
        }

        /// <summary>
        ///     Select / unselect parameters
        /// </summary>
        private void SelectParameters(OptimizerButtons button)
        {
            for (int param = 0; param < parameters; param++)
            {
                if (button == OptimizerButtons.SelectAll)
                    achbxParameterName[param].Checked = true;
                else if (button == OptimizerButtons.SelectNone)
                    achbxParameterName[param].Checked = false;
                else if (button == OptimizerButtons.SelectRandom)
                    SelectRandomParameters();
            }
        }

        /// <summary>
        ///     Selects parameters randomly.
        /// </summary>
        private void SelectRandomParameters()
        {
            for (int param = 0; param < protections; param++)
            {
                achbxParameterName[param].Checked = rand.Next(100) > 40;
            }

            int checkedChBoxes = 0;
            for (int param = protections; param < parameters; param++)
            {
                int slot = aParameter[param].SlotNumber;
                int numParam = aParameter[param].NumParam;

                achbxParameterName[param].Checked = false;

                if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption == "Level")
                {
                    if (Data.Strategy.Slot[slot].IndParam.ListParam[0].Text.Contains("Level"))
                    {
                        if (rand.Next(100) > 20 && checkedChBoxes < 6)
                        {
                            achbxParameterName[param].Checked = true;
                            checkedChBoxes++;
                        }
                    }
                    else
                    {
                        achbxParameterName[param].Checked = false;
                    }
                }
                else
                {
                    if (rand.Next(100) > 40 && checkedChBoxes < 6)
                    {
                        achbxParameterName[param].Checked = true;
                        checkedChBoxes++;
                    }
                }
            }
        }

        /// <summary>
        ///     Optimize.
        /// </summary>
        private void BtnOptimizeClick(object sender, EventArgs e)
        {
            if (isOptimizing)
            {
                // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            if (chbOptimizerWritesReport.Checked)
                InitReport();

            // Counts the checked parameters
            checkedParams = 0;
            for (int i = 0; i < parameters; i++)
                if (achbxParameterName[i].Checked)
                    checkedParams++;

            // If there are no checked returns
            if (checkedParams < 1)
            {
                SystemSounds.Hand.Play();
                return;
            }

            // Contains the checked parameters only
            aiChecked = new int[checkedParams];
            int indexChecked = 0;
            for (int i = 0; i < parameters; i++)
                if (achbxParameterName[i].Checked)
                    aiChecked[indexChecked++] = i;

            SetNecessaryCycles();

            Cursor = Cursors.WaitCursor;
            progressBar.Value = 1;
            progressPercent = 0;
            computedCycles = 0;
            isOptimizing = true;
            btnCancel.Enabled = false;
            btnAccept.Enabled = false;
            btnOptimize.Text = Language.T("Stop");

            for (int i = 0; i <= (int) OptimizerButtons.ResetStrategy; i++)
                aOptimizerButtons[i].Enabled = false;

            foreach (Control control in pnlParams.Controls)
                if (control.GetType() != typeof (Label))
                    control.Enabled = false;

            foreach (Control control in criteriaControls.Controls)
                control.Enabled = false;

            foreach (Control control in pnlSettings.Controls)
                control.Enabled = false;
            chbHideFSB.Enabled = true;

            criteriaControls.OOSTesting = chbOutOfSample.Checked;
            criteriaControls.BarOOS = (int)nudOutOfSample.Value;
            criteriaControls.TargetBalanceRatio = targetBalanceRatio;


            // Start the bgWorker
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Counts the necessary optimization cycles.
        /// </summary>
        private void SetNecessaryCycles()
        {
            cycles = 0;
            for (int i = 0; i < checkedParams; i++)
            {
                var min = (double) anudParameterMin[aiChecked[i]].Value;
                var max = (double) anudParameterMax[aiChecked[i]].Value;
                var step = (double) anudParameterStep[aiChecked[i]].Value;

                for (double value = min; value <= max; value += step)
                    cycles += 1;
            }

            for (int i = 0; i < checkedParams - 1; i++)
            {
                for (int j = 0; j < checkedParams; j++)
                {
                    if (i >= j) continue;

                    var min1 = (double) anudParameterMin[aiChecked[i]].Value;
                    var max1 = (double) anudParameterMax[aiChecked[i]].Value;
                    var step1 = (double) anudParameterStep[aiChecked[i]].Value;
                    var min2 = (double) anudParameterMin[aiChecked[j]].Value;
                    var max2 = (double) anudParameterMax[aiChecked[j]].Value;
                    var step2 = (double) anudParameterStep[aiChecked[j]].Value;

                    for (double value1 = min1; value1 <= max1; value1 += step1)
                        for (double value2 = min2; value2 <= max2; value2 += step2)
                            cycles += 1;
                }
            }
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            var worker = sender as BackgroundWorker;

            // Optimize all Parameters
            OptimizeParams(worker, e);
        }

        /// <summary>
        ///     This event handler updates the progress bar.
        /// </summary>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        ///     This event handler deals with the results of the background operation.
        /// </summary>
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Backtester.Calculate();
            Backtester.CalculateAccountStats();
            balanceChart.SetChartData();
            balanceChart.InitChart();
            balanceChart.Invalidate();

            if (!e.Cancelled && Configs.PlaySounds)
                SystemSounds.Exclamation.Play();

            isOptimizing = false;
            btnCancel.Enabled = true;
            btnAccept.Enabled = true;
            btnOptimize.Text = Language.T("Optimize");
            progressBar.Value = 1;

            if (pnlParams.Visible)
                for (int i = 0; i <= (int) OptimizerButtons.ResetStrategy; i++)
                    aOptimizerButtons[i].Enabled = true;

            for (int i = 0; i < parameters; i++)
                alblParameterValue[i].Text = GetParameterText(i);

            foreach (Control control in pnlParams.Controls)
                control.Enabled = true;
            foreach (Control control in criteriaControls.Controls)
                control.Enabled = true;
            foreach (Control control in pnlSettings.Controls)
                control.Enabled = true;

            if (chbOptimizerWritesReport.Checked)
                SaveReport();

            Cursor = Cursors.Default;

            if (Data.AutoOptimize != "")
            {
                //Save strategy and exit

                double profit = Backtester.NetMoneyBalance - Configs.InitialAccount;

                String drawdown = Backtester.MoneyEquityPercentDrawdown.ToString("F2");
                String filename = System.IO.Path.GetFileName(Data.AutoOptimize);

                //Create dir
                string subPath = "Optimized";
                System.IO.Directory.CreateDirectory(subPath);

                //Save result to CSV
                System.IO.StreamWriter file = new System.IO.StreamWriter(subPath + "/results.csv", true);
                file.WriteLine(Data.Symbol + ";" + Data.PeriodString + ";" + Data.Strategy.EntryLots + ";" + filename + ";" + profit.ToString("F2") + ";" + drawdown);
                //balanceChart.
                file.Close();

                //Autosave strategy                
                Data.Strategy.Save(subPath + "/" + filename);

                Close();
            }
        }

        /// <summary>
        ///     Saves the Generator History
        /// </summary>
        private void SetStrategyToGeneratorHistory()
        {
            Data.GeneratorHistory.Add(Data.Strategy.Clone());

            if (Data.GeneratorHistory.Count >= 110)
                Data.GeneratorHistory.RemoveRange(0, 10);

            Data.GenHistoryIndex = Data.GeneratorHistory.Count - 1;
        }

        /// <summary>
        ///     Optimize all the checked parameters
        /// </summary>
        private void OptimizeParams(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int bestBalance = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);

            // First Optimization Cycle
            for (int round = 0; round < checkedParams && isOptimizing; round++)
            {
                if (worker.CancellationPending) break;

                int param = aiChecked[round];

                var min = (double) anudParameterMin[param].Value;
                var max = (double) anudParameterMax[param].Value;
                var step = (double) anudParameterStep[param].Value;

                for (double value = min; value <= max; value += step)
                {
                    if (worker.CancellationPending) break;

                    aParameter[param].Value = value;
                    if (aParameter[param].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param].SlotNumber);

                    Backtester.Calculate();
                    Backtester.CalculateAccountStats();
                    if (chbOptimizerWritesReport.Checked)
                        FillInReport();

                    int balance = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
                    bool isCriteriaFulfilled = criteriaControls.IsCriteriaFulfilled();
                    if (balance > bestBalance && isCriteriaFulfilled)
                    {
                        bestBalance = balance;
                        aParameter[param].BestValue = value;
                        ShowParamBestValue(param);
                        balanceChart.SetChartData();
                        balanceChart.InitChart();
                        balanceChart.Invalidate();
                        isStartegyChanged = true;
                        SetStrategyToGeneratorHistory();
                    }

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    int percentComplete = 100*computedCycles/cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > progressPercent)
                    {
                        progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }

                aParameter[param].Value = aParameter[param].BestValue;
                if (aParameter[param].Type == OptimizerParameterType.Indicator)
                    CalculateIndicator(aParameter[param].SlotNumber);
                Backtester.Calculate();
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (checkedParams < 2)
                return;

            // Counts the necessary round
            int rounds = 0;
            for (int i = 0; i < checkedParams - 1; i++)
                for (int j = 0; j < checkedParams; j++)
                    if (i < j) rounds++;

            var aCp = new CoupleOfParams[rounds];
            var aCpTemp = new CoupleOfParams[rounds];

            rounds = 0;
            for (int i = 0; i < checkedParams - 1; i++)
                for (int j = 0; j < checkedParams; j++)
                    if (i < j)
                    {
                        aCpTemp[rounds].Param1 = aiChecked[i];
                        aCpTemp[rounds].Param2 = aiChecked[j];
                        aCpTemp[rounds].IsPassed = false;
                        rounds++;
                    }

            // Shaking the parameters
            for (int round = 0; round < rounds; round++)
            {
                int couple;
                do
                {
                    couple = rand.Next(rounds);
                } while (aCpTemp[couple].IsPassed);
                aCpTemp[couple].IsPassed = true;
                aCp[round] = aCpTemp[couple];
            }

            // The Optimization Cycle
            for (int round = 0; round < rounds; round++)
            {
                if (worker.CancellationPending) break;

                int param1 = aCp[round].Param1;
                int param2 = aCp[round].Param2;
                bool isOneIndicator = (aParameter[param1].Type == OptimizerParameterType.Indicator &&
                                       aParameter[param2].Type == OptimizerParameterType.Indicator &&
                                       aParameter[param1].IndParam.IndicatorName ==
                                       aParameter[param2].IndParam.IndicatorName);

                var min1 = (double) anudParameterMin[param1].Value;
                var max1 = (double) anudParameterMax[param1].Value;
                var step1 = (double) anudParameterStep[param1].Value;

                var min2 = (double) anudParameterMin[param2].Value;
                var max2 = (double) anudParameterMax[param2].Value;
                var step2 = (double) anudParameterStep[param2].Value;

                for (double value1 = min1; value1 <= max1; value1 += step1)
                {
                    if (worker.CancellationPending) break;

                    if (!isOneIndicator)
                    {
                        aParameter[param1].Value = value1;
                        if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                            CalculateIndicator(aParameter[param1].SlotNumber);
                    }

                    for (double value2 = min2; value2 <= max2; value2 += step2)
                    {
                        if (worker.CancellationPending) break;

                        if (isOneIndicator)
                        {
                            aParameter[param1].Value = value1;
                            aParameter[param2].Value = value2;
                            if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                                CalculateIndicator(aParameter[param1].SlotNumber);
                        }
                        else
                        {
                            aParameter[param2].Value = value2;
                            if (aParameter[param2].Type == OptimizerParameterType.Indicator)
                                CalculateIndicator(aParameter[param2].SlotNumber);
                        }

                        // Calculates the Strategy
                        Backtester.Calculate();
                        Backtester.CalculateAccountStats();
                        if (chbOptimizerWritesReport.Checked)
                            FillInReport();

                        int balance = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
                        bool isCriteriaFulfilled = criteriaControls.IsCriteriaFulfilled();
                        if (balance > bestBalance && isCriteriaFulfilled)
                        {
                            bestBalance = balance;
                            aParameter[param1].BestValue = value1;
                            aParameter[param2].BestValue = value2;
                            ShowParamBestValue(param1);
                            ShowParamBestValue(param2);
                            balanceChart.SetChartData();
                            balanceChart.InitChart();
                            balanceChart.Invalidate();
                            isStartegyChanged = true;
                            SetStrategyToGeneratorHistory();
                        }

                        // Report progress as a percentage of the total task.
                        computedCycles++;
                        int percentComplete = 100*computedCycles/cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > progressPercent)
                        {
                            progressPercent = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }
                }

                aParameter[param1].Value = aParameter[param1].BestValue;
                aParameter[param2].Value = aParameter[param2].BestValue;

                if (isOneIndicator)
                {
                    if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param1].SlotNumber);
                }
                else
                {
                    if (aParameter[param1].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param1].SlotNumber);
                    if (aParameter[param2].Type == OptimizerParameterType.Indicator)
                        CalculateIndicator(aParameter[param2].SlotNumber);
                }

                Backtester.Calculate();
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        ///     Calculates the indicator in the designated slot
        /// </summary>
        private void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Data.Strategy.Slot[slot].IndParam;
            Indicator indicator = IndicatorManager.ConstructIndicator(ip.IndicatorName);
            indicator.Initialize(ip.SlotType);

            // List parameters
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric parameters
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check parameters
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(Data.DataSet);

            // Sets Data.Strategy
            Data.Strategy.Slot[slot].IndicatorName = indicator.IndicatorName;
            Data.Strategy.Slot[slot].IndParam = indicator.IndParam;
            Data.Strategy.Slot[slot].Component = indicator.Component;
            Data.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[slot].SpecValue = indicator.SpecialValues;
            Data.Strategy.Slot[slot].MinValue = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[slot].MaxValue = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[slot].IsDefined = true;

            // Search the indicators' components to determine the Data.FirstBar
            Data.FirstBar = Data.Strategy.SetFirstBar();
        }

        /// <summary>
        ///     Shows the best value of a parameter during optimization.
        /// </summary>
        private void ShowParamBestValue(int param)
        {
            if (alblParameterValue[param].InvokeRequired)
            {
                BeginInvoke(new ShowParamBestValueCallback(ShowParamBestValue), new object[] {param});
            }
            else
            {
                alblParameterValue[param].Text = GetParameterText(param);
                alblParameterValue[param].Font = fontParamValueBold;
                if (aParameter[param].OldBestValue < aParameter[param].BestValue)
                    alblParameterValue[param].ForeColor = LayoutColors.ColorTradeLong;
                if (aParameter[param].OldBestValue > aParameter[param].BestValue)
                    alblParameterValue[param].ForeColor = LayoutColors.ColorTradeShort;

                var timer = new Timer {Interval = 1500, Tag = param};
                timer.Tick += TimerTick;
                timer.Start();
            }
        }

        /// <summary>
        ///     Generates the parameter text.
        /// </summary>
        private string GetParameterText(int param)
        {
            string stringFormat = "{0:F" + aParameter[param].Point + "}";
            string newText = string.Format(stringFormat, aParameter[param].BestValue);

            if (alblParameterValue[param].Text == newText)
                return newText;

            double bestValue = aParameter[param].BestValue;
            if (Math.Abs(bestValue - (double) anudParameterMin[param].Value) < 0.000001)
                newText = "[" + newText;
            if (Math.Abs(bestValue - aParameter[param].Minimum) < 0.000001)
                newText = "[" + newText;
            if (Math.Abs(bestValue - (double) anudParameterMax[param].Value) < 0.000001)
                newText = newText + "]";
            if (Math.Abs(bestValue - aParameter[param].Maximum) < 0.000001)
                newText = newText + "]";

            return newText;
        }

        /// <summary>
        ///     Recovers the font of a parameter value label.
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            var timer = (Timer) sender;
            var param = (int) timer.Tag;

            alblParameterValue[param].ForeColor = LayoutColors.ColorControlText;
            alblParameterValue[param].Font = fontParamValueRegular;

            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        ///     Resets parameters and recalculates the strategy.
        /// </summary>
        private void ResetStrategyParameters()
        {
            for (int param = 0; param < parameters; param++)
            {
                int point = aParameter[param].Point;
                string stringFormat = "{0:F" + point + "}";
                double value = initialValues[param];

                aParameter[param].Value = value;
                alblParameterValue[param].Text = string.Format(stringFormat, value);
            }

            int lastSlot = -1;
            for (int param = 0; param < parameters; param++)
            {
                if (aParameter[param].Type != OptimizerParameterType.Indicator) continue;
                if (aParameter[param].SlotNumber == lastSlot) continue;
                lastSlot = aParameter[param].SlotNumber;
                CalculateIndicator(lastSlot);
            }

            Backtester.Calculate();
            Backtester.CalculateAccountStats();
            balanceChart.SetChartData();
            balanceChart.InitChart();
            balanceChart.Invalidate();
            isStartegyChanged = false;
        }

        #region Nested type: ShowParamBestValueCallback

        private delegate void ShowParamBestValueCallback(int param);

        #endregion
    }
}