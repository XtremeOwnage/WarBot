using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WarBot.Core
{
    public static class AttributeHelper
    {
        public static string GetEnumDescriptionAttribute<T>(this T Enum)
            where T : System.Enum
        {
            var AttributeType = typeof(DescriptionAttribute);
            var MemberInfo = typeof(T).GetMember(Enum.ToString());
            return MemberInfo[0].GetCustomAttributes(AttributeType, false)
                .OfType<DescriptionAttribute>()
                .Select(o => o?.Description ?? "")
                .FirstOrDefault();
        }

    }
}
