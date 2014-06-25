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
using System.IO;
using System.Text;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;

namespace ForexStrategyBuilder
{
    public class Exporter
    {
        /// <summary>
        ///     Exports the data
        /// </summary>
        public void ExportCsvData()
        {
            string ff = Data.Ff; // Format modifier to print float numbers
            string df = Data.Df; // Format modifier to print date
            var sb = new StringBuilder();
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append(Data.Time[bar].ToString(df) + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.High[bar].ToString(ff) + "\t");
                sb.Append(Data.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.Volume[bar] + Environment.NewLine);
            }

            string fileName = Data.Symbol + (int) Data.Period + ".csv";
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Exports the data
        /// </summary>
        public void ExportDataOnly()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramReleaseCandidate)
                stage = " " + "RC";

            string ff = Data.Ff; // Format modifier to print float numbers
            string df = Data.Df; // Format modifier to print date
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + Environment.NewLine);
            sb.Append("Number of bars: " + Data.Bars + Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Date" + "\t");
            sb.Append("Hour" + "\t");
            sb.Append("Open" + "\t");
            sb.Append("High" + "\t");
            sb.Append("Low" + "\t");
            sb.Append("Close" + "\t");
            sb.Append("Volume" + Environment.NewLine);

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append(Data.Time[bar].ToString(df) + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.High[bar].ToString(ff) + "\t");
                sb.Append(Data.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.Volume[bar] + Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.Period.ToString();
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Exports data and indicators values
        /// </summary>
        public void ExportIndicators()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramReleaseCandidate)
                stage = " " + "RC";

            string ff = Data.Ff; // Format modifier to print float numbers
            string df = Data.Df; // Format modifier to print date
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + Environment.NewLine);
            sb.Append("Number of bars: " + Data.Bars);

            sb.Append("\t\t\t\t\t\t\t\t");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                string strSlotType = "";
                switch (Data.Strategy.Slot[slot].SlotType)
                {
                    case SlotTypes.Open:
                        strSlotType = "Opening point of the position";
                        break;
                    case SlotTypes.OpenFilter:
                        strSlotType = "Opening logic condition";
                        break;
                    case SlotTypes.Close:
                        strSlotType = "Closing point of the position";
                        break;
                    case SlotTypes.CloseFilter:
                        strSlotType = "Closing logic condition";
                        break;
                }

                sb.Append(strSlotType + "\t");
                for (int i = 0; i < Data.Strategy.Slot[slot].Component.Length; i++)
                    sb.Append("\t");
            }
            sb.Append(Environment.NewLine);


            // Names of the indicator components
            sb.Append("\t\t\t\t\t\t\t\t");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                Indicator indicator = IndicatorManager.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName);
                indicator.Initialize(Data.Strategy.Slot[slot].SlotType);

                sb.Append(indicator + "\t");
                for (int i = 0; i < Data.Strategy.Slot[slot].Component.Length; i++)
                    sb.Append("\t");
            }
            sb.Append(Environment.NewLine);

            // Data
            sb.Append("Date" + "\t");
            sb.Append("Hour" + "\t");
            sb.Append("Open" + "\t");
            sb.Append("High" + "\t");
            sb.Append("Low" + "\t");
            sb.Append("Close" + "\t");
            sb.Append("Volume" + "\t");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                sb.Append("\t");
                foreach (IndicatorComp comp in Data.Strategy.Slot[slot].Component)
                    sb.Append(comp.CompName + "\t");
            }
            sb.Append(Environment.NewLine);

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append(Data.Time[bar].ToString(df) + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.High[bar].ToString(ff) + "\t");
                sb.Append(Data.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.Volume[bar] + "\t");

                for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                {
                    sb.Append("\t");
                    foreach (IndicatorComp comp in Data.Strategy.Slot[slot].Component)
                        sb.Append(comp.Value[bar] + "\t");
                }
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.Period;
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Exports the bar summary
        /// </summary>
        public void ExportBarSummary()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramReleaseCandidate)
                stage = " " + "RC";

            string ff = Data.Ff; // Format modifier to print float numbers
            string df = Data.Df; // Format modifier to print date
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in points" + Environment.NewLine);

            sb.Append("Bar Numb\t");
            sb.Append("Date\t");
            sb.Append("Hour\t");
            sb.Append("Open\t");
            sb.Append("High\t");
            sb.Append("Low\t");
            sb.Append("Close\t");
            sb.Append("Volume\t");
            sb.Append("Direction\t");
            sb.Append("Lots\t");
            sb.Append("Transaction\t");
            sb.Append("Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Spread\t");
            sb.Append("Rollover\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append("Interpolation" + Environment.NewLine);

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append((bar + 1) + "\t");
                sb.Append(Data.Time[bar].ToString(df) + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(ff) + "\t");
                sb.Append(Data.High[bar].ToString(ff) + "\t");
                sb.Append(Data.Low[bar].ToString(ff) + "\t");
                sb.Append(Data.Close[bar].ToString(ff) + "\t");
                sb.Append(Data.Volume[bar] + "\t");
                if (Backtester.IsPos(bar))
                {
                    sb.Append(Backtester.SummaryDir(bar) + "\t");
                    sb.Append(Backtester.SummaryLots(bar) + "\t");
                    sb.Append(Backtester.SummaryTrans(bar) + "\t");
                    sb.Append(Backtester.SummaryPrice(bar).ToString(ff) + "\t");
                    sb.Append(Backtester.ProfitLoss(bar) + "\t");
                    sb.Append(Backtester.FloatingPL(bar) + "\t");
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t");
                }
                sb.Append(Backtester.ChargedSpread(bar) + "\t");
                sb.Append(Backtester.ChargedRollOver(bar) + "\t");
                sb.Append(Backtester.Balance(bar) + "\t");
                sb.Append(Backtester.Equity(bar) + "\t");
                sb.Append(Backtester.BackTestEvalToString(bar) + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.Period.ToString();
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Exports the positions info
        /// </summary>
        public void ExportPositions(bool showTransfers)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramReleaseCandidate)
                stage = " " + "RC";

            string ff = Data.Ff; // Format modifier to print float numbers
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in points" + Environment.NewLine);

            sb.Append("Pos Numb\t");
            sb.Append("Bar Numb\t");
            sb.Append("Bar Opening Time\t");
            sb.Append("Direction\t");
            sb.Append("Lots\t");
            sb.Append("Transaction\t");
            sb.Append("Order Price\t");
            sb.Append("Average Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append(Environment.NewLine);

            for (int iPos = 0; iPos < Backtester.PositionsTotal; iPos++)
            {
                Position position = Backtester.PosFromNumb(iPos);
                int bar = Backtester.PosCoordinates[iPos].Bar;

                if (!showTransfers && position.Transaction == Transaction.Transfer)
                    continue;

                sb.Append((position.PosNumb + 1) + "\t");
                sb.Append((bar + 1) + "\t");
                sb.Append(Data.Time[bar] + "\t");
                sb.Append(position.PosDir + "\t");
                sb.Append(position.PosLots + "\t");
                sb.Append(position.Transaction + "\t");
                sb.Append(position.FormOrdPrice.ToString(ff) + "\t");
                sb.Append(position.PosPrice.ToString(ff) + "\t");
                sb.Append(Math.Round(position.ProfitLoss) + "\t");
                sb.Append(Math.Round(position.FloatingPL) + "\t");
                sb.Append(Math.Round(position.Balance) + "\t");
                sb.Append(Math.Round(position.Equity) + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.Period;
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Exports the positions info in currency
        /// </summary>
        public void ExportPositionsInMoney(bool showTransfers)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramReleaseCandidate)
                stage = " " + "RC";

            string ff = Data.Ff; // Format modifier to print float numbers
            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in " + Configs.AccountCurrency +
                      Environment.NewLine);

            sb.Append("Pos Numb\t");
            sb.Append("Bar Numb\t");
            sb.Append("Bar Opening Time\t");
            sb.Append("Direction\t");
            sb.Append("Amount\t");
            sb.Append("Transaction\t");
            sb.Append("Order Price\t");
            sb.Append("Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append(Environment.NewLine);

            for (int pos = 0; pos < Backtester.PositionsTotal; pos++)
            {
                Position position = Backtester.PosFromNumb(pos);
                int bar = Backtester.PosCoordinates[pos].Bar;

                if (!showTransfers && position.Transaction == Transaction.Transfer)
                    continue;

                sb.Append((position.PosNumb + 1) + "\t");
                sb.Append((bar + 1) + "\t");
                sb.Append(Data.Time[bar] + "\t");
                sb.Append(position.PosDir + "\t");
                sb.Append((position.PosDir == PosDirection.Long ? "" : "-") +
                          (position.PosLots*Data.InstrProperties.LotSize) + "\t");
                sb.Append(position.Transaction + "\t");
                sb.Append(position.FormOrdPrice.ToString(ff) + "\t");
                sb.Append(position.PosPrice.ToString(ff) + "\t");
                sb.Append(position.MoneyProfitLoss.ToString("F2") + "\t");
                sb.Append(position.MoneyFloatingPL.ToString("F2") + "\t");
                sb.Append(position.MoneyBalance.ToString("F2") + "\t");
                sb.Append(position.MoneyEquity.ToString("F2") + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol + "-" + Data.Period;
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Exports histogram data as CSV file
        /// </summary>
        /// <param name="s">string</param>
        public void ExportHistogramData(string s)
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramReleaseCandidate)
                stage = " " + "RC";

            var sb = new StringBuilder();

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + Environment.NewLine);
            sb.Append("Histogram Data");
            sb.Append(Environment.NewLine);

            sb.Append("Index" + "\t");
            sb.Append("Count" + "\t");
            sb.Append("Result" + "\t");
            sb.Append("Total" + Environment.NewLine);

            sb.Append(s);

            string fileName = Data.Strategy.StrategyName + "_HistogramData";
            SaveData(fileName, sb);
        }

        /// <summary>
        ///     Saves the data file
        /// </summary>
        private void SaveData(string fileName, StringBuilder data)
        {
            var sfdExport = new SaveFileDialog
                {
                    AddExtension = true,
                    InitialDirectory = Data.UserFilesDir,
                    Title = Language.T("Export"),
                    FileName = fileName
                };
            if (fileName.EndsWith(".csv"))
            {
                sfdExport.InitialDirectory = Data.OfflineDataDir;
                sfdExport.Filter = "FSB data (*.csv)|*.csv|All files (*.*)|*.*";
            }
            else
                sfdExport.Filter =
                    "Excel file (*.xls)|*.xls|FSB data (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (sfdExport.ShowDialog() != DialogResult.OK) return;
            try
            {
                var sw = new StreamWriter(sfdExport.FileName);
                sw.Write(data.ToString());
                sw.Close();
            }
            catch (Exception exc)
            {
                if (!Data.AutostartGenerator)
                    MessageBox.Show(exc.Message);
            }
        }
    }
}