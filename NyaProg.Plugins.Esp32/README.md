# NyaProg.Plugins.Esp32

Плагин-программатор **ESP32** для NyaProg. Построен на библиотеке
[`Esp32Flasher.Core`](../../../../Dev/esp32) (нативный .NET-прошивальщик, без внешних утилит).

Работает в обеих UI (.NET 8) и в CLI.

## Поддерживаемые чипы (допустимые названия)

| Семейство | Chip-ID / Magic | Bootloader offset |
|-----------|-----------------|-------------------|
| **ESP32** | magic `0x00F01D83` (напр. `ESP32-D0WD-V3`) | `0x1000` |
| **ESP32-S3** | id 9 | `0x0` |
| **ESP32-C3** | id 5 | `0x0` |
| **ESP32-C6** | id 13 | `0x0` |

ESP32-S2 пока не поддерживается. В `<check chip="..."/>` указывается **подстрока**
имени чипа (без учёта регистра), например `ESP32` подойдёт для `ESP32-D0WD-V3`.

## Команды

| Команда | Назначение |
|---------|-----------|
| `scan.ports` | Список COM-портов (источник для динамической опции «Порт») |
| `chipinfo` | Инфо о чипе: имя, ревизия, MAC (в переменные `chip`/`revision`/`mac`) |
| `check` | **Проверка перед прошивкой**: сверка имени чипа и размера flash |
| `wflash` | Прошивка образов |
| `erase` | Стирание всего flash |

### `check` — проверка контроллера

Параметры: `port` (обязателен), `chip` (подстрока имени), `flash-size` (напр. `4MB`),
`baud`, `before`.

Подключается, читает чип и JEDEC-ID flash; при несовпадении возвращает **ошибку** —
шаг и весь прогон прерываются (до прошивки дело не дойдёт).

```xml
<check chip="ESP32" flash-size="4MB" comment="Проверка контроллера" />
```

### `wflash` — прошивка

Параметры: `port` (обязателен), `images` (`offset:файл` через `;`), `flash-size`
(`keep|detect|1MB|2MB|4MB|8MB|16MB`), `flash-mode` (`keep|qio|qout|dio|dout`),
`flash-freq` (`keep|80m|40m|26m|20m`), `baud` (по умолч. `921600`), `verify`, `before`.

Файлы разрешаются относительно каталога проекта (или абсолютные пути).

Полная прошивка (чистая плата) — bootloader + partitions + boot_app0 + app:
```xml
<wflash images="0x1000:bootloader.bin;0x8000:partitions.bin;0xe000:boot_app0.bin;0x10000:firmware.bin"
        flash-size="detect" flash-mode="dio" flash-freq="80m" />
```

Обновление только приложения (если bootloader/partitions уже на плате) — один образ:
```xml
<wflash images="0x10000:firmware.bin" flash-size="detect" />
```

## Порт

Порт выбирается динамической опцией, значения которой отдаёт `scan.ports`:

```xml
<options>
  <dynamic label="Порт" name="port" id="esp32.port" source="scan.ports" />
</options>
```

Если порт в системе один — он выбирается автоматически; кнопка **↻** рядом с
списком обновляет его.

## Пример проекта

```xml
<project>
  <name value="ESP32 GasCamera" />
  <type value="esp32" />
  <target>
    <name value="ESP32-CAM" />
    <device value="ESP32" />
    <options>
      <dynamic label="Порт" name="port" id="esp32.gascamera.port" source="scan.ports" />
    </options>
    <steps>
      <check chip="ESP32" flash-size="4MB" comment="Проверка контроллера" />
      <wflash comment="Прошивка приложения" images="0x10000:esp32-gas-camera.ino.bin" flash-size="detect" />
    </steps>
  </target>
</project>
```

## Протокол

Детали протокола (SLIP, команды, checksum) — в README библиотеки
`G:\Dev\esp32\README.md` и `G:\Dev\esp32\docs\protocol.md`.
