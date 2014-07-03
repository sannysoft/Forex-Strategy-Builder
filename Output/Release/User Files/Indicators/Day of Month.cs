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
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators.Store
{
    public class DayOfMonth : Indicator
    {
        public DayOfMonth()
        {
            IndicatorName   = "Day of Month";
            PossibleSlots   = SlotTypes.OpenFilter;

            IndicatorAuthor = "Miroslav Popov";
            IndicatorVersion = "2.0";
            IndicatorDescription = "A custom indicator for FSB and FST.";
        }

        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            // Setting up the indicator parameters
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Enter the market between the specified days"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.NumParam[0].Caption  = "From (incl.)";
            IndParam.NumParam[0].Value    = 1;
            IndParam.NumParam[0].Min      = 1;
            IndParam.NumParam[0].Max      = 31;
            IndParam.NumParam[0].Point    = 0;
            IndParam.NumParam[0].Enabled  = true;
            IndParam.NumParam[0].ToolTip  = "Day of beginning for the entry period.";

            IndParam.NumParam[1].Caption  = "Until (excl.)";
            IndParam.NumParam[1].Value    = 31;
            IndParam.NumParam[1].Min      = 1;
            IndParam.NumParam[1].Max      = 31;
            IndParam.NumParam[1].Point    = 0;
            IndParam.NumParam[1].Enabled  = true;
            IndParam.NumParam[1].ToolTip  = "Day of ending for the entry period.";
        }

        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            // Reading the parameters
            int iFromDay  = (int)IndParam.NumParam[0].Value;
            int iUntilDay = (int)IndParam.NumParam[1].Value;

            // Calculation
            int iFirstBar = 1;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                if (iFromDay < iUntilDay)
                    adBars[iBar] = Time[iBar].Day >= iFromDay && Time[iBar].Day <  iUntilDay ? 1 : 0;
                else if (iFromDay > iUntilDay)
                    adBars[iBar] = Time[iBar].Day >= iFromDay || Time[iBar].Day <  iUntilDay ? 1 : 0;
                else
                    adBars[iBar] = 1;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Allow long entry";
            Component[0].DataType      = IndComponentType.AllowOpenLong;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = iFirstBar;
            Component[0].Value         = adBars;

            Component[1] = new IndicatorComp();
            Component[1].CompName      = "Allow short entry";
            Component[1].DataType      = IndComponentType.AllowOpenShort;
            Component[1].ChartType     = IndChartType.NoChart;
            Component[1].ShowInDynInfo = false;
            Component[1].FirstBar      = iFirstBar;
            Component[1].Value         = adBars;
        }

        public override void SetDescription()
        {
            int iFromDay  = (int)IndParam.NumParam[0].Value;
            int iUntilDay = (int)IndParam.NumParam[1].Value;

            EntryFilterLongDescription  = "the day of month is from " + iFromDay + " (incl.) to " + iUntilDay + " (excl.)";
            EntryFilterShortDescription = "the day of month is from " + iFromDay + " (incl.) to " + iUntilDay + " (excl.)";
        }

        public override string ToString()
        {
            int iFromDay  = (int)IndParam.NumParam[0].Value;
            int iUntilDay = (int)IndParam.NumParam[1].Value;

            return IndicatorName + " (" +
                iFromDay  + ", " + // From
                iUntilDay + ")";   // Until
        }
    }
}
