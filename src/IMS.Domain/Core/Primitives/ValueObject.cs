using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Domain.Core.Primitives
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {



        public abstract IEnumerable<object> GetAtomicValues();



        public bool Equals(ValueObject? other)
        {
            if (other is null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return this.GetAtomicValues().SequenceEqual(other.GetAtomicValues());
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (this.GetType() != obj.GetType())
                return false;

            if (obj is not ValueObject other)
                return false;

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            var combinedHashcode = 0;
            foreach (var value in GetAtomicValues())
            {
                combinedHashcode = HashCode.Combine(combinedHashcode, value);
            }

            return combinedHashcode;
        }

        public static bool operator ==(ValueObject? v1, ValueObject? v2)
        {
            if (v1 is null && v2 is null)
                return true;
            if (v1 is null || v2 is null)
                return false;

            return v1.Equals(v2);


        }


        public static bool operator !=(ValueObject? v1, ValueObject? v2) => !(v1 == v2);






    }
}
