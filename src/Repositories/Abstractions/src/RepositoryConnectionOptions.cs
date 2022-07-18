namespace ClickView.GoodStuff.Repositories.Abstractions
{
    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// The host name or network address of the Server to which to connect
        /// </summary>
        public virtual string? Host
        {
            set => SetParameter("host", value);
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

        protected virtual string FormatParameter(string key, string value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : key + "=" + value + ";";
    }
}
