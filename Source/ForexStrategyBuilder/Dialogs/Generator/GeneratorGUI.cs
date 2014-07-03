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
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder.Dialogs.Generator
{
    /// <summary>
    ///     Strategy Generator
    /// </summary>
    public sealed partial class Generator : Form
    {
        private readonly SmallBalanceChart balanceChart;
        private readonly BackgroundWorker bgWorker;
        private readonly Button btnAccept;
        private readonly Button btnCancel;
        private readonly Button btnGenerate;
        private readonly CheckBox chbGenerateNewStrategy;
        private readonly CheckBox chbInitialOptimization;
        private readonly CheckBox chbPreserveBreakEven;
        private readonly CheckBox chbPreserveOppDirAction;
        private readonly CheckBox chbPreservePermSL;
        private readonly CheckBox chbPreservePermTP;
        private readonly CheckBox chbPreserveSameDirAction;
        private readonly CheckBox chbWorkingMinutes;
        private readonly Color colorText;
        private readonly CriteriaControls criteriaControls;
        private readonly ScrollFlowPanel criteriaPanel;
        private readonly InfoPanel infpnlAccountStatistics;
        private readonly Label lblBenchmarkInfo;
        private readonly Label lblBenchmarkNumb;
        private readonly Label lblCalcStrInfo;
        private readonly Label lblCalcStrNumb;
        private readonly NumericUpDown nudWorkingMinutes;
        private readonly FancyPanel pnlCommon;
        private readonly FancyPanel pnlCriteriaBase;
        private readonly FancyPanel pnlIndicators;
        private readonly FancyPanel pnlSettings;
        private readonly FancyPanel pnlSorting;
        private readonly FancyPanel pnlTop10;
        private readonly ProgressBar progressBar;
        private readonly Random random = new Random();
        private readonly StrategyLayout strategyField;
        private readonly ToolStrip toolStrip;
        private readonly ToolTip toolTip = new ToolTip();
        private Button btnReset;
        private double buttonWidthMultiplier = 1; // It's used in OnResize().
        private ComboBox cbxCustomSortingAdvanced;
        private ComboBox cbxCustomSortingAdvancedCompareTo;
        private ComboBox cbxCustomSortingSimple;

        private CheckBox chbHideFsb;
        private CheckBox chbMaxClosingLogicSlots;
        private CheckBox chbMaxOpeningLogicSlots;
        private CheckBox chbOutOfSample;
        private CheckBox chbSaveStrategySlotStatus;
        private CheckBox chbUseDefaultIndicatorValues;

        private IndicatorsLayout indicatorsField;
        private bool isReset;
        private Label lblCustomSortingAdvancedCompareTo;

        private NumericUpDown nudMaxClosingLogicSlots;
        private NumericUpDown nudMaxOpeningLogicSlots;
        private NumericUpDown nudOutOfSample;
        private RadioButton rbnCustomSortingAdvanced;
        private RadioButton rbnCustomSortingNone;
        private RadioButton rbnCustomSortingSimple;
        private Top10Layout top10Field;
        private ToolStripButton tsbtLinkAll;
        private ToolStripButton tsbtLockAll;
        private ToolStripButton tsbtOverview;
        private ToolStripButton tsbtShowCriteria;
        private ToolStripButton tsbtShowIndicators;
        private ToolStripButton tsbtShowOptions;
        private ToolStripButton tsbtShowSettings;
        private ToolStripButton tsbtShowSorting;
        private ToolStripButton tsbtShowTop10;
        private ToolStripButton tsbtStrategyInfo;
        private ToolStripButton tsbtStrategySize1;
        private ToolStripButton tsbtStrategySize2;
        private ToolStripButton tsbtUnlockAll;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Generator()
        {
            GeneratedDescription = string.Empty;
            strategyBest = Data.Strategy.Clone();
            bestValue = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
            isGenerating = false;
            isStartegyChanged = false;
            indicatorBlackList = new List<string>();

            colorText = LayoutColors.ColorControlText;

            toolStrip = new ToolStrip();
            strategyField = new StrategyLayout(strategyBest);
            pnlCommon = new FancyPanel(Language.T("Common"));
            pnlCriteriaBase = new FancyPanel(Language.T("Acceptance Criteria"));
            criteriaPanel = new ScrollFlowPanel();
            criteriaControls = new CriteriaControls();
            pnlSettings = new FancyPanel(Language.T("Settings"));
            pnlSorting = new FancyPanel(Language.T("Custom Sorting"));
            pnlTop10 = new FancyPanel(Language.T("Top 10"));
            pnlIndicators = new FancyPanel(Language.T("Indicators"));
            balanceChart = new SmallBalanceChart();
            infpnlAccountStatistics = new InfoPanel();
            progressBar = new ProgressBar();
            lblCalcStrInfo = new Label();
            lblCalcStrNumb = new Label();
            lblBenchmarkInfo = new Label();
            lblBenchmarkNumb = new Label();
            btnAccept = new Button();
            btnGenerate = new Button();
            btnCancel = new Button();
            chbGenerateNewStrategy = new CheckBox();
            chbPreserveSameDirAction = new CheckBox();
            chbPreserveOppDirAction = new CheckBox();
            chbPreservePermSL = new CheckBox();
            chbPreservePermTP = new CheckBox();
            chbPreserveBreakEven = new CheckBox();
            chbInitialOptimization = new CheckBox();
            nudWorkingMinutes = new NumericUpDown();
            chbWorkingMinutes = new CheckBox();

            MaximizeBox = false;
            Icon = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor = LayoutColors.ColorFormBack;
            AcceptButton = btnGenerate;
            Text = Language.T("Strategy Generator") + " - " + Data.Symbol + " " + Data.PeriodString + ", " +
                   Data.Bars + " " + Language.T("bars");
            FormClosing += GeneratorFormClosing;

            // Tool Strip
            toolStrip.Parent = this;
            toolStrip.Dock = DockStyle.Top;
            toolStrip.AutoSize = true;

            // Creates a Strategy Layout
            strategyField.Parent = this;
            strategyField.ShowAddSlotButtons = false;
            strategyField.ShowRemoveSlotButtons = false;
            strategyField.ShowPadlockImg = true;
            strategyField.SlotPropertiesTipText = Language.T("Lock or unlock the slot.");
            strategyField.SlotToolTipText = Language.T("Lock, link, or unlock the slot.");

            pnlCommon.Parent = this;
            pnlCriteriaBase.Parent = this;
            pnlSettings.Parent = this;
            pnlSorting.Parent = this;
            pnlTop10.Parent = this;
            pnlIndicators.Parent = this;

            // Small Balance Chart
            balanceChart.Parent = this;
            balanceChart.BackColor = LayoutColors.ColorControlBack;
            balanceChart.Visible = true;
            balanceChart.Cursor = Cursors.Hand;
            balanceChart.IsContextButtonVisible = true;
            balanceChart.PopUpContextMenu.Items.AddRange(GetBalanceChartContextMenuItems());
            balanceChart.Click += AccountOutputClick;
            balanceChart.DoubleClick += AccountOutputClick;
            toolTip.SetToolTip(balanceChart, Language.T("Show account statistics."));
            balanceChart.SetChartData();

            // Info Panel Account Statistics
            infpnlAccountStatistics.Parent = this;
            infpnlAccountStatistics.Visible = false;
            infpnlAccountStatistics.Cursor = Cursors.Hand;
            infpnlAccountStatistics.IsContextButtonVisible = true;
            infpnlAccountStatistics.PopUpContextMenu.Items.AddRange(GetInfoPanelContextMenuItems());
            infpnlAccountStatistics.Click += AccountOutputClick;
            infpnlAccountStatistics.DoubleClick += AccountOutputClick;
            toolTip.SetToolTip(infpnlAccountStatistics, Language.T("Show account chart."));

            // ProgressBar
            progressBar.Parent = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step = 1;

            //Button Generate
            btnGenerate.Parent = this;
            btnGenerate.Name = "Generate";
            btnGenerate.Text = Language.T("Generate");
            btnGenerate.Click += BtnGenerateClick;
            btnGenerate.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent = this;
            btnAccept.Name = "Accept";
            btnAccept.Text = Language.T("Accept");
            btnAccept.Enabled = false;
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent = this;
            btnCancel.Text = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // BackgroundWorker
            bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bgWorker.DoWork += BgWorkerDoWork;
            bgWorker.ProgressChanged += BgWorkerProgressChanged;
            bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

            // Apply a Cryptographic Random Seed
            var rng = new RNGCryptoServiceProvider();
            var rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);
            random = new Random(rand);

            SetButtonsStrategy();
            SetButtonsGenerator();
            SetPanelCommon();
            SetCriteriaPanel();
            SetPanelSettings();
            SetPanelSorting();
            SetPanelTop10();
            SetPanelIndicators();
            LoadOptions();
            SetCustomSortingUI();
            SetStrategyDescriptionButton();

            chbHideFsb.CheckedChanged += HideFSBClick;

            System.Threading.Thread.Sleep(5000);

            if (Data.AutostartGenerator)
                BtnGenerateClick(this, new EventArgs());
        }

        public Form ParrentForm { private get; set; }

        /// <summary>
        ///     Gets the strategy description
        /// </summary>
        public string GeneratedDescription { get; private set; }

        /// <summary>
        ///     Whether the strategy was modified or entirely generated
        /// </summary>
        public bool IsStrategyModified { get; private set; }

        private void SetCriteriaPanel()
        {
            criteriaControls.BackColor = LayoutColors.ColorControlBack;
            criteriaControls.ForeColor = LayoutColors.ColorControlText;
            criteriaControls.SetCriteriaPanel();
            criteriaControls.SetSettings(Configs.CriteriaSettings);

            criteriaPanel.Parent = pnlCriteriaBase;
            criteriaPanel.Padding = new Padding(0);
            criteriaPanel.Margin = new Padding(0);
            criteriaPanel.ClearControls();
            criteriaPanel.AddControl(criteriaControls);
            criteriaPanel.SetControls();
        }

        private void SetPanelSorting()
        {
            rbnCustomSortingNone = new RadioButton
                {
                    Parent = pnlSorting,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Do not use custom sorting"),
                    AutoSize = true,
                    Checked = true,
                    Cursor = Cursors.Default
                };
            rbnCustomSortingNone.Click += RbnCustomSortingClick;

            rbnCustomSortingSimple = new RadioButton
                {
                    Parent = pnlSorting,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Simple"),
                    AutoSize = true,
                    Enabled = true,
                    Cursor = Cursors.Default
                };
            rbnCustomSortingSimple.Click += RbnCustomSortingClick;

            rbnCustomSortingAdvanced = new RadioButton
                {
                    Parent = pnlSorting,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Advanced"),
                    AutoSize = true,
                    Enabled = true,
                    Cursor = Cursors.Default
                };
            rbnCustomSortingAdvanced.Click += RbnCustomSortingClick;

            cbxCustomSortingSimple = new ComboBox
                {
                    Parent = pnlSorting,
                    AutoSize = true,
                    Cursor = Cursors.Default,
                    Enabled = true,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

            cbxCustomSortingAdvanced = new ComboBox
                {
                    Parent = pnlSorting,
                    AutoSize = true,
                    Cursor = Cursors.Default,
                    Enabled = true,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

            lblCustomSortingAdvancedCompareTo = new Label
                {
                    Parent = pnlSorting,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Comparison Curve"),
                    AutoSize = true,
                    Cursor = Cursors.Default,
                    Enabled = true
                };

            cbxCustomSortingAdvancedCompareTo = new ComboBox
                {
                    Parent = pnlSorting,
                    AutoSize = true,
                    Cursor = Cursors.Default,
                    Enabled = true,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

            // Set the Simple Custom Sorting Options
            List<string> simpleCustomSortingOptions = GetSimpleCustomSortingOptions();
            foreach (string option in simpleCustomSortingOptions)
                cbxCustomSortingSimple.Items.Add(option);

            if (simpleCustomSortingOptions.Count > 0)
            {
                customSortingSimpleEnabled = true;
                cbxCustomSortingSimple.Text = simpleCustomSortingOptions[0];
            }

            // Set the Advanced Custom Sorting Options
            List<string> advancedCustomSortingOptions = GetAdvancedCustomSortingOptions();
            foreach (string option in advancedCustomSortingOptions)
                cbxCustomSortingAdvanced.Items.Add(option);

            if (advancedCustomSortingOptions.Count > 0)
            {
                customSortingAdvancedEnabled = true;
                cbxCustomSortingAdvanced.Text = advancedCustomSortingOptions[0];
            }
            else
            {
                rbnCustomSortingAdvanced.Visible = false;
                cbxCustomSortingAdvanced.Visible = false;
                lblCustomSortingAdvancedCompareTo.Visible = false;
                cbxCustomSortingAdvancedCompareTo.Visible = false;
            }

            cbxCustomSortingAdvancedCompareTo.Items.Add("Balance");
            cbxCustomSortingAdvancedCompareTo.Items.Add("Balance (with Transfers)");
            cbxCustomSortingAdvancedCompareTo.Items.Add("Equity");
            cbxCustomSortingAdvancedCompareTo.Items.Add("Equity (with Transfers)");
            cbxCustomSortingAdvancedCompareTo.Text = "Balance";
        }

        /// <summary>
        ///     Loads and parses the generator's options.
        /// </summary>
        private void LoadOptions()
        {
            if (Data.WorkingMinutes != 0)
            {
                chbWorkingMinutes.Checked = true;
                nudWorkingMinutes.Value = Data.WorkingMinutes;
            }

            if (string.IsNullOrEmpty(Configs.GeneratorOptions))
                return;

            string[] options = Configs.GeneratorOptions.Split(';');
            int i = 0;
            try
            {
                chbGenerateNewStrategy.Checked = bool.Parse(options[i++]);
                chbPreserveSameDirAction.Checked = bool.Parse(options[i++]);
                chbPreserveOppDirAction.Checked = bool.Parse(options[i++]);
                chbPreservePermSL.Checked = bool.Parse(options[i++]);
                chbPreservePermTP.Checked = bool.Parse(options[i++]);
                chbPreserveBreakEven.Checked = bool.Parse(options[i++]);
                chbInitialOptimization.Checked = bool.Parse(options[i++]);
                chbMaxOpeningLogicSlots.Checked = bool.Parse(options[i++]);
                nudMaxOpeningLogicSlots.Value = Math.Min(int.Parse(options[i++]), Strategy.MaxOpenFilters);
                chbMaxClosingLogicSlots.Checked = bool.Parse(options[i++]);
                nudMaxClosingLogicSlots.Value = Math.Min(int.Parse(options[i++]), Strategy.MaxCloseFilters);
                chbOutOfSample.Checked = bool.Parse(options[i++]);
                nudOutOfSample.Value = int.Parse(options[i++]);
                chbWorkingMinutes.Checked = bool.Parse(options[i++]);
                if (Data.WorkingMinutes > 0)                                    
                    i++;
                 else
                    nudWorkingMinutes.Value = int.Parse(options[i++]);
                chbUseDefaultIndicatorValues.Checked = bool.Parse(options[i++]);
                chbSaveStrategySlotStatus.Checked = bool.Parse(options[i++]);
                chbHideFsb.Checked = bool.Parse(options[i++]);
                rbnCustomSortingAdvanced.Checked = bool.Parse(options[i++]);
                rbnCustomSortingSimple.Checked = bool.Parse(options[i++]);
                rbnCustomSortingNone.Checked = bool.Parse(options[i++]);
                cbxCustomSortingSimple.Text = options[i++];
                cbxCustomSortingAdvanced.Text = options[i++];
                cbxCustomSortingAdvancedCompareTo.Text = options[i];
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///     Saves the generator's options.
        /// </summary>
        private void SaveOptions()
        {
            string options =
                chbGenerateNewStrategy.Checked + ";" +
                chbPreserveSameDirAction.Checked + ";" +
                chbPreserveOppDirAction.Checked + ";" +
                chbPreservePermSL.Checked + ";" +
                chbPreservePermTP.Checked + ";" +
                chbPreserveBreakEven.Checked + ";" +
                chbInitialOptimization.Checked + ";" +
                chbMaxOpeningLogicSlots.Checked + ";" +
                nudMaxOpeningLogicSlots.Value + ";" +
                chbMaxClosingLogicSlots.Checked + ";" +
                nudMaxClosingLogicSlots.Value + ";" +
                chbOutOfSample.Checked + ";" +
                nudOutOfSample.Value + ";" +
                chbWorkingMinutes.Checked + ";" +
                nudWorkingMinutes.Value + ";" +
                chbUseDefaultIndicatorValues.Checked + ";" +
                chbSaveStrategySlotStatus.Checked + ";" +
                chbHideFsb.Checked + ";" +
                rbnCustomSortingAdvanced.Checked + ";" +
                rbnCustomSortingSimple.Checked + ";" +
                rbnCustomSortingNone.Checked + ";" +
                cbxCustomSortingSimple.Text + ";" +
                cbxCustomSortingAdvanced.Text + ";" +
                cbxCustomSortingAdvancedCompareTo.Text;

            Configs.GeneratorOptions = options;
        }

        /// <summary>
        ///     Sets controls in panel Common
        /// </summary>
        private void SetPanelCommon()
        {
            // chbPreserveSameDirAction
            chbPreserveSameDirAction.Parent = pnlCommon;
            chbPreserveSameDirAction.Text = Language.T("Do not change the Same direction signal");
            chbPreserveSameDirAction.AutoSize = true;
            chbPreserveSameDirAction.Checked = true;
            chbPreserveSameDirAction.ForeColor = LayoutColors.ColorControlText;
            chbPreserveSameDirAction.BackColor = Color.Transparent;

            // chbPreserveOppDirAction
            chbPreserveOppDirAction.Parent = pnlCommon;
            chbPreserveOppDirAction.Text = Language.T("Do not change the Opposite direction signal");
            chbPreserveOppDirAction.AutoSize = true;
            chbPreserveOppDirAction.Checked = true;
            chbPreserveOppDirAction.ForeColor = LayoutColors.ColorControlText;
            chbPreserveOppDirAction.BackColor = Color.Transparent;

            // chbPreservPermSL
            chbPreservePermSL.Parent = pnlCommon;
            chbPreservePermSL.Text = Language.T("Do not change the Permanent Stop Loss");
            chbPreservePermSL.AutoSize = true;
            chbPreservePermSL.Checked = true;
            chbPreservePermSL.ForeColor = LayoutColors.ColorControlText;
            chbPreservePermSL.BackColor = Color.Transparent;

            // chbPreservPermTP
            chbPreservePermTP.Parent = pnlCommon;
            chbPreservePermTP.Text = Language.T("Do not change the Permanent Take Profit");
            chbPreservePermTP.AutoSize = true;
            chbPreservePermTP.Checked = true;
            chbPreservePermTP.ForeColor = LayoutColors.ColorControlText;
            chbPreservePermTP.BackColor = Color.Transparent;

            // chbPreservbreakEven
            chbPreserveBreakEven.Parent = pnlCommon;
            chbPreserveBreakEven.Text = Language.T("Do not change the Break Even");
            chbPreserveBreakEven.AutoSize = true;
            chbPreserveBreakEven.Checked = true;
            chbPreserveBreakEven.ForeColor = LayoutColors.ColorControlText;
            chbPreserveBreakEven.BackColor = Color.Transparent;

            chbMaxOpeningLogicSlots = new CheckBox
                {
                    Parent = pnlCommon,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum number of opening logic slots"),
                    Checked = true,
                    AutoSize = true
                };

            nudMaxOpeningLogicSlots = new NumericUpDown {Parent = pnlCommon, TextAlign = HorizontalAlignment.Center};
            nudMaxOpeningLogicSlots.BeginInit();
            nudMaxOpeningLogicSlots.Minimum = 0;
            nudMaxOpeningLogicSlots.Maximum = Strategy.MaxOpenFilters;
            nudMaxOpeningLogicSlots.Increment = 1;
            nudMaxOpeningLogicSlots.Value = 2;
            nudMaxOpeningLogicSlots.EndInit();

            chbMaxClosingLogicSlots = new CheckBox
                {
                    Parent = pnlCommon,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Maximum number of closing logic slots"),
                    Checked = true,
                    AutoSize = true
                };

            nudMaxClosingLogicSlots = new NumericUpDown {Parent = pnlCommon, TextAlign = HorizontalAlignment.Center};
            nudMaxClosingLogicSlots.BeginInit();
            nudMaxClosingLogicSlots.Minimum = 0;
            nudMaxClosingLogicSlots.Maximum = Strategy.MaxCloseFilters;
            nudMaxClosingLogicSlots.Increment = 1;
            nudMaxClosingLogicSlots.Value = 1;
            nudMaxClosingLogicSlots.EndInit();

            // Label Calculated Strategies Caption
            lblCalcStrInfo.Parent = pnlCommon;
            lblCalcStrInfo.AutoSize = true;
            lblCalcStrInfo.ForeColor = LayoutColors.ColorControlText;
            lblCalcStrInfo.BackColor = Color.Transparent;
            lblCalcStrInfo.Text = Language.T("Calculations");

            // Label Calculated Strategies Number
            lblCalcStrNumb.Parent = pnlCommon;
            lblCalcStrNumb.BorderStyle = BorderStyle.FixedSingle;
            lblCalcStrNumb.ForeColor = LayoutColors.ColorControlText;
            lblCalcStrNumb.BackColor = LayoutColors.ColorControlBack;
            lblCalcStrNumb.TextAlign = ContentAlignment.MiddleCenter;
            lblCalcStrNumb.Text = "0";

            // Benchmark
            lblBenchmarkInfo.Parent = pnlCommon;
            lblBenchmarkInfo.AutoSize = true;
            lblBenchmarkInfo.ForeColor = LayoutColors.ColorControlText;
            lblBenchmarkInfo.BackColor = Color.Transparent;
            lblBenchmarkInfo.Text = Language.T("Benchmark");

            lblBenchmarkNumb.Parent = pnlCommon;
            lblBenchmarkNumb.BorderStyle = BorderStyle.FixedSingle;
            lblBenchmarkNumb.ForeColor = LayoutColors.ColorControlText;
            lblBenchmarkNumb.BackColor = LayoutColors.ColorControlBack;
            lblBenchmarkNumb.TextAlign = ContentAlignment.MiddleCenter;
            lblBenchmarkNumb.Text = "0";
        }

        /// <summary>
        ///     Sets controls in panel Settings
        /// </summary>
        private void SetPanelSettings()
        {
            //chbWorkingMinutes
            chbWorkingMinutes.Parent = pnlSettings;
            chbWorkingMinutes.Text = Language.T("Working time limit in minutes");
            chbWorkingMinutes.AutoSize = true;
            chbWorkingMinutes.Checked = true;
            chbWorkingMinutes.ForeColor = LayoutColors.ColorControlText;
            chbWorkingMinutes.BackColor = Color.Transparent;

            // numUpDownWorkingTime
            nudWorkingMinutes.Parent = pnlSettings;
            nudWorkingMinutes.Value = 5;
            nudWorkingMinutes.Minimum = 1;
            nudWorkingMinutes.Maximum = 10000;
            nudWorkingMinutes.TextAlign = HorizontalAlignment.Center;

            // chbGenerateNewStrategy
            chbGenerateNewStrategy.Parent = pnlSettings;
            chbGenerateNewStrategy.Text = Language.T("Generate from zero profit at every start");
            chbGenerateNewStrategy.AutoSize = true;
            chbGenerateNewStrategy.Checked = true;
            chbGenerateNewStrategy.ForeColor = LayoutColors.ColorControlText;
            chbGenerateNewStrategy.BackColor = Color.Transparent;

            // chbPseudoOpt
            chbInitialOptimization.Parent = pnlSettings;
            chbInitialOptimization.Text = Language.T("Perform an initial optimization");
            chbInitialOptimization.AutoSize = true;
            chbInitialOptimization.Checked = true;
            chbInitialOptimization.ForeColor = LayoutColors.ColorControlText;
            chbInitialOptimization.BackColor = Color.Transparent;

            chbOutOfSample = new CheckBox
                {
                    Parent = pnlSettings,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Out of sample testing, percent of OOS bars"),
                    Checked = false,
                    AutoSize = true
                };
            chbOutOfSample.CheckedChanged += ChbOutOfSampleCheckedChanged;

            nudOutOfSample = new NumericUpDown {Parent = pnlSettings, TextAlign = HorizontalAlignment.Center};
            nudOutOfSample.BeginInit();
            nudOutOfSample.Minimum = 10;
            nudOutOfSample.Maximum = 60;
            nudOutOfSample.Increment = 1;
            nudOutOfSample.Value = 30;
            nudOutOfSample.EndInit();
            nudOutOfSample.ValueChanged += NudOutOfSampleValueChanged;

            chbUseDefaultIndicatorValues = new CheckBox
                {
                    Parent = pnlSettings,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Only use default numeric indicator values"),
                    Checked = false,
                    AutoSize = true
                };

            chbSaveStrategySlotStatus = new CheckBox
                {
                    Parent = pnlSettings,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Save strategy slots Lock status"),
                    Checked = false,
                    AutoSize = true
                };

            chbHideFsb = new CheckBox
                {
                    Parent = pnlSettings,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Hide FSB when Generator starts"),
                    Checked = true,
                    AutoSize = true,
                    Cursor = Cursors.Default
                };

            btnReset = new Button
                {
                    Parent = pnlSettings,
                    UseVisualStyleBackColor = true,
                    Text = Language.T("Reset all parameters and settings")
                };
            btnReset.Click += BtnResetClick;
        }

        /// <summary>
        ///     Sets controls in panel Top 10
        /// </summary>
        private void SetPanelTop10()
        {
            top10Field = new Top10Layout(10) {Parent = pnlTop10};
        }

        /// <summary>
        ///     Sets controls in panel Indicators
        /// </summary>
        private void SetPanelIndicators()
        {
            indicatorsField = new IndicatorsLayout {Parent = pnlIndicators};
        }

        /// <summary>
        ///     Sets tool strip buttons
        /// </summary>
        private void SetButtonsStrategy()
        {
            tsbtLockAll = new ToolStripButton
                {
                    Name = "tsbtLockAll",
                    Image = Resources.padlock_img,
                    ToolTipText = Language.T("Lock all slots.")
                };
            tsbtLockAll.Click += ChangeSlotStatus;
            toolStrip.Items.Add(tsbtLockAll);

            tsbtUnlockAll = new ToolStripButton
                {
                    Name = "tsbtUnlockAll",
                    Image = Resources.open_padlock_img,
                    ToolTipText = Language.T("Unlock all slots.")
                };
            tsbtUnlockAll.Click += ChangeSlotStatus;
            toolStrip.Items.Add(tsbtUnlockAll);

            tsbtLinkAll = new ToolStripButton
                {
                    Name = "tsbtLinkAll",
                    Image = Resources.linked,
                    ToolTipText = Language.T("Link all slots.")
                };
            tsbtLinkAll.Click += ChangeSlotStatus;
            toolStrip.Items.Add(tsbtLinkAll);

            toolStrip.Items.Add(new ToolStripSeparator());

            // Button Overview
            tsbtOverview = new ToolStripButton
                {
                    Name = "Overview",
                    Text = Language.T("Overview"),
                    ToolTipText = Language.T("See the strategy overview.")
                };
            tsbtOverview.Click += ShowOverview;
            toolStrip.Items.Add(tsbtOverview);

            toolStrip.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategySize1
            tsbtStrategySize1 = new ToolStripButton
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.slot_size_max,
                    Tag = 1,
                    ToolTipText = Language.T("Show detailed info in the slots."),
                };
            tsbtStrategySize1.Click += BtnSlotSizeClick;
            toolStrip.Items.Add(tsbtStrategySize1);

            // Button tsbtStrategySize2
            tsbtStrategySize2 = new ToolStripButton
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.slot_size_min,
                    Tag = 2,
                    ToolTipText = Language.T("Show minimum info in the slots."),
                };
            tsbtStrategySize2.Click += BtnSlotSizeClick;
            toolStrip.Items.Add(tsbtStrategySize2);

            // Button tsbtStrategyInfo
            tsbtStrategyInfo = new ToolStripButton
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.str_info_infook,
                    ToolTipText = Language.T("Show the strategy description."),
                };
            tsbtStrategyInfo.Click += BtnStrategyDescriptionClick;
            toolStrip.Items.Add(tsbtStrategyInfo);

            toolStrip.Items.Add(new ToolStripSeparator());
        }

        /// <summary>
        ///     Sets tool strip buttons
        /// </summary>
        private void SetButtonsGenerator()
        {
            // Button Options
            tsbtShowOptions = new ToolStripButton
                {
                    Name = "tsbtShowOptions",
                    Text = Language.T("Common"),
                    Enabled = false
                };
            tsbtShowOptions.Click += ChangeGeneratorPanel;
            toolStrip.Items.Add(tsbtShowOptions);

            // Button Criteria
            tsbtShowCriteria = new ToolStripButton {Name = "tsbtShowCriteria", Text = Language.T("Criteria")};
            tsbtShowCriteria.Click += ChangeGeneratorPanel;
            toolStrip.Items.Add(tsbtShowCriteria);

            // Button Settings
            tsbtShowSettings = new ToolStripButton {Name = "tsbtShowSettings", Text = Language.T("Settings")};
            tsbtShowSettings.Click += ChangeGeneratorPanel;
            toolStrip.Items.Add(tsbtShowSettings);

            // Button Sorting
            tsbtShowSorting = new ToolStripButton {Name = "tsbtShowSorting", Text = Language.T("Sorting")};
            tsbtShowSorting.Click += ChangeGeneratorPanel;
            toolStrip.Items.Add(tsbtShowSorting);

            // Button Top10
            tsbtShowTop10 = new ToolStripButton {Name = "tsbtShowTop10", Text = Language.T("Top 10")};
            tsbtShowTop10.Click += ChangeGeneratorPanel;
            toolStrip.Items.Add(tsbtShowTop10);

            // Button Indicators
            tsbtShowIndicators = new ToolStripButton {Name = "tsbtIndicators", Text = Language.T("Indicators")};
            tsbtShowIndicators.Click += ChangeGeneratorPanel;
            toolStrip.Items.Add(tsbtShowIndicators);
        }

        /// <summary>
        ///     Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ParrentForm.Visible = !chbHideFsb.Checked;

            // Find correct size
            int maxCheckBoxWidth = 250;
            foreach (Control control in pnlCommon.Controls)
            {
                if (maxCheckBoxWidth < control.Width)
                    maxCheckBoxWidth = control.Width;
            }
            foreach (Control control in criteriaControls.Controls)
            {
                if (maxCheckBoxWidth < control.Width)
                    maxCheckBoxWidth = control.Width;
            }

            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            const int nudWidth = 55;
            pnlCommon.Width = 3*buttonWidth + 2*btnHrzSpace;
            int borderWidth = (pnlCommon.Width - pnlCommon.ClientSize.Width)/2;
            maxCheckBoxWidth = maxCheckBoxWidth + 2 + SystemInformation.VerticalScrollBarWidth;

            if (maxCheckBoxWidth + 3*btnHrzSpace + nudWidth + 4 > pnlCommon.ClientSize.Width)
                buttonWidthMultiplier = ((maxCheckBoxWidth + nudWidth + 3*btnHrzSpace + 2*borderWidth + 4)/3.0)/
                                        buttonWidth;
            var height = (int) (528*Data.VDpiScale);
            ClientSize = new Size(2*((int) (3*buttonWidth*buttonWidthMultiplier) + 2*btnHrzSpace) + 3*btnHrzSpace,
                                  height);

            OnResize(e);

            RebuildStrategyLayout(strategyBest);
            RefreshAccountStatistics();
            Top10AddStrategy();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*60*buttonWidthMultiplier);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int border = btnHrzSpace;
            int rightSideWidth = 3*buttonWidth + 2*btnHrzSpace;
            int rightSideLocation = ClientSize.Width - rightSideWidth - btnHrzSpace;
            int leftSideWidth = ClientSize.Width - 3*buttonWidth - 5*btnHrzSpace;
            const int nudWidth = 55;

            var height = (int)(228 * Data.VDpiScale);
            int optionsHeight = height;

            //Button Cancel
            btnCancel.Size = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Accept
            btnAccept.Size = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Generate
            btnGenerate.Size = new Size(buttonWidth, buttonHeight);
            btnGenerate.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace,
                                             ClientSize.Height - buttonHeight - btnVertSpace);

            // Progress Bar
            progressBar.Size = new Size(ClientSize.Width - leftSideWidth - 3*border, (int) (Data.VerticalDlu*9));
            progressBar.Location = new Point(leftSideWidth + 2*border, btnAccept.Top - progressBar.Height - btnVertSpace);

            // Panel Common
            pnlCommon.Size = new Size(rightSideWidth, optionsHeight);
            pnlCommon.Location = new Point(rightSideLocation, toolStrip.Bottom + border);

            // Panel pnlCriteriaBase
            pnlCriteriaBase.Size = new Size(rightSideWidth, optionsHeight);
            pnlCriteriaBase.Location = new Point(rightSideLocation, toolStrip.Bottom + border);

            // Panel Settings
            pnlSettings.Size = new Size(rightSideWidth, optionsHeight);
            pnlSettings.Location = new Point(rightSideLocation, toolStrip.Bottom + border);

            // Panel Sorting
            pnlSorting.Size = new Size(rightSideWidth, optionsHeight);
            pnlSorting.Location = new Point(rightSideLocation, toolStrip.Bottom + border);

            // Panel Top 10
            pnlTop10.Size = new Size(rightSideWidth, optionsHeight);
            pnlTop10.Location = new Point(rightSideLocation, toolStrip.Bottom + border);

            // Panel Indicators
            pnlIndicators.Size = new Size(rightSideWidth, optionsHeight);
            pnlIndicators.Location = new Point(rightSideLocation, toolStrip.Bottom + border);

            // Panel StrategyLayout
            strategyField.Size = new Size(leftSideWidth, ClientSize.Height - border - toolStrip.Bottom - border);
            strategyField.Location = new Point(border, toolStrip.Bottom + border);

            // Panel Balance Chart
            balanceChart.Size = new Size(ClientSize.Width - leftSideWidth - 3*border,
                                         progressBar.Top - 3*border - pnlCommon.Bottom);
            balanceChart.Location = new Point(strategyField.Right + border, pnlCommon.Bottom + border);

            // Account Statistics
            infpnlAccountStatistics.Size = new Size(ClientSize.Width - leftSideWidth - 3*border,
                                                    progressBar.Top - 3*border - pnlCommon.Bottom);
            infpnlAccountStatistics.Location = new Point(strategyField.Right + border, pnlCommon.Bottom + border);

            int nudLeft = pnlCommon.ClientSize.Width - border - 2 - nudWidth;

            // Strategy Properties
            chbPreserveSameDirAction.Location = new Point(border + 2, 26);
            chbPreserveOppDirAction.Location = new Point(border + 2, chbPreserveSameDirAction.Bottom + border + 4);
            chbPreservePermSL.Location = new Point(border + 2, chbPreserveOppDirAction.Bottom + border + 4);
            chbPreservePermTP.Location = new Point(border + 2, chbPreservePermSL.Bottom + border + 4);
            chbPreserveBreakEven.Location = new Point(border + 2, chbPreservePermTP.Bottom + border + 4);

            // chbMaxOpeningLogicSlots
            chbMaxOpeningLogicSlots.Location = new Point(border + 2, chbPreserveBreakEven.Bottom + border + 4);
            nudMaxOpeningLogicSlots.Width = nudWidth;
            nudMaxOpeningLogicSlots.Location = new Point(nudLeft, chbMaxOpeningLogicSlots.Top - 1);

            // chbMaxClosingLogicSlots
            chbMaxClosingLogicSlots.Location = new Point(border + 2, chbMaxOpeningLogicSlots.Bottom + border + 4);
            nudMaxClosingLogicSlots.Width = nudWidth;
            nudMaxClosingLogicSlots.Location = new Point(nudLeft, chbMaxClosingLogicSlots.Top - 1);

            // Labels Strategy Calculations
            lblCalcStrInfo.Location = new Point(border - 1, pnlCommon.Height - nudMaxOpeningLogicSlots.Height - border);
            lblCalcStrNumb.Size = new Size(nudWidth, nudMaxOpeningLogicSlots.Height - 1);
            lblCalcStrNumb.Location = new Point(lblCalcStrInfo.Right + border, lblCalcStrInfo.Top - 3);

            // Labels Benchmark
            lblBenchmarkInfo.Location = new Point(lblBenchmarkNumb.Left - lblBenchmarkInfo.Width - border,
                                                  pnlCommon.Height - nudMaxOpeningLogicSlots.Height - border);
            lblBenchmarkNumb.Size = new Size(nudWidth, nudMaxOpeningLogicSlots.Height - 1);
            lblBenchmarkNumb.Location = new Point(nudLeft, lblCalcStrNumb.Top);


            // Working Minutes
            chbWorkingMinutes.Location = new Point(border + 2, 26);
            nudWorkingMinutes.Width = nudWidth;
            nudWorkingMinutes.Location = new Point(nudLeft, chbWorkingMinutes.Top - 1);

            //chbGenerateNewStrategy
            chbGenerateNewStrategy.Location = new Point(border + 2, chbWorkingMinutes.Bottom + border + 4);

            // chbInitialOptimization
            chbInitialOptimization.Location = new Point(border + 2, chbGenerateNewStrategy.Bottom + border + 4);

            // chbOutOfSample
            chbOutOfSample.Location = new Point(border + 2, chbInitialOptimization.Bottom + border + 4);
            nudOutOfSample.Width = nudWidth;
            nudOutOfSample.Location = new Point(nudLeft, chbOutOfSample.Top - 1);

            // Use default indicator values
            chbUseDefaultIndicatorValues.Location = new Point(border + 2, chbOutOfSample.Bottom + border + 4);

            // Save strategy slot status
            chbSaveStrategySlotStatus.Location = new Point(border + 2, chbUseDefaultIndicatorValues.Bottom + border + 4);

            // Hide FSB when generator starts
            chbHideFsb.Location = new Point(border + 2, chbSaveStrategySlotStatus.Bottom + border + 4);


            // Custom Sorting Radio Buttons
            rbnCustomSortingNone.Location = new Point(border + 2, 25);
            rbnCustomSortingSimple.Location = new Point(border + 2, rbnCustomSortingNone.Bottom + border + 4);
            rbnCustomSortingAdvanced.Location = new Point(border + 2, rbnCustomSortingSimple.Bottom + border + 4);

            // Custom Sorting (Simple) Combo Box
            cbxCustomSortingSimple.Width = 180;
            cbxCustomSortingSimple.Location =
                new Point(pnlSorting.ClientSize.Width - cbxCustomSortingSimple.Width - border - 2,
                          rbnCustomSortingSimple.Top - 1);

            // Custom Sorting (Advanced) Combo Box
            cbxCustomSortingAdvanced.Location = new Point(cbxCustomSortingSimple.Left, rbnCustomSortingAdvanced.Top - 1);
            cbxCustomSortingAdvanced.Width = cbxCustomSortingSimple.Width;

            // Custom Sorting (Advanced Comparison Curve) Combo Box
            cbxCustomSortingAdvancedCompareTo.Location = new Point(cbxCustomSortingAdvanced.Left,
                                                                   cbxCustomSortingAdvanced.Bottom + border);
            cbxCustomSortingAdvancedCompareTo.Width = cbxCustomSortingAdvanced.Width;

            // Custom Sorting (Advanced Comparison Curve) Label
            lblCustomSortingAdvancedCompareTo.Location = new Point(rbnCustomSortingAdvanced.Left + 17,
                                                                   cbxCustomSortingAdvancedCompareTo.Top + 2);

            // Button Reset
            btnReset.Width = pnlSettings.ClientSize.Width - 2*(border + 2);
            btnReset.Location = new Point(border + 2, pnlSettings.Height - btnReset.Height - border - 2);

            // Top 10 Layout
            top10Field.Size = new Size(pnlTop10.Width - 2*2, pnlTop10.Height - (int) pnlTop10.CaptionHeight - 2);
            top10Field.Location = new Point(2, (int) pnlTop10.CaptionHeight);

            // Criteria Panel
            criteriaPanel.Size = new Size(pnlCriteriaBase.Width - 2*2,
                                          pnlCriteriaBase.Height - (int) pnlCriteriaBase.CaptionHeight - 2);
            criteriaPanel.Location = new Point(2, (int) pnlCriteriaBase.CaptionHeight);

            // Indicators Layout
            indicatorsField.Size = new Size(pnlIndicators.Width - 2*2,
                                            pnlIndicators.Height - (int) pnlIndicators.CaptionHeight - 2);
            indicatorsField.Location = new Point(2, (int) pnlIndicators.CaptionHeight);
        }

        /// <summary>
        ///     Check whether the strategy have been changed
        /// </summary>
        private void GeneratorFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReset)
            {
                SaveOptions();
                Configs.CriteriaSettings = criteriaControls.GetSettings();
            }

            if (isGenerating)
            {
                // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
                return;
            }            

            if (DialogResult == DialogResult.Cancel && isStartegyChanged && !Data.AutoSave)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the generated strategy?"),
                                                  Data.ProgramName, MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question);

                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        DialogResult = DialogResult.OK;
                        break;
                    case DialogResult.No:
                        DialogResult = DialogResult.Cancel;
                        break;
                }
            }
            else if (DialogResult == DialogResult.OK && !isStartegyChanged)
            {
                DialogResult = DialogResult.Cancel;
            }

            if (!isReset)
                indicatorsField.SetConfigFile();

            if (!chbSaveStrategySlotStatus.Checked)
                Data.Strategy = ClearStrategySlotsStatus(Data.Strategy);

            ParrentForm.Visible = true;            
        }

        /// <summary>
        ///     Refreshes the balance chart
        /// </summary>
        private void RefreshSmallBalanceChart()
        {
            if (balanceChart.InvokeRequired)
            {
                Invoke(new DelegateRefreshBalanceChart(RefreshSmallBalanceChart), new object[] {});
            }
            else
            {
                balanceChart.SetChartData();
                balanceChart.InitChart();
                balanceChart.Invalidate();
            }
        }

        /// <summary>
        ///     Refreshes the AccountStatistics
        /// </summary>
        private void RefreshAccountStatistics()
        {
            if (infpnlAccountStatistics.InvokeRequired)
            {
                Invoke(new DelegateRefreshAccountStatisticas(RefreshAccountStatistics), new object[] {});
            }
            else
            {
                infpnlAccountStatistics.Update(
                    Backtester.AccountStatsParam,
                    Backtester.AccountStatsValue,
                    Backtester.AccountStatsFlags,
                    Language.T("Account Statistics"));
            }
        }

        /// <summary>
        ///     Creates a new strategy layout according to the given strategy.
        /// </summary>
        private void RebuildStrategyLayout(Strategy strategy)
        {
            if (strategyField.InvokeRequired)
            {
                Invoke(new DelegateRebuildStrategyLayout(RebuildStrategyLayout), new object[] {strategy});
            }
            else
            {
                strategyField.RebuildStrategyControls(strategy);

                strategyField.PanelProperties.Click += PnlPropertiesClick;
                strategyField.PanelProperties.DoubleClick += PnlPropertiesClick;
                for (int slot = 0; slot < strategy.Slots; slot++)
                {
                    strategyField.SlotPanelsList[slot].Click += PnlSlotClick;
                    strategyField.SlotPanelsList[slot].DoubleClick += PnlSlotClick;
                }
            }
        }

        /// <summary>
        ///     Sets the lblCalcStrNumb.Text
        /// </summary>
        private void SetLabelCyclesText(string text)
        {
            customAnalytics.Cycles = Convert.ToInt32(text);

            if (lblCalcStrNumb.InvokeRequired)
            {
                BeginInvoke(new SetCyclesCallback(SetLabelCyclesText), new object[] {text});
            }
            else
            {
                lblCalcStrNumb.Text = text;
            }
        }

        /// <summary>
        ///     Sets Benchmark
        /// </summary>
        private void SetBenchmarkText(double value)
        {
            if (lblBenchmarkNumb.InvokeRequired)
            {
                BeginInvoke(new SetBenchmarkCallback(SetBenchmarkText), new object[] {value});
            }
            else
            {
                lblBenchmarkNumb.Text = ((int) Math.Round(value)).ToString(CultureInfo.InvariantCulture);
            }
        }

        private ToolStripItem[] GetBalanceChartContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
                {
                    Image = Resources.info_panel,
                    Text = Language.T("Account Statistics")
                };
            mi1.Click += AccountOutputClick;

            var itemCollection = new ToolStripItem[]
                {
                    mi1
                };

            return itemCollection;
        }

        private ToolStripItem[] GetInfoPanelContextMenuItems()
        {
            var mi1 = new ToolStripMenuItem
                {
                    Image = Resources.chart_balance_equity,
                    Text = Language.T("Account Chart")
                };
            mi1.Click += AccountOutputClick;

            var itemCollection = new ToolStripItem[]
                {
                    mi1
                };

            return itemCollection;
        }

        /// <summary>
        ///     Composes an informative error message. It presumes that the reason for the error is a custom indicator. Ohhh!!
        /// </summary>
        private string GenerateCalculationErrorMessage(string exceptionMessage)
        {
            string text = "<h1>Error: " + exceptionMessage + "</h1>";
            string customIndicators = "";
            int customIndCount = 0;

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                string indName = Data.Strategy.Slot[slot].IndicatorName;
                Indicator indicator = IndicatorManager.ConstructIndicator(indName);
                indicator.Initialize(Data.Strategy.Slot[slot].SlotType);
                if (indicator.CustomIndicator)
                {
                    customIndCount++;
                    indicatorBlackList.Add(indName);
                    customIndicators += "<li>" + Data.Strategy.Slot[slot].IndicatorName + "</li>" + Environment.NewLine;
                }
            }

            if (customIndCount > 0)
            {
                string plural = (customIndCount > 1 ? "s" : "");

                text +=
                    "<p>" +
                    "An error occurred when calculating the strategy." + " " +
                    "The error can be a result of the following custom indicator" + plural + ":" +
                    "</p>" +
                    "<ul>" +
                    customIndicators +
                    "</ul>" +
                    "<p>" +
                    "Please report this error to the author of the indicator" + plural + "!<br />" +
                    "You may remove this indicator" + plural + " from the Custom Indicators folder." +
                    "</p>";
            }
            else
            {
                text +=
                    "<p>" +
                    "Please report this error in the support forum!" +
                    "</p>";
            }

            return text;
        }

        /// <summary>
        ///     Report Indicator Error
        /// </summary>
        private void ReportIndicatorError(string text, string caption)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateReportIndicatorError(ReportIndicatorError), new object[] {text, caption});
            }
            else
            {
                var msgBox = new FancyMessageBox(text, caption) {BoxWidth = 450, BoxHeight = 250};
                msgBox.Show();
            }
        }

        /// <summary>
        ///     Out of Sample
        /// </summary>
        private void NudOutOfSampleValueChanged(object sender, EventArgs e)
        {
            SetOOS();
            balanceChart.OOSBar = barOOS;

            if (!isOOS) return;
            balanceChart.SetChartData();
            balanceChart.InitChart();
            balanceChart.Invalidate();
        }

        /// <summary>
        ///     Out of Sample
        /// </summary>
        private void ChbOutOfSampleCheckedChanged(object sender, EventArgs e)
        {
            SetOOS();

            balanceChart.IsOOS = isOOS;
            balanceChart.OOSBar = barOOS;

            balanceChart.SetChartData();
            balanceChart.InitChart();
            balanceChart.Invalidate();
        }

        /// <summary>
        ///     Out of Sample
        /// </summary>
        private void SetOOS()
        {
            isOOS = chbOutOfSample.Checked;
            barOOS = Data.Bars - Data.Bars*(int) nudOutOfSample.Value/100 - 1;
            targetBalanceRatio = 1 + (int) nudOutOfSample.Value/100.0F;
        }

        /// <summary>
        ///     Generates a description
        /// </summary>
        private string GenerateDescription()
        {
            // Description
            if (lockedEntryFilters == 0 && lockedExitFilters == 0 &&
                lockedEntrySlot == null && lockedExitSlot == null &&
                strategyBest.PropertiesStatus == StrategySlotStatus.Open)
            {
                IsStrategyModified = false;
                GeneratedDescription = Language.T("Automatically generated on") + " ";
            }
            else
            {
                IsStrategyModified = true;
                GeneratedDescription = Language.T("Modified by the strategy generator on") + " ";
            }

            GeneratedDescription += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ".";

            if (isOOS)
            {
                GeneratedDescription += Environment.NewLine + Language.T("Out of sample testing, percent of OOS bars") +
                                        ": " + nudOutOfSample.Value.ToString(CultureInfo.InvariantCulture) + "%";
                GeneratedDescription += Environment.NewLine + Language.T("Balance") + ": " +
                                        (Configs.AccountInMoney
                                             ? Backtester.MoneyBalance(barOOS).ToString("F2") + " " +
                                               Configs.AccountCurrency
                                             : Backtester.Balance(barOOS).ToString(CultureInfo.InvariantCulture) + " " +
                                               Language.T("points"));
                GeneratedDescription += " (" + Data.Time[barOOS].ToShortDateString() + " " +
                                        Data.Time[barOOS].ToShortTimeString() + "  " + Language.T("Bar") + ": " +
                                        barOOS.ToString(CultureInfo.InvariantCulture) + ")";
            }

            return GeneratedDescription;
        }

        /// <summary>
        ///     Toggles FSB visibility.
        /// </summary>
        private void HideFSBClick(object sender, EventArgs e)
        {
            ParrentForm.Visible = !chbHideFsb.Checked;
        }

        /// <summary>
        ///     Toggles panels.
        /// </summary>
        private void ChangeGeneratorPanel(object sender, EventArgs e)
        {
            var button = (ToolStripButton) sender;
            string name = button.Name;

            if (name == "tsbtShowOptions")
            {
                pnlCommon.Visible = true;
                pnlCriteriaBase.Visible = false;
                pnlSettings.Visible = false;
                pnlSorting.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = false;
                tsbtShowCriteria.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowSorting.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowCriteria")
            {
                pnlCommon.Visible = false;
                pnlCriteriaBase.Visible = true;
                pnlSettings.Visible = false;
                pnlSorting.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowCriteria.Enabled = false;
                tsbtShowSettings.Enabled = true;
                tsbtShowSorting.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowSettings")
            {
                pnlCommon.Visible = false;
                pnlCriteriaBase.Visible = false;
                pnlSettings.Visible = true;
                pnlSorting.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowCriteria.Enabled = true;
                tsbtShowSettings.Enabled = false;
                tsbtShowSorting.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowSorting")
            {
                pnlCommon.Visible = false;
                pnlCriteriaBase.Visible = false;
                pnlSettings.Visible = false;
                pnlSorting.Visible = true;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowCriteria.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowSorting.Enabled = false;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowTop10")
            {
                pnlCommon.Visible = false;
                pnlCriteriaBase.Visible = false;
                pnlSettings.Visible = false;
                pnlSorting.Visible = false;
                pnlTop10.Visible = true;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowCriteria.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowSorting.Enabled = true;
                tsbtShowTop10.Enabled = false;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtIndicators")
            {
                pnlCommon.Visible = false;
                pnlCriteriaBase.Visible = false;
                pnlSettings.Visible = false;
                pnlSorting.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = true;

                tsbtShowOptions.Enabled = true;
                tsbtShowCriteria.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowSorting.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = false;
            }
        }

        /// <summary>
        ///     Shows strategy overview.
        /// </summary>
        private void ShowOverview(object sender, EventArgs e)
        {
            if (GeneratedDescription != string.Empty)
                Data.Strategy.Description = GeneratedDescription;

            var so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHtmlOverview());
            so.Show();
        }

        /// <summary>
        ///     Lock, unlock, link all slots.
        /// </summary>
        private void ChangeSlotStatus(object sender, EventArgs e)
        {
            var button = (ToolStripButton) sender;
            string name = button.Name;

            if (name == "tsbtLockAll")
            {
                strategyBest.PropertiesStatus = StrategySlotStatus.Locked;
                for (int slot = 0; slot < strategyBest.Slots; slot++)
                    strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Locked;
            }
            else if (name == "tsbtUnlockAll")
            {
                strategyBest.PropertiesStatus = StrategySlotStatus.Open;
                for (int slot = 0; slot < strategyBest.Slots; slot++)
                    strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Open;
            }
            else if (name == "tsbtLinkAll")
            {
                strategyBest.PropertiesStatus = StrategySlotStatus.Open;
                for (int slot = 0; slot < strategyBest.Slots; slot++)
                    strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Linked;
            }

            strategyField.RepaintStrategyControls(strategyBest);
        }

        /// <summary>
        ///     Lock, link, or unlock the strategy slot.
        /// </summary>
        private void PnlSlotClick(object sender, EventArgs e)
        {
            if (isGenerating)
                return;

            var slot = (int) ((Panel) sender).Tag;

            if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Open)
                strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Locked;
            else if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Locked)
                strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Linked;
            else if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Open;

            strategyField.RepaintStrategyControls(strategyBest);
        }

        /// <summary>
        ///     Lock, link, or unlock the strategy properties slot.
        /// </summary>
        private void PnlPropertiesClick(object sender, EventArgs e)
        {
            if (isGenerating)
                return;

            if (strategyBest.PropertiesStatus == StrategySlotStatus.Open)
                strategyBest.PropertiesStatus = StrategySlotStatus.Locked;
            else if (strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                strategyBest.PropertiesStatus = StrategySlotStatus.Open;

            strategyField.RepaintStrategyControls(strategyBest);
        }

        /// <summary>
        ///     Changes the slot size
        /// </summary>
        private void BtnSlotSizeClick(object sender, EventArgs e)
        {
            var tag = (int) ((ToolStripButton) sender).Tag;

            if (tag == 1)
            {
                if (strategyField.SlotMinMidMax == SlotSizeMinMidMax.min ||
                    strategyField.SlotMinMidMax == SlotSizeMinMidMax.mid)
                {
                    tsbtStrategySize1.Image = Resources.slot_size_mid;
                    tsbtStrategySize1.ToolTipText = Language.T("Show regular info in the slots.");
                    tsbtStrategySize2.Image = Resources.slot_size_min;
                    tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    strategyField.SlotMinMidMax = SlotSizeMinMidMax.max;
                }
                else if (strategyField.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    tsbtStrategySize1.Image = Resources.slot_size_max;
                    tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    tsbtStrategySize2.Image = Resources.slot_size_min;
                    tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    strategyField.SlotMinMidMax = SlotSizeMinMidMax.mid;
                }
            }
            else
            {
                if (strategyField.SlotMinMidMax == SlotSizeMinMidMax.min)
                {
                    tsbtStrategySize1.Image = Resources.slot_size_max;
                    tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    tsbtStrategySize2.Image = Resources.slot_size_min;
                    tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    strategyField.SlotMinMidMax = SlotSizeMinMidMax.mid;
                }
                else if (strategyField.SlotMinMidMax == SlotSizeMinMidMax.mid ||
                         strategyField.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    tsbtStrategySize1.Image = Resources.slot_size_max;
                    tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    tsbtStrategySize2.Image = Resources.slot_size_mid;
                    tsbtStrategySize2.ToolTipText = Language.T("Show regular info in the slots.");
                    strategyField.SlotMinMidMax = SlotSizeMinMidMax.min;
                }
            }

            strategyField.RearrangeStrategyControls();
        }

        /// <summary>
        ///     View and edit the strategy description
        /// </summary>
        private void BtnStrategyDescriptionClick(object sender, EventArgs e)
        {
            if (GeneratedDescription != string.Empty)
                Data.Strategy.Description = GeneratedDescription;
            var si = new StrategyDescription();
            si.ShowDialog();
            GeneratedDescription = Data.Strategy.Description;
        }

        /// <summary>
        ///     Sets the strategy description button icon
        /// </summary>
        private void SetStrategyDescriptionButton()
        {
            if (GeneratedDescription != string.Empty)
                Data.Strategy.Description = GeneratedDescription;

            if (Data.Strategy.Description == "")
                tsbtStrategyInfo.Image = Resources.str_info_noinfo;
            else
            {
                tsbtStrategyInfo.Image = Data.IsStrDescriptionRelevant()
                                             ? Resources.str_info_infook
                                             : Resources.str_info_warning;
            }
        }

        /// <summary>
        ///     Clears the slots status of the given strategy.
        /// </summary>
        private Strategy ClearStrategySlotsStatus(Strategy strategy)
        {
            Strategy tempStrategy = strategy.Clone();
            tempStrategy.PropertiesStatus = StrategySlotStatus.Open;
            foreach (IndicatorSlot slot in tempStrategy.Slot)
                slot.SlotStatus = StrategySlotStatus.Open;

            return tempStrategy;
        }

        /// <summary>
        ///     Saves the Generator History
        /// </summary>
        private void AddStrategyToGeneratorHistory(string description)
        {
            Strategy strategy = chbSaveStrategySlotStatus.Checked
                                    ? strategyBest.Clone()
                                    : ClearStrategySlotsStatus(strategyBest);

            Data.GeneratorHistory.Add(strategy);
            Data.GeneratorHistory[Data.GeneratorHistory.Count - 1].Description = description;

            if (Data.GeneratorHistory.Count >= 110)
                Data.GeneratorHistory.RemoveRange(0, 10);

            Data.GenHistoryIndex = Data.GeneratorHistory.Count - 1;
        }

        /// <summary>
        ///     Updates the last strategy in Generator History
        /// </summary>
        private void UpdateStrategyInGeneratorHistory(string description)
        {
            if (Data.GeneratorHistory.Count == 0)
                return;

            Strategy strategy = chbSaveStrategySlotStatus.Checked
                                    ? strategyBest.Clone()
                                    : ClearStrategySlotsStatus(strategyBest);

            Data.GeneratorHistory[Data.GeneratorHistory.Count - 1] = strategy;
            Data.GeneratorHistory[Data.GeneratorHistory.Count - 1].Description = description;
        }

        /// <summary>
        ///     Adds a strategy to Top 10 list.
        /// </summary>
        private void Top10AddStrategy()
        {
            if (top10Field.InvokeRequired)
            {
                Invoke(new DelegateTop10AddStrategy(Top10AddStrategy), new object[] {});
            }
            else
            {
                var top10Slot = new Top10Slot {Width = 290, Height = 65};
                top10Slot.InitSlot();
                top10Slot.CustomSortingOption = customSortingOptionDisplay;
                top10Slot.CustomSortingValue = bestValue;
                top10Slot.Click += Top10SlotClick;
                top10Slot.DoubleClick += Top10SlotClick;

                int balance = Configs.AccountInMoney
                                  ? (int) Math.Round(Backtester.NetMoneyBalance)
                                  : Backtester.NetBalance;
                var top10StrategyInfo = new Top10StrategyInfo
                    {
                        Balance = balance,
                        Value = String.IsNullOrEmpty(top10Slot.CustomSortingOption)
                                    ? balance
                                    : bestValue,
                        Top10Slot = top10Slot,
                        TheStrategy = Data.Strategy.Clone()
                    };
                top10Field.AddStrategyInfo(top10StrategyInfo);
            }
        }

        /// <summary>
        ///     Loads a strategy from the clicked Top 10 slot.
        /// </summary>
        private void Top10SlotClick(object sender, EventArgs e)
        {
            if (isGenerating)
                return;

            var top10Slot = (Top10Slot) sender;

            if (top10Slot.IsSelected)
                return;

            top10Field.ClearSelectionOfSelectedSlot();

            top10Slot.IsSelected = true;
            top10Slot.Invalidate();

            Data.Strategy = top10Field.GetStrategy(top10Slot.Balance);
            bestValue = 0;
            CalculateTheResult(true);
        }

        /// <summary>
        ///     Toggles the account chart and statistics.
        /// </summary>
        private void AccountOutputClick(object sender, EventArgs e)
        {
            bool isChartVisible = balanceChart.Visible;
            balanceChart.Visible = !isChartVisible;
            infpnlAccountStatistics.Visible = isChartVisible;
        }

        /// <summary>
        ///     Resets Generator
        /// </summary>
        private void BtnResetClick(object sender, EventArgs e)
        {
            Configs.GeneratorOptions = "";
            Configs.BannedIndicators = "";
            Configs.CriteriaSettings = "";
            isReset = true;
            Close();
        }

        /// <summary>
        ///     Radio buttons for custom sorting event handler
        /// </summary>
        private void RbnCustomSortingClick(object sender, EventArgs e)
        {
            SetCustomSortingUI();
        }

        /// <summary>
        ///     Custom sorting user interface settings
        /// </summary>
        private void SetCustomSortingUI()
        {
            // Simple Custom Sorting is NOT Enabled
            if (!customSortingSimpleEnabled)
            {
                if (rbnCustomSortingSimple.Checked)
                    rbnCustomSortingNone.Checked = true;
                rbnCustomSortingSimple.Enabled = false;
            }

            // Advanced Custom Sorting is NOT Enabled
            if (!customSortingAdvancedEnabled)
            {
                if (rbnCustomSortingAdvanced.Checked)
                    rbnCustomSortingNone.Checked = true;
                rbnCustomSortingAdvanced.Enabled = false;
            }

            // Update the GUI
            if (rbnCustomSortingNone.Checked)
            {
                cbxCustomSortingSimple.Enabled = false;
                cbxCustomSortingAdvanced.Enabled = false;
                cbxCustomSortingAdvancedCompareTo.Enabled = false;
                lblCustomSortingAdvancedCompareTo.Enabled = false;
            }
            else if (rbnCustomSortingSimple.Checked)
            {
                cbxCustomSortingSimple.Enabled = true;
                cbxCustomSortingAdvanced.Enabled = false;
                cbxCustomSortingAdvancedCompareTo.Enabled = false;
                lblCustomSortingAdvancedCompareTo.Enabled = false;
            }
            else if (rbnCustomSortingAdvanced.Checked)
            {
                cbxCustomSortingSimple.Enabled = false;
                cbxCustomSortingAdvanced.Enabled = true;
                cbxCustomSortingAdvancedCompareTo.Enabled = true;
                lblCustomSortingAdvancedCompareTo.Enabled = true;
            }
        }

        #region Nested type: DelegateRebuildStrategyLayout

        private delegate void DelegateRebuildStrategyLayout(Strategy strategy);

        #endregion

        #region Nested type: DelegateRefreshAccountStatisticas

        private delegate void DelegateRefreshAccountStatisticas();

        #endregion

        #region Nested type: DelegateRefreshBalanceChart

        private delegate void DelegateRefreshBalanceChart();

        #endregion

        #region Nested type: DelegateReportIndicatorError

        private delegate void DelegateReportIndicatorError(string text, string caption);

        #endregion

        #region Nested type: DelegateTop10AddStrategy

        private delegate void DelegateTop10AddStrategy();

        #endregion

        #region Nested type: SetCyclesCallback

        private delegate void SetCyclesCallback(string text);

        #endregion

        #region Nested type: SetBenchmarkCallback

        private delegate void SetBenchmarkCallback(double value);

        #endregion
    }
}