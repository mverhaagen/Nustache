﻿using System.IO;
using System.Data;
using System.Xml;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nustache.Core
{
    public static class Render
    {
        public static string StringToString(string template, object data)
        {
            return StringToString(template, data, null);
        }

        public static string StringToString(string template, object data, TemplateLocator templateLocator)
        {
            var reader = new StringReader(template);
            var writer = new StringWriter();
            Template(reader, data, writer, templateLocator);
            return writer.GetStringBuilder().ToString();
        }

        public static string FileToString(string templatePath, object data)
        {
            var template = File.ReadAllText(templatePath);
            var templateLocator = GetTemplateLocator(templatePath);
            return StringToString(template, data, templateLocator.GetTemplate);
        }

        public static string DataSetToString(string templatePath, DataSet dataSet)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataSet.GetXml());
            string jsonText = JsonConvert.SerializeObject(doc);

            var serializer = new JavaScriptSerializer();
            object data = serializer.Deserialize<IDictionary<string, object>>(jsonText);

            return FileToString(templatePath, data);
        }

        public static void StringToFile(string template, object data, string outputPath)
        {
            StringToFile(template, data, outputPath, null);
        }

        public static void StringToFile(string template, object data, string outputPath, TemplateLocator templateLocator)
        {
            var reader = new StringReader(template);
            using (var writer = File.CreateText(outputPath))
            {
                Template(reader, data, writer, templateLocator);
            }
        }

        public static void FileToFile(string templatePath, object data, string outputPath)
        {
            var reader = new StringReader(File.ReadAllText(templatePath));
            var templateLocator = GetTemplateLocator(templatePath);
            using (var writer = File.CreateText(outputPath))
            {
                Template(reader, data, writer, templateLocator.GetTemplate);
            }
        }

        public static void Template(TextReader reader, object data, TextWriter writer)
        {
            Template(reader, data, writer, null);
        }

        public static void Template(TextReader reader, object data, TextWriter writer, TemplateLocator templateLocator)
        {
            var template = new Template();
            template.Load(reader);
            template.Render(data, writer, templateLocator);
        }

        private static FileSystemTemplateLocator GetTemplateLocator(string templatePath)
        {
            string dir = Path.GetDirectoryName(templatePath);
            string ext = Path.GetExtension(templatePath);
            return new FileSystemTemplateLocator(ext, dir);
        }
    }
}