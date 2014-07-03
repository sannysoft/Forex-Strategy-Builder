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
using System.Xml.Serialization;

namespace ForexStrategyBuilder.Library
{
    public class Libraries
    {
        private LibrariesSettings settings;

        public bool IsSourceCompiled(string sourcePath)
        {
            string name = Path.GetFileNameWithoutExtension(sourcePath);
            LibRecord record = ReadRecord(name);

            if (record == null)
                return false;

            var sourceInfo = new FileInfo(sourcePath);

            if (record.SourceLastWriteTime != sourceInfo.LastWriteTime)
                return false;

            string dllPath = Path.Combine(Data.LibraryDir, Path.GetFileNameWithoutExtension(sourcePath) + ".dll");
            if (!File.Exists(dllPath))
                return false;

            return true;
        }

        public void AddRecord(LibRecord record)
        {
            if (settings.Records.ContainsKey(record.SourceFileName))
                settings.Records[record.SourceFileName] = record;
            else
                settings.Records.Add(record.SourceFileName, record);
        }

        private LibRecord ReadRecord(string name)
        {
            return settings.Records.ContainsKey(name) ? settings.Records[name] : null;
        }

        public void LoadSettings(string path)
        {
            settings = new LibrariesSettings();

            if (!File.Exists(path))
                return;

            var xmlSerializer = new XmlSerializer(typeof (LibrariesSettings));

            var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var reader = new StreamReader(file, System.Text.Encoding.ASCII))
            {
                try
                {
                    settings = xmlSerializer.Deserialize(reader) as LibrariesSettings;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                reader.Close();
            }
        }

        public void SaveSettings(string path)
        {
            var xmlSerializer = new XmlSerializer(typeof (LibrariesSettings));
            using (var writer = new StreamWriter(path))
            {
                xmlSerializer.Serialize(writer, settings);
            }
        }
    }
}