using System;

namespace ScrapBot.Extensions
{
    public static partial class Extensions
    {
        public static T GetService<T>(this IServiceProvider provider) => (T) provider.GetService(typeof(T));
    }
}