using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Options.Rewrite;

namespace Programmer.Tool.InternalTools
{
    /// <summary>
    /// Копирование файлов
    /// </summary>
    class TlFileTools : TlBase
    {
        public TlFileTools() : base("FileTools", new string[] { "copy", "copyto", "delete" }) { }

        protected override ExecResult Exec(string Command, string Dir, string Option, ArgumentList Args, OptRewrite[] Rewrite)
        {
            switch(Command)
            {
                case "copy": return Copy(Dir, Option);
                case "copyto": return CopyTo(Dir, Option);
                case "delete": return Delete(Dir, Option);
                default: return base.Exec(Command, Dir, Option, Args, Rewrite);
            }
        }


        private bool CompareFile(string A, string B)
        {
            if (!File.Exists(A) || !File.Exists(B)) return false;

            // Проверим-ка их даты изготовления
            var AI = new FileInfo(A);
            var BI = new FileInfo(B);

            if (AI.Length != BI.Length) return false;
            if (AI.LastWriteTimeUtc.CompareTo(BI.LastWriteTimeUtc) != 0) return false;

            return true;
        }

        protected ExecResult Delete(string Dir, string Option)
        {
            if (File.Exists(Option))
            {
                try
                {
                    File.Delete(Option);
                }
                catch (Exception E)
                {
                    return new ERError($"Could not delete file: {Option} ({E.Message})");
                }
            }
            return new EROk();
        }

        protected ExecResult CopyTo(string Dir, string Option)
        {
            if (File.Exists(Option))
            {
                string NewFn = Dir + Path.GetFileName(Option);

                if (CompareFile(NewFn, Option))
                    return new ERWarning("Files are equal");
                else
                {
                    File.Delete(Option);
                    File.Copy(NewFn, Option);
                    return new EROk();
                }
            }
            else
                return new ERError("No source file");
        }

        protected ExecResult Copy(string Dir, string Option)
        {
            if (File.Exists(Option))
            {
                string NewFn = Dir + Path.GetFileName(Option);

                if (CompareFile(NewFn, Option))
                    return new ERWarning("Files are equal");
                else
                {
                    File.Delete(NewFn);
                    File.Copy(Option, NewFn);
                    return new EROk();
                }
            }
            else
                return new ERError("No source file");
        }
    }
}
