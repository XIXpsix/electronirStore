using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumHelper
{
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()?
                        .GetName() ?? enumValue.ToString();
    }
}