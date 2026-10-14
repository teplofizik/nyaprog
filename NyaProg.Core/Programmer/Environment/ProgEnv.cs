using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Tool;
using Programmer.Tool.Toolset;
using Programmer.Project;
using Programmer.Options.Rewrite;
using Programmer.Modules;
using Programmer.Platform;
using System.IO;
using Programmer.Input;

namespace Programmer.Environment
{
    public class ProgEnv : IModuleHost
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        public Config Conf = new Config();

        /// <summary>
        /// Список программаторов
        /// </summary>
        ToolSet ProgTS = new ProgToolset();

        /// <summary>
        /// Список утилит
        /// </summary>
        ToolSet CustTS = new CustomToolset();

        /// <summary>
        /// Список проектов
        /// </summary>
        ProjLibrary Lib = new ProjLibrary();

        /// <summary>
        /// Предпочтения в программаторах
        /// </summary>
        Preference Pref = new Preference();

        /// <summary>
        /// Библиотека типов пользовательского ввода
        /// </summary>
        InputTypeLibrary InTypes = new InputTypeLibrary();

        /// <summary>
        /// Перезапись параметров
        /// </summary>
        List<OptRewrite> OptionAlias = new List<OptRewrite>();

        /// <summary>
        /// Взаимодействие с пользователем (передаётся извне).
        /// </summary>
        public IUserInterface UI { get; }

        /// <summary>
        /// Реестр команд.
        /// </summary>
        CommandRegistry CmdRegistry = new CommandRegistry();

        /// <summary>
        /// Соответствие legacy-инструментов их модулям (для приоритета в реестре).
        /// </summary>
        Dictionary<UniTool, ITool> Modules = new Dictionary<UniTool, ITool>();

        /// <summary>
        /// Конструктор. UI опционален — по умолчанию headless-заглушка.
        /// </summary>
        public ProgEnv(IUserInterface UI = null)
        {
            this.UI = UI ?? new NullUserInterface();
        }

        /// **************************************************************
        /// Свойства
        /// **************************************************************

        /// <summary>
        /// Полный список загруженных проектов
        /// </summary>
        public Project.Project[] Projects => Lib.Projects.ToArray();

        /// <summary>
        /// Полный список загруженных программаторов
        /// </summary>
        public UniTool[] Tools => ProgTS.Tools.ToArray();
        
        /// <summary>
        /// Список правил перезаписи параметров
        /// </summary>
        public OptRewrite[] OptRewriteRules => OptionAlias.ToArray();

        /// <summary>
        /// Библиотека поддерживаемых типов пользовательского ввода
        /// </summary>
        public InputTypeLibrary InputTypes => InTypes;

        /// <summary>
        /// Реестр команд (IModuleHost).
        /// </summary>
        public ICommandRegistry Commands => CmdRegistry;

