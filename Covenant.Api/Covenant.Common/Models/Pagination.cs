using Newtonsoft.Json;

namespace Covenant.Common.Models
{
    public class Pagination
    {
        private int _pageSize = 50;
        private int _pageIndex = 1;
        public static readonly Pagination Default = new Pagination();

        public virtual bool IsDescending { get; set; }

        [JsonProperty]
        public int PageIndex
        {
            get => _pageIndex <= 0 ? 1 : _pageIndex;
            set => _pageIndex = value;
        }

        public int PageSize
        {
            get
            {
                if (_pageSize > 100) return 50;
                return _pageSize <= 0 ? 50 : _pageSize;
            }
            set => _pageSize = value;
        }
    }
}
