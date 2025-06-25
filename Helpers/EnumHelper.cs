using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace TaskManager.Helpers
{
    public static class EnumHelper
    {
        // Método para obtener el nombre display de un valor enum
        public static string GetDisplayName(Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }

        public static SelectList ToSelectListWithDisplayNames<TEnum>() where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var items = values.Select(value => new SelectListItem
            {
                Text = GetDisplayName(value as Enum),
                Value = value.ToString()
            }).ToList();

            return new SelectList(items, "Value", "Text");
        }
    }
}