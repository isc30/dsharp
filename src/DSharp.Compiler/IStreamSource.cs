// IStreamSource.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.IO;

namespace DSharp.Compiler
{
    public interface IStreamSource
    {
        string FullName { get; }

        string Name { get; }

        Stream GetStream();
    }
}
