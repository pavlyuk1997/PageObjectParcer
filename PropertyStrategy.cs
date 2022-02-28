using Atata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcerTry
{
    public static class PropertyStrategy
    {
        public static string GetElementPropertyInStringForm<Page, T>(Page page, T element)
            where T : Control<OrdinaryPage>
        {
            //var type = ReflectionHelper.GetFriendlyTypeName(typeof(T));
            var type = typeof(T).Name;
            var index = type.IndexOf('`');
            if (index > 0)
            {
                type = type.Substring(0, index);
            }
            var propertyName = TermResolver.ToString(element.Attributes.TextContent.Value, new TermOptions { Case = TermCase.Pascal}) ;

            return $"public {type}<_> {propertyName} {{get; private set;}}";
        }
    }
}
