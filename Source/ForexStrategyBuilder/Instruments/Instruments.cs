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
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public static class Instruments
    {
        private static readonly string PathToInstrumentsFile;
        private static readonly XmlDocument XmlInstruments;
        private static Dictionary<String, InstrumentProperties> dictInstrument;
        private static bool isReset;

        /// <summary>
        ///     Public constructor.
        /// </summary>
        static Instruments()
        {
            string externalInstrumentsFile = string.Empty;
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-instrumentsfile="))
                    externalInstrumentsFile = CommandLineParser.GetValue(arg);

            XmlInstruments = new XmlDocument();

            PathToInstrumentsFile = String.IsNullOrEmpty(externalInstrumentsFile)
                                        ? Path.Combine(Path.Combine(Data.UserFilesDir, "System"), "instruments.xml")
                                        : externalInstrumentsFile;
        }

        /// <summary>
        ///     Gets the symbols list.
        /// </summary>
        public static IEnumerable<string> SymbolList
        {
            get
            {
                var symbols = new string[dictInstrument.Count];
                dictInstrument.Keys.CopyTo(symbols, 0);
                return symbols;
            }
        }

        /// <summary>
        ///     Gets or sets the instruments list.
        /// </summary>
        public static Dictionary<String, InstrumentProperties> InstrumentList
        {
            get { return dictInstrument; }
        }

        /// <summary>
        ///     Loads the instruments file.
        /// </summary>
        public static void LoadInstruments()
        {
            try
            {
                XmlInstruments.Load(PathToInstrumentsFile);
            }
            catch (FileNotFoundException)
            {
                XmlInstruments.LoadXml(Resources.instruments);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Load Instruments");
                XmlInstruments.LoadXml(Resources.instruments);
            }

            try
            {
                ParseInstruments();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Parse Instruments");
            }
        }

        /// <summary>
        ///     Resets the instruments file.
        /// </summary>
        public static void ResetInstruments()
        {
            XmlInstruments.LoadXml(Resources.instruments);
            ParseInstruments();
            SaveInstruments();
            isReset = true;
        }

        /// <summary>
        ///     Saves the config file.
        /// </summary>
        public static void SaveInstruments()
        {
            if (isReset) return;

            try
            {
                GenerateXmlFile().Save(PathToInstrumentsFile);
            }
            catch (Exception e)
            {
                if (!Data.AutostartGenerator && Data.AutoOptimize == "")
                    MessageBox.Show(e.Message, "Save Instruments");
            }
        }

        /// <summary>
        ///     Parses the instruments file.
        /// </summary>
        private static void ParseInstruments()
        {
            int instrumentsCount = XmlInstruments.GetElementsByTagName("instrument").Count;
            dictInstrument = new Dictionary<string, InstrumentProperties>(instrumentsCount);

            try
            {
                foreach (XmlNode nodeInstr in XmlInstruments.GetElementsByTagName("instrument"))
                {
                    string symbol = nodeInstr.SelectSingleNode("symbol").InnerText;
                    var instrType =
                        (InstrumetType)
                        Enum.Parse(typeof (InstrumetType), nodeInstr.SelectSingleNode("instrumentType").InnerText);
                    var instrProp = new InstrumentProperties(symbol, instrType)
                        {
                            Comment = nodeInstr.SelectSingleNode("comment").InnerText,
                            Digits = int.Parse(nodeInstr.SelectSingleNode("digits").InnerText),
                            LotSize = int.Parse(nodeInstr.SelectSingleNode("contractSize").InnerText),
                            Spread = StringToFloat(nodeInstr.SelectSingleNode("spread").InnerText),
                            SwapUnit = ParseChargeUnit(nodeInstr.SelectSingleNode("swapType").InnerText),
                            SwapLong = StringToFloat(nodeInstr.SelectSingleNode("swapLong").InnerText),
                            SwapShort = StringToFloat(nodeInstr.SelectSingleNode("swapShort").InnerText),
                            CommissionUnit = ParseChargeUnit(nodeInstr.SelectSingleNode("commissionType").InnerText),
                            CommissionScope =
                                (CommissionScope)
                                Enum.Parse(typeof (CommissionScope),
                                           nodeInstr.SelectSingleNode("commissionScope").InnerText),
                            CommissionTime =
                                (CommissionTime)
                                Enum.Parse(typeof (CommissionTime),
                                           nodeInstr.SelectSingleNode("commissionTime").InnerText),
                            Commission = StringToFloat(nodeInstr.SelectSingleNode("commission").InnerText),
                            Slippage = int.Parse(nodeInstr.SelectSingleNode("slippage").InnerText),
                            PriceIn = nodeInstr.SelectSingleNode("priceIn").InnerText,
                            RateToUSD = StringToFloat(nodeInstr.SelectSingleNode("rateToUSD").InnerText),
                            RateToEUR = StringToFloat(nodeInstr.SelectSingleNode("rateToEUR").InnerText),
                            BaseFileName = nodeInstr.SelectSingleNode("baseFileName").InnerText
                        };
                    dictInstrument.Add(symbol, instrProp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Parsing Instruments");
            }
        }

        /// <summary>
        ///     Generates instrument.xml file.
        /// </summary>
        private static XmlDocument GenerateXmlFile()
        {
            // Create the XmlDocument.
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<fsb></fsb>");

            // Create the XML declaration.
            XmlDeclaration xmldecl = xmlDoc.CreateXmlDeclaration("1.0", null, null);

            // Add new node to the document.
            XmlElement root = xmlDoc.DocumentElement;
            xmlDoc.InsertBefore(xmldecl, root);

            foreach (var kvp in dictInstrument)
            {
                InstrumentProperties instrProp = kvp.Value;

                // Creates an instrument element.
                XmlElement instrument = xmlDoc.CreateElement("instrument");

                XmlElement element = xmlDoc.CreateElement("symbol");
                element.InnerText = instrProp.Symbol;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("instrumentType");
                element.InnerText = instrProp.InstrType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("comment");
                element.InnerText = instrProp.Comment;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("digits");
                element.InnerText = instrProp.Digits.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("contractSize");
                element.InnerText = instrProp.LotSize.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("spread");
                element.InnerText = instrProp.Spread.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapType");
                element.InnerText = instrProp.SwapUnit.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapLong");
                element.InnerText = instrProp.SwapLong.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapShort");
                element.InnerText = instrProp.SwapShort.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionType");
                element.InnerText = instrProp.CommissionUnit.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionScope");
                element.InnerText = instrProp.CommissionScope.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionTime");
                element.InnerText = instrProp.CommissionTime.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commission");
                element.InnerText = instrProp.Commission.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("slippage");
                element.InnerText = instrProp.Slippage.ToString(CultureInfo.InvariantCulture);
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("priceIn");
                element.InnerText = instrProp.PriceIn;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("rateToUSD");
                element.InnerText = instrProp.RateToUSD.ToString("F4");
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("rateToEUR");
                element.InnerText = instrProp.RateToEUR.ToString("F4");
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("baseFileName");
                element.InnerText = instrProp.BaseFileName;
                instrument.AppendChild(element);

                if (xmlDoc.DocumentElement != null) xmlDoc.DocumentElement.AppendChild(instrument);
            }

            return xmlDoc;
        }

        /// <summary>
        ///     Parses string values to float.
        /// </summary>
        private static float StringToFloat(string input)
        {
            float output = 0;
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            input = input.Replace(",", decimalSeparator);
            input = input.Replace(".", decimalSeparator);

            try
            {
                output = float.Parse(input);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Parsing Instruments");
            }

            return output;
        }

        private static ChargeUnit ParseChargeUnit(string input)
        {
            if (input == "Points" || input == "pips")
                return ChargeUnit.Points;
            if (input == "Money" || input == "money")
                return ChargeUnit.Money;
            if (input == "Percents" || input == "percents")
                return ChargeUnit.Percents;
            return ChargeUnit.Points;
        }
    }
}