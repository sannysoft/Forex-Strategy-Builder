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
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Instrument Editor
    /// </summary>
    public sealed class InstrumentEditor : Form
    {
        private readonly float captionHeight;
        private readonly Color colorText;
        private readonly Font fontCaption;
        private readonly ToolTip toolTip = new ToolTip();
        private InstrumentProperties instrPropSelectedInstrument;

        /// <summary>
        ///     Constructor
        /// </summary>
        public InstrumentEditor()
        {
            PnlInstruments = new FancyPanel(Language.T("Instruments"));
            PnlProperties = new FancyPanel(Language.T("Instrument Properties"));
            PnlAddInstrument = new FancyPanel(Language.T("Add an Instrument"));

            // Instruments' controls
            LbxInstruments = new ListBox();
            BtnDelete = new Button();
            BtnUp = new Button();
            BtnDown = new Button();

            // Properties' controls
            LblPropSymbol = new Label();
            LblPropType = new Label();
            LblPropComment = new Label();
            LblPropDigits = new Label();
            LblPropPoint = new Label();
            LblPropLots = new Label();
            LblPropSpread = new Label();
            LblPropSwap = new Label();
            LblPropCommission = new Label();
            LblPropSlippage = new Label();
            LblPropPriceIn = new Label();
            LblPropAccountIn = new Label();
            LblPropAccountRate = new Label();
            LblPropFileName = new Label();
            LblPropDataFiles = new Label();

            TbxPropSymbol = new TextBox();
            TbxPropType = new TextBox();
            TbxPropComment = new TextBox();
            TbxPropPoint = new TextBox();
            TbxPropSpread = new TextBox();
            TbxPropSlippage = new TextBox();
            TbxPropPriceIn = new TextBox();
            TbxPropAccountIn = new TextBox();
            TbxPropAccountRate = new TextBox();
            TbxPropFileName = new TextBox();

            CbxPropSwap = new ComboBox();
            CbxPropCommission = new ComboBox();
            CbxPropCommScope = new ComboBox();
            CbxPropCommTime = new ComboBox();

            NudPropDigits = new NumericUpDown();
            NudPropLotSize = new NumericUpDown();
            NudPropSpread = new NumericUpDown();
            NudPropSwapLong = new NumericUpDown();
            NudPropSwapShort = new NumericUpDown();
            NudPropCommission = new NumericUpDown();
            NudPropSlippage = new NumericUpDown();
            NudPropAccountRate = new NumericUpDown();

            BtnAccept = new Button();

            // Add an Instrument's controls
            LblAddInstrSymbol = new Label();
            LblAddInstrType = new Label();
            TbxAddInstrSymbol = new TextBox();
            CbxAddInstrType = new ComboBox();
            BtnAddInstrAdd = new Button();

            BtnClose = new Button();

            fontCaption = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(fontCaption.Height, 18);
            colorText = LayoutColors.ColorControlText;
            NeedReset = false;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Instrument Editor");
            FormClosing += InstrumentEditorFormClosing;

            // PnlInstruments
            PnlInstruments.Parent = this;

            // PnlProperties
            PnlProperties.Parent = this;

            // PnlAddInstrument
            PnlAddInstrument.Parent = this;

            // LbxInstruments
            LbxInstruments.Parent = PnlInstruments;
            LbxInstruments.BackColor = LayoutColors.ColorControlBack;
            //LbxInstruments.BorderStyle = BorderStyle.None;
            LbxInstruments.ForeColor = colorText;
            foreach (string symbol in Instruments.SymbolList)
                LbxInstruments.Items.Add(symbol);

            // Button UP
            BtnUp.Parent = PnlInstruments;
            BtnUp.Text = Language.T("Up");
            BtnUp.UseVisualStyleBackColor = true;
            BtnUp.Click += BtnUpClick;

            // Button Down
            BtnDown.Parent = PnlInstruments;
            BtnDown.Text = Language.T("Down");
            BtnDown.UseVisualStyleBackColor = true;
            BtnDown.Click += BtnDownClick;

            // Button Delete
            BtnDelete.Parent = PnlInstruments;
            BtnDelete.Text = Language.T("Delete");
            BtnDelete.UseVisualStyleBackColor = true;
            BtnDelete.Click += BtnDeleteClick;

            // LblAddInstrSymbol
            LblAddInstrSymbol.Parent = PnlAddInstrument;
            LblAddInstrSymbol.ForeColor = colorText;
            LblAddInstrSymbol.BackColor = Color.Transparent;
            LblAddInstrSymbol.AutoSize = false;
            LblAddInstrSymbol.TextAlign = ContentAlignment.MiddleRight;
            LblAddInstrSymbol.Text = Language.T("Symbol");

            // TbxAddInstrSymbol
            TbxAddInstrSymbol.Parent = PnlAddInstrument;
            TbxAddInstrSymbol.ForeColor = colorText;

            // LblAddInstrType
            LblAddInstrType.Parent = PnlAddInstrument;
            LblAddInstrType.ForeColor = colorText;
            LblAddInstrType.BackColor = Color.Transparent;
            LblAddInstrType.AutoSize = false;
            LblAddInstrType.TextAlign = ContentAlignment.MiddleRight;
            LblAddInstrType.Text = Language.T("Type");

            // CbxAddInstrType
            CbxAddInstrType.Parent = PnlAddInstrument;
            CbxAddInstrType.Name = "cbxAddInstrType";
            CbxAddInstrType.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] instrTypes = Enum.GetNames(typeof (InstrumetType));
            foreach (string type in instrTypes)
                CbxAddInstrType.Items.Add(type);
            CbxAddInstrType.SelectedIndex = 0;

            // BtnAddInstrAdd
            BtnAddInstrAdd.Parent = PnlAddInstrument;
            BtnAddInstrAdd.Name = "btnAddInstrAdd";
            BtnAddInstrAdd.Text = Language.T("Add");
            BtnAddInstrAdd.UseVisualStyleBackColor = true;
            BtnAddInstrAdd.Click += BtnAddInstrAddClick;

            // LblPropSymbol
            LblPropSymbol.Parent = PnlProperties;
            LblPropSymbol.ForeColor = colorText;
            LblPropSymbol.BackColor = Color.Transparent;
            LblPropSymbol.AutoSize = false;
            LblPropSymbol.TextAlign = ContentAlignment.MiddleRight;
            LblPropSymbol.Text = Language.T("Symbol");

            // LblPropType
            LblPropType.Parent = PnlProperties;
            LblPropType.ForeColor = colorText;
            LblPropType.BackColor = Color.Transparent;
            LblPropType.AutoSize = false;
            LblPropType.TextAlign = ContentAlignment.MiddleRight;
            LblPropType.Text = Language.T("Type");

            // LblPropComment
            LblPropComment.Parent = PnlProperties;
            LblPropComment.ForeColor = colorText;
            LblPropComment.BackColor = Color.Transparent;
            LblPropComment.AutoSize = false;
            LblPropComment.TextAlign = ContentAlignment.MiddleRight;
            LblPropComment.Text = Language.T("Comment");

            // LblPropDigits
            LblPropDigits.Parent = PnlProperties;
            LblPropDigits.ForeColor = colorText;
            LblPropDigits.BackColor = Color.Transparent;
            LblPropDigits.AutoSize = false;
            LblPropDigits.TextAlign = ContentAlignment.MiddleRight;
            LblPropDigits.Text = Language.T("Digits");

            // LblPropPoint
            LblPropPoint.Parent = PnlProperties;
            LblPropPoint.ForeColor = colorText;
            LblPropPoint.BackColor = Color.Transparent;
            LblPropPoint.AutoSize = false;
            LblPropPoint.TextAlign = ContentAlignment.MiddleRight;
            LblPropPoint.Text = Language.T("Point value");

            // LblPropLots
            LblPropLots.Parent = PnlProperties;
            LblPropLots.ForeColor = colorText;
            LblPropLots.BackColor = Color.Transparent;
            LblPropLots.AutoSize = false;
            LblPropLots.TextAlign = ContentAlignment.MiddleRight;
            LblPropLots.Text = Language.T("Lot size");

            // LblPropSpread
            LblPropSpread.Parent = PnlProperties;
            LblPropSpread.ForeColor = colorText;
            LblPropSpread.BackColor = Color.Transparent;
            LblPropSpread.AutoSize = false;
            LblPropSpread.TextAlign = ContentAlignment.MiddleRight;
            LblPropSpread.Text = Language.T("Spread in");

            // LblPropSwap
            LblPropSwap.Parent = PnlProperties;
            LblPropSwap.ForeColor = colorText;
            LblPropSwap.BackColor = Color.Transparent;
            LblPropSwap.AutoSize = false;
            LblPropSwap.TextAlign = ContentAlignment.MiddleRight;
            LblPropSwap.Text = Language.T("Swap in");

            // LblPropCommission
            LblPropCommission.Parent = PnlProperties;
            LblPropCommission.ForeColor = colorText;
            LblPropCommission.BackColor = Color.Transparent;
            LblPropCommission.AutoSize = false;
            LblPropCommission.TextAlign = ContentAlignment.MiddleRight;
            LblPropCommission.Text = Language.T("Commission in");

            // LblPropSlippage
            LblPropSlippage.Parent = PnlProperties;
            LblPropSlippage.ForeColor = colorText;
            LblPropSlippage.BackColor = Color.Transparent;
            LblPropSlippage.AutoSize = false;
            LblPropSlippage.TextAlign = ContentAlignment.MiddleRight;
            LblPropSlippage.Text = Language.T("Slippage in");

            // LblPropPriceIn
            LblPropPriceIn.Parent = PnlProperties;
            LblPropPriceIn.ForeColor = colorText;
            LblPropPriceIn.BackColor = Color.Transparent;
            LblPropPriceIn.AutoSize = false;
            LblPropPriceIn.TextAlign = ContentAlignment.MiddleRight;
            LblPropPriceIn.Text = Language.T("Price in");

            // LblPropAccountIn
            LblPropAccountIn.Parent = PnlProperties;
            LblPropAccountIn.ForeColor = colorText;
            LblPropAccountIn.BackColor = Color.Transparent;
            LblPropAccountIn.AutoSize = false;
            LblPropAccountIn.TextAlign = ContentAlignment.MiddleRight;
            LblPropAccountIn.Text = Language.T("Account in");

            // LblPropAccountRate
            LblPropAccountRate.Parent = PnlProperties;
            LblPropAccountRate.ForeColor = colorText;
            LblPropAccountRate.BackColor = Color.Transparent;
            LblPropAccountRate.AutoSize = false;
            LblPropAccountRate.TextAlign = ContentAlignment.MiddleRight;
            LblPropAccountRate.Text = Language.T("Account exchange rate");

            // LblPropFileName
            LblPropFileName.Parent = PnlProperties;
            LblPropFileName.BackColor = Color.Transparent;
            LblPropFileName.ForeColor = colorText;
            LblPropFileName.AutoSize = false;
            LblPropFileName.TextAlign = ContentAlignment.MiddleRight;
            LblPropFileName.Text = Language.T("Base name of the data files");

            // LblPropDataFiles
            LblPropDataFiles.Parent = PnlProperties;
            LblPropDataFiles.BackColor = Color.Transparent;
            LblPropDataFiles.ForeColor = colorText;
            LblPropDataFiles.AutoSize = false;
            LblPropDataFiles.TextAlign = ContentAlignment.TopLeft;
            LblPropDataFiles.Text = "";

            // TbxPropSymbol
            TbxPropSymbol.Parent = PnlProperties;
            TbxPropSymbol.BackColor = LayoutColors.ColorControlBack;
            TbxPropSymbol.ForeColor = colorText;
            TbxPropSymbol.Enabled = false;

            // TbxPropType
            TbxPropType.Parent = PnlProperties;
            TbxPropType.BackColor = LayoutColors.ColorControlBack;
            TbxPropType.ForeColor = colorText;
            TbxPropType.Enabled = false;

            // TbxPropComment
            TbxPropComment.Parent = PnlProperties;
            TbxPropComment.BackColor = LayoutColors.ColorControlBack;
            TbxPropComment.ForeColor = colorText;

            // TbxPropPoint
            TbxPropPoint.Parent = PnlProperties;
            TbxPropPoint.BackColor = LayoutColors.ColorControlBack;
            TbxPropPoint.ForeColor = colorText;
            TbxPropPoint.Enabled = false;

            // TbxPropSpread
            TbxPropSpread.Parent = PnlProperties;
            TbxPropSpread.BackColor = LayoutColors.ColorControlBack;
            TbxPropSpread.ForeColor = colorText;
            TbxPropSpread.Enabled = false;
            TbxPropSpread.Text = Language.T("points");

            // TbxPropSlippage
            TbxPropSlippage.Parent = PnlProperties;
            TbxPropSlippage.BackColor = LayoutColors.ColorControlBack;
            TbxPropSlippage.ForeColor = colorText;
            TbxPropSlippage.Enabled = false;
            TbxPropSlippage.Text = Language.T("points");

            // TbxPropPriceIn
            TbxPropPriceIn.Parent = PnlProperties;
            TbxPropPriceIn.BackColor = LayoutColors.ColorControlBack;
            TbxPropPriceIn.ForeColor = colorText;
            TbxPropPriceIn.TextChanged += TbxPropPriceInTextChanged;

            // TbxPropAccountIn
            TbxPropAccountIn.Parent = PnlProperties;
            TbxPropAccountIn.BackColor = LayoutColors.ColorControlBack;
            TbxPropAccountIn.ForeColor = colorText;
            TbxPropAccountIn.Enabled = false;
            TbxPropAccountIn.Text = Configs.AccountCurrency;

            // TbxPropAccountRate
            TbxPropAccountRate.Parent = PnlProperties;
            TbxPropAccountRate.BackColor = LayoutColors.ColorControlBack;
            TbxPropAccountRate.ForeColor = colorText;
            TbxPropAccountRate.Enabled = false;
            TbxPropAccountRate.Text = "Deal price";

            // TbxPropFileName
            TbxPropFileName.Parent = PnlProperties;
            TbxPropFileName.BackColor = LayoutColors.ColorControlBack;
            TbxPropFileName.ForeColor = colorText;
            TbxPropFileName.TextChanged += TbxPropFileNameTextChanged;

            // CbxPropSwap
            CbxPropSwap.Parent = PnlProperties;
            CbxPropSwap.Name = "CbxPropSwap";
            CbxPropSwap.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPropSwap.Items.AddRange(new object[] {Language.T("points"), Language.T("percent"), Language.T("money")});
            CbxPropSwap.SelectedIndex = 0;

            // CbxPropCommission
            CbxPropCommission.Parent = PnlProperties;
            CbxPropCommission.Name = "CbxPropCommission";
            CbxPropCommission.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPropCommission.Items.AddRange(new object[]
                {Language.T("points"), Language.T("percent"), Language.T("money")});
            CbxPropCommission.SelectedIndex = 0;
            CbxPropCommission.SelectedIndexChanged += CbxPropCommissionSelectedIndexChanged;

            // CbxPropCommScope
            CbxPropCommScope.Parent = PnlProperties;
            CbxPropCommScope.Name = "CbxPropCommScope";
            CbxPropCommScope.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPropCommScope.Items.AddRange(new object[] {Language.T("per lot"), Language.T("per deal")});
            CbxPropCommScope.SelectedIndex = 0;

            // CbxPropCommTime
            CbxPropCommTime.Parent = PnlProperties;
            CbxPropCommTime.Name = "CbxPropCommTime";
            CbxPropCommTime.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPropCommTime.Items.AddRange(new object[] {Language.T("at opening"), Language.T("at open/close")});
            CbxPropCommTime.SelectedIndex = 0;

            // NumericUpDown Digits
            NudPropDigits.BeginInit();
            NudPropDigits.Parent = PnlProperties;
            NudPropDigits.Name = "NUDPropDigits";
            NudPropDigits.Minimum = 0;
            NudPropDigits.Maximum = 5;
            NudPropDigits.Increment = 1;
            NudPropDigits.Value = 4;
            NudPropDigits.TextAlign = HorizontalAlignment.Center;
            NudPropDigits.ValueChanged += NudPropDigitsValueChanged;
            NudPropDigits.EndInit();

            // NUDPropLotSize
            NudPropLotSize.BeginInit();
            NudPropLotSize.Parent = PnlProperties;
            NudPropLotSize.Name = "NUDPropLotSize";
            NudPropLotSize.Minimum = 0;
            NudPropLotSize.Maximum = 100000;
            NudPropLotSize.Increment = 1;
            NudPropLotSize.Value = 10000;
            NudPropLotSize.TextAlign = HorizontalAlignment.Center;
            NudPropLotSize.EndInit();

            // NUDPropSpread
            NudPropSpread.BeginInit();
            NudPropSpread.Parent = PnlProperties;
            NudPropSpread.Name = "NUDPropSpread";
            NudPropSpread.TextAlign = HorizontalAlignment.Center;
            NudPropSpread.Minimum = 0;
            NudPropSpread.Maximum = 5000;
            NudPropSpread.Increment = 0.01M;
            NudPropSpread.DecimalPlaces = 2;
            NudPropSpread.Value = 4;
            NudPropSpread.EndInit();
            toolTip.SetToolTip(NudPropSpread, Language.T("Difference between Bid and Ask prices."));

            // NumericUpDown Swap Long
            NudPropSwapLong.BeginInit();
            NudPropSwapLong.Parent = PnlProperties;
            NudPropSwapLong.Name = "NUDPropSwapLong";
            NudPropSwapLong.TextAlign = HorizontalAlignment.Center;
            NudPropSwapLong.Minimum = -5000;
            NudPropSwapLong.Maximum = 5000;
            NudPropSwapLong.Increment = 0.01M;
            NudPropSwapLong.DecimalPlaces = 2;
            NudPropSwapLong.Value = 1;
            NudPropSwapLong.EndInit();
            toolTip.SetToolTip(NudPropSwapLong,
                               Language.T("Swap number for a long position rollover") + Environment.NewLine +
                               Language.T("A positive value decreases your profit."));

            // NumericUpDown Swap Short
            NudPropSwapShort.BeginInit();
            NudPropSwapShort.Parent = PnlProperties;
            NudPropSwapShort.Name = "NUDPropSwapShort";
            NudPropSwapShort.TextAlign = HorizontalAlignment.Center;
            NudPropSwapShort.Minimum = -5000;
            NudPropSwapShort.Maximum = 5000;
            NudPropSwapShort.Increment = 0.01M;
            NudPropSwapShort.DecimalPlaces = 2;
            NudPropSwapShort.Value = -1;
            NudPropSwapShort.EndInit();
            toolTip.SetToolTip(NudPropSwapShort,
                               Language.T("Swap number for a short position rollover") + Environment.NewLine +
                               Language.T("A negative value decreases your profit."));

            // NumericUpDown NUDPropCommission
            NudPropCommission.BeginInit();
            NudPropCommission.Parent = PnlProperties;
            NudPropCommission.Name = "NUDPropCommission";
            NudPropCommission.TextAlign = HorizontalAlignment.Center;
            NudPropCommission.Minimum = -5000;
            NudPropCommission.Maximum = 5000;
            NudPropCommission.Increment = 0.01M;
            NudPropCommission.DecimalPlaces = 2;
            NudPropCommission.Value = 0;
            NudPropCommission.EndInit();

            // NumericUpDown NUDPropSlippage
            NudPropSlippage.BeginInit();
            NudPropSlippage.Parent = PnlProperties;
            NudPropSlippage.Name = "NUDPropSlippage";
            NudPropSlippage.TextAlign = HorizontalAlignment.Center;
            NudPropSlippage.Minimum = 0;
            NudPropSlippage.Maximum = 200;
            NudPropSlippage.Increment = 1;
            NudPropSlippage.DecimalPlaces = 0;
            NudPropSlippage.Value = 0;
            NudPropSlippage.EndInit();
            toolTip.SetToolTip(NudPropSlippage,
                               Language.T("Number of points you lose due to an inaccurate order execution."));

            // NumericUpDown NUDPropAccountRate
            NudPropAccountRate.BeginInit();
            NudPropAccountRate.Parent = PnlProperties;
            NudPropAccountRate.Name = "NUDPropAccountRate";
            NudPropAccountRate.TextAlign = HorizontalAlignment.Center;
            NudPropAccountRate.Minimum = 0;
            NudPropAccountRate.Maximum = 100000;
            NudPropAccountRate.Increment = 0.0001M;
            NudPropAccountRate.DecimalPlaces = 4;
            NudPropAccountRate.Value = 1;
            NudPropAccountRate.ValueChanged += NudPropAccountRateValueChanged;
            NudPropAccountRate.EndInit();

            // Button Accept
            BtnAccept.Parent = PnlProperties;
            BtnAccept.Name = "btnAccept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.Click += BtnAcceptClick;
            BtnAccept.UseVisualStyleBackColor = true;

            //Button Close
            BtnClose.Parent = this;
            BtnClose.Text = Language.T("Close");
            BtnClose.DialogResult = DialogResult.Cancel;
            BtnClose.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlInstruments { get; set; }
        private FancyPanel PnlProperties { get; set; }
        private FancyPanel PnlAddInstrument { get; set; }

        // Instruments' controls
        private ListBox LbxInstruments { get; set; }
        private Button BtnUp { get; set; }
        private Button BtnDown { get; set; }
        private Button BtnDelete { get; set; }

        // Properties' controls
        private Label LblPropSymbol { get; set; }
        private Label LblPropType { get; set; }
        private Label LblPropComment { get; set; }
        private Label LblPropDigits { get; set; }
        private Label LblPropPoint { get; set; }
        private Label LblPropLots { get; set; }
        private Label LblPropSpread { get; set; }
        private Label LblPropSwap { get; set; }
        private Label LblPropCommission { get; set; }
        private Label LblPropSlippage { get; set; }
        private Label LblPropPriceIn { get; set; }
        private Label LblPropAccountIn { get; set; }
        private Label LblPropAccountRate { get; set; }
        private Label LblPropFileName { get; set; }
        private Label LblPropDataFiles { get; set; }

        private TextBox TbxPropSymbol { get; set; }
        private TextBox TbxPropType { get; set; }
        private TextBox TbxPropComment { get; set; }
        private TextBox TbxPropPoint { get; set; }
        private TextBox TbxPropSpread { get; set; }
        private TextBox TbxPropSlippage { get; set; }
        private TextBox TbxPropPriceIn { get; set; }
        private TextBox TbxPropAccountIn { get; set; }
        private TextBox TbxPropAccountRate { get; set; }
        private TextBox TbxPropFileName { get; set; }

        private ComboBox CbxPropSwap { get; set; }
        private ComboBox CbxPropCommission { get; set; }
        private ComboBox CbxPropCommScope { get; set; }
        private ComboBox CbxPropCommTime { get; set; }

        private NumericUpDown NudPropDigits { get; set; }
        private NumericUpDown NudPropLotSize { get; set; }
        private NumericUpDown NudPropSpread { get; set; }
        private NumericUpDown NudPropSwapLong { get; set; }
        private NumericUpDown NudPropSwapShort { get; set; }
        private NumericUpDown NudPropCommission { get; set; }
        private NumericUpDown NudPropSlippage { get; set; }
        private NumericUpDown NudPropAccountRate { get; set; }

        private Button BtnAccept { get; set; }

        // Add an Instrument's controls
        private Label LblAddInstrSymbol { get; set; }
        private TextBox TbxAddInstrSymbol { get; set; }
        private Label LblAddInstrType { get; set; }
        private ComboBox CbxAddInstrType { get; set; }
        private Button BtnAddInstrAdd { get; set; }

        private Button BtnClose { get; set; }

        /// <summary>
        ///     If a reset is necessary.
        /// </summary>
        public bool NeedReset { get; private set; }

        private void NudPropAccountRateValueChanged(object sender, EventArgs e)
        {
            NudPropAccountRate.ForeColor = Color.Black;
        }

        /// <summary>
        ///     Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDlu*65);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);

            var width = (int)(540 * Data.HDpiScale);

            ClientSize = new Size(6*buttonWidth + 11*btnHrzSpace + 4, width);

            LbxInstruments.SelectedValueChanged += LbxInstrumentsSelectedValueChanged;
            LbxInstruments.SelectedIndex = LbxInstruments.Items.IndexOf(Data.Symbol);

            BtnClose.Focus();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var buttonWidth = (int) (Data.HorizontalDlu*65);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            const int border = 2;

            // Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - 2*space - 1,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlInstruments
            PnlInstruments.Size = new Size(buttonWidth + 2*space + 2, ClientSize.Height - 2*space);
            PnlInstruments.Location = new Point(space, space);

            // pnlAddInstrument
            PnlAddInstrument.Size = new Size(ClientSize.Width - 2*space - PnlInstruments.Right,
                                             buttonHeight + 2*space + (int) captionHeight + 2);
            PnlAddInstrument.Location = new Point(PnlInstruments.Right + space,
                                                  BtnClose.Top - btnVertSpace - PnlAddInstrument.Height);

            // pnlProperties
            PnlProperties.Size = new Size(ClientSize.Width - 2*space - PnlInstruments.Right,
                                          PnlAddInstrument.Top - 2*space);
            PnlProperties.Location = new Point(PnlInstruments.Right + space, space);

            // Button Delete
            BtnDelete.Size = new Size(buttonWidth, buttonHeight);
            BtnDelete.Location = new Point(btnHrzSpace, PnlInstruments.ClientSize.Height - buttonHeight - space);

            // Button Up
            BtnUp.Size = new Size((buttonWidth - space)/2, buttonHeight);
            BtnUp.Location = new Point(btnHrzSpace, BtnDelete.Top - buttonHeight - space);

            // Button Down
            BtnDown.Size = new Size((buttonWidth - space)/2, buttonHeight);
            BtnDown.Location = new Point(BtnUp.Right + btnHrzSpace, BtnDelete.Top - buttonHeight - space);

            // lbxInstruments
            LbxInstruments.Size = new Size(PnlInstruments.ClientSize.Width - 2*btnHrzSpace - 2*border,
                                           BtnUp.Top - space - (int) captionHeight);
            LbxInstruments.Location = new Point(space + border, space + (int) captionHeight);

            // Properties' controls
            LblPropSymbol.Width = buttonWidth;
            LblPropType.Width = buttonWidth;
            LblPropComment.Width = buttonWidth;
            LblPropDigits.Width = buttonWidth;
            LblPropPoint.Width = buttonWidth;
            LblPropLots.Width = buttonWidth;
            LblPropSpread.Width = buttonWidth;
            LblPropSwap.Width = buttonWidth;
            LblPropCommission.Width = buttonWidth;
            LblPropSlippage.Width = buttonWidth;
            LblPropPriceIn.Width = buttonWidth;
            LblPropAccountIn.Width = buttonWidth;
            LblPropAccountRate.Width = 2*buttonWidth + space;
            LblPropFileName.Width = 2*buttonWidth + space;
            LblPropDataFiles.Width = 4*buttonWidth + 3*space;
            LblPropDataFiles.Height = 2*buttonWidth + 1*space;
            LblPropSymbol.Location = new Point(X(1), Y(1) + 1);
            LblPropType.Location = new Point(X(3), Y(1) + 1);
            LblPropComment.Location = new Point(X(1), Y(2) + 1);
            LblPropDigits.Location = new Point(X(1), Y(3) + 1);
            LblPropPoint.Location = new Point(X(3), Y(3) + 1);
            LblPropLots.Location = new Point(X(1), Y(4) + 1);
            LblPropSpread.Location = new Point(X(1), Y(5) + 1);
            LblPropSwap.Location = new Point(X(1), Y(6) + 1);
            LblPropCommission.Location = new Point(X(1), Y(7) + 1);
            LblPropSlippage.Location = new Point(X(1), Y(8) + 1);
            LblPropPriceIn.Location = new Point(X(1), Y(9) + 1);
            LblPropAccountIn.Location = new Point(X(3), Y(9) + 1);
            LblPropAccountRate.Location = new Point(X(1), Y(10) + 1);
            LblPropFileName.Location = new Point(X(1), Y(11) + 1);
            LblPropDataFiles.Location = new Point(X(1), Y(12) + 1);

            TbxPropSymbol.Width = buttonWidth;
            TbxPropType.Width = buttonWidth;
            TbxPropComment.Width = 3*buttonWidth + 2*space;
            TbxPropSpread.Width = buttonWidth;
            TbxPropPriceIn.Width = buttonWidth;
            TbxPropPoint.Width = buttonWidth;
            TbxPropSlippage.Width = buttonWidth;
            TbxPropAccountIn.Width = buttonWidth;
            TbxPropAccountRate.Width = buttonWidth;
            TbxPropFileName.Width = buttonWidth;
            TbxPropSymbol.Location = new Point(X(2), Y(1) + 3);
            TbxPropType.Location = new Point(X(4), Y(1) + 2);
            TbxPropComment.Location = new Point(X(2), Y(2) + 3);
            TbxPropPoint.Location = new Point(X(4), Y(3) + 3);
            TbxPropSpread.Location = new Point(X(2), Y(5) + 2);
            TbxPropSlippage.Location = new Point(X(2), Y(8) + 3);
            TbxPropPriceIn.Location = new Point(X(2), Y(9) + 3);
            TbxPropAccountIn.Location = new Point(X(4), Y(9) + 3);
            TbxPropAccountRate.Location = new Point(X(3), Y(10) + 3);
            TbxPropFileName.Location = new Point(X(3), Y(11) + 3);

            CbxPropSwap.Width = buttonWidth;
            CbxPropCommission.Width = buttonWidth;
            CbxPropCommScope.Width = buttonWidth;
            CbxPropCommTime.Width = buttonWidth;
            CbxPropSwap.Location = new Point(X(2), Y(6) + 2);
            CbxPropCommission.Location = new Point(X(2), Y(7) + 2);
            CbxPropCommScope.Location = new Point(X(3), Y(7) + 2);
            CbxPropCommTime.Location = new Point(X(4), Y(7) + 2);

            NudPropDigits.Width = buttonWidth;
            NudPropLotSize.Width = buttonWidth;
            NudPropSpread.Width = buttonWidth;
            NudPropSwapLong.Width = buttonWidth;
            NudPropSwapShort.Width = buttonWidth;
            NudPropCommission.Width = buttonWidth;
            NudPropSlippage.Width = buttonWidth;
            NudPropAccountRate.Width = buttonWidth;
            NudPropDigits.Location = new Point(X(2), Y(3) + 4);
            NudPropLotSize.Location = new Point(X(2), Y(4) + 4);
            NudPropSpread.Location = new Point(X(3), Y(5) + 4);
            NudPropSwapLong.Location = new Point(X(3), Y(6) + 4);
            NudPropSwapShort.Location = new Point(X(4), Y(6) + 4);
            NudPropCommission.Location = new Point(X(5), Y(7) + 4);
            NudPropSlippage.Location = new Point(X(3), Y(8) + 4);
            NudPropAccountRate.Location = new Point(X(4), Y(10) + 4);

            // pnlAddInstrument's controls
            LblAddInstrSymbol.Width = buttonWidth;
            LblAddInstrSymbol.Location = new Point(X(1), Y(1) + 1);
            TbxAddInstrSymbol.Width = buttonWidth;
            TbxAddInstrSymbol.Location = new Point(X(2), Y(1) + 3);
            LblAddInstrType.Width = buttonWidth;
            LblAddInstrType.Location = new Point(X(3), Y(1) + 1);
            CbxAddInstrType.Width = buttonWidth;
            CbxAddInstrType.Location = new Point(X(4), Y(1) + 2);
            BtnAddInstrAdd.Size = new Size(buttonWidth, buttonHeight);
            BtnAddInstrAdd.Location = new Point(X(5), Y(1));

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(PnlProperties.ClientSize.Width - buttonWidth - space,
                                           PnlProperties.ClientSize.Height - buttonHeight - space);
        }

        /// <summary>
        ///     Gives the horizontal position.
        /// </summary>
        private int X(int i)
        {
            var buttonWidth = (int) (Data.HorizontalDlu*65);
            var border = (int) (Data.HorizontalDlu*3);
            int position = i*border + (i - 1)*buttonWidth;

            return position;
        }

        /// <summary>
        ///     Gives the vertical position.
        /// </summary>
        private int Y(int i)
        {
            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var border = (int) (Data.HorizontalDlu*3);
            int position = (int) captionHeight + i*border + (i - 1)*buttonHeight;

            return position;
        }

        /// <summary>
        ///     Validates the instrument properties
        /// </summary>
        private bool ValidatePropertiesForm()
        {
            bool isResult = true;
            string errorMessage = "";

            // Symbol
            if (
                !ValidateSymbol(TbxPropSymbol.Text, (InstrumetType) Enum.Parse(typeof (InstrumetType), TbxPropType.Text)))
                errorMessage += Environment.NewLine + Language.T("Wrong Symbol!");

            // Price In
            var regexPriceIn = new Regex(@"^[A-Z]{3}$");

            if (!regexPriceIn.IsMatch(TbxPropPriceIn.Text))
                errorMessage += Environment.NewLine + Language.T("Wrong Price!");

            // Commission
            if (CbxPropCommission.SelectedIndex == 1 && CbxPropCommScope.SelectedIndex != 1)
                errorMessage += Environment.NewLine + Language.T("Wrong commission settings!");


            // Base file name
            var regexFileName = new Regex(@"^[a-zA-Z\$\#][\w- ]*$");

            if (!regexFileName.IsMatch(TbxPropFileName.Text))
                errorMessage += Environment.NewLine + Language.T("Wrong Base name of the data files!");

            if (errorMessage != "")
            {
                isResult = false;
                MessageBox.Show(errorMessage, Language.T("Instrument Properties"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }

            return isResult;
        }

        /// <summary>
        ///     Validates the instrument properties
        /// </summary>
        private bool ValidateSymbol(string symbol, InstrumetType instrType)
        {
            bool isResult;

            if (instrType == InstrumetType.Forex)
            {
                var regex = new Regex(@"^[A-Z]{6}$");
                isResult = (regex.IsMatch(symbol) && symbol.Substring(0, 3) != symbol.Substring(3, 3));
            }
            else
            {
                var regex = new Regex(@"^[a-zA-Z\$\#]");
                isResult = regex.IsMatch(symbol);
            }

            return isResult;
        }

        /// <summary>
        ///     Sets the properties form.
        /// </summary>
        private void SetPropertiesForm()
        {
            TbxPropSymbol.Text = instrPropSelectedInstrument.Symbol;
            TbxPropType.Text = instrPropSelectedInstrument.InstrType.ToString();
            TbxPropComment.Text = instrPropSelectedInstrument.Comment;
            TbxPropPoint.Text = (1/Math.Pow(10, (float) NudPropDigits.Value)).ToString("0.#####");
            TbxPropPriceIn.Text = instrPropSelectedInstrument.PriceIn;
            TbxPropFileName.Text = instrPropSelectedInstrument.BaseFileName;
            CbxPropSwap.SelectedIndex = (int) instrPropSelectedInstrument.SwapUnit;
            CbxPropCommission.SelectedIndex = (int) instrPropSelectedInstrument.CommissionUnit;
            CbxPropCommScope.SelectedIndex = (int) instrPropSelectedInstrument.CommissionScope;
            CbxPropCommTime.SelectedIndex = (int) instrPropSelectedInstrument.CommissionTime;
            NudPropDigits.Value = instrPropSelectedInstrument.Digits;
            NudPropLotSize.Value = instrPropSelectedInstrument.LotSize;
            NudPropSpread.Value = (decimal) instrPropSelectedInstrument.Spread;
            NudPropSwapLong.Value = (decimal) instrPropSelectedInstrument.SwapLong;
            NudPropSwapShort.Value = (decimal) instrPropSelectedInstrument.SwapShort;
            NudPropCommission.Value = (decimal) instrPropSelectedInstrument.Commission;
            NudPropSlippage.Value = instrPropSelectedInstrument.Slippage;

            TbxPropPriceIn.Enabled = instrPropSelectedInstrument.InstrType != InstrumetType.Forex;
            TbxPropFileName.Enabled = instrPropSelectedInstrument.InstrType != InstrumetType.Forex;

            SetAcountExchangeRate();
        }

        /// <summary>
        ///     Sets the properties form.
        /// </summary>
        private void SetSelectedInstrument()
        {
            instrPropSelectedInstrument.Symbol = TbxPropSymbol.Text;
            instrPropSelectedInstrument.InstrType =
                (InstrumetType) Enum.Parse(typeof (InstrumetType), TbxPropType.Text);
            instrPropSelectedInstrument.Comment = TbxPropComment.Text;
            instrPropSelectedInstrument.PriceIn = TbxPropPriceIn.Text;
            instrPropSelectedInstrument.BaseFileName = TbxPropFileName.Text;
            instrPropSelectedInstrument.SwapUnit =
                (ChargeUnit) Enum.GetValues(typeof (ChargeUnit)).GetValue(CbxPropSwap.SelectedIndex);
            instrPropSelectedInstrument.CommissionUnit =
                (ChargeUnit) Enum.GetValues(typeof (ChargeUnit)).GetValue(CbxPropCommission.SelectedIndex);
            instrPropSelectedInstrument.CommissionScope =
                (CommissionScope) Enum.GetValues(typeof (CommissionScope)).GetValue(CbxPropCommScope.SelectedIndex);
            instrPropSelectedInstrument.CommissionTime =
                (CommissionTime) Enum.GetValues(typeof (CommissionTime)).GetValue(CbxPropCommTime.SelectedIndex);
            instrPropSelectedInstrument.Digits = (int) NudPropDigits.Value;
            instrPropSelectedInstrument.LotSize = (int) NudPropLotSize.Value;
            instrPropSelectedInstrument.Spread = (float) NudPropSpread.Value;
            instrPropSelectedInstrument.SwapLong = (float) NudPropSwapLong.Value;
            instrPropSelectedInstrument.SwapShort = (float) NudPropSwapShort.Value;
            instrPropSelectedInstrument.Commission = (float) NudPropCommission.Value;
            instrPropSelectedInstrument.Slippage = (int) NudPropSlippage.Value;
            if (TbxPropAccountIn.Text == "USD")
                instrPropSelectedInstrument.RateToUSD = (float) NudPropAccountRate.Value;
            else
                instrPropSelectedInstrument.RateToEUR = (float) NudPropAccountRate.Value;
        }

        /// <summary>
        ///     Calculates the account exchange rate.
        /// </summary>
        private void SetAcountExchangeRate()
        {
            if (TbxPropPriceIn.Text == TbxPropAccountIn.Text)
            {
                TbxPropAccountRate.Text = Language.T("Not used");
                NudPropAccountRate.Value = 1;
                NudPropAccountRate.Enabled = false;
                if (TbxPropAccountIn.Text == "USD")
                    instrPropSelectedInstrument.RateToUSD = 1;
                else if (TbxPropAccountIn.Text == "EUR")
                    instrPropSelectedInstrument.RateToEUR = 1;
            }
            else if (TbxPropType.Text == "Forex" && TbxPropSymbol.Text.StartsWith(TbxPropAccountIn.Text))
            {
                TbxPropAccountRate.Text = Language.T("Deal price");
                NudPropAccountRate.Value = 0;
                NudPropAccountRate.Enabled = false;
                if (TbxPropAccountIn.Text == "USD")
                    instrPropSelectedInstrument.RateToUSD = 0;
                else if (TbxPropAccountIn.Text == "EUR")
                    instrPropSelectedInstrument.RateToEUR = 0;
            }
            else
            {
                TbxPropAccountRate.Text = TbxPropAccountIn.Text + TbxPropPriceIn.Text;
                if (TbxPropAccountIn.Text == "USD")
                    NudPropAccountRate.Value = (decimal) instrPropSelectedInstrument.RateToUSD;
                else if (TbxPropAccountIn.Text == "EUR")
                    NudPropAccountRate.Value = (decimal) instrPropSelectedInstrument.RateToEUR;
                NudPropAccountRate.Enabled = true;
                NudPropAccountRate.ForeColor = Color.Red;
            }
        }

        /// <summary>
        ///     The lbxInstruments selected index changed
        /// </summary>
        private void LbxInstrumentsSelectedValueChanged(object sender, EventArgs e)
        {
            if (LbxInstruments.SelectedItem == null) return;

            instrPropSelectedInstrument = Instruments.InstrumentList[LbxInstruments.SelectedItem.ToString()].Clone();
            SetPropertiesForm();
        }

        /// <summary>
        ///     Digit changed
        /// </summary>
        private void NudPropDigitsValueChanged(object sender, EventArgs e)
        {
            TbxPropPoint.Text = (1/Math.Pow(10, (float) NudPropDigits.Value)).ToString("0.#####");
        }

        /// <summary>
        ///     Checks the instrument currency.
        /// </summary>
        private void TbxPropPriceInTextChanged(object sender, EventArgs e)
        {
            var regexPriceIn = new Regex(@"^[A-Z]{3}$");

            if (regexPriceIn.IsMatch(TbxPropPriceIn.Text))
            {
                TbxPropPriceIn.ForeColor = colorText;

                SetAcountExchangeRate();
            }
            else
            {
                TbxPropPriceIn.ForeColor = Color.Red;
            }
        }

        /// <summary>
        ///     Checks the commission time
        /// </summary>
        private void CbxPropCommissionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxPropCommission.SelectedIndex == 1)
            {
                CbxPropCommScope.SelectedIndex = 1;
                CbxPropCommScope.Enabled = false;
            }
            else
                CbxPropCommScope.Enabled = true;
        }

        /// <summary>
        ///     Sets the data file names.
        /// </summary>
        private void TbxPropFileNameTextChanged(object sender, EventArgs e)
        {
            string text = TbxPropFileName.Text;
            var regexFileName = new Regex(@"^[a-zA-Z\$\#][\w- ]*$");

            if (regexFileName.IsMatch(text))
            {
                TbxPropFileName.ForeColor = colorText;
                LblPropDataFiles.Text =
                    "   1 Minute    -  " + text + "1.csv;    1 Hour    -  " + text + "60.csv" + Environment.NewLine +
                    "   5 Minutes  -  " + text + "5.csv;    4 Hours  -  " + text + "240.csv" + Environment.NewLine +
                    " 15 Minutes  -  " + text + "15.csv;  1 Day     -  " + text + "1440.csv" + Environment.NewLine +
                    " 30 Minutes  -  " + text + "30.csv;  1 Week  -  " + text + "10080.csv";
            }
            else
            {
                TbxPropFileName.ForeColor = Color.Red;
                LblPropDataFiles.Text = "";
            }
        }

        /// <summary>
        ///     BtnAccept Clicked.
        /// </summary>
        private void BtnAcceptClick(object sender, EventArgs e)
        {
            if (!ValidatePropertiesForm()) return;
            SetSelectedInstrument();
            if (Instruments.InstrumentList.ContainsKey(instrPropSelectedInstrument.Symbol))
            {
                // The instrument exists. We change it.
                Instruments.InstrumentList[instrPropSelectedInstrument.Symbol] = instrPropSelectedInstrument.Clone();
            }
            else
            {
                // The instrument doesn't exist. We create it.
                Instruments.InstrumentList.Add(instrPropSelectedInstrument.Symbol, instrPropSelectedInstrument.Clone());
                LbxInstruments.Items.Add(instrPropSelectedInstrument.Symbol);
                NeedReset = true;
            }
        }

        /// <summary>
        ///     BtnAdd Clicked.
        /// </summary>
        private void BtnAddInstrAddClick(object sender, EventArgs e)
        {
            if (
                ValidateSymbol(TbxAddInstrSymbol.Text,
                               (InstrumetType) Enum.Parse(typeof (InstrumetType), CbxAddInstrType.Text)) &&
                !LbxInstruments.Items.Contains(TbxAddInstrSymbol.Text))
            {
                instrPropSelectedInstrument = new InstrumentProperties(TbxAddInstrSymbol.Text,
                                                                       (InstrumetType)
                                                                       Enum.Parse(typeof (InstrumetType),
                                                                                  CbxAddInstrType.Text));
                SetPropertiesForm();
                SetSelectedInstrument();
            }
            else
            {
                MessageBox.Show(Language.T("Wrong Symbol!"), Language.T("Instrument Properties"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        ///     BtnDelete Clicked.
        /// </summary>
        private void BtnDeleteClick(object sender, EventArgs e)
        {
            string symbol = LbxInstruments.SelectedItem.ToString();
            int index = LbxInstruments.SelectedIndex;

            if (symbol == "EURUSD" || symbol == "GBPUSD" ||
                symbol == "USDCHF" || symbol == "USDJPY" ||
                symbol == Data.Symbol)
            {
                MessageBox.Show(
                    Language.T("You cannot delete this instrument!"),
                    Language.T("Instrument Editor"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            DialogResult dr = MessageBox.Show(
                Language.T("Do you want to delete the selected instrument?"),
                Language.T("Instrument Editor"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Instruments.InstrumentList.Remove(symbol);
                LbxInstruments.Items.Remove(symbol);
                if (index > 0)
                    LbxInstruments.SelectedIndex = index - 1;
                else
                    LbxInstruments.SelectedIndex = index;

                NeedReset = true;
            }
        }

        /// <summary>
        ///     BtnUp Clicked.
        /// </summary>
        private void BtnUpClick(object sender, EventArgs e)
        {
            string symbol = LbxInstruments.SelectedItem.ToString();
            int index = LbxInstruments.SelectedIndex;

            if (index > 0)
            {
                LbxInstruments.Items.RemoveAt(index);
                LbxInstruments.Items.Insert(index - 1, symbol);
                LbxInstruments.SelectedIndex = index - 1;
                NeedReset = true;
            }
        }

        /// <summary>
        ///     BtnDown Clicked.
        /// </summary>
        private void BtnDownClick(object sender, EventArgs e)
        {
            string symbol = LbxInstruments.SelectedItem.ToString();
            int index = LbxInstruments.SelectedIndex;

            if (index < LbxInstruments.Items.Count - 1)
            {
                LbxInstruments.Items.RemoveAt(index);
                LbxInstruments.Items.Insert(index + 1, symbol);
                LbxInstruments.SelectedIndex = index + 1;
                NeedReset = true;
            }
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        ///     Check whether the restart is necessary.
        /// </summary>
        private void InstrumentEditorFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!NeedReset) return;
            var temp = new Dictionary<string, InstrumentProperties>(Instruments.InstrumentList.Count);

            foreach (var kvp in Instruments.InstrumentList)
                temp.Add(kvp.Key, kvp.Value.Clone());

            Instruments.InstrumentList.Clear();

            foreach (string symbol in LbxInstruments.Items)
                Instruments.InstrumentList.Add(symbol, temp[symbol].Clone());
        }
    }
}