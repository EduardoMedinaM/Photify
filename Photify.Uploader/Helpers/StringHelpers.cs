using Microsoft.Extensions.Primitives;
using System;

namespace Photify.Uploader.Helpers
{
    public static class StringHelpers
    {
        public static bool IsEqualsTo(this StringValues a, string b) => string.Equals(a.ToString(), b, StringComparison.InvariantCultureIgnoreCase);
    }
}
