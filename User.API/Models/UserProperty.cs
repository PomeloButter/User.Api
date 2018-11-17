namespace User.API.Models
{
    public class UserProperty
    {
        private int? _requestdHashCode;
        public string Key { get; set; }
        public int AppUserId { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestdHashCode.HasValue)

                    _requestdHashCode = (Key + Value).GetHashCode() ^ 31;
                return _requestdHashCode.Value;
            }
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UserProperty)) return false;

            if (ReferenceEquals(this, obj)) return true;
            var item = (UserProperty) obj;
            if (item.IsTransient() || IsTransient())
                return false;
            return item.Key == Key && item.Value == Value;
        }

        public bool IsTransient()
        {
            return string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value);
        }
    }
}