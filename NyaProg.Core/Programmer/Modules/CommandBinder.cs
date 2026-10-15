using Extension.Argument;
using Programmer.Input;

namespace Programmer.Modules
{
    /// <summary>
    /// Применение схемы параметров команды: значения по умолчанию,
    /// обязательность и проверка типа (через InputTypeLibrary).
    /// </summary>
    static class CommandBinder
    {
        public static bool Bind(ICommand Cmd, ArgumentList Args, InputTypeLibrary Types, out string Error)
        {
            Error = null;

            var D = Cmd.Descriptor;
            if ((D == null) || (D.Parameters == null))
                return true;

            foreach (var P in D.Parameters)
            {
                var Value = Args.Get(P.Name);

                if ((Value == null) && (P.Default != null))
                {
                    Args.Set(P.Name, P.Default);
                    Value = P.Default;
                }

                if (P.Required && (Value == null))
                {
                    Error = $"Missing required parameter '{P.Name}' for command '{Cmd.Id}'";
                    return false;
                }

                if ((Value != null) && !string.IsNullOrEmpty(P.Type) && (Types != null))
                {
                    var T = Types.GetInputType(P.Type);
                    if ((T != null) && !T.Check(Value))
                    {
                        Error = $"Invalid value for parameter '{P.Name}' (type {P.Type}): {Value}";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
