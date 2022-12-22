using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Extensions
    {
        public static string CuentaUnificadaFormat(this string value)
        {
            var codigoPostal = value.Substring(0, 4);
            var subcodigo = value.Substring(4, 1);
            var anioAlta = value.Substring(5, 2);
            var mesAlta = value.Substring(7, 2);
            var nroCuenta = value.Substring(9, 7);
            var digito = value.Substring(16, 1);
            return $"{codigoPostal}/{subcodigo}-{anioAlta}-{mesAlta}-{nroCuenta}/{digito}";
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }

            return null; // could also return string.Empty
        }

        public static bool IsMatchRegexFactura(this string value)
        {
            var regex = new Regex("^[0-9]{5}[-][0-9]{8}[/][0-9]$");
            return regex.IsMatch(value);
        }

        public static bool IsMatchRegexAvisoDeuda(this string value)
        {
            var regex = new Regex("^[0-9]{5}[-][0-9]{8}[/][0-9]$");
            return regex.IsMatch(value);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortColumn, bool descending)
        {
            // Dynamically creates a call like this: query.OrderBy(p =&gt; p.SortColumn)
            var parameter = Expression.Parameter(typeof(T), "p");

            string command = "OrderBy";

            if (descending)
            {
                command = "OrderByDescending";
            }

            Expression resultExpression = null;

            var property = typeof(T).GetProperty(sortColumn);
            // this is the part p.SortColumn
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            // this is the part p =&gt; p.SortColumn
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType },
                query.Expression, Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(resultExpression);
        }

    }
}
