using Ardalis.SmartEnum;

namespace SasReader
{
    /**
     * Enumeration of all page types used in sas7bdat files.
     */
    public abstract class PageType : SmartEnum<PageType>
    {
        public static readonly PageType PAGE_TYPE_META = new PAGE_TYPE_META_IMPL();
        public static readonly PageType PAGE_TYPE_MIX = new PAGE_TYPE_MIX_IMPL();
        public static readonly PageType PAGE_TYPE_AMD = new PAGE_TYPE_AMD_IMPL();
        public static readonly PageType PAGE_TYPE_DATA = new PAGE_TYPE_DATA_IMPL();

        private PageType(string name, int value) : base(name, value) { }

        /**
         * The method to check if one of PageType enumeration contains the specified page type.
         *
         * @param value - the page type for check.
         * @return true if PageType enumeration contains the specified page type, false otherwise.
         */
        public abstract bool Contains(int value);
        /**
         * The page type storing metadata as a set of subheaders. It can also store compressed row data in subheaders.
         * The sas7bdat format has two values that correspond to the page type 'meta':
         * {@link SasFileConstants#PAGE_META_TYPE_1} and {@link SasFileConstants#PAGE_META_TYPE_2}.
         */
        private sealed class PAGE_TYPE_META_IMPL : PageType
        {
            public PAGE_TYPE_META_IMPL() : base(nameof(PAGE_TYPE_META), 0) { }

            public override bool Contains(int value) =>
                value == SasFileConstants.PAGE_META_TYPE_1 || value == SasFileConstants.PAGE_META_TYPE_2 || value == SasFileConstants.PAGE_CMETA_TYPE;
        }

        /**
         * The page type storing metadata as a set of subheaders and data as a number of table rows.
         * The sas7bdat format has two values that correspond to the page type 'mix':
         * {@link SasFileConstants#PAGE_MIX_TYPE_1} and {@link SasFileConstants#PAGE_MIX_TYPE_2}.
         */
        private sealed class PAGE_TYPE_MIX_IMPL : PageType
        {
            public PAGE_TYPE_MIX_IMPL() : base(nameof(PAGE_TYPE_MIX), 1) { }

            public override bool Contains(int value) => value == SasFileConstants.PAGE_MIX_TYPE_1 || value == SasFileConstants.PAGE_MIX_TYPE_2;
        }
        /**
         * The page type amd. The value that correspond to the page type 'amd' is {@link SasFileConstants#PAGE_AMD_TYPE}.
         */
        private sealed class PAGE_TYPE_AMD_IMPL : PageType
        {
            public PAGE_TYPE_AMD_IMPL() : base(nameof(PAGE_TYPE_AMD), 2) { }

            public override bool Contains(int value) => value == SasFileConstants.PAGE_AMD_TYPE;
        }
        /**
         * The page type storing only data as a number of table rows. The value that correspond to the page type 'data' is
         * {@link SasFileConstants#PAGE_DATA_TYPE}.
         */
        private sealed class PAGE_TYPE_DATA_IMPL : PageType
        {
            public PAGE_TYPE_DATA_IMPL() : base(nameof(PAGE_TYPE_DATA), 3) { }

            public override bool Contains(int value) => value == SasFileConstants.PAGE_DATA_TYPE ||  value == SasFileConstants.PAGE_DATA_TYPE_2;
        }
    }
}