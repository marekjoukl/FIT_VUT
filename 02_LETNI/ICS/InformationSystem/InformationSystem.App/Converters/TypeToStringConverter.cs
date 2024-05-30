using System.Globalization;
using CommunityToolkit.Maui.Converters;
using InformationSystem.App.Resources.Texts;
using InformationSystem.Common.Enums;

namespace InformationSystem.App.Converters;


public class TypeToStringConverter : BaseConverterOneWay<ActivityType, string>
{
    public override string ConvertFrom(ActivityType value, CultureInfo? culture)
        => TypeText.ResourceManager.GetString(value.ToString(), culture)
           ?? TypeText.None;

    public override string DefaultConvertReturnValue { get; set; } = TypeText.None;
}
