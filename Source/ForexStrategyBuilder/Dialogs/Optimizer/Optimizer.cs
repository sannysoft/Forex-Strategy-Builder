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
using System.Text;
using System.Windows.Forms;

namespace ForexStrategyBuilder.Dialogs.Optimizer
{
    /// <summary>
    ///     The Optimizer
    /// </summary>
    public sealed partial class Optimizer : Form
    {
        private const int Border = 2;
        private const int OptionsVersion = 2;
        private readonly Random rand = new Random();
        private readonly ToolTip toolTip = new ToolTip();
        private ToolStripButton[] aOptimizerButtons;
        private Parameter[] aParameter;
        private CheckBox[] achbxParameterName;
        private int[] aiChecked; // An array of the checked parameters
        private Label[] alblIndicatorName;
        private Label[] alblInitialValue;
        private Label[] alblParameterValue;
        private NumericUpDown[] anudParameterMax;
        private NumericUpDown[] anudParameterMin;
        private NumericUpDown[] anudParameterStep;
        private SmallBalanceChart balanceChart;
        private int barOOS = Data.Bars - 1;
        private BackgroundWorker bgWorker;
        private Button btnAccept;
        private Button btnCancel;
        private Button btnOptimize;
        private Button btnResetSettings;
        private CheckBox chbHideFSB;
        private CheckBox chbOptimizerWritesReport;
        private CheckBox chbOutOfSample;
        private NumericUpDown nudOutOfSample;
        private int checkedParams; // Count of the checked parameters
        private Color colorText;
        private int computedCycles; // Currently completed cycles
        private int cycles; // Count of the cycles
        private Font fontIndicator;
        private Font fontParamValueBold;
        private Font fontParamValueRegular;
        private Form formFSB;
        private int formHeight;
        private bool isOOS;
        private bool isOptimizing; // It is true when the optimizer is running
        private bool isReset;
        private bool isStartegyChanged;
        private OptimizerButtons lastSelectButton = OptimizerButtons.SelectRandom;
        private int lastSetStepButtonValue = 5;
        private Label lblNoParams;
        private int parameters; // Count of the NumericParameters
        private Panel pnlCaptions;
        private FancyPanel pnlCriteriaBase;
        private Panel pnlParams;
        private Panel pnlParamsBase;
        private Panel pnlParamsBase2;
        private FancyPanel pnlSettings;
        private ScrollFlowPanel criteriaPanel;
        private CriteriaControls criteriaControls;
        private ProgressBar progressBar;
        private int progressPercent; // Reached progress in %
        private int protections; // Count of permanent protections
        private StringBuilder sbReport;
        private VScrollBar scrollBar;
        private ToolStrip tsOptimizerButtons;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Optimizer()
        {
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            AcceptButton = btnAccept;
            CancelButton = btnCancel;
            Text = Language.T("Optimizer");
            FormClosing += OptimizerFormClosing;

            InitializeControls();
            SetupButtons();
            SetCriteriaPanel();
            SetPanelSettings();
            LoadOptions();

            chbHideFSB.CheckedChanged += HideFSBClick;
        }

        public Form SetParrentForm
        {
            set { formFSB = value; }
        }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            formFSB.Visible = !chbHideFSB.Checked;

            SetIndicatorParams();
            SelectParameters(lastSelectButton);

            Width = 560;
            var height = (int) (570*Data.VDpiScale);
            Height = Math.Max(formHeight, height);
            MinimumSize = new Size(560, height);

            if (Data.AutoOptimize != "")
            {
                SelectParameters(OptimizerButtons.SelectAll);
                SetParamsMinMax(10);
                BtnOptimizeClick(null, null);
            }
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int buttonWidth = (ClientSize.Width - 4*btnHrzSpace)/3;
            int space = btnHrzSpace;
            const int nudWidth = 55;

            // Button Cancel
            btnCancel.Size = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Optimize
            btnOptimize.Size = new Size(buttonWidth, buttonHeight);
            btnOptimize.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace,
                                             ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            progressBar.Size = new Size(ClientSize.Width - 2*space, (int) (Data.VerticalDlu*9));
            progressBar.Location = new Point(space, btnCancel.Top - progressBar.Height - btnVertSpace);

            // Balance Chart
            balanceChart.Size = new Size(ClientSize.Width - 2*space, 200);
            balanceChart.Location = new Point(space, progressBar.Top - space - balanceChart.Height);

            // Panel Parameters Base
            pnlParamsBase.Size = new Size(ClientSize.Width - 2*space,
                                          balanceChart.Top - 2*space - tsOptimizerButtons.Bottom);
            pnlParamsBase.Location = new Point(space, tsOptimizerButtons.Bottom + space);

            // Panel Parameters Base 2
            pnlParamsBase2.Size = new Size(pnlParamsBase.Width - 2*Border,
                                           pnlParamsBase.Height - pnlCaptions.Height - Border);
            pnlParamsBase2.Location = new Point(Border, pnlCaptions.Height);

            // Panel Parameters
            pnlParams.Width = pnlParamsBase2.ClientSize.Width - scrollBar.Width;

            // No Parameters
            lblNoParams.Location = new Point(5, 5);

            // Criteria Panel
            pnlCriteriaBase.Size = pnlParamsBase.Size;
            pnlCriteriaBase.Location = pnlParamsBase.Location;
            criteriaPanel.Size = new Size(pnlCriteriaBase.Width - 2 * 2, pnlCriteriaBase.Height - (int)pnlCriteriaBase.CaptionHeight - 2);
            criteriaPanel.Location = new Point(2, (int)pnlCriteriaBase.CaptionHeight);

            // Panel Settings
            pnlSettings.Size = pnlParamsBase.Size;
            pnlSettings.Location = pnlParamsBase.Location;

            // Out Of Sample
            chbOutOfSample.Location = new Point(Border + 5, 27);
            nudOutOfSample.Width = nudWidth;
            nudOutOfSample.Location = new Point(pnlSettings.ClientSize.Width - nudWidth - Border - 5,
            chbOutOfSample.Top - 1);

            // chbOptimizerWritesReport
            chbOptimizerWritesReport.Location = new Point(Border + 5, chbOutOfSample.Bottom + Border + 4);

            // Hide FSB when generator starts
            chbHideFSB.Location = new Point(Border + 5, chbOptimizerWritesReport.Bottom + Border + 4);

            // Button Reset
            btnResetSettings.Width = pnlSettings.ClientSize.Width - 2*(Border + 5);
            btnResetSettings.Location = new Point(Border + 5, pnlSettings.Height - btnResetSettings.Height - Border - 2);

            // pnlCaptions
            pnlCaptions.Height = 20;
            pnlCaptions.Invalidate();
        }

        /// <summary>
        ///     Check whether the strategy have been changed.
        /// </summary>
        private void OptimizerFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReset)
            {
                SaveOptions();
                Configs.CriteriaSettings = criteriaControls.GetSettings();
            }

            if (isOptimizing)
            {
                // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
                return;
            }

            if (DialogResult == DialogResult.Cancel && isStartegyChanged)
            {
                DialogResult dr = DialogResult.Yes;

                if(Data.AutoOptimize=="")
                    dr = MessageBox.Show(Language.T("Do you want to accept changes to the strategy?"),
                                                  Language.T("Optimizer"), MessageBoxButtons.YesNoCancel,
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

            formFSB.Visible = true;
        }
    }
}