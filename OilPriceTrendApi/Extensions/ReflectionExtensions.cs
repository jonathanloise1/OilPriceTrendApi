using System.Reflection;
using System.Runtime.CompilerServices;

namespace OilPriceTrendApi.Extensions
{
    public static class ReflectionExtensions
    {
        public static string GetCurrentAsyncMethodName(this MethodBase source, [CallerMemberName] string name = "") => name;
    }
}