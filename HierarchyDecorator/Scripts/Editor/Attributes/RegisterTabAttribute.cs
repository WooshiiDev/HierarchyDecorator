using System;

namespace HierarchyDecorator
{
    public class RegisterTabAttribute : Attribute
    {
        public int priority = 0;

        public RegisterTabAttribute(int priority = 0)
        {
            this.priority = priority;
        }
    }
}
