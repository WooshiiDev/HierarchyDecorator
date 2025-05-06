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
        public readonly string Name;
        public readonly string Filter;
        public readonly FilterType FilterType;

        public CategoryFilter(string name, string filter, FilterType filterType)
        {
            this.Name = name;
            this.Filter = filter;
            this.FilterType = filterType;
        }
        
        public bool IsValidFilter(Type type)
        {
            switch (FilterType)
            {
                case FilterType.NONE:
                    return false;

                case FilterType.NAME:
                    return type.FullName.Contains (Filter);

                case FilterType.TYPE:
                    Type baseType = Type.GetType (Filter);
                    if (baseType == null)
                        return false;
                    return type.IsAssignableFrom (baseType) || type.IsSubclassOf (baseType);
            }

            return false;
        }
    }
}
