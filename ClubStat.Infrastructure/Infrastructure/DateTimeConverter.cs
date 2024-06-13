// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Wed 12-Jun-2024
// ***********************************************************************
// <copyright file="DateTimeConverter.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure;
/// <summary>
/// Class DateTimeConverter is needed to get proper date time in json making the date "round-tripable".
/// Implements the <see cref="System.Text.Json.Serialization.JsonConverter{System.DateTime}" />
/// </summary>
/// <remarks>
/// https://learn.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support
/// native date time lozes the timezone 
/// </remarks>
/// <seealso cref="System.Text.Json.Serialization.JsonConverter{System.DateTime}" />
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
        {
            return value;
        }

        throw new FormatException();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        Span<byte> utf8Date = new byte[29];

        bool result = Utf8Formatter.TryFormat(value, utf8Date, out _, new StandardFormat('R'));
       

        writer.WriteStringValue(utf8Date);
    }
}
