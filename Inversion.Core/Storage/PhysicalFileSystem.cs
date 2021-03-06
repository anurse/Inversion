﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Inversion.Utils;

namespace Inversion.Storage
{
    [ExcludeFromCodeCoverage]
    public class PhysicalFileSystem : IFileSystem
    {
        public string Root { get; private set; }

        public PhysicalFileSystem(string root)
        {
            if (String.IsNullOrEmpty(root)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, CommonResources.Argument_Cannot_Be_Null_Or_Empty, "root"), "root"); }
            Root = root;
        }

        public Stream Open(string relativePath, FileAccess access, bool create)
        {
            if (String.IsNullOrEmpty(relativePath)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, CommonResources.Argument_Cannot_Be_Null_Or_Empty, "relativePath"), "relativePath"); }
            if (access < FileAccess.Read || access > FileAccess.ReadWrite) { throw new ArgumentOutOfRangeException("access"); }
            return new FileStream(GetFullPath(relativePath), create ? FileMode.OpenOrCreate : FileMode.Open, access);
        }

        public string GetFullPath(string relativePath)
        {
            if (String.IsNullOrEmpty(relativePath)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, CommonResources.Argument_Cannot_Be_Null_Or_Empty, "relativePath"), "relativePath"); }
            
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }
            return Path.Combine(Root, relativePath);
        }

        public bool Exists(string relativePath)
        {
            if (String.IsNullOrEmpty(relativePath)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, CommonResources.Argument_Cannot_Be_Null_Or_Empty, "relativePath"), "relativePath"); }
            return File.Exists(GetFullPath(relativePath));
        }

        public string[] ResolveWildcard(string relativeWildCardPath)
        {
            try
            {
                return Directory.GetFiles(Root, relativeWildCardPath, SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                return new string[0];
            }
            catch (FileNotFoundException)
            {
                return new string[0];
            }
        }
    }
}
