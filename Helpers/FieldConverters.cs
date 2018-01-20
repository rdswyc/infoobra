using System;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Phone.Application;

using InfoObra.Services;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InfoObra.Helpers
{
    public static class FieldConverters
    {
        public static void RegisterAll()
        {
            // Text Fields
            Converter.RegisterDisplayFieldValueConverter(FieldType.Text, GetTextValue);
            Converter.RegisterDisplayFieldValueConverter(FieldType.Note, GetTextValue);
            Converter.RegisterDisplayFieldValueConverter(FieldType.Number, GetTextValue);

            // Lookup Field
            Converter.RegisterDisplayFieldValueConverter(FieldType.Lookup, GetLookupValue);
            Converter.RegisterEditFieldValueConverter(FieldType.Lookup, GetLookupValue, SetLookupValue);
            Converter.RegisterNewFieldValueConverter(FieldType.Lookup, GetLookupValue, SetLookupValue);

            // Choice Field
            Converter.RegisterEditFieldValueConverter(FieldType.Choice, SetChoiceValue);
            Converter.RegisterNewFieldValueConverter(FieldType.Choice, SetChoiceValue);
        }

        public static object GetTextValue(string fieldName, ListItem item, ConversionContext context)
        {
            string fieldValue = string.Empty;

            if (item != null && item.IsObjectPropertyInstantiated("FieldValuesAsText"))
            {
                fieldValue = (item.FieldValuesAsText[fieldName] == Constants.Empty ? string.Empty : item.FieldValuesAsText[fieldName]);
            }

            return fieldValue;
        }

        public static object GetLookupValue(string fieldName, ListItem item, ConversionContext context)
        {
            string fieldValue = string.Empty;

            try
            {
                if (item != null && item.IsObjectPropertyInstantiated("FieldValuesAsText"))
                {
                    fieldValue = item.FieldValuesAsText[fieldName];
                }
            }
            catch (Exception)
            { }

            return fieldValue;
        }

        public static void SetLookupValue(string fieldName, object fieldValue, ListItem item, ConversionContext context)
        {
            if (item == null)
                return;

            int id;

            if (int.TryParse(fieldValue as string, out id))
            {
                item[fieldName] = new FieldLookupValue() { LookupId = int.Parse(fieldValue.ToString()) };
            }
        }

        public static void SetChoiceValue(string fieldName, object fieldValue, ListItem item, ConversionContext context)
        {
            if (item != null)
            {
                item[fieldName] = fieldValue;
            }
        }
    }
}