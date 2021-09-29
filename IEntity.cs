using System;
using System.Collections.Generic;

namespace Code.Sample
{
    public interface IEntity : IDisposable
    {
        Guid Id { get; set; }
        bool Changed { get; set; }
    }
}