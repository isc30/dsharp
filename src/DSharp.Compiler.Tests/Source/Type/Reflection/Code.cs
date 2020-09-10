using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("test")]

namespace TypeTests {

    public class MyClass
    {
        public MyClass<int> Other { get; set; }

        public object this[int index]
        {
            get { return index; }
            set { }
        }

        public int Getter { get; set; }

        public void Method()
        {
        }

        public T Method2<T>() { throw new Exception();  }

        public IEnumerable Method3() { throw new Exception();  }

        public IList<int> Method4() { throw new Exception();  }

        public MyClass() {
            MemberInfo[] members = typeof(MyClass).GetMembers();
            Assert(members[0].Name == "get_Item");
            Assert(members[1].Name == "set_Item");
            Assert(members[2].Name == "Getter");
            Assert(((PropertyInfo)members[2]).PropertyType == typeof(int));
            Assert(members[3].Name == "Method");
            Assert(((MethodInfo)members[3]).ReturnType == null);
        }

        private static void Assert(bool assertion)
        {

        }
    }

    public class MyClass<T>
    {
        public MyClass Other { get; set; }
    }
}
