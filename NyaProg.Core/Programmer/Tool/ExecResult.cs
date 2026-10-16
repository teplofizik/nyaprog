using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Tool
{
    public class ExecResult
    {
        public ExecResultType Result;

        public string Message;

        /// <summary>
        /// Выходные переменные команды (извлечённые значения и т.п.).
        /// </summary>
        public ArgumentList Outputs = new ArgumentList();

        public ExecResult(ExecResultType R, string Text)
        {
            Result = R;
            Message = Text;
        }
    }

    /// <summary>
    /// Успешно выполнено
    /// </summary>
    public class EROk : ExecResult
    {
        public EROk() : base(ExecResultType.Ok, "") { }

        public EROk(string Text) : base(ExecResultType.Ok, Text ?? "") { }
    }
    
    /// <summary>
    /// Выполнено с замечаниями
    /// </summary>
    public class ERWarning : ExecResult
    {
        public ERWarning(string Text) : base(ExecResultType.Warning, Text) { }
    }

    /// <summary>
    /// Не выполнено
    /// </summary>
    public class ERError : ExecResult
    {
        public ERError(string Text) : base(ExecResultType.Error, Text) { }
    }
    
    public enum ExecResultType
    {
        Ok,
        Warning,
        Error
    }
}
