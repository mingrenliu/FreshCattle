using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace InheritAnalyzer.TransformInfo
{
    public class FileContent:IEquatable<FileContent>
    {
        public string FileName { get; set; }
        public SyntaxTree DocumentTree { get; set; }
        public static bool operator==(FileContent left, FileContent right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(FileContent left, FileContent right)
        {
            return !left.Equals(right);
        }
        public bool Equals(FileContent other)
        {

            throw new NotImplementedException();
        }
    }
}
