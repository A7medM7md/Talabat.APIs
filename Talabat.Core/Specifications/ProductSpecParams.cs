/*
 ✅ لو عايز تعرّف القيمة دي بشكل مش ثابت (لكن برضو مش هتتغير وقت الرن):
==> private static readonly int MAX_PAGE_SIZE = 100;
static readonly بتتحدد وقت تشغيل البرنامج، بس متتغيرش بعد كده.
مفيدة لو القيمة هتيجي من config مثلاً.
*/
namespace Talabat.Core.Specifications
{
    public class ProductSpecParams
    {
        private const int MAX_PAGE_SIZE = 10; // Consumer Cannot Exceed The Max Size


        private int pageSize = 5;
        public int PageSize // Full Property
        {
            get => pageSize;
            set => pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }
        public int PageIndex { get; set; } = 1;
        public string? Sort { get; set; }
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }

		private string? search;
		public string? Search
		{
			get => search;
			set => search = value?.ToLower() ?? string.Empty;
		}

	}
}
