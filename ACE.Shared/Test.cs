using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Shared;
public class Test<T>(T Foo)
{
    public T Foo { get; set; } = Foo;
}
