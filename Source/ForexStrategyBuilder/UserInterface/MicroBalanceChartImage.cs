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

namespace ForexStrategyBuilder
{
    public class MicroBalanceChartImage
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="width">Chart Width</param>
        /// <param name="height">Chart Height</param>
        public MicroBalanceChartImage(int width, int height)
        {
            InitChart(width, height);
        }

        public Bitmap Chart { get; private set; }

        /// <summary>
        ///     Sets the chart parameters
        /// </summary>
        private void InitChart(int width, int height)
        {
            try
            {
                Chart = new Bitmap(width, height);

                if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

                const int border = 1;
                const int space = 2;

                int maximum;
                int minimum;

                int firstBar = Data.FirstBar;
                int bars = Data.Bars;
                int chartBars = Data.Bars - firstBar;
                int maxBalance = Configs.AccountInMoney ? (int)Backtester.MaxMoneyBalance : Backtester.MaxBalance;
                int minBalance = Configs.AccountInMoney ? (int)Backtester.MinMoneyBalance : Backtester.MinBalance;
                int maxEquity = Configs.AccountInMoney ? (int)Backtester.MaxMoneyEquity : Backtester.MaxEquity;
                int minEquity = Configs.AccountInMoney ? (int)Backtester.MinMoneyEquity : Backtester.MinEquity;

                if (Configs.AdditionalStatistics)
                {
                    int maxLongBalance = Configs.AccountInMoney
                                             ? (int)Backtester.MaxLongMoneyBalance
                                             : Backtester.MaxLongBalance;
                    int minLongBalance = Configs.AccountInMoney
                                             ? (int)Backtester.MinLongMoneyBalance
                                             : Backtester.MinLongBalance;
                    int maxShortBalance = Configs.AccountInMoney
                                              ? (int)Backtester.MaxShortMoneyBalance
                                              : Backtester.MaxShortBalance;
                    int minShortBalance = Configs.AccountInMoney
                                              ? (int)Backtester.MinShortMoneyBalance
                                              : Backtester.MinShortBalance;
                    int maxLsBalance = Math.Max(maxLongBalance, maxShortBalance);
                    int minLsBalance = Math.Min(minLongBalance, minShortBalance);

                    maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLsBalance) + 1;
                    minimum = Math.Min(Math.Min(minBalance, minEquity), minLsBalance) - 1;
                }
                else
                {
                    maximum = Math.Max(maxBalance, maxEquity) + 1;
                    minimum = Math.Min(minBalance, minEquity) - 1;
                }

                const int yTop = border + space;
                int yBottom = height - border - space;
                const int xLeft = border;
                int xRight = width - border - space;
                float xScale = (xRight - xLeft) / (float)chartBars;
                float yScale = (yBottom - yTop) / (float)(maximum - minimum);

                var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                        border);

                var balancePoints = new PointF[chartBars];
                var equityPoints = new PointF[chartBars];
                var longBalancePoints = new PointF[chartBars];
                var shortBalancePoints = new PointF[chartBars];

                int index = 0;
                for (int bar = firstBar; bar < bars; bar++)
                {
                    balancePoints[index].X = xLeft + index * xScale;
                    equityPoints[index].X = xLeft + index * xScale;
                    if (Configs.AccountInMoney)
                    {
                        balancePoints[index].Y = (float)(yBottom - (Backtester.MoneyBalance(bar) - minimum) * yScale);
                        equityPoints[index].Y = (float)(yBottom - (Backtester.MoneyEquity(bar) - minimum) * yScale);
                    }
                    else
                    {
                        balancePoints[index].Y = yBottom - (Backtester.Balance(bar) - minimum) * yScale;
                        equityPoints[index].Y = yBottom - (Backtester.Equity(bar) - minimum) * yScale;
                    }

                    if (Configs.AdditionalStatistics)
                    {
                        longBalancePoints[index].X = xLeft + index * xScale;
                        shortBalancePoints[index].X = xLeft + index * xScale;
                        if (Configs.AccountInMoney)
                        {
                            longBalancePoints[index].Y =
                                (float)(yBottom - (Backtester.LongMoneyBalance(bar) - minimum) * yScale);
                            shortBalancePoints[index].Y =
                                (float)(yBottom - (Backtester.ShortMoneyBalance(bar) - minimum) * yScale);
                        }
                        else
                        {
                            longBalancePoints[index].Y = yBottom - (Backtester.LongBalance(bar) - minimum) * yScale;
                            shortBalancePoints[index].Y = yBottom - (Backtester.ShortBalance(bar) - minimum) * yScale;
                        }
                    }

                    index++;
                }

                Graphics g = Graphics.FromImage(Chart);

                // Paints the background by gradient
                var rectField = new RectangleF(1, 1, width - 2, height - 2);
                g.FillRectangle(new SolidBrush(LayoutColors.ColorChartBack), rectField);

                // Border
                g.DrawRectangle(penBorder, 0, 0, width - 1, height - 1);

                // Equity line
                g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), equityPoints);

                // Draw Long and Short balance
                if (Configs.AdditionalStatistics)
                {
                    g.DrawLines(new Pen(Color.Red), shortBalancePoints);
                    g.DrawLines(new Pen(Color.Green), longBalancePoints);
                }

                // Draw the balance line
                g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), balancePoints);

            } catch { }
        }       
    }
}