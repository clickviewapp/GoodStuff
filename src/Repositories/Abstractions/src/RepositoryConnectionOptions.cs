namespace ClickView.GoodStuff.Repositories.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public abstract class RepositoryConnectionOptions
    {
        private readonly Dictionary<string, string> _parameters = new(StringComparer.OrdinalIgnoreCase);

        protected void SetParameter(string key, string? value)
        {
            // null check here is because of warnings in older .net versions
            // once we remove netstandard2.0 targets this null can be removed
            if (value is null || string.IsNullOrEmpty(value))
            {
                _parameters.Remove(key);
            }
            else
            {
                _parameters[key] = value;
            }
        }

        protected string? GetParameter(string key)
        {
            _parameters.TryGetValue(key, out var value);
            return value;
        }

        protected T? GetParameter<T>(string key) where T : struct
        {
            var value = GetParameter(key);

            if (value is null)
                return null;

            var type = typeof(T);

            if (type.IsEnum)
            {
                try
                {
#if NET
                    return Enum.Parse<T>(value, ignoreCase: true);
#else
                    return (T) Enum.Parse(typeof(T), value, ignoreCase: true);
#endif
                }
                catch (Exception ex) when (ex is not ArgumentException)
                {
                    throw new ArgumentException($"Value '{value}' not supported for option '{type.Name}'.", ex);
                }
            }

            try
            {
                return (T) Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid value '{value}' for '{key}' connection string option.", ex);
            }
        }

        /// <summary>
        /// The host name or network address of the Server to which to connect
        /// </summary>
        public virtual string? Host
        {
            set => SetParameter("host", value);
            get => GetParameter("host");
        }

        public virtual string GetConnectionString()
        {
            if (_parameters.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var option in _parameters)
            {
                sb.Append(FormatParameter(option.Key, option.Value));
            }

            return sb.ToString();
        }

        protected virtual string FormatParameter(string key, string? value)
            => string.IsNullOrEmpty(value) ? string.Empty : key + "=" + value + ";";
    }
}
