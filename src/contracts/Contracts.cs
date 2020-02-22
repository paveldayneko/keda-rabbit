namespace Keda.Contracts
{
    using System;
    using System.Collections.Generic;

    public class Person
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public Guid Id { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public Guid Id { get; set; }
    }

    public class CompositeItem
    {
        public IEnumerable<ChildA> A { get; set; }
        public IEnumerable<ChildB> B { get; set; }
    }

    public class ChildA
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ChildB
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
    }
}