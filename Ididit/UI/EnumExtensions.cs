using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Ididit.UI;

internal static class EnumExtensions
{
    // get description from enum:

    public static string GetDescription(this Enum value)
    {
        return value.GetType().
            GetMember(value.ToString()).
            First().
            GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attribute
            ? attribute.Description
            : throw new Exception($"Enum member '{value.GetType()}.{value}' doesn't have a [DescriptionAttribute]!");
    }

    // get enum from description:

    public static T GetEnum<T>(this string description) where T : Enum
    {
        foreach (FieldInfo fieldInfo in typeof(T).GetFields())
        {
            if (fieldInfo.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attribute && attribute.Description == description && fieldInfo.GetRawConstantValue() is T t)
                return t;
        }

        throw new Exception($"Enum '{typeof(T)}' doesn't have a member with a [DescriptionAttribute('{description}')]!");
    }
}
