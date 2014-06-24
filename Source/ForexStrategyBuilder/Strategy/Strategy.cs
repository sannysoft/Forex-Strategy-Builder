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
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Indicators.Store;
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Exceptions;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Strategy Class.
    /// </summary>
    public partial class Strategy
    {
        /// <summary>
        ///     Sets a new strategy.
        /// </summary>
        public Strategy(int openFilters, int closeFilters)
        {
            StrategyName = "Unnamed";
            Symbol = "EURUSD";
            DataPeriod = DataPeriod.D1;
            Description = string.Empty;
            OppSignalAction = OppositeDirSignalAction.Nothing;
            SameSignalAction = SameDirSignalAction.Nothing;
            PermanentTP = 1000;
            PermanentTPType = PermanentProtectionType.Relative;
            PermanentSL = 1000;
            PermanentSLType = PermanentProtectionType.Relative;
            BreakEven = 1000;
            MaxOpenLots = 20;
            EntryLots = 1;
            AddingLots = 1;
            ReducingLots = 1;
            MartingaleMultiplier = 2.0;
            CreateStrategy(openFilters, closeFilters);
        }

        /// <summary>
        ///     Gets the max count of Open Filters.
        /// </summary>
        public static int MaxOpenFilters
        {
            get { return Configs.MaxEntryFilters; }
        }

        /// <summary>
        ///     Gets the max count of Close Filters.
        /// </summary>
        public static int MaxCloseFilters
        {
            get { return Configs.MaxExitFilters; }
        }

        /// <summary>
        ///     Gets or sets the strategy name.
        /// </summary>
        public string StrategyName { get; set; }

        /// <summary>
        ///     Gets or sets the count of Open Filters.
        /// </summary>
        public int OpenFilters { get; private set; }

        /// <summary>
        ///     Gets or sets the count of Close Filters.
        /// </summary>
        public int CloseFilters { get; private set; }

        /// <summary>
        ///     Gets or sets the Data Period.
        /// </summary>
        public DataPeriod DataPeriod { get; set; }

        /// <summary>
        ///     Gets or sets the Symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Gets or sets the UsePermanentSL
        /// </summary>
        public bool UsePermanentSL { get; set; }

        /// <summary>
        ///     Gets or sets the type of Permanent SL
        /// </summary>
        public PermanentProtectionType PermanentSLType { get; set; }

        /// <summary>
        ///     Gets or sets the PermanentSL
        /// </summary>
        public int PermanentSL { get; set; }

        /// <summary>
        ///     Gets or sets the UsePermanentTP
        /// </summary>
        public bool UsePermanentTP { get; set; }

        /// <summary>
        ///     Gets or sets the type of Permanent TP
        /// </summary>
        public PermanentProtectionType PermanentTPType { get; set; }

        /// <summary>
        ///     Gets or sets the PermanentTP
        /// </summary>
        public int PermanentTP { get; set; }

        /// <summary>
        ///     Gets or sets the UseBreakEven
        /// </summary>
        public bool UseBreakEven { get; set; }

        /// <summary>
        ///     Gets or sets the BreakEven
        /// </summary>
        public int BreakEven { get; set; }

        /// <summary>
        ///     Gets or sets the UseAccountPercentEntry
        /// </summary>
        public bool UseAccountPercentEntry { get { return true; } set{} }

        /// <summary>
        ///     Gets or sets the max number of open lots to enter the market
        /// </summary>
        public double MaxOpenLots { get { return 100; } set{} }

        /// <summary>
        ///     Gets or sets the Number of lots to enter the market
        /// </summary>
        public double EntryLots { get; set; }

        /// <summary>
        ///     Gets or sets the Number of lots to add to the position
        /// </summary>
        public double AddingLots { get { return EntryLots; } set {} }

        /// <summary>
        ///     Gets or sets the Number of lots to reduce the position
        /// </summary>
        public double ReducingLots { get { return EntryLots; } set {} }

        /// <summary>
        ///     Gets or sets if the strategy uses Martingale Money Management.
        /// </summary>
        public bool UseMartingale { get { return false; } set {} }

        /// <summary>
        ///     Gets or sets the Martingale multiplier
        /// </summary>
        public double MartingaleMultiplier { get; set; }

        /// <summary>
        ///     Gets or sets the Strategy description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets the count of slots.
        /// </summary>
        public int Slots
        {
            get { return OpenFilters + CloseFilters + 2; }
        }

        /// <summary>
        ///     Gets the number of Open Slot.
        /// </summary>
        public int OpenSlot
        {
            get { return 0; }
        }

        /// <summary>
        ///     Gets the number of Close Slot.
        /// </summary>
        public int CloseSlot
        {
            get { return OpenFilters + 1; }
        }

        /// <summary>
        ///     Gets or sets the indicators build up the strategy.
        /// </summary>
        public IndicatorSlot[] Slot { get; private set; }

        /// <summary>
        ///     Gets or sets a value representing how the new opposite signal reflects on the position
        /// </summary>
        public OppositeDirSignalAction OppSignalAction { get; set; }

        /// <summary>
        ///     Gets or sets a value representing how the new same dir signal reflects on the position
        /// </summary>
        public SameDirSignalAction SameSignalAction { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Strategy Properties panel is locked (Generator)
        /// </summary>
        public StrategySlotStatus PropertiesStatus { get; set; }

        /// <summary>
        ///     The time when the position entry occurs
        /// </summary>
        private ExecutionTime EntryExecutionTime
        {
            get { return Slot[OpenSlot].IndParam.ExecutionTime; }
        }

        /// <summary>
        ///     The time when the position exit occurs
        /// </summary>
        private ExecutionTime ExitExecutionTime
        {
            get { return Slot[CloseSlot].IndParam.ExecutionTime; }
        }

        /// <summary>
        ///     Gets the strategy first bar.
        /// </summary>
        public int FirstBar { get; private set; }

        /// <summary>
        ///     Creates a strategy
        /// </summary>
        private void CreateStrategy(int openFilters, int closeFilters)
        {
            StrategyName = "Unnamed";
            OpenFilters = openFilters;
            CloseFilters = closeFilters;
            Slot = new IndicatorSlot[Slots];

            for (int slot = 0; slot < Slots; slot++)
            {
                Slot[slot] = new IndicatorSlot {SlotNumber = slot, SlotType = GetSlotType(slot)};
            }
        }

        /// <summary>
        ///     Generates a new strategy.
        /// </summary>
        public static void GenerateNew()
        {
            Data.Strategy = new Strategy(0, 0);

            int openSlotNum = Data.Strategy.OpenSlot;
            int closeSlotNum = Data.Strategy.CloseSlot;

            Data.Strategy.StrategyName = "New";

            var barOpening = new BarOpening();
            barOpening.Initialize(SlotTypes.Open);
            barOpening.Calculate(new DataSet());
            Data.Strategy.Slot[openSlotNum].IndParam = barOpening.IndParam;
            Data.Strategy.Slot[openSlotNum].IndicatorName = barOpening.IndicatorName;
            Data.Strategy.Slot[openSlotNum].Component = barOpening.Component;
            Data.Strategy.Slot[openSlotNum].SeparatedChart = barOpening.SeparatedChart;
            Data.Strategy.Slot[openSlotNum].SpecValue = barOpening.SpecialValues;
            Data.Strategy.Slot[openSlotNum].MaxValue = barOpening.SeparatedChartMaxValue;
            Data.Strategy.Slot[openSlotNum].MinValue = barOpening.SeparatedChartMinValue;
            Data.Strategy.Slot[openSlotNum].IsDefined = true;

            var barClosing = new BarClosing();
            barClosing.Initialize(SlotTypes.Close);
            barClosing.Calculate(new DataSet());
            Data.Strategy.Slot[closeSlotNum].IndParam = barClosing.IndParam;
            Data.Strategy.Slot[closeSlotNum].IndicatorName = barClosing.IndicatorName;
            Data.Strategy.Slot[closeSlotNum].Component = barClosing.Component;
            Data.Strategy.Slot[closeSlotNum].SeparatedChart = barClosing.SeparatedChart;
            Data.Strategy.Slot[closeSlotNum].SpecValue = barClosing.SpecialValues;
            Data.Strategy.Slot[closeSlotNum].MaxValue = barClosing.SeparatedChartMaxValue;
            Data.Strategy.Slot[closeSlotNum].MinValue = barClosing.SeparatedChartMinValue;
            Data.Strategy.Slot[closeSlotNum].IsDefined = true;
        }

        /// <summary>
        ///     Gets the type of the slot.
        /// </summary>
        public SlotTypes GetSlotType(int slot)
        {
            SlotTypes slotType;

            if (slot == OpenSlot)
                slotType = SlotTypes.Open;
            else if (slot < CloseSlot)
                slotType = SlotTypes.OpenFilter;
            else if (slot == CloseSlot)
                slotType = SlotTypes.Close;
            else
                slotType = SlotTypes.CloseFilter;

            return slotType;
        }

        /// <summary>
        ///     Gets the default logical group for the designated slot number.
        /// </summary>
        public string GetDefaultGroup(int slot)
        {
            string group = "";
            string indicatorName = Slot[slot].IndicatorName;
            SlotTypes slotType = GetSlotType(slot);
            if (slotType == SlotTypes.OpenFilter)
            {
                if (indicatorName == "Data Bars Filter" ||
                    indicatorName == "Date Filter" ||
                    indicatorName == "Day of Month" ||
                    indicatorName == "Enter Once" ||
                    indicatorName == "Entry Time" ||
                    indicatorName == "Long or Short" ||
                    indicatorName == "Lot Limiter" ||
                    indicatorName == "Random Filter")
                    group = "All";
                else
                    group = "A";
            }
            if (slotType == SlotTypes.CloseFilter)
            {
                int index = slot - CloseSlot - 1;
                group = char.ConvertFromUtf32(char.ConvertToUtf32("a", 0) + index);
            }

            return group;
        }

        /// <summary>
        ///     Adds a new Open Filter to the strategy.
        /// </summary>
        /// <returns>The number of new Open Filter Slot.</returns>
        public int AddOpenFilter()
        {
            OpenFilters++;
            var aIndSlotOld = (IndicatorSlot[]) Slot.Clone();
            Slot = new IndicatorSlot[Slots];
            int newSlotNumb = OpenFilters; // The number of new open filter slot.

            // Copy the open slot and all old open filters.
            for (int slot = 0; slot < newSlotNumb; slot++)
                Slot[slot] = aIndSlotOld[slot];

            // Copy the close slot and all close filters.
            for (int slot = newSlotNumb + 1; slot < Slots; slot++)
                Slot[slot] = aIndSlotOld[slot - 1];

            // Create the new slot.
            Slot[newSlotNumb] = new IndicatorSlot {SlotType = SlotTypes.OpenFilter};

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                Slot[slot].SlotNumber = slot;

            return newSlotNumb;
        }

        /// <summary>
        ///     Adds a new Close Filter to the strategy.
        /// </summary>
        /// <returns>The number of new Close Filter Slot.</returns>
        public int AddCloseFilter()
        {
            CloseFilters++;
            var aIndSlotOld = (IndicatorSlot[]) Slot.Clone();
            Slot = new IndicatorSlot[Slots];
            int newSlotNumb = Slots - 1; // The number of new close filter slot.

            // Copy all old slots.
            for (int slot = 0; slot < newSlotNumb; slot++)
                Slot[slot] = aIndSlotOld[slot];

            // Create the new slot.
            Slot[newSlotNumb] = new IndicatorSlot {SlotType = SlotTypes.CloseFilter};

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                Slot[slot].SlotNumber = slot;

            return newSlotNumb;
        }

        /// <summary>
        ///     Removes a filter from the strategy.
        /// </summary>
        public void RemoveFilter(int slotToRemove)
        {
            if (Slot[slotToRemove].SlotType != SlotTypes.OpenFilter &&
                Slot[slotToRemove].SlotType != SlotTypes.CloseFilter)
                return;

            if (slotToRemove < CloseSlot)
                OpenFilters--;
            else
                CloseFilters--;
            var indSlotOld = (IndicatorSlot[]) Slot.Clone();
            Slot = new IndicatorSlot[Slots];

            // Copy all filters before this that has to be removed.
            for (int slot = 0; slot < slotToRemove; slot++)
                Slot[slot] = indSlotOld[slot];

            // Copy all filters after this that has to be removed.
            for (int slot = slotToRemove; slot < Slots; slot++)
                Slot[slot] = indSlotOld[slot + 1];

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                Slot[slot].SlotNumber = slot;
        }

        /// <summary>
        ///     Removes all close filters from the strategy.
        /// </summary>
        public void RemoveAllCloseFilters()
        {
            CloseFilters = 0;
            var indSlotOld = (IndicatorSlot[]) Slot.Clone();
            Slot = new IndicatorSlot[Slots];

            // Copy all slots except the close filters.
            for (int slot = 0; slot < Slots; slot++)
                Slot[slot] = indSlotOld[slot];
        }

        /// <summary>
        ///     Moves a filter upwards.
        /// </summary>
        public void MoveFilterUpwards(int slotToMove)
        {
            if (slotToMove <= 1 || Slot[slotToMove].SlotType != Slot[slotToMove - 1].SlotType) return;
            IndicatorSlot tempSlot = Slot[slotToMove - 1].Clone();
            Slot[slotToMove - 1] = Slot[slotToMove].Clone();
            Slot[slotToMove] = tempSlot.Clone();

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                Slot[slot].SlotNumber = slot;
        }

        /// <summary>
        ///     Moves a filter downwards.
        /// </summary>
        public void MoveFilterDownwards(int slotToMove)
        {
            if (slotToMove >= Slots - 1 || Slot[slotToMove].SlotType != Slot[slotToMove + 1].SlotType) return;
            IndicatorSlot tempSlot = Slot[slotToMove + 1].Clone();
            Slot[slotToMove + 1] = Slot[slotToMove].Clone();
            Slot[slotToMove] = tempSlot.Clone();

            // Sets the slot numbers.
            for (int slot = 0; slot < Slots; slot++)
                Slot[slot].SlotNumber = slot;
        }

        /// <summary>
        ///     Duplicates an logic condition.
        /// </summary>
        public void DuplicateFilter(int slotToDuplicate)
        {
            if (Slot[slotToDuplicate].SlotType == SlotTypes.OpenFilter && OpenFilters < MaxOpenFilters ||
                Slot[slotToDuplicate].SlotType == SlotTypes.CloseFilter && CloseFilters < MaxCloseFilters)
            {
                IndicatorSlot tempSlot = Slot[slotToDuplicate].Clone();

                if (Slot[slotToDuplicate].SlotType == SlotTypes.OpenFilter)
                {
                    int iAddedslot = AddOpenFilter();
                    Slot[iAddedslot] = tempSlot.Clone();
                }

                if (Slot[slotToDuplicate].SlotType == SlotTypes.CloseFilter)
                {
                    int addedslot = AddCloseFilter();
                    Slot[addedslot] = tempSlot.Clone();
                }

                // Sets the slot numbers.
                for (int slot = 0; slot < Slots; slot++)
                    Slot[slot].SlotNumber = slot;
            }
        }

        /// <summary>
        ///     Sets the strategy First Bar. It depends on the indicators periods.
        /// </summary>
        public int SetFirstBar()
        {
            // Searches the indicators' components to determine the first bar.
            FirstBar = 2;
            foreach (IndicatorSlot slot in Slot)
                foreach (IndicatorComp comp in slot.Component)
                    if (FirstBar < comp.FirstBar)
                        FirstBar = comp.FirstBar;

            return FirstBar;
        }

        /// <summary>
        ///     Sets Use previous bar value automatically
        /// </summary>
        public bool AdjustUsePreviousBarValue()
        {
            bool isSomethingChanged = false;
            if (Data.AutoUsePrvBarValue == false)
                return false;

            for (int slot = 0; slot < Slots; slot++)
            {
                isSomethingChanged = SetUsePrevBarValueCheckBox(slot) || isSomethingChanged;
            }

            // Recalculates the indicators.
            if (isSomethingChanged)
            {
                for (int slot = 0; slot < Slots; slot++)
                {
                    string sIndicatorName = Data.Strategy.Slot[slot].IndicatorName;
                    SlotTypes slotType = Data.Strategy.Slot[slot].SlotType;
                    Indicator indicator = IndicatorManager.ConstructIndicator(sIndicatorName);
                    indicator.Initialize(slotType);
                    indicator.IndParam = Data.Strategy.Slot[slot].IndParam;
                    indicator.Calculate(Data.DataSet);

                    // Set the Data.Strategy
                    Slot[slot].IndicatorName = indicator.IndicatorName;
                    Slot[slot].IndParam = indicator.IndParam;
                    Slot[slot].Component = indicator.Component;
                    Slot[slot].SeparatedChart = indicator.SeparatedChart;
                    Slot[slot].SpecValue = indicator.SpecialValues;
                    Slot[slot].MinValue = indicator.SeparatedChartMinValue;
                    Slot[slot].MaxValue = indicator.SeparatedChartMaxValue;
                    Slot[slot].IsDefined = true;
                }
            }

            return isSomethingChanged;
        }

        /// <summary>
        ///     Sets the "Use previous bar value" checkbox.
        /// </summary>
        /// <returns>Is any Changes</returns>
        private bool SetUsePrevBarValueCheckBox(int slot)
        {
            bool isChanged = false;

            for (int param = 0; param < Slot[slot].IndParam.CheckParam.Length; param++)
            {
                if (Slot[slot].IndParam.CheckParam[param].Caption == "Use previous bar value")
                {
                    bool isOrigChecked = Slot[slot].IndParam.CheckParam[param].Checked;
                    bool isChecked = true;

                    // Open slot
                    switch (Slot[slot].SlotType)
                    {
                        case SlotTypes.OpenFilter:
                            isChecked = EntryExecutionTime != ExecutionTime.AtBarClosing;
                            break;
                        case SlotTypes.CloseFilter:
                            isChecked = ExitExecutionTime != ExecutionTime.AtBarClosing;
                            break;
                    }

                    if (isChecked)
                    {
                        for (int iPar = 0; iPar < Slot[slot].IndParam.ListParam.Length; iPar++)
                        {
                            if (Slot[slot].IndParam.ListParam[iPar].Caption == "Base price" &&
                                Slot[slot].IndParam.ListParam[iPar].Text == "Open")
                            {
                                isChecked = false;
                            }
                        }
                    }

                    if (isChecked != isOrigChecked)
                    {
                        isChanged = true;
                        Slot[slot].IndParam.CheckParam[param].Checked = isChecked;
                    }
                }
            }

            return isChanged;
        }

        /// <summary>
        ///     Prepare the checkbox.
        /// </summary>
        /// <returns>IsChecked</returns>
        public bool PrepareUsePrevBarValueCheckBox(SlotTypes slotType)
        {
            bool isChecked = true;
            switch (slotType)
            {
                case SlotTypes.OpenFilter:
                    if (Data.Strategy.Slot[Data.Strategy.OpenSlot].IndParam.ExecutionTime == ExecutionTime.AtBarClosing)
                        isChecked = false;
                    break;
                case SlotTypes.CloseFilter:
                    if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.ExecutionTime == ExecutionTime.AtBarClosing)
                        isChecked = false;
                    break;
            }
            return isChecked;
        }

        /// <summary>
        ///     Saves the strategy in XML format.
        /// </summary>
        public void Save(string fileName)
        {
            StrategyName = Path.GetFileNameWithoutExtension(fileName);
            Symbol = Data.Symbol;
            DataPeriod = Data.Period;

            XmlDocument xmlDocStrategy = StrategyXML.CreateStrategyXmlDoc(this);

            try
            {
                xmlDocStrategy.Save(fileName); // Save the document to a file.
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        ///     Loads the strategy from a file in XML format.
        /// </summary>
        public static bool Load(string filename)
        {
            var xmlDocStrategy = new XmlDocument();

            try
            {
                xmlDocStrategy.Load(filename);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Language.T("Strategy Loading"));

                return false;
            }

            var strategyXML = new StrategyXML();


            try
            {
                Data.Strategy = strategyXML.ParseXmlStrategy(xmlDocStrategy);
            }
            catch (MissingIndicatorException exception)
            {
                string message = string.Format(
                    "Cannot load \"{2}\" strategy. {1} {0} {1} Please find this indicator in Repository or in Custom Indicators forum.",
                    exception.Message, Environment.NewLine, Path.GetFileNameWithoutExtension(filename));
                MessageBox.Show(message, "Load strategy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return false;
            }


            return true;
        }

        /// <summary>
        ///     Returns a copy of the current strategy.
        /// </summary>
        public Strategy Clone()
        {
            // Creates a new strategy.
            var tempStrategy = new Strategy(OpenFilters, CloseFilters)
                {
                    FirstBar = FirstBar,
                    OpenFilters = OpenFilters,
                    CloseFilters = CloseFilters,
                    StrategyName = StrategyName,
                    Description = Description,
                    Symbol = Symbol,
                    DataPeriod = DataPeriod,
                    SameSignalAction = SameSignalAction,
                    OppSignalAction = OppSignalAction,
                    UseAccountPercentEntry = UseAccountPercentEntry,
                    MaxOpenLots = MaxOpenLots,
                    EntryLots = EntryLots,
                    AddingLots = AddingLots,
                    ReducingLots = ReducingLots,
                    UseMartingale = UseMartingale,
                    MartingaleMultiplier = MartingaleMultiplier,
                    UsePermanentSL = UsePermanentSL,
                    PermanentSLType = PermanentSLType,
                    PermanentSL = PermanentSL,
                    UsePermanentTP = UsePermanentTP,
                    PermanentTPType = PermanentTPType,
                    PermanentTP = PermanentTP,
                    UseBreakEven = UseBreakEven,
                    BreakEven = BreakEven
                };

            // Reading the slots
            for (int slot = 0; slot < Slots; slot++)
                tempStrategy.Slot[slot] = Slot[slot].Clone();

            return tempStrategy;
        }
    }
}