using System;
using System.Text.RegularExpressions;

using InfoObra.Resources;

namespace InfoObra.Helpers
{
    public static class FieldValidators
    {
        public static bool Validate(string listTile, string fieldName, object value, out string errorMessage)
        {
            string fieldValue = (value == null) ? null : value.ToString();

            switch (fieldName)
            {
                case "Title":

                    if (string.IsNullOrEmpty(fieldValue) && listTile != "qualidade")
                    {
                        errorMessage = AppResources.RequiredField;
                        return false;
                    }
                    else
                    {
                        break;
                    }

                case "quantidade_oficial":
                case "quantidade_meio_oficial":

                    int x;
                    if (int.TryParse(fieldValue, out x) || string.IsNullOrEmpty(fieldValue))
                    {
                        break;
                    }
                    else
                    {
                        errorMessage = AppResources.IncorrectValue;
                        return false;
                    }

                case "inicio_planejado":
                case "novo_inicio_planejado":

                    if (string.IsNullOrEmpty(fieldValue))
                        break;

                    Regex Rgx = new Regex(@"^\d{2}\/\d{4}$");

                    if (Rgx.IsMatch(fieldValue))
                    {
                        break;
                    }
                    else
                    {
                        errorMessage = string.Format("{0}. {1}: {2}", AppResources.IncorrectValue, AppResources.CurrentWeek, FilterPage.GetCurrentWeek());
                        return false;
                    }

                default:
                    break;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
