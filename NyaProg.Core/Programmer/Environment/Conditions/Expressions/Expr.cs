using System;
using System.Globalization;

using Extension.Argument;

namespace Programmer.Environment.Conditions.Expressions
{
    /// <summary>
    /// Узел выражения. Значение — строка (как в окружении), логика — через Test.
    /// </summary>
    public abstract class Expr
    {
        public abstract string Eval(ArgumentList Args);

        /// <summary>
        /// Значение операнда сравнения: неизвестный идентификатор трактуется как
        /// литерал (совместимость с прежним форматом <c>opt==value</c>).
        /// </summary>
        public virtual string EvalCmp(ArgumentList Args) => Eval(Args);

        public virtual bool Test(ArgumentList Args) => Truthy(Eval(Args));

        public static bool Truthy(string Value)
        {
            if (string.IsNullOrEmpty(Value)) return false;
            if (Value == "0") return false;
            if (string.Equals(Value, "false", StringComparison.OrdinalIgnoreCase)) return false;

            return true;
        }
    }

    /// <summary>Строковый/числовой литерал.</summary>
    public sealed class LiteralExpr : Expr
    {
        private readonly string Value;

        public LiteralExpr(string Value) { this.Value = Value; }

        public override string Eval(ArgumentList Args) => Value;
    }

    /// <summary>Ссылка на переменную окружения.</summary>
    public sealed class VarExpr : Expr
    {
        private readonly string Name;

        public VarExpr(string Name) { this.Name = Name; }

        public override string Eval(ArgumentList Args) => (Args != null) ? Args.Get(Name) : null;

        public override string EvalCmp(ArgumentList Args)
        {
            var V = Eval(Args);
            return (V != null) ? V : Name;
        }
    }

    /// <summary>Логическое НЕ.</summary>
    public sealed class NotExpr : Expr
    {
        private readonly Expr Child;

        public NotExpr(Expr Child) { this.Child = Child; }

        public override bool Test(ArgumentList Args) => !Child.Test(Args);

        public override string Eval(ArgumentList Args) => Child.Test(Args) ? "" : "1";
    }

    /// <summary>Логическое И.</summary>
    public sealed class AndExpr : Expr
    {
        private readonly Expr Left, Right;

        public AndExpr(Expr Left, Expr Right) { this.Left = Left; this.Right = Right; }

        public override bool Test(ArgumentList Args) => Left.Test(Args) && Right.Test(Args);

        public override string Eval(ArgumentList Args) => Test(Args) ? "1" : "";
    }

    /// <summary>Логическое ИЛИ.</summary>
    public sealed class OrExpr : Expr
    {
        private readonly Expr Left, Right;

        public OrExpr(Expr Left, Expr Right) { this.Left = Left; this.Right = Right; }

        public override bool Test(ArgumentList Args) => Left.Test(Args) || Right.Test(Args);

        public override string Eval(ArgumentList Args) => Test(Args) ? "1" : "";
    }

    /// <summary>Сравнение.</summary>
    public sealed class CmpExpr : Expr
    {
        private readonly Expr Left, Right;
        private readonly string Op;

        public CmpExpr(Expr Left, string Op, Expr Right)
        {
            this.Left = Left;
            this.Op = Op;
            this.Right = Right;
        }

        public override string Eval(ArgumentList Args) => Compare(Args) ? "1" : "";

        public override bool Test(ArgumentList Args) => Compare(Args);

        private bool Compare(ArgumentList Args)
        {
            var A = Left.EvalCmp(Args) ?? "";
            var B = Right.EvalCmp(Args) ?? "";

            double Da, Db;
            var Na = double.TryParse(A, NumberStyles.Any, CultureInfo.InvariantCulture, out Da);
            var Nb = double.TryParse(B, NumberStyles.Any, CultureInfo.InvariantCulture, out Db);
            var Num = Na && Nb;

            switch (Op)
            {
                case "==": return Num ? (Da == Db) : string.Equals(A, B, StringComparison.Ordinal);
                case "!=": return Num ? (Da != Db) : !string.Equals(A, B, StringComparison.Ordinal);
                case "<":  return Num ? (Da <  Db) : (string.CompareOrdinal(A, B) <  0);
                case ">":  return Num ? (Da >  Db) : (string.CompareOrdinal(A, B) >  0);
                case "<=": return Num ? (Da <= Db) : (string.CompareOrdinal(A, B) <= 0);
                case ">=": return Num ? (Da >= Db) : (string.CompareOrdinal(A, B) >= 0);
            }

            return false;
        }
    }
}
