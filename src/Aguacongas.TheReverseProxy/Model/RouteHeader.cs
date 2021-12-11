﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.TheReverseProxy.Model
{
    /// <summary>
    /// Route criteria for a header that must be present on the incoming request.
    /// </summary>
    public class RouteHeader
    {
        /// <summary>
        /// Name of the header to look for. This field is case insensitive and required.
        /// </summary>
        public string? Name
        {
            get;
            set;
        }

        /// <summary>
        /// A collection of acceptable header values used during routing. Only one value
        /// must match. The list must not be empty unless using Yarp.ReverseProxy.Configuration.HeaderMatchMode.Exists.
        /// </summary>
        public IEnumerable<string>? Values
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies how header values should be compared (e.g. exact matches Vs. by prefix).
        /// Defaults to Yarp.ReverseProxy.Configuration.HeaderMatchMode.ExactHeader.
        /// </summary>
        public HeaderMatchMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies whether header value comparisons should ignore case. When true, System.StringComparison.Ordinal
        /// is used. When false, System.StringComparison.OrdinalIgnoreCase is used. Defaults
        /// to false.
        /// </summary>
        public bool IsCaseSensitive
        {
            get;
            set;
        }
    }
}
