using System.Globalization;
using CommunityToolkit.Maui.Converters;
using InformationSystem.App.Resources.Texts;
using InformationSystem.Common.Enums;

namespace InformationSystem.App.Converters;

public class RoomToStringConverter : BaseConverterOneWay<ActivityRoom, string>
{
    public override string ConvertFrom(ActivityRoom value, CultureInfo? culture)
        => RoomText.ResourceManager.GetString(value.ToString(), culture)
           ?? RoomText.None;

    public override string DefaultConvertReturnValue { get; set; } = RoomText.None;
}
