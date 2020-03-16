using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 国家信息
    /// </summary>
    public class CountryInfo
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
    }
}
