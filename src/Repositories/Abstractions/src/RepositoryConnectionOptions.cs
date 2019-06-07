namespace ClickView.GoodStuff.Repositories.Abstractions
{
    using System.Collections.Generic;
    using System.Text;

    public abstract class RepositoryConnectionOptions
    {
        protected Dictionary<string, string> Parameters = new Dictionary<string, string>();

        protected void SetParameter(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Parameters.Remove(key);
            }
            else
            {
                Parameters[key] = value;
            }
        }

        public string GetParameter(string key)
        {
            Parameters.TryGetValue(key, out var value);
            return value;
        }

        public virtual string Host
        {
            set => SetParameter("host", value);
        }

        public virtual ushort? Port
        {
            set => SetParameter("port", value.ToString());
        }

        public virtual string GetConnectionString()
        {
            if (Parameters.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var option in Parameters)
            {
                sb.Append(FormatParameter(option.Key, option.Value));
            }

            return sb.ToString();
        }

        protected virtual string FormatParameter(string key, string value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : key + "=" + value + ";";
    }
}
