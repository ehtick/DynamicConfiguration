﻿using Microsoft.AspNetCore.Components;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Aguacongas.Configuration.Razor
{
    public partial class InputSetting
    {
        [Parameter]
        public object? Model { get; set; }

        [Parameter]
        public object? Value { get; set; }

        [Parameter]
        public PropertyInfo? Property { get; set; }

        [Parameter]
        public string? Path { get; set; }

        private string? Error { get; set; }

        private string? PropertyName
            => (Property?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? Property?.Name;

        private Type PropertyType => Property?.PropertyType ?? typeof(object);

        private Type UnderlyingType => Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;

        private string? Placeholder => IsTimeSpan ? "00:00:00" : null;

        private bool IsTimeSpan => UnderlyingType.IsAssignableTo(typeof(TimeSpan));

        private bool IsString => UnderlyingType.IsAssignableTo(typeof(string)) || IsTimeSpan;

        private bool IsNumber => Type.GetTypeCode(UnderlyingType) switch
        {
            TypeCode.Int16 or
            TypeCode.Int32 or
            TypeCode.Int64 or
            TypeCode.Decimal or
            TypeCode.Double or
            TypeCode.Single or
            TypeCode.Byte or
            TypeCode.SByte or
            TypeCode.UInt16 or
            TypeCode.UInt32 or
            TypeCode.UInt64 => true,
            _ => false,
        };

        private bool IsDate => UnderlyingType.IsAssignableTo(typeof(DateTime)) || UnderlyingType.IsAssignableTo(typeof(DateTimeOffset));

        private bool IsBool => UnderlyingType.IsAssignableTo(typeof(bool));

        private bool IsEnum => UnderlyingType.IsEnum;

        private bool IsEnumerable => UnderlyingType.IsAssignableTo(typeof(IEnumerable));

        private string? ValueAsString
        {
            get { return Value?.ToString(); }
            set
            {
                if (UnderlyingType.IsAssignableTo(typeof(string)))
                {
                    SetValue(value);
                    return;
                }
                if (IsTimeSpan)
                {
                    if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                    {
                        Error = $"Cannot parse '{value}'";
                        return;
                    }
                    SetValue(timeSpan);
                }
                Error = null;
            }
        }

        private double? ValueAsDouble
        {
            get
            {
                if (Value is null)
                {
                    return null;
                }
                return Convert.ToDouble(Value);
            }
            set
            {
                if (value is null)
                {
                    SetValue(null);
                    return;
                }

                Value = Type.GetTypeCode(UnderlyingType) switch
                {
                    TypeCode.Int16 => Convert.ToInt16(value),
                    TypeCode.Int32 => Convert.ToInt32(value),
                    TypeCode.Int64 => Convert.ToInt64(value),
                    TypeCode.Decimal => Convert.ToDecimal(value),
                    TypeCode.Double => Convert.ToDouble(value),
                    TypeCode.Single => Convert.ToSingle(value),
                    TypeCode.Byte => Convert.ToByte(value),
                    TypeCode.SByte => Convert.ToSByte(value),
                    TypeCode.UInt16 => Convert.ToUInt16(value),
                    TypeCode.UInt32 => Convert.ToUInt32(value),
                    TypeCode.UInt64 => Convert.ToUInt64(value),
                    _ => null
                };

                SetValue(Value);
            }
        }

        private bool ValueAsBool
        {
            get
            {
                if (Value is null)
                {
                    return false;
                }
                return (bool)Value;
            }
            set
            {
                SetValue(value);
            }
        }

        private DateTimeOffset? ValueAsDate
        {
            get
            {
                if (Value is null)
                {
                    return null;
                }
                if (UnderlyingType.IsAssignableTo(typeof(DateTimeOffset)))
                {
                    return (DateTimeOffset)Value;
                }
                var dateTime = (DateTime)Value;
                if (dateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime)
                {
                    return DateTimeOffset.MinValue;
                }
                if (dateTime.ToUniversalTime() >= DateTimeOffset.MaxValue.UtcDateTime)
                {
                    return DateTimeOffset.MaxValue;
                }
                return new DateTimeOffset(dateTime);
            }
            set
            {
                if (UnderlyingType.IsAssignableTo(typeof(DateTimeOffset)))
                {
                    SetValue(value);
                    return;
                }
                SetValue(value?.DateTime);
            }
        }

        private Enum? ValueAsEnum
        {
            get
            {
                if (Value is null)
                {
                    return null;
                }
                return (Enum)Value;
            }
            set
            {
                SetValue(value);
            }
        }

        private void SetValue(object? value)
        {
            Property?.SetValue(Model, value);
            Value = value;
        }

        private void CreateValue()
        {
            var constructor = UnderlyingType.GetConstructor(Array.Empty<Type>());
            if (constructor is null)
            {
                throw new InvalidOperationException("Cannot create value empty constructor not found.");
            }

            SetValue(constructor.Invoke(null));
        }

        private void DeleteValue()
        {
            SetValue(null);
        }
    }
}
