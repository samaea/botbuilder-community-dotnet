// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.Serialization;

namespace Bot.Builder.Community.Dialogs.FormFlow.Internals.Fibers
{
    public static partial class SetField
    {
        public static void NotNull<T>(out T field, string name, T value) where T : class
        {
            CheckNull(name, value);
            field = value;
        }

        public static void CheckNull<T>(string name, T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullFrom<T>(out T field, string name, SerializationInfo info) where T : class
        {
            var value = (T)info.GetValue(name, typeof(T));
            SetField.NotNull(out field, name, value);
        }

        public static void From<T>(out T field, string name, SerializationInfo info)
        {
            var value = (T)info.GetValue(name, typeof(T));
            field = value;
        }
    }
}
