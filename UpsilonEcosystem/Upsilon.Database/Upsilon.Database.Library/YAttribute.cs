using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class YDatasetAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class YFieldAttribute : Attribute { }
}
