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
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder.Dialogs.Optimizer
{
    /// <summary>
    ///     The Optimizer - GUI
    /// </summary>
    public sealed partial class Optimizer
    {
        /// <summary>
        ///     Initializes main controls of optimizer.
        /// </summary>
        private void InitializeControls()
        {
            pnlParamsBase = new Panel();
            pnlParamsBase2 = new Panel();
            pnlCaptions = new Panel();
            pnlParams = new Panel();
            pnlCriteriaBase = new FancyPanel(Language.T("Acceptance Criteria"));
            criteriaPanel = new ScrollFlowPanel();
            criteriaControls = new CriteriaControls();
            pnlSettings = new FancyPanel(Language.T("Settings"));
            scrollBar = new VScrollBar();
            balanceChart = new SmallBalanceChart();
            progressBar = new ProgressBar();
            btnOptimize = new Button();
            btnAccept = new Button();
            btnCancel = new Button();

            lblNoParams = new Label();

            fontIndicator = new Font(Font.FontFamily, 11);
            colorText = LayoutColors.ColorControlText;

            Text = Language.T("Optimizer") + " - " + Data.Symbol + " " + Data.PeriodString + ", " + Data.Bars + " " + Language.T("bars");

            // Panel Parameters Base
            pnlParamsBase.Parent = this;
            pnlParamsBase.BackColor = LayoutColors.ColorControlBack;
            pnlParamsBase.Paint += PnlParamsBasePaint;

            // Panel Captions
            pnlCaptions.Parent = pnlParamsBase;
            pnlCaptions.Dock = DockStyle.Top;
            pnlCaptions.BackColor = LayoutColors.ColorCaptionBack;
            pnlCaptions.ForeColor = LayoutColors.ColorCaptionText;
            pnlCaptions.Paint += PnlCaptionsPaint;

            // Panel Parameters Base 2
            pnlParamsBase2.Parent = pnlParamsBase;
            pnlParamsBase2.BackColor = LayoutColors.ColorControlBack;
            pnlParamsBase2.Resize += PnlParamsBase2Resize;

            // ScrollBar
            scrollBar.Parent = pnlParamsBase2;
            scrollBar.Dock = DockStyle.Right;
            scrollBar.TabStop = true;
            scrollBar.ValueChanged += ScrollBarValueChanged;
            scrollBar.MouseWheel += ScrollBarMouseWheel;

            // Panel Parameters
            pnlParams.Parent = pnlParamsBase2;
            pnlParams.BackColor = LayoutColors.ColorControlBack;

            // Label No Parameters
            lblNoParams.Parent = pnlParams;
            lblNoParams.Text = Language.T("There are no parameters suitable for optimization.");
            lblNoParams.AutoSize = true;
            lblNoParams.Visible = false;

            // Panel Limitations
            pnlCriteriaBase.Parent = this;
            pnlCriteriaBase.Visible = false;

            // Panel Settings
            pnlSettings.Parent = this;
            pnlSettings.Visible = false;

            // Small Balance Chart
            balanceChart.Parent = this;
            balanceChart.BackColor = LayoutColors.ColorControlBack;
            balanceChart.SetChartData();

            // ProgressBar
            progressBar.Parent = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step = 1;

            // Button Optimize
            btnOptimize.Parent = this;
            btnOptimize.Name = "btnOptimize";
            btnOptimize.Text = Language.T("Optimize");
            btnOptimize.TabIndex = 0;
            btnOptimize.Click += BtnOptimizeClick;
            btnOptimize.UseVisualStyleBackColor = true;

            // Button Accept
            btnAccept.Parent = this;
            btnAccept.Name = "btnAccept";
            btnAccept.Text = Language.T("Accept");
            btnAccept.TabIndex = 1;
            btnAccept.Enabled = false;
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            // Button Cancel
            btnCancel.Parent = this;
            btnCancel.Text = Language.T("Cancel");
            btnCancel.TabIndex = 2;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bgWorker.DoWork += BgWorkerDoWork;
            bgWorker.ProgressChanged += BgWorkerProgressChanged;
            bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
        }

        /// <summary>
        ///     Sets controls in panel Limitations
        /// </summary>
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

        /// <summary>
        ///     Sets controls in panel Settings
        /// </summary>
        private void SetPanelSettings()
        {
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

            chbOptimizerWritesReport = new CheckBox
                {
                    Parent = pnlSettings,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Optimizer writes a report for each optimized strategy"),
                    Checked = false,
                    AutoSize = true
                };

            chbHideFSB = new CheckBox
                {
                    Parent = pnlSettings,
                    ForeColor = colorText,
                    BackColor = Color.Transparent,
                    Text = Language.T("Hide FSB when Optimizer starts"),
                    Checked = true,
                    AutoSize = true
                };

            btnResetSettings = new Button
                {
                    Parent = pnlSettings,
                    UseVisualStyleBackColor = true,
                    Text = Language.T("Reset all parameters and settings")
                };
            btnResetSettings.Click += BtnResetClick;
        }

        /// <summary>
        ///     Sets ups the chart's buttons.
        /// </summary>
        private void SetupButtons()
        {
            tsOptimizerButtons = new ToolStrip {Parent = this};

            aOptimizerButtons = new ToolStripButton[Enum.GetValues(typeof (OptimizerButtons)).Length];
            for (int i = 0; i < aOptimizerButtons.Length; i++)
            {
                aOptimizerButtons[i] = new ToolStripButton {Tag = (OptimizerButtons) i};
                aOptimizerButtons[i].Click += ButtonsClick;
                aOptimizerButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsOptimizerButtons.Items.Add(aOptimizerButtons[i]);
                if (i == (int) OptimizerButtons.SelectRandom ||
                    i == (int) OptimizerButtons.SetStep20 ||
                    i == (int) OptimizerButtons.ResetStrategy)
                    tsOptimizerButtons.Items.Add(new ToolStripSeparator());
            }

            // Select All
            aOptimizerButtons[(int) OptimizerButtons.SelectAll].Image = Resources.optimizer_select_all;
            aOptimizerButtons[(int) OptimizerButtons.SelectAll].DisplayStyle = ToolStripItemDisplayStyle.Image;
            aOptimizerButtons[(int) OptimizerButtons.SelectAll].ToolTipText = Language.T("Select all parameters.");

            // Select None
            aOptimizerButtons[(int) OptimizerButtons.SelectNone].Image = Resources.optimizer_select_none;
            aOptimizerButtons[(int) OptimizerButtons.SelectNone].DisplayStyle = ToolStripItemDisplayStyle.Image;
            aOptimizerButtons[(int) OptimizerButtons.SelectNone].ToolTipText =
                Language.T("Select none of the parameters.");

            // Select Random
            aOptimizerButtons[(int) OptimizerButtons.SelectRandom].Image = Resources.optimizer_select_random;
            aOptimizerButtons[(int) OptimizerButtons.SelectRandom].DisplayStyle = ToolStripItemDisplayStyle.Image;
            aOptimizerButtons[(int) OptimizerButtons.SelectRandom].ToolTipText =
                Language.T("Select a random number of parameters.");

            // Set step 5
            aOptimizerButtons[(int) OptimizerButtons.SetStep5].Text = "±5";
            aOptimizerButtons[(int) OptimizerButtons.SetStep5].ToolTipText =
                Language.T("Set Min / Max ± # steps.").Replace("#", "5");

            // Set step 10
            aOptimizerButtons[(int) OptimizerButtons.SetStep10].Text = "±10";
            aOptimizerButtons[(int) OptimizerButtons.SetStep10].ToolTipText =
                Language.T("Set Min / Max ± # steps.").Replace("#", "10");

            // Set step 15
            aOptimizerButtons[(int) OptimizerButtons.SetStep15].Text = "±15";
            aOptimizerButtons[(int) OptimizerButtons.SetStep15].ToolTipText =
                Language.T("Set Min / Max ± # steps.").Replace("#", "15");

            // Set step 20
            aOptimizerButtons[(int) OptimizerButtons.SetStep20].Text = "±20";
            aOptimizerButtons[(int) OptimizerButtons.SetStep20].ToolTipText =
                Language.T("Set Min / Max ± # steps.").Replace("#", "20");

            // Reset Strategy
            aOptimizerButtons[(int) OptimizerButtons.ResetStrategy].Image = Resources.refresh;
            aOptimizerButtons[(int) OptimizerButtons.ResetStrategy].DisplayStyle = ToolStripItemDisplayStyle.Image;
            aOptimizerButtons[(int) OptimizerButtons.ResetStrategy].ToolTipText =
                Language.T("Reset strategy parameters.");

            // Show Parameters
            aOptimizerButtons[(int) OptimizerButtons.ShowParams].Text = Language.T("Parameters");
            aOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = false;

            // Show Limitations
            aOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Text = Language.T("Criteria");

            // Show Settings
            aOptimizerButtons[(int) OptimizerButtons.ShowSettings].Text = Language.T("Settings");
        }

        /// <summary>
        ///     Loads and parses the optimizer's options.
        /// </summary>
        private void LoadOptions()
        {
            if (string.IsNullOrEmpty(Configs.OptimizerOptions))
                return;

            string[] options = Configs.OptimizerOptions.Split(';');
            int i = 0;

            try
            {
                if (int.Parse(options[i++]) < OptionsVersion) return;
                chbOutOfSample.Checked = bool.Parse(options[i++]);
                nudOutOfSample.Value = int.Parse(options[i++]);
                chbOptimizerWritesReport.Checked = bool.Parse(options[i++]);
                chbHideFSB.Checked = bool.Parse(options[i++]);
                formHeight = int.Parse(options[i++]);
                lastSelectButton = (OptimizerButtons) Enum.Parse(typeof (OptimizerButtons), options[i++]);
                lastSetStepButtonValue = int.Parse(options[i]);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Optimizer.LoadOptions: " + exception.Message);
            }
        }

        /// <summary>
        ///     Saves the generator's options.
        /// </summary>
        private void SaveOptions()
        {
            string options =
                OptionsVersion + ";" +
                chbOutOfSample.Checked + ";" +
                nudOutOfSample.Value + ";" +
                chbOptimizerWritesReport.Checked + ";" +
                chbHideFSB.Checked + ";" +
                Height + ";" +
                lastSelectButton + ";" +
                lastSetStepButtonValue;

            Configs.OptimizerOptions = options;
        }

        /// <summary>
        ///     Paints pnlCaptions
        /// </summary>
        private void PnlCaptionsPaint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            Data.GradientPaint(g, pnl.ClientRectangle, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            Brush brush = new SolidBrush(pnl.ForeColor);
            var stringFormat = new StringFormat {Alignment = StringAlignment.Center};

            g.DrawString(Language.T("Parameter"), Font, brush, 25, 3);
            g.DrawString(Language.T("Initial"), Font, brush, 190, 3, stringFormat);
            g.DrawString(Language.T("Value"), Font, brush, 250, 3, stringFormat);
            g.DrawString(Language.T("Minimum"), Font, brush, 293, 3);
            g.DrawString(Language.T("Maximum"), Font, brush, 367, 3);
            g.DrawString(Language.T("Step"), Font, brush, 452, 3);
        }

        /// <summary>
        ///     Paints PnlParamsBase
        /// </summary>
        private void PnlParamsBasePaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            var penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption),
                                    Border);

            g.DrawLine(penBorder, 1, 0, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - Border + 1, 0, pnl.ClientSize.Width - Border + 1,
                       pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - Border + 1, pnl.ClientSize.Width,
                       pnl.ClientSize.Height - Border + 1);
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
            barOOS = Data.Bars - Data.Bars * (int)nudOutOfSample.Value / 100 - 1;
            targetBalanceRatio = 1 + (int)nudOutOfSample.Value / 100.0F;
        }


        /// <summary>
        ///     Changes chart's settings after a button click.
        /// </summary>
        private void ButtonsClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            var button = (OptimizerButtons) btn.Tag;

            switch (button)
            {
                case OptimizerButtons.ShowParams:
                    pnlParamsBase.Visible = true;
                    pnlCriteriaBase.Visible = false;
                    pnlSettings.Visible = false;
                    if (isOptimizing == false)
                        for (int i = 0; i <= (int) OptimizerButtons.SetStep15; i++)
                            aOptimizerButtons[i].Enabled = true;
                    aOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = false;
                    aOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Enabled = true;
                    aOptimizerButtons[(int) OptimizerButtons.ShowSettings].Enabled = true;
                    break;
                case OptimizerButtons.ShowLimitations:
                    pnlParamsBase.Visible = false;
                    pnlCriteriaBase.Visible = true;
                    pnlSettings.Visible = false;
                    for (int i = 0; i <= (int) OptimizerButtons.SetStep15; i++)
                        aOptimizerButtons[i].Enabled = false;
                    aOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = true;
                    aOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Enabled = false;
                    aOptimizerButtons[(int) OptimizerButtons.ShowSettings].Enabled = true;
                    break;
                case OptimizerButtons.ShowSettings:
                    pnlParamsBase.Visible = false;
                    pnlCriteriaBase.Visible = false;
                    pnlSettings.Visible = true;
                    for (int i = 0; i <= (int) OptimizerButtons.SetStep15; i++)
                        aOptimizerButtons[i].Enabled = false;
                    aOptimizerButtons[(int) OptimizerButtons.ShowParams].Enabled = true;
                    aOptimizerButtons[(int) OptimizerButtons.ShowLimitations].Enabled = true;
                    aOptimizerButtons[(int) OptimizerButtons.ShowSettings].Enabled = false;
                    break;
            }

            if (isOptimizing)
                return;

            switch (button)
            {
                case OptimizerButtons.SelectAll:
                    SelectParameters(OptimizerButtons.SelectAll);
                    lastSelectButton = OptimizerButtons.SelectAll;
                    break;
                case OptimizerButtons.SelectNone:
                    SelectParameters(OptimizerButtons.SelectNone);
                    lastSelectButton = OptimizerButtons.SelectNone;
                    break;
                case OptimizerButtons.SelectRandom:
                    SelectParameters(OptimizerButtons.SelectRandom);
                    lastSelectButton = OptimizerButtons.SelectRandom;
                    break;
                case OptimizerButtons.SetStep5:
                    SetParamsMinMax(5);
                    lastSetStepButtonValue = 5;
                    break;
                case OptimizerButtons.SetStep10:
                    SetParamsMinMax(10);
                    lastSetStepButtonValue = 10;
                    break;
                case OptimizerButtons.SetStep15:
                    SetParamsMinMax(15);
                    lastSetStepButtonValue = 15;
                    break;
                case OptimizerButtons.SetStep20:
                    SetParamsMinMax(20);
                    lastSetStepButtonValue = 20;
                    break;
                case OptimizerButtons.ResetStrategy:
                    ResetStrategyParameters();
                    break;
            }
        }

        /// <summary>
        ///     Check Box checked changed
        /// </summary>
        private void OptimizerCheckedChanged(object sender, EventArgs e)
        {
            btnOptimize.Focus();
        }

        /// <summary>
        ///     Arranges the controls into the pnlParams
        /// </summary>
        private void PnlParamsBase2Resize(object sender, EventArgs e)
        {
            if (pnlParams.Height > pnlParamsBase2.Height)
            {
                scrollBar.Maximum = pnlParams.Height - pnlParamsBase2.Height + 40;
                scrollBar.Value = 0;
                scrollBar.SmallChange = 20;
                scrollBar.LargeChange = 40;
                scrollBar.Visible = true;
            }
            else
            {
                scrollBar.Visible = false;
                scrollBar.Minimum = 0;
                scrollBar.Maximum = 0;
                scrollBar.Value = 0;
            }

            pnlParams.Location = new Point(0, -scrollBar.Value);
        }

        /// <summary>
        ///     Invalidate the Panel Parameters
        /// </summary>
        private void ScrollBarValueChanged(object sender, EventArgs e)
        {
            pnlParams.Location = new Point(0, -scrollBar.Value);
        }

        /// <summary>
        ///     Shift the pnlParams viewpoint
        /// </summary>
        private void ScrollBarMouseWheel(object sender, MouseEventArgs e)
        {
            if (!scrollBar.Visible) return;
            int newValue = scrollBar.Value - e.Delta/120;

            if (newValue < scrollBar.Minimum)
                scrollBar.Value = scrollBar.Minimum;
            else if (newValue > scrollBar.Maximum)
                scrollBar.Value = scrollBar.Maximum;
            else
                scrollBar.Value = newValue;
        }

        /// <summary>
        ///     Toggles FSB visibility.
        /// </summary>
        private void HideFSBClick(object sender, EventArgs e)
        {
            formFSB.Visible = !chbHideFSB.Checked;
        }

        /// <summary>
        ///     Resets Generator
        /// </summary>
        private void BtnResetClick(object sender, EventArgs e)
        {
            Configs.OptimizerOptions = "";
            isReset = true;
            Close();
        }
    }
}