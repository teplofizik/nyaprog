# NyaProg.Plugins.HexConv

Плагин конвертации форматов **bin ↔ Intel HEX**.

## Команды

### `bintohex` — BIN → Intel HEX

| Параметр | Обяз. | По умолч. | Описание |
|---|---|---|---|
| `in`     | да    |           | Входной bin |
| `out`    | да    |           | Выходной hex |
| `offset` | нет   | `0`       | Стартовый адрес (dec или `0x...`) |

Выходы: `file`, `size`, `offset`.

Если `offset > 0xFFFF` — добавляется запись *extended linear address* (тип 04);
иначе адреса 16-битные. Завершается записью *EOF*.

### `hextobin` — Intel HEX → BIN

| Параметр | Обяз. | По умолч. | Описание |
|---|---|---|---|
| `in`     | да    |           | Входной hex |
| `out`    | да    |           | Выходной bin |
| `offset` | нет   | `0`       | Базовый адрес результата (`0` — авто, от младшего адреса) |
| `fill`   | нет   | `0xFF`    | Заполнение пропусков между записями |

Выходы: `file`, `size`, `base`.

Поддерживаются записи Intel HEX: данные (00), EOF (01), extended segment (02),
extended linear (04). Пропуски адресов заполняются байтом `fill`.

## Примеры

Проект:

```xml
<steps>
  <bintohex in="app.bin" out="app.hex" offset="0x08000000" comment="bin → hex" />
  <hextobin in="dump.hex" out="dump.bin" comment="hex → bin" />
</steps>
```

CLI:

```
NyaProg.Cli config.xml --exec bintohex in=app.bin out=app.hex offset=0x08000000
NyaProg.Cli config.xml --exec hextobin in=app.hex out=app.bin
```

Проверено: round-trip `bin → hex → bin` даёт идентичные байты (64 байта, offset
`0x08000000`); пропуски в hex заполняются `0xFF`.
