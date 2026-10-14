# Programmer.Abstractions — SDK NyaProg

Сборка контрактов для плагинов NyaProg. Ссылайтесь **только на неё**, если пишете
свой модуль (программатор, генератор настроек, конвертер прошивок и т.п.).

Целевой фреймворк: `netstandard2.0`. Никаких зависимостей от UI/ОС.

---

## Документация

- **SDK (этот файл):**
  [IModule](#imodule--modulebase) ·
  [ITool/ToolBase/CodedToolBase](#itool--toolbase--codedtoolbase) ·
  [ICommand/CommandBase](#icommand--commandbase) ·
  [CommandDescriptor/CommandParameter](#commanddescriptor--commandparameter) ·
  [CommandResult](#commandresult--commandstatus) ·
  [Сервисы хоста](#сервисы-хоста--imodulehost) ·
  [Абстракции платформы](#абстракции-платформы-programmerplatform) ·
  [Соглашения](#соглашения)
- **[Формат `.etool`](../docs/etool.md):**
  [структура](../docs/etool.md#общая-структура) ·
  [опции](../docs/etool.md#опции-options--только-для-программаторов) ·
  [команды](../docs/etool.md#команды-actions) ·
  [детекторы](../docs/etool.md#детекторы-error--warning) ·
  [outputs](../docs/etool.md#outputs)
- **[Внешние команды и подстановки](../docs/external-commands.md):**
  [сборка аргументов](../docs/external-commands.md#сборка-аргументов) ·
  [подстановка {…}](../docs/external-commands.md#подстановка-имя) ·
  [алиасы](../docs/external-commands.md#правила-перезаписи-алиасы) ·
  [запуск процесса](../docs/external-commands.md#запуск-процесса) ·
  [external](../docs/external-commands.md#нативная-команда-external)
- **[Формат `.nyap` — прошивочный проект](../docs/nyap.md):**
  [структура проекта](../docs/nyap.md#структура-проекта) ·
  [цель (контроллер)](../docs/nyap.md#цель-target) ·
  [поле ввода](../docs/nyap.md#поле-ввода-input) ·
  [шаги](../docs/nyap.md#шаги-steps) ·
  [set](../docs/nyap.md#присвоение-set) ·
  [ветвление if/else](../docs/nyap.md#ветвление-if--else) ·
  [условия](../docs/nyap.md#условия-выражения)
- **[Сетевые команды download/lookup](../NyaProg.Modules.Net/README.md)**

---

## Слои

```
IModule            — точка входа плагина (находит хост)
  └ ITool          — контейнер команд + опции + архитектуры
      └ ICommand   — единица работы (одно действие)

IModuleHost        — сервисы, которые ядро даёт модулю (UI, процессы, сеть, файлы, реестр)
```

Все интерфейсы имеют необязательные базовые классы (`ModuleBase`, `ToolBase`,
`CodedToolBase`, `CommandBase`) — наследуйтесь для экономии кода, но это не
обязательно.

---

## Быстрый старт: минимальный плагин

```csharp
using System.Collections.Generic;
using Extension.Argument;
using Programmer.Modules;

public sealed class MyModule : ModuleBase
{
    public override string Id => "my.plugin";
    public override string Name => "My plugin";
    public override IEnumerable<ITool> GetTools() => new ITool[] { new MyTool() };
}

public sealed class MyTool : CodedToolBase
{
    public MyTool() : base("my.tool", "My tool", ToolKind.Utility)
    {
        AddCommand(new EchoCommand());
    }
}

sealed class EchoCommand : CommandBase
{
    public EchoCommand() : base("echo", "Эхо") { }

    public override CommandResult Execute(CommandContext Ctx)
    {
        var Outputs = new ArgumentList();
        Outputs.Set("echo", Ctx.Args.Get("value") ?? "");
        return new CommandResult(CommandStatus.Ok, "", Outputs);
    }
}
```

Соберите DLL и положите её в каталог `plugins/` рядом с приложением — хост
обнаружит модуль автоматически. Полный рабочий пример: `NyaProg.Plugins.Sample`.

---

## Контракты

### `IModule` / `ModuleBase`
Точка входа плагина. Хост находит реализации в загружаемых сборках (нужен
публичный тип с конструктором без параметров), вызывает `Initialize(IModuleHost)`
и регистрирует инструменты из `GetTools()`.

```csharp
public interface IModule
{
    string Id { get; }
    string Name { get; }
    Version Version { get; }
    IEnumerable<ITool> GetTools();
    void Initialize(IModuleHost Host);
}
```

### `ITool` / `ToolBase` / `CodedToolBase`
Инструмент: команды, опции, архитектуры, теги.

```csharp
public interface ITool
{
    string Id { get; }              // напр. "my.tool"
    string Name { get; }
    ToolKind Kind { get; }          // Programmer | Utility | Internal
    string[] SupportedArch { get; } // пусто — без ограничения
    string[] Tags { get; }
    Option[] GetOptions();
    ICommand[] GetCommands();
    bool SupportsArch(string[] Arch);
}
```

`ToolBase` — общая реализация; `CodedToolBase` — для инструментов на C#
(значение `Kind` по умолчанию `Internal`). Полезные методы базы:
`AddCommand`, `AddOption`, `AddArch`, `AddTag`.

### `ICommand` / `CommandBase`
Единица работы.

```csharp
public interface ICommand
{
    string Id { get; }                      // простое имя, как в .nyap: "wflash", "lookup"
    CommandDescriptor Descriptor { get; }
    CommandResult Execute(CommandContext Ctx);
}
```

`CommandBase(Id, Title)` сам заполняет `Descriptor`.

### `CommandDescriptor` / `CommandParameter`
Метаданные и **схема параметров**. Движок применяет `Default`, проверяет
`Required` и тип (`Type` — имя из `InputTypeLibrary`), подставляя значения перед
вызовом команды.

```csharp
Descriptor.Parameters = new List<CommandParameter>
{
    new CommandParameter("url", "str", Required: true, Default: null, Description: "URL"),
    new CommandParameter("method", null, false, "GET"),
};
```

### `CommandContext`
Контекст выполнения: готовые аргументы, рабочий каталог, сервисы хоста.

```csharp
public sealed class CommandContext
{
    public string CommandId;
    public ArgumentList Args;     // разрешённые {переменные}
    public string WorkingDir;     // {dir}
    public IModuleHost Host;
}
```

### `CommandResult` / `CommandStatus`
Результат: статус, сообщение и **выходные переменные**. Выходы вливаются в
окружение запуска скрипта — доступны последующим командам (как `{имя}`) и в
условиях шагов.

```csharp
return new CommandResult(CommandStatus.Ok, "", Outputs);
// или: CommandResult.Ok(), CommandResult.Warning("..."), CommandResult.Error("...")
```

---

## Сервисы хоста — `IModuleHost`

Доступны через `CommandContext.Host`:

| Член | Назначение |
|------|------------|
| `Commands` | реестр команд (`ICommandRegistry`) |
| `UI` | диалоги (`IUserInterface`) |
| `ProcessRunner` | запуск процессов (`IProcessRunner`) |
| `Http` | сетевые запросы (`IHttpFetcher`) |
| `Files` | файловые операции (`IFileSystem`) |
| `Root`, `Soft` | каталоги `{root}`, `{soft}` |
| `ResolveArgument(name, args)` | значение плейсхолдера с учётом алиасов |

Подстановка `{name}` в строках: `CommandTemplating.Process(template, name => Host.ResolveArgument(name, Args))`.

---

## Абстракции платформы (`Programmer.Platform`)

- `IUserInterface` — `Message(text, title)`, `Question(text, title)`; `NullUserInterface` для headless.
- `IProcessRunner` — `Run(program, args, dir) → ProcessResult { ExitCode, Output }`.
- `IHttpFetcher` — `GetText(url, method, headers)`, `GetBytes(...)`.
- `IFileSystem` — `Exists/ReadAllText/WriteAllText/ReadAllBytes/WriteAllBytes/Delete/Copy/Move`.

---

## Примитивы

- `Extension.Argument.ArgumentList` — `Get/Set/Keys/Append/Reset`.
- `Programmer.Options` — `Option` (abstract), `OptionText`, `OptionHidden`,
  `OptionList` (+ `OptionListItem`), `OptionInput`. Каждая опция умеет `GetDefault()`.
- `Programmer.Input.InputType` — `Type`, `CanIncrement`, `Check`, `Convert`, `Increment`.

---

## Соглашения

- **Id команды** — простое имя (`wflash`, `copy`, `lookup`); используется в `.nyap`.
- **Выходы → переменные** — то, что команда положила в `Outputs`, попадает в
  окружение запуска и подставляется в `{имя}` последующих команд/условий.
- **Параметры** объявляйте через `Descriptor.Parameters` — получите defaults,
  проверку обязательности/типа и документацию бесплатно.
- **Деплой** — DLL плагина (и его зависимости) в каталог `plugins/`
  (настраивается в `config.xml` секцией `<plugins>`).

---

## Совместимость

Контракты стабильны в рамках мажорной версии. При добавлении членов
предпочитаем новые интерфейсы/базовые классы, а не изменение существующих.
