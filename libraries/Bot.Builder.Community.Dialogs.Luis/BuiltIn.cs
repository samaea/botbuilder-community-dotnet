using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Bot.Builder.Community.Dialogs.Luis
{
    public static partial class BuiltIn
    {
        public static partial class DateTime
        {
            public enum DayPart
            {
                [Description("morning")]
                MO,
                [Description("midday")]
                MI,
                [Description("afternoon")]
                AF,
                [Description("evening")]
                EV,
                [Description("night")]
                NI
            }

            public enum Reference
            {
                [Description("past")]
                PAST_REF,
                [Description("present")]
                PRESENT_REF,
                [Description("future")]
                FUTURE_REF
            }

            [Serializable]
            public sealed class DateTimeResolution : Resolution, IEquatable<DateTimeResolution>
            {
                public const string PatternDate =
                @"
                    (?:
                        (?<year>X+|\d+)
                        (?:
                            -
                            (?<weekM>W)?
                            (?<month>X+|\d+)
                            (?:
                                -
                                (?<weekD>W)?
                                (?<day>X+|\d+)
                            )?
                        )?
                    )
                ";

                public const string PatternTime =
                @"
                    (?:
                        T
                        (?:
                            (?<part>MO|MI|AF|EV|NI)
                        |
                            (?<hour>X+|\d+)
                            (?:
                                :
                                (?<minute>X+|\d+)
                                (?:
                                    :
                                    (?<second>X+|\d+)
                                )?
                            )?
                        )
                    )
                ";

                public const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;

                public static readonly string Pattern = $"^({PatternDate}{PatternTime} | {PatternDate} | {PatternTime})$";

                public static readonly Regex Regex = new Regex(Pattern, Options);

                private static readonly IReadOnlyDictionary<string, Reference> ReferenceByText
                    = new Dictionary<string, Reference>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "PAST_REF", DateTime.Reference.PAST_REF },
                        { "PRESENT_REF", DateTime.Reference.PRESENT_REF },
                        { "FUTURE_REF", DateTime.Reference.FUTURE_REF },
                    };

                public DateTimeResolution(
                    Reference? reference = null,
                    int? year = null,
                    int? month = null,
                    int? day = null,
                    int? week = null,
                    DayOfWeek? dayOfWeek = null,
                    DayPart? dayPart = null,
                    int? hour = null,
                    int? minute = null,
                    int? second = null)
                {
                    this.Reference = reference;
                    this.Year = year;
                    this.Month = month;
                    this.Day = day;
                    this.Week = week;
                    this.DayOfWeek = dayOfWeek;
                    this.DayPart = dayPart;
                    this.Hour = hour;
                    this.Minute = minute;
                    this.Second = second;
                }

                public DateTimeResolution(System.DateTime dateTime)
                {
                    this.Year = dateTime.Year;
                    this.Month = dateTime.Month;
                    this.Day = dateTime.Day;
                    this.Hour = dateTime.Hour;
                    this.Minute = dateTime.Minute;
                    this.Second = dateTime.Second;
                }

                public Reference? Reference { get; }

                public int? Year { get; }

                public int? Month { get; }

                public int? Day { get; }

                public int? Week { get; }

                public DayOfWeek? DayOfWeek { get; }

                public DayPart? DayPart { get; }

                public int? Hour { get; }

                public int? Minute { get; }

                public int? Second { get; }

                public static bool TryParse(string text, out DateTimeResolution resolution)
                {
                    DateTime.Reference reference;
                    if (ReferenceByText.TryGetValue(text, out reference))
                    {
                        resolution = new DateTimeResolution(reference);
                        return true;
                    }

                    var match = Regex.Match(text);
                    if (match.Success)
                    {
                        var groups = match.Groups;
                        bool weekM = groups["weekM"].Success;
                        bool weekD = weekM || groups["weekD"].Success;

                        resolution = new DateTimeResolution(
                                year: ParseIntOrNull(groups["year"]),
                                week: weekM ? ParseIntOrNull(groups["month"]) : null,
                                month: !weekM ? ParseIntOrNull(groups["month"]) : null,
                                dayOfWeek: weekD ? (DayOfWeek?)ParseIntOrNull(groups["day"]) : null,
                                day: !weekD ? ParseIntOrNull(groups["day"]) : null,
                                dayPart: ParseEnumOrNull<DayPart>(groups["part"]),
                                hour: ParseIntOrNull(groups["hour"]),
                                minute: ParseIntOrNull(groups["minute"]),
                                second: ParseIntOrNull(groups["second"]));

                        return true;
                    }

                    resolution = null;
                    return false;
                }

                public bool Equals(DateTimeResolution other)
                {
                    return other != null
                        && this.Reference == other.Reference
                        && this.Year == other.Year
                        && this.Month == other.Month
                        && this.Day == other.Day
                        && this.Week == other.Week
                        && this.DayOfWeek == other.DayOfWeek
                        && this.DayPart == other.DayPart
                        && this.Hour == other.Hour
                        && this.Minute == other.Minute
                        && this.Second == other.Second;
                }

                public override bool Equals(object other)
                {
                    return this.Equals(other as DateTimeResolution);
                }

                public override int GetHashCode()
                {
                    throw new NotImplementedException();
                }

                private static int? ParseIntOrNull(Group group)
                {
                    if (group.Success)
                    {
                        var text = group.Value;
                        int number;
                        if (int.TryParse(text, out number))
                        {
                            return number;
                        }
                        else if (text.Length > 0)
                        {
                            for (int index = 0; index < text.Length; ++index)
                            {
                                switch (text[index])
                                {
                                    case 'X':
                                    case 'x':
                                        continue;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }

                            // -1 means some variable X rather than missing "null" or specified constant value
                            return -1;
                        }
                    }

                    return null;
                }

                private static E? ParseEnumOrNull<E>(Group group)
                    where E : struct
                {
                    if (group.Success)
                    {
                        var text = group.Value;
                        E result;
                        if (Enum.TryParse<E>(text, out result))
                        {
                            return result;
                        }
                    }

                    return null;
                }
            }

            public sealed class DurationResolution
            {
            }
        }
    }
}