        /// <summary>
        /// Корневой каталог приложения ({root}) — каталог сборки.
        /// </summary>
        public string Root => Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location));

        /// <summary>
        /// Каталог Soft ({soft}).
        /// </summary>
        public string Soft => Path.Combine(Root, "Soft");

        /// <summary>
        /// Запуск внешних процессов (IModuleHost).
        /// </summary>
        public IProcessRunner ProcessRunner { get; } = new SystemProcessRunner();

        /// <summary>
        /// Сетевые запросы (IModuleHost).
        /// </summary>
        public IHttpFetcher Http { get; } = new HttpFetcher();

        /// <summary>
        /// Файловые операции (IModuleHost).
        /// </summary>
        public IFileSystem Files { get; } = new FileSystem();

        /// <summary>
        /// Разрешить значение плейсхолдера с учётом правил перезаписи (алиасов).
        /// </summary>
        public string ResolveArgument(string Name, ArgumentList Args)
        {
            var Value = Args.Get(Name);
            if (Value != null) return Value;

            foreach (var R in OptionAlias)
            {
                if (R.Check(Name))
                    return R.GetValue(Name, Args);
            }

            return null;
        }

        /// **************************************************************
        /// Методы
        /// **************************************************************

        /// <summary>
        /// Загрузить конфигурацию
        /// </summary>
        /// <param name="FileName"></param>
        protected void LoadConfig(string FileName) => Conf.Load(FileName);

        /// <summary>
        /// Выгрузить набор программаторов и утилит
        /// </summary>
        protected void UnloadToolset()
        {

        }

        /// <summary>
        /// Загрузить набор программаторов и утилит
        /// </summary>
        protected void LoadToolset()
        {
            UnloadToolset();

            ProgTS = new ProgToolset();
            CustTS = new CustomToolset();
            
            foreach(var P in Conf.ProgToolsets)  ProgTS.Load(P);
            foreach (var P in Conf.CustToolsets) CustTS.Load(P);
        }

        /// <summary>
        /// Построить реестр команд из загруженных инструментов.
        /// </summary>
        protected void BuildRegistry()
        {
            CmdRegistry = new CommandRegistry();
            Modules = new Dictionary<UniTool, ITool>();

            foreach (var T in ProgTS.Tools) RegisterLegacy(T, ToolKind.Programmer);
            foreach (var T in CustTS.Tools) RegisterLegacy(T, T.Kind);
        }

        /// <summary>
        /// Зарегистрировать модуль (плагин) и его инструменты.
        /// </summary>
        public void RegisterModule(IModule Module)
        {
            if (Module == null) return;

            Module.Initialize(this);
            foreach (var T in Module.GetTools())
                CmdRegistry.Register(T);
        }

        /// <summary>
        /// Зарегистрировать отдельный инструмент.
        /// </summary>
        public void RegisterTool(ITool Tool) => CmdRegistry.Register(Tool);

        private void RegisterLegacy(UniTool T, ToolKind Kind)
        {
            var M = new LegacyToolModule(T, Kind, OptRewriteRules);
            CmdRegistry.Register(M);
            Modules[T] = M;
        }

        /// <summary>
        /// Модуль (<see cref="ITool"/>) для legacy-инструмента (для приоритета в реестре).
        /// </summary>
        public ITool GetModule(UniTool T)
        {
            ITool M;
            return ((T != null) && Modules.TryGetValue(T, out M)) ? M : null;
        }

        /// <summary>
        /// Выгрузить набор программаторов и утилит
        /// </summary>
        protected void UnloadProjects()
        {

        }

        /// <summary>
        /// Загрузить список проектов
        /// </summary>
        protected void LoadProjects()
        {
            UnloadProjects();

            Lib = new ProjLibrary();
            foreach (var P in Conf.Projects) Lib.Load(P);
        }

        /// <summary>
        /// Добавить перезапись параметров
        /// </summary>
        protected void LoadOptRewrite()
        {
            OptionAlias = new List<OptRewrite>();
            // Обычно в командах конфигурации используется option вместо value
            OptionAlias.Add(new ORAlias("option", "value"));
            // Обычно в командах записи ПО используется filename вместо value
            OptionAlias.Add(new ORAlias("filename", "value"));
            // То же, но без расширения
            OptionAlias.Add(new ORAliasConv("filenamewe", "value", s => Path.GetFileNameWithoutExtension(s)));
            // Переписывание опции побайтово (для фьюзов, напимер)
            OptionAlias.Add(new OptHexExtractor("option", "value"));
        }
        
        /// <summary>
        /// Полная загрузка окружения
        /// </summary>
        public void Load(string ConfigFN)
        {
            Conf = new Config();
            if(ConfigFN != null) LoadConfig(ConfigFN);

            LoadToolset();
            LoadProjects();
            LoadOptRewrite();
            BuildRegistry();
            Pref.Load(Conf.Preferences);
        }

        /// <summary>
        /// Сохранение окружения
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {
            Pref.Save(Conf.Preferences);
        }

        /// <summary>
        /// Список программаторов по списку поддерживаемых архитектур
        /// </summary>
        /// <param name="Arch"></param>
        /// <returns></returns>
        public UniTool[] GetToolsByArch(string[] Arch)
        {
            var Tools = new List<UniTool>();

            foreach(var T in ProgTS.Tools)
            {
                if (T.IsSupportedArch(Arch))
                    Tools.Add(T);
            }

            return Tools.ToArray();
        }

        /// <summary>
        /// Получить список значений динамической опции у команды-поставщика.
        /// </summary>
        public IReadOnlyList<CommandItem> GetListItems(string Source, ArgumentList Args)
        {
            if (string.IsNullOrEmpty(Source)) return new CommandItem[0];

            ICommand Cmd;
            if (!CmdRegistry.TryResolve(Source, out Cmd)) return new CommandItem[0];

            try
            {
                var Ctx = new CommandContext(Source, Args ?? new ArgumentList(), Root, this);
                var R = Cmd.Execute(Ctx);
                return ((R != null) && (R.Items != null)) ? R.Items : new CommandItem[0];
            }
            catch (Exception E)
            {
                Log.WriteLine($"GetListItems('{Source}'): {E.Message}");
                return new CommandItem[0];
            }
        }

        /// <summary>
        /// Установить предпочтения
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Value"></param>
        public void SetPreference(string ID, string Value)
        {
            Pref.Set(ID, Value);
        }

        /// <summary>
        /// Прочитать предпочтения
        /// </summary>
        /// <param name="ID"></param>
        public string GetPreference(string ID)
        {
            return Pref.Get(ID);
        }
    }
}
