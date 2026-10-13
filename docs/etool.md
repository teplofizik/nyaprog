# Формат `.etool` — декларативные инструменты

`.etool` — XML-описание внешней утилиты (CLI) или программатора: путь к
исполняемому файлу, поддерживаемые архитектуры, опции и набор команд с
шаблонами аргументов и правилами разбора вывода.

## Где лежат

| Каталог | Загрузчик | Роль |
|---------|-----------|------|
| `Tools/` | `ProgToolset` (`Custom=false`) | **Программаторы** (есть архитектуры и опции) |
| `Tools/Custom/` | `CustomToolset` (`Custom=true`) | **Утилиты** (конвертеры, хелперы) |

Пути задаются в `config.xml` секциями `<toolkits>` и `<custom>`.

## Общая структура

```xml
<tool>
  <name value="ST-Link STM32" />          <!-- название -->
  <path value=".\Soft\ST-LINK_CLI.exe" /> <!-- путь; поддерживает {root}, {soft} -->
  <interpreter value="python" />          <!-- необязательно: интерпретатор -->

  <supported>                             <!-- только программаторы -->
    <type value="stm32" />
  </supported>

  <tags>                                  <!-- необязательно -->
    <tag value="stlink" />
  </tags>

  <options> ... </options>                <!-- только программаторы -->

  <actions> ... </actions>
</tool>
```

`{root}` — каталог приложения, `{soft}` — каталог `Soft` рядом с ним.

> Программаторы (без `interpreter`) должны существовать по `path`, иначе
> инструмент не загрузится (`Valid()` проверяет `File.Exists(path)`).

## Опции (`<options>`) — только для программаторов

Показываются пользователю; их значения подставляются как `{имя}`.

```xml
<options>
  <!-- Список (выпадающий): вариант задаёт набор значений -->
  <list label="Interface" id="st-link.stm32.interface">
    <option label="SWD" default="">        <!-- default="" помечает вариант по умолчанию -->
      <value name="interface" val="SWD" />
    </option>
    <option label="JTAG">
      <value name="interface" val="JTAG" />
    </option>
  </list>

  <!-- Текст: пользователь вводит строку -->
  <text label="Number" name="number" default="0" autoincrement="1" id="proj.number" />

  <!-- Скрытое: значение по умолчанию без UI -->
  <hidden label="Programmer name" name="toolname" val="jtagice3" />
</options>
```

- `id` — ключ для сохранения выбранного значения (предпочтения).
- `autoincrement="1"` — поле можно инкрементировать (`text`).
- `default` у `<option>` (любое значение) помечает вариант по умолчанию.

## Команды (`<actions>`)

Каждый дочерний элемент — **одна команда**; имя элемента = имя команды
(используется в `.nyap`, напр. `wflash`, `erase`, `option`).

```xml
<actions>
  <wflash>
    <command value='-c {interface} UR -Q -P "{filename}" -V -Rst' />
    <toolpath value=".\Soft\other.exe" />   <!-- необязательно: свой путь для команды -->
    <params>                                <!-- необязательно: значения по умолчанию -->
      <version default="0x0000" />
    </params>
    <error>                                 <!-- детекторы ошибки в выводе -->
      <string value="No target connected" />
      <regex value="error\s+\d+" />
      <exitcode />
    </error>
    <warning> ... </warning>                <!-- детекторы предупреждений (те же виды) -->
    <outputs>                               <!-- извлечение значений в переменные -->
      <regex name="deviceid" value="Device ID:\s*([0-9A-F]+)" group="1" />
    </outputs>
  </wflash>
</actions>
```

### `command`
Шаблон аргументов (не путь!). Путь берётся из `<path>` или `<toolpath>`.
`{имя}` подставляются из аргументов/опций (см. `external-commands.md`).

### `params`
Значения по умолчанию для плейсхолдеров. Если в аргументах нет значения —
подставится `default`. Проверка обязательности/типов выполняется движком
(схема параметров).

### Детекторы `error` / `warning`
Проверяются по порядку; первое совпадение — результат (сообщение = найденный текст).

| Элемент | Условие |
|---------|---------|
| `<string value="X" />` | строка вывода содержит `X` |
| `<regex value="P" />` | вывод соответствует регулярке `P` (без учёта регистра) |
| `<exitcode />` | код возврата процесса ≠ 0 |

Порядок обработки: сначала `error` (→ ошибка), затем `warning` (→ предупреждение),
иначе успех.

### `outputs`
Извлечение значений из вывода в **выходные переменные** команды (доступны
последующим шагам как `{имя}` и в условиях):

| Элемент | Значение |
|---------|----------|
| `<regex name="имя" value="шаблон" group="N" />` | группа `N` (по умолчанию 1) первого совпадения |

## Отличия наборов `Tools/Custom/`

Для утилит читаются только `name`, `path`, `interpreter`, `actions`, `tags`.
Секции `supported` и `options` игнорируются (архитектур у утилиты нет).

```xml
<tool>
  <name value="HEXNyan" />
  <path value=".\Soft\hexnyan.exe" />
  <actions>
    <hexnyan>
      <command value="{option}" />
      <error><string value="error" /></error>
    </hexnyan>
  </actions>
</tool>
```

## Зарезервировано / не реализовано

- `<modules>` (DLL-программаторы) — встречается в описаниях, но **не
  обрабатывается**. Механизм заменён системой плагинов (`IModule`, каталог `plugins/`).
- `<comment>` внутри `.etool`-команды — игнорируется (комментарий шага задаётся
  в `.nyap`: `<wflash value="..." comment="..." />`).
- `<photo>` у скрипта — зарезервировано.

## Минимальный пример (программатор)

```xml
<tool>
  <name value="ST-Link STM32" />
  <path value=".\Soft\ST-LINK Utility\ST-LINK_CLI.exe" />
  <supported><type value="stm32" /></supported>
  <options>
    <list label="Interface" id="st-link.stm32.interface">
      <option label="SWD" default=""><value name="interface" val="SWD" /></option>
      <option label="JTAG"><value name="interface" val="JTAG" /></option>
    </list>
  </options>
  <actions>
    <erase>
      <command value="-c {interface} UR -Q -ME" />
      <error><string value="Error" /><exitcode /></error>
    </erase>
    <wflash>
      <command value='-c {interface} UR -Q -P "{filename}" -V -Rst' />
      <error><string value="Error" /><exitcode /></error>
    </wflash>
  </actions>
</tool>
```

См. также: [Внешние команды и подстановки](external-commands.md).
