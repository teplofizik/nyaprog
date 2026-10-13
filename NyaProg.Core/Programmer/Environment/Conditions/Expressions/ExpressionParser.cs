using System;
using System.Collections.Generic;
using System.Text;

namespace Programmer.Environment.Conditions.Expressions
{
    /// <summary>
    /// Рекурсивный парсер условий/выражений.
    /// Грамматика: or := and ('||' and)* ; and := not ('&amp;&amp;' not)* ;
    /// not := '!' not | cmp ; cmp := atom (('=='|'!='|'&lt;'|'&gt;'|'&lt;='|'&gt;=') atom)? ;
    /// atom := '(' or ')' | string | number | identifier.
    /// </summary>
    public static class ExpressionParser
    {
        public static Expr Parse(string Text)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return new LiteralExpr("");

            var P = new Parser(Text);
            var E = P.ParseOr();
            P.ExpectEnd();
            return E;
        }

        private enum TokenType { Op, Str, Num, Ident, End }

        private sealed class Token
        {
            public TokenType Type;
            public string Text;

            public Token(TokenType Type, string Text)
            {
                this.Type = Type;
                this.Text = Text;
            }
        }

        private sealed class Parser
        {
            private readonly List<Token> T = new List<Token>();
            private int I;

            public Parser(string S) { Lex(S); }

            private void Lex(string S)
            {
                var i = 0;

                while (i < S.Length)
                {
                    var C = S[i];

                    if (char.IsWhiteSpace(C)) { i++; continue; }

                    if ((C == '(') || (C == ')')) { T.Add(new Token(TokenType.Op, C.ToString())); i++; continue; }

                    if (C == '!')
                    {
                        if ((i + 1 < S.Length) && (S[i + 1] == '=')) { T.Add(new Token(TokenType.Op, "!=")); i += 2; }
                        else { T.Add(new Token(TokenType.Op, "!")); i++; }
                        continue;
                    }

                    if (C == '=')
                    {
                        if ((i + 1 < S.Length) && (S[i + 1] == '=')) { T.Add(new Token(TokenType.Op, "==")); i += 2; continue; }
                        throw new Exception("Unexpected '=' (use '==')");
                    }

                    if (C == '<')
                    {
                        if ((i + 1 < S.Length) && (S[i + 1] == '=')) { T.Add(new Token(TokenType.Op, "<=")); i += 2; }
                        else { T.Add(new Token(TokenType.Op, "<")); i++; }
                        continue;
                    }

                    if (C == '>')
                    {
                        if ((i + 1 < S.Length) && (S[i + 1] == '=')) { T.Add(new Token(TokenType.Op, ">=")); i += 2; }
                        else { T.Add(new Token(TokenType.Op, ">")); i++; }
                        continue;
                    }

                    if (C == '&')
                    {
                        if ((i + 1 < S.Length) && (S[i + 1] == '&')) { T.Add(new Token(TokenType.Op, "&&")); i += 2; continue; }
                        throw new Exception("Unexpected '&' (use '&&')");
                    }

                    if (C == '|')
                    {
                        if ((i + 1 < S.Length) && (S[i + 1] == '|')) { T.Add(new Token(TokenType.Op, "||")); i += 2; continue; }
                        throw new Exception("Unexpected '|' (use '||')");
                    }

                    if ((C == '\'') || (C == '"'))
                    {
                        var Q = C;
                        i++;
                        var Sb = new StringBuilder();
                        while ((i < S.Length) && (S[i] != Q)) { Sb.Append(S[i]); i++; }
                        if (i >= S.Length) throw new Exception("Unterminated string");
                        i++;
                        T.Add(new Token(TokenType.Str, Sb.ToString()));
                        continue;
                    }

                    if (char.IsDigit(C))
                    {
                        var St = i;
                        while ((i < S.Length) && (char.IsDigit(S[i]) || (S[i] == '.'))) i++;
                        T.Add(new Token(TokenType.Num, S.Substring(St, i - St)));
                        continue;
                    }

                    if (char.IsLetter(C) || (C == '_'))
                    {
                        var St = i;
                        while ((i < S.Length) && (char.IsLetterOrDigit(S[i]) || (S[i] == '_') || (S[i] == '.') || (S[i] == '-') || (S[i] == ':'))) i++;
                        T.Add(new Token(TokenType.Ident, S.Substring(St, i - St)));
                        continue;
                    }

                    throw new Exception($"Unexpected char '{C}'");
                }

                T.Add(new Token(TokenType.End, ""));
            }

            private Token Peek() => T[I];
            private Token Next() => T[I++];
            private bool IsOp(string Op) => (Peek().Type == TokenType.Op) && (Peek().Text == Op);

            public Expr ParseOr()
            {
                var L = ParseAnd();
                while (IsOp("||")) { Next(); L = new OrExpr(L, ParseAnd()); }
                return L;
            }

            private Expr ParseAnd()
            {
                var L = ParseNot();
                while (IsOp("&&")) { Next(); L = new AndExpr(L, ParseNot()); }
                return L;
            }

            private Expr ParseNot()
            {
                if (IsOp("!")) { Next(); return new NotExpr(ParseNot()); }
                return ParseCmp();
            }

            private Expr ParseCmp()
            {
                var L = ParseAtom();
                var Tok = Peek();

                if ((Tok.Type == TokenType.Op) &&
                    ((Tok.Text == "==") || (Tok.Text == "!=") || (Tok.Text == "<") ||
                     (Tok.Text == ">") || (Tok.Text == "<=") || (Tok.Text == ">=")))
                {
                    Next();
                    return new CmpExpr(L, Tok.Text, ParseAtom());
                }

                return L;
            }

            private Expr ParseAtom()
            {
                var Tok = Next();

                if ((Tok.Type == TokenType.Op) && (Tok.Text == "("))
                {
                    var E = ParseOr();
                    if (!IsOp(")")) throw new Exception("Expected ')'");
                    Next();
                    return E;
                }

                if (Tok.Type == TokenType.Str) return new LiteralExpr(Tok.Text);
                if (Tok.Type == TokenType.Num) return new LiteralExpr(Tok.Text);
                if (Tok.Type == TokenType.Ident) return new VarExpr(Tok.Text);

                throw new Exception($"Unexpected token '{Tok.Text}'");
            }

            public void ExpectEnd()
            {
                if (Peek().Type != TokenType.End)
                    throw new Exception($"Unexpected token '{Peek().Text}'");
            }
        }
    }
}
