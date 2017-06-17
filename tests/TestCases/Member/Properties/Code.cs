using System;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("test")]

namespace MemberTests {

    public interface ITest
    {
        int XYZ { get; set; }
        bool IsFoo { get; }
    }

    public class Test {

        public static int StaticProp {
            get { return 2006; }
            set { }
        }

        public int XYZ {
            get { return 0; }
            set { }
        }

        public bool IsFoo {
            get { return true; }
        }

        protected string Name { get { return "nk"; } }

        private bool IsInitialized { get { return false; } }

        public Test() {
            XYZ = 1;
            this.XYZ = this.Name.Length;

            ITest inter = (ITest)this;

            inter.XYZ = Name.Length;

            inter.XYZ++;
            --this.XYZ;

            int v = StaticProp;
            v = Test.StaticProp;
        }
    }

    public class Test2 : Test {

        public Test2() {
            int n = base.XYZ;
            base.XYZ = 6;
            if (n == XYZ) {
            }
            if (XYZ == n) {
            }

            n = Test.StaticProp;
        }
    }
}
