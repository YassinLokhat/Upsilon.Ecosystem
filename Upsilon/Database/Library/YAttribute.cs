using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    /// <summary>
    /// Tag a <c><see cref="YDataSet{T}"/></c> as a dataset.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class YDatasetAttribute : Attribute { }

    /// <summary>
    /// Tag a property as a table field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class YFieldAttribute : Attribute { }
}
