using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Options.Rewrite;

namespace Programmer.Tool
{
    /// <summary>
    /// Класс, описывающий  универсальный программатор
    /// </summary>
    public abstract class UniTool
    {
        /// <summary>
        /// Список тегов
        /// </summary>
        protected List<string> Tags = new List<string>();

        /// <summary>
        /// Список архитектур (для не программаторов -- пусто)
        /// </summary>
        protected List<string> Arch = new List<string>();

        /// <summary>
        /// Название программатора
        /// </summary>
        public string Name;

        /// <summary>
        /// Список тегов
        /// </summary>
        /// <returns></returns>
        public string[] GetTags() => Tags.ToArray();

        /// <summary>
        /// Список поддерживаемых архитектур
        /// </summary>
        /// <returns></returns>
        public string[] GetSupportedArch() => Arch.ToArray();

        /// <summary>
        /// Опции
        /// </summary>
        public abstract Options.Option[] GetOptions();

        /// <summary>
        /// Поддерживается ли какая-то из архитектур программатором
        /// </summary>
        /// <param name="Arch"></param>
        /// <returns></returns>
        public bool IsSupportedArch(string[] Arch)
        {
            foreach(var A in this.Arch)
            {
                if (Arch.Contains(A))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Выполнить команду (с набором аргументов)
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public abstract ExecResult Exec(string Command, ArgumentList Args, OptRewrite[] Rewrite);

        /// <summary>
        /// Поддерживается ли команда этим набором
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public abstract bool CmdAvail(string Command);

        /// <summary>
        /// Доступен ли программатор
        /// </summary>
        /// <returns></returns>
        public abstract bool Avail();

        /// <summary>
        /// Можно ли использовать этот набор команд
        /// </summary>
        /// <returns></returns>
        public abstract bool Valid();

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="Name"></param>
        public UniTool(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Предупреждение
        /// </summary>
        /// <param name="Text"></param>
        protected void Warning(string Text)
        {

        }

        /// <summary>
        /// Произошла ошибка
        /// </summary>
        /// <param name="Text"></param>
        protected void Error(string Text)
        {

        }

        /// <summary>
        /// Получить значение аргумента по его названию 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="Name"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        protected string GetArgument(Argument[] Args, string Name, string Default)
        {
            foreach(var A in Args)
            {
                if (A.Name.CompareTo(Name) == 0)
                    return A.Value;
            }

            return Default;
        }

        public ArgumentList GetDefaultOptionsValues()
        {
            var A = new ArgumentList();
            foreach (var O in GetOptions())
                A.Append(O.GetDefault());
            return A;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
