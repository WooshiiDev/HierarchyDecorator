using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchyDecorator
{
    public enum FilterType { NONE, NAME, TYPE }

    public struct CategoryFilter
    {
        public string name;
        public string filter;
        public FilterType type;

        public CategoryFilter(string name, string filter, FilterType type)
        {
            this.name = name;
            this.filter = filter;
            this.type = type;
        }
    }

}
