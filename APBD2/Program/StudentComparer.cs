using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    internal class StudentComparer : IEqualityComparer<Student>
    {
        public bool Equals(Student? x, Student? y)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals($"{x.Name} {x.Surname} {x.Index}", $"{y.Name} {y.Surname} {y.Index}");
        }

        public int GetHashCode([DisallowNull] Student obj)
        {
            return obj.Index.GetHashCode();
        }
    }
}
