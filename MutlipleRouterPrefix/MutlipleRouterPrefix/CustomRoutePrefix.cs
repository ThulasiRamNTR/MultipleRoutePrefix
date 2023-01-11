///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Hubstream
//
// @Author: Thulasi
//
// Purpose: Custom RoutePrefix
//  Our DataPlatform RoutePrefix helps us to do MultipleRoutePrefix handling in the controller
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Web.Http;

namespace Hubstream.Data.Platform.Service
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class CustomRoutePrefix: RoutePrefixAttribute
    {
        #region Public properties
        /// <summary>
        /// Order of the prefix
        /// </summary>
        public Int32 Order { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">Prefix</param>
        public CustomRoutePrefix(String prefix) : this(prefix, 0)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="order">Order</param>
        public CustomRoutePrefix(String prefix, Int32 order) : base(prefix)
        {
            this.Order = order;
        }
        #endregion
    }
}
