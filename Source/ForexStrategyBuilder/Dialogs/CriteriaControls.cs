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
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs
{
    public class CriteriaControls : Panel
    {
        private readonly ToolTip toolTip;
        private CheckBox chbAmbiguousBars;
        private CheckBox chbEquityPercent;
        private CheckBox chbMaxDrawdown;
        private CheckBox chbMaxRedGreenDeviation;
        private CheckBox chbMaxTrades;
        private CheckBox chbMinProfitPerDay;
        private CheckBox chbMinSharpeRatio;
        private CheckBox chbMaxConsecLosses;
        private CheckBox chbMinTrades;
        private CheckBox chbOOSPatternFilter;
        private CheckBox chbSmoothBalanceLines;
        private CheckBox chbWinLossRatio;

        private NumericUpDown nudAmbiguousBars;
        private NumericUpDown nudEquityPercent;
        private NumericUpDown nudMaxDrawdown;
        private NumericUpDown nudMaxRedGreenDeviation;
        private NumericUpDown nudMaxTrades;
        private NumericUpDown nudMinProfitPerDay;
        private NumericUpDown nudMinSharpeRatio;
        private NumericUpDown nudMaxConsecLosses;
        private NumericUpDown nudMinTrades;
        private NumericUpDown nudSmoothBalanceCheckPoints;
        private NumericUpDown nudSmoothBalancePercent;
        private NumericUpDown nudWinLossRatio;
        private NumericUpDown nudoosPatternPercent;

        public CriteriaControls()
        {
            toolTip = new ToolTip();

            Height = 400;
            Margin = new Padding(0);
        }

        public double TargetBalanceRatio { get; set; }
        public int BarOOS { get; set; }
        public bool OOSTesting { get; set; }

        public void SetCriteriaPanel()
        {
            chbAmbiguousBars = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum number of ambiguous bars"),
                    Checked = true,
                    AutoSize = true
                };

            nudAmbiguousBars = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudAmbiguousBars.BeginInit();
            nudAmbiguousBars.Minimum = 0;
            nudAmbiguousBars.Maximum = 100;
            nudAmbiguousBars.Increment = 1;
            nudAmbiguousBars.Value = 10;
            nudAmbiguousBars.EndInit();

            chbMinProfitPerDay = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum Profit Per Day") + " [" + Configs.AccountCurrency + "]",
                    Checked = true,
                    AutoSize = true
                };

            nudMinProfitPerDay = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMinProfitPerDay.BeginInit();
            nudMinProfitPerDay.Minimum = 1;
            nudMinProfitPerDay.Maximum = 500;
            nudMinProfitPerDay.Increment = 1;
            nudMinProfitPerDay.Value = 1;
            nudMinProfitPerDay.EndInit();

            chbMaxDrawdown = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum equity drawdown") + " [" +
                           (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("points") + "]"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxDrawdown = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMaxDrawdown.BeginInit();
            nudMaxDrawdown.Minimum = 0;
            nudMaxDrawdown.Maximum = Configs.InitialAccount;
            nudMaxDrawdown.Increment = 10;
            nudMaxDrawdown.Value = (decimal) Math.Round(Configs.InitialAccount/4.0);
            nudMaxDrawdown.EndInit();

            chbEquityPercent = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text =
                        Language.T("Maximum equity drawdown") + " [% " + Configs.AccountCurrency +
                        "]",
                    Checked = true,
                    AutoSize = true
                };

            nudEquityPercent = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudEquityPercent.BeginInit();
            nudEquityPercent.Minimum = 1;
            nudEquityPercent.Maximum = 100;
            nudEquityPercent.Increment = 1;
            nudEquityPercent.Value = 25;
            nudEquityPercent.EndInit();

            chbMinTrades = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum number of trades"),
                    Checked = true,
                    AutoSize = true
                };

            nudMinTrades = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMinTrades.BeginInit();
            nudMinTrades.Minimum = 10;
            nudMinTrades.Maximum = 1000;
            nudMinTrades.Increment = 10;
            nudMinTrades.Value = 100;
            nudMinTrades.EndInit();

            chbMaxTrades = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum number of trades"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxTrades = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMaxTrades.BeginInit();
            nudMaxTrades.Minimum = 10;
            nudMaxTrades.Maximum = 10000;
            nudMaxTrades.Increment = 10;
            nudMaxTrades.Value = 1000;
            nudMaxTrades.EndInit();

            chbWinLossRatio = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum win / loss trades ratio"),
                    Checked = false,
                    AutoSize = true
                };

            nudWinLossRatio = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudWinLossRatio.BeginInit();
            nudWinLossRatio.Minimum = 0.10M;
            nudWinLossRatio.Maximum = 1;
            nudWinLossRatio.Increment = 0.01M;
            nudWinLossRatio.Value = 0.30M;
            nudWinLossRatio.DecimalPlaces = 2;
            nudWinLossRatio.EndInit();

            chbMinSharpeRatio = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Minimum Sharpe ratio"),
                    Checked = false,
                    AutoSize = true
                };

            nudMinSharpeRatio = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMinSharpeRatio.BeginInit();
            nudMinSharpeRatio.Minimum = 0.1M;
            nudMinSharpeRatio.Maximum = 10.0M;
            nudMinSharpeRatio.Increment = 0.1M;
            nudMinSharpeRatio.Value = 0.2M;
            nudMinSharpeRatio.DecimalPlaces = 1;
            nudMinSharpeRatio.EndInit();

            chbMaxConsecLosses = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Max consecutive losses"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxConsecLosses = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMaxConsecLosses.BeginInit();
            nudMaxConsecLosses.Minimum = 1;
            nudMaxConsecLosses.Maximum = 100;
            nudMaxConsecLosses.Increment = 1;
            nudMaxConsecLosses.Value = 5;
            nudMaxConsecLosses.EndInit();

            chbMaxRedGreenDeviation = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Max long/short balance deviation"),
                    Checked = false,
                    AutoSize = true
                };

            nudMaxRedGreenDeviation = new NumericUpDown {Parent = this, TextAlign = HorizontalAlignment.Center};
            nudMaxRedGreenDeviation.BeginInit();
            nudMaxRedGreenDeviation.Minimum = 1;
            nudMaxRedGreenDeviation.Maximum = 100;
            nudMaxRedGreenDeviation.Value = 40;
            nudMaxRedGreenDeviation.EndInit();
            toolTip.SetToolTip(nudMaxRedGreenDeviation, Language.T("Deviation percent."));

            chbOOSPatternFilter = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Filter bad OOS performance"),
                    Checked = false,
                    AutoSize = true
                };

            nudoosPatternPercent = new NumericUpDown
                {
                    Parent = this,
                    TextAlign = HorizontalAlignment.Center
                };
            nudoosPatternPercent.BeginInit();
            nudoosPatternPercent.Minimum = 1;
            nudoosPatternPercent.Maximum = 50;
            nudoosPatternPercent.Value = 20;
            nudoosPatternPercent.EndInit();
            toolTip.SetToolTip(nudoosPatternPercent, Language.T("Deviation percent."));

            chbSmoothBalanceLines = new CheckBox
                {
                    Parent = this,
                    ForeColor = ForeColor,
                    BackColor = Color.Transparent,
                    Text = Language.T("Filter non-linear balance pattern"),
                    Checked = false,
                    AutoSize = true
                };

            nudSmoothBalancePercent = new NumericUpDown
                {
                    Parent = this,
                    TextAlign = HorizontalAlignment.Center
                };
            nudSmoothBalancePercent.BeginInit();
            nudSmoothBalancePercent.Minimum = 1;
            nudSmoothBalancePercent.Maximum = 50;
            nudSmoothBalancePercent.Value = 20;
            nudSmoothBalancePercent.EndInit();
            toolTip.SetToolTip(nudSmoothBalancePercent, Language.T("Deviation percent."));

            nudSmoothBalanceCheckPoints = new NumericUpDown
                {
                    Parent = this,
                    TextAlign = HorizontalAlignment.Center
                };
            nudSmoothBalanceCheckPoints.BeginInit();
            nudSmoothBalanceCheckPoints.Minimum = 1;
            nudSmoothBalanceCheckPoints.Maximum = 50;
            nudSmoothBalanceCheckPoints.Value = 1;
            nudSmoothBalanceCheckPoints.EndInit();
            toolTip.SetToolTip(nudSmoothBalanceCheckPoints, Language.T("Check points count."));

            Resize += CriteriaControls_Resize;
        }

        public void SetSettings(string settingsString)
        {
            if (string.IsNullOrEmpty(settingsString))
                return;

            string[] options = settingsString.Split(';');
            int i = 0;
            try
            {
                chbAmbiguousBars.Checked = bool.Parse(options[i++]);
                nudAmbiguousBars.Value = int.Parse(options[i++]);
                chbMaxDrawdown.Checked = bool.Parse(options[i++]);
                nudMaxDrawdown.Value = int.Parse(options[i++]);
                chbMinTrades.Checked = bool.Parse(options[i++]);
                nudMinTrades.Value = int.Parse(options[i++]);
                chbMaxTrades.Checked = bool.Parse(options[i++]);
                nudMaxTrades.Value = int.Parse(options[i++]);
                chbWinLossRatio.Checked = bool.Parse(options[i++]);
                nudWinLossRatio.Value = int.Parse(options[i++])/100M;
                chbEquityPercent.Checked = bool.Parse(options[i++]);
                nudEquityPercent.Value = int.Parse(options[i++]);
                chbOOSPatternFilter.Checked = bool.Parse(options[i++]);
                nudoosPatternPercent.Value = int.Parse(options[i++]);
                chbSmoothBalanceLines.Checked = bool.Parse(options[i++]);
                nudSmoothBalancePercent.Value = int.Parse(options[i++]);
                nudSmoothBalanceCheckPoints.Value = int.Parse(options[i++]);
                chbMinSharpeRatio.Checked = bool.Parse(options[i++]);
                nudMinSharpeRatio.Value = int.Parse(options[i++])/100M;
                chbMaxConsecLosses.Checked = bool.Parse(options[i++]);
                nudMaxConsecLosses.Value = int.Parse(options[i++]);
                chbMinProfitPerDay.Checked = bool.Parse(options[i++]);
                nudMinProfitPerDay.Value = int.Parse(options[i++]);
                chbMaxRedGreenDeviation.Checked = bool.Parse(options[i++]);
                nudMaxRedGreenDeviation.Value = int.Parse(options[i]);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public string GetSettings()
        {
            string settingsString =
                chbAmbiguousBars.Checked + ";" +
                nudAmbiguousBars.Value + ";" +
                chbMaxDrawdown.Checked + ";" +
                nudMaxDrawdown.Value + ";" +
                chbMinTrades.Checked + ";" +
                nudMinTrades.Value + ";" +
                chbMaxTrades.Checked + ";" +
                nudMaxTrades.Value + ";" +
                chbWinLossRatio.Checked + ";" +
                ((int) (nudWinLossRatio.Value*100M)) + ";" +
                chbEquityPercent.Checked + ";" +
                nudEquityPercent.Value + ";" +
                chbOOSPatternFilter.Checked + ";" +
                nudoosPatternPercent.Value + ";" +
                chbSmoothBalanceLines.Checked + ";" +
                nudSmoothBalancePercent.Value + ";" +
                nudSmoothBalanceCheckPoints.Value + ";" +
                chbMinSharpeRatio.Checked + ";" +
                ((int) (nudMinSharpeRatio.Value*100M)) + ";" +
                chbMaxConsecLosses.Checked + ";" +
                nudMaxConsecLosses.Value + ";" +
                chbMinProfitPerDay.Checked + ";" +
                nudMinProfitPerDay.Value + ";" +
                chbMaxRedGreenDeviation.Checked + ";" +
                nudMaxRedGreenDeviation.Value;

            return settingsString;
        }

        private void CriteriaControls_Resize(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int border = btnHrzSpace;
            const int nudWidth = 55;
            int nudLeft = panel.ClientSize.Width - border - nudWidth;

            // chbAmbiguousBars
            chbAmbiguousBars.Location = new Point(border + 2, 6);

            // nudAmbiguousBars
            nudAmbiguousBars.Width = nudWidth;
            nudAmbiguousBars.Location = new Point(nudLeft, chbAmbiguousBars.Top - 1);

            // Min Profit Per Day
            chbMinProfitPerDay.Location = new Point(border + 2, chbAmbiguousBars.Bottom + border + 4);
            nudMinProfitPerDay.Width = nudWidth;
            nudMinProfitPerDay.Location = new Point(nudLeft, chbMinProfitPerDay.Top - 1);

            // MaxDrawdown
            chbMaxDrawdown.Location = new Point(border + 2, chbMinProfitPerDay.Bottom + border + 4);
            nudMaxDrawdown.Width = nudWidth;
            nudMaxDrawdown.Location = new Point(nudLeft, chbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            chbEquityPercent.Location = new Point(border + 2, nudMaxDrawdown.Bottom + border + 4);
            nudEquityPercent.Width = nudWidth;
            nudEquityPercent.Location = new Point(nudLeft, chbEquityPercent.Top - 1);

            // MinTrades
            chbMinTrades.Location = new Point(border + 2, chbEquityPercent.Bottom + border + 4);
            nudMinTrades.Width = nudWidth;
            nudMinTrades.Location = new Point(nudLeft, chbMinTrades.Top - 1);

            // MaxTrades
            chbMaxTrades.Location = new Point(border + 2, chbMinTrades.Bottom + border + 4);
            nudMaxTrades.Width = nudWidth;
            nudMaxTrades.Location = new Point(nudLeft, chbMaxTrades.Top - 1);

            // Win/Loss Ratios
            chbWinLossRatio.Location = new Point(border + 2, chbMaxTrades.Bottom + border + 4);
            nudWinLossRatio.Width = nudWidth;
            nudWinLossRatio.Location = new Point(nudLeft, chbWinLossRatio.Top - 1);

            // Sharpe Ratios
            chbMinSharpeRatio.Location = new Point(border + 2, chbWinLossRatio.Bottom + border + 4);
            nudMinSharpeRatio.Width = nudWidth;
            nudMinSharpeRatio.Location = new Point(nudLeft, chbMinSharpeRatio.Top - 1);

            // Max consecutive losses
            chbMaxConsecLosses.Location = new Point(border + 2, chbMinSharpeRatio.Bottom + border + 4);
            nudMaxConsecLosses.Width = nudWidth;
            nudMaxConsecLosses.Location = new Point(nudLeft, chbMaxConsecLosses.Top - 1);

            // Red/Green Deviation
            chbMaxRedGreenDeviation.Location = new Point(border + 2, chbMaxConsecLosses.Bottom + border + 4);
            nudMaxRedGreenDeviation.Width = nudWidth;
            nudMaxRedGreenDeviation.Location = new Point(nudLeft, chbMaxRedGreenDeviation.Top - 1);

            // OOS Pattern Filter
            chbOOSPatternFilter.Location = new Point(border + 2, chbMaxRedGreenDeviation.Bottom + border + 4);
            nudoosPatternPercent.Width = nudWidth;
            nudoosPatternPercent.Location = new Point(nudLeft, chbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            chbSmoothBalanceLines.Location = new Point(border + 2, chbOOSPatternFilter.Bottom + border + 4);
            nudSmoothBalancePercent.Width = nudWidth;
            nudSmoothBalancePercent.Location = new Point(nudLeft, chbSmoothBalanceLines.Top - 1);
            nudSmoothBalanceCheckPoints.Width = nudWidth;
            nudSmoothBalanceCheckPoints.Location = new Point(nudSmoothBalancePercent.Left - nudWidth - border,
                                                             chbSmoothBalanceLines.Top - 1);
        }

        public bool IsCriteriaFulfilled()
        {
            // Criterion Max Ambiguous Bars
            if (chbAmbiguousBars.Checked && Backtester.AmbiguousBars > nudAmbiguousBars.Value)
            {
                return false;
            }

            // Criterion Min Profit per Day
            if (chbMinProfitPerDay.Checked && Backtester.MoneyProfitPerDay < (double) nudMinProfitPerDay.Value)
            {
                return false;
            }

            // Criterion Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney
                                           ? Backtester.MaxMoneyEquityDrawdown
                                           : Backtester.MaxEquityDrawdown;
            if (chbMaxDrawdown.Checked && maxEquityDrawdown > (double) nudMaxDrawdown.Value)
            {
                return false;
            }

            // Criterion Max Equity percent drawdown
            if ((chbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double) nudEquityPercent.Value) || Backtester.MoneyEquityPercentDrawdown > Data.MaxDD)
            {
                return false;
            }

            // Criterion Min Trades
            if (chbMinTrades.Checked && Backtester.ExecutedOrders < nudMinTrades.Value)
            {
                return false;
            }

            // Criterion Max Trades
            if (chbMaxTrades.Checked && Backtester.ExecutedOrders > nudMaxTrades.Value)
            {
                return false;
            }

            // Criterion Win / Loss ratio
            if (chbWinLossRatio.Checked && Backtester.WinLossRatio < (double) nudWinLossRatio.Value)
            {
                return false;
            }

            // Criterion Minimum Sharpe ratio
            if (chbMinSharpeRatio.Checked && Backtester.SharpeRatio < (double) nudMinSharpeRatio.Value)
            {
                return false;
            }

            // Max consecutive losses
            if (chbMaxConsecLosses.Checked && Backtester.MaxConsecutiveLosses > nudMaxConsecLosses.Value)
            {
                return false;
            }

            // Criterion Red/Green Deviation
            if (chbMaxRedGreenDeviation.Checked &&
                Backtester.RedGreenBalanceDev > (double) nudMaxRedGreenDeviation.Value)
            {
                return false;
            }

            // OOS Pattern filter
            if (chbOOSPatternFilter.Checked && OOSTesting)
            {
                int netBalance = Backtester.NetBalance;
                int oosBalance = Backtester.Balance(BarOOS);
                var targetBalance = (int) (oosBalance*TargetBalanceRatio);
                var minBalance = (int) (targetBalance*(1 - nudoosPatternPercent.Value/100));
                if (netBalance < oosBalance || netBalance < minBalance)
                {
                    return false;
                }
            }

            // Smooth Balance Line
            if (chbSmoothBalanceLines.Checked)
            {
                var checkPoints = (int) nudSmoothBalanceCheckPoints.Value;
                var maxPercentDeviation = (double) (nudSmoothBalancePercent.Value/100);

                for (int i = 1; i <= checkPoints; i++)
                {
                    int firstBar = Data.FirstBar;
                    var bar = (int) (Data.FirstBar + i*(Data.Bars - firstBar)/(checkPoints + 1.0));
                    double netBalance = Backtester.NetMoneyBalance;
                    double startBalance = Backtester.MoneyBalance(firstBar);
                    double checkPointBalance = Backtester.MoneyBalance(bar);
                    double targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1.0);
                    double minBalance = targetBalance*(1 - maxPercentDeviation);
                    double maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                    {
                        return false;
                    }

                    // Long balance line
                    netBalance = Backtester.NetLongMoneyBalance;
                    checkPointBalance = Backtester.LongMoneyBalance(bar);
                    startBalance = Backtester.LongMoneyBalance(firstBar);
                    targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    minBalance = targetBalance*(1 - maxPercentDeviation);
                    maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                    {
                        return false;
                    }

                    // Short balance line
                    netBalance = Backtester.NetShortMoneyBalance;
                    checkPointBalance = Backtester.ShortMoneyBalance(bar);
                    startBalance = Backtester.ShortMoneyBalance(firstBar);
                    targetBalance = startBalance + i*(netBalance - startBalance)/(checkPoints + 1);
                    minBalance = targetBalance*(1 - maxPercentDeviation);
                    maxBalance = targetBalance*(1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}