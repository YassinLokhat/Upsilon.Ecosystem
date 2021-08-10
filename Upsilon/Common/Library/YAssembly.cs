﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// All types of binaries.
    /// </summary>
    public enum YBinaryType
    {
        /// <summary>
        /// The assembly is a dll class library.
        /// </summary>
        ClassLibrary = 0,

        /// <summary>
        /// The assembly is a window application.
        /// </summary>
        WindowApplication = 1,

        /// <summary>
        /// The assembly is a console application.
        /// </summary>
        ConsoleApplication = 2,
    }

    /// <summary>
    /// This class represents an assembly.
    /// </summary>
    public sealed class YAssembly
    {
        /// <summary>
        /// The name of the assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the assembly.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The description of the assembly.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of binary of the assembly.
        /// </summary>
        public YBinaryType BinaryType { get; set; }

        /// <summary>
        /// The url to the assembly.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Url { get; set; }

        /// <summary>
        /// Get or Set if the assembly is depreciated.
        /// </summary>
        public bool Depreciated { get; set; }

        /// <summary>
        /// The assembly's dependencies.
        /// <seealso cref="YDependency"/>
        /// </summary>
        public YDependency[] Dependencies { get; set; }

        /// <summary>
        /// Get the assembly's version as a <c><see cref="YVersion"/></c>.
        /// </summary>
        [JsonIgnore]
        public YVersion YVersion { get { return new(this.Version); } }
    }

    /// <summary>
    /// This class represents an assembly used by another assembly.
    /// </summary>
    public sealed class YDependency
    {
        /// <summary>
        /// The name of the assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimal version needed by the using assembly.
        /// </summary>
        public string MinimalVersion { get; set; }

        /// <summary>
        /// The maximal version needed by the using assembly.
        /// </summary>
        public string MaximalVersion { get; set; }

        /// <summary>
        /// Get the minimal version needed by the using assembly as a <c><see cref="YVersion"/></c>.
        /// </summary>
        [JsonIgnore]
        public YVersion YMinimalVersion { get { return new(this.MinimalVersion); } }

        /// <summary>
        /// Get the maximal version needed by the using assembly as a <c><see cref="YVersion"/></c>.
        /// </summary>
        [JsonIgnore]
        public YVersion YMaximalVersion { get { return new(this.MaximalVersion); } }
    }
}
