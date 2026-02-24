namespace GuardianStock.Domain.Core.Primitives
{
    public abstract class Entity
    {

        public Guid Id { get; private set; }

        protected Entity(Guid id) : this()
        {
            Id = id;
        }
        protected Entity() { }

        public override bool Equals(object? obj)
        {

            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            Entity other = obj as Entity;

            if (this.Id == default || other!.Id == default)
                return false;
            else
                return this.Id == other.Id;
        }


        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null))
            {
                return (ReferenceEquals(b, null)) ? true : false;
            }
            return a.Equals(b);

        }

        public static bool operator !=(Entity a, Entity b) => !(a == b);


        public override int GetHashCode()
        {
            if (this.Id != default)
                return this.Id.GetHashCode() ^ 31;
            return base.GetHashCode();
        }



    }
}
