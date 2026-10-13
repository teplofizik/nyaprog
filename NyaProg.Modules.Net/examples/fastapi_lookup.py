"""
Простой REST-сервис для проверки команды NyaProg `lookup`.

Отдаёт данные устройства по серийному номеру: серийник, MAC/IP, модель.
Формат — тот же, что разбирает `lookup` в режиме json: ($.network.mac и т.п.).

Запуск:
    pip install fastapi uvicorn
    uvicorn fastapi_lookup:app --port 8099

Примеры запросов:
    GET /device/SN12345
    GET /device/SN12345/firmware.bin     (бинарник для команды `download`)
    GET /serial/SN12345                  (простой текст для regex/text)
"""

from fastapi import FastAPI, HTTPException, Response

app = FastAPI(title="NyaProg lookup demo")

# Учебная "база". В реальности — БД или внешний сервис.
DEVICES = {
    "SN12345": {
        "serial": "SN12345",
        "model": "GC-0611W4",
        "network": {"mac": "AA:BB:CC:DD:EE:FF", "ip": "10.0.0.5"},
    },
    "SN77777": {
        "serial": "SN77777",
        "model": "GC-0611W4",
        "network": {"mac": "11:22:33:44:55:66", "ip": "10.0.0.6"},
    },
}


@app.get("/device/{serial}")
def device(serial: str):
    """JSON-описание устройства — для lookup с extract=...json:$.network.mac..."""
    dev = DEVICES.get(serial)
    if dev is None:
        raise HTTPException(status_code=404, detail="unknown serial")
    return dev


@app.get("/serial/{serial}")
def serial_text(serial: str):
    """Текстовый ответ — удобно для regex:/text:."""
    dev = DEVICES.get(serial)
    if dev is None:
        raise HTTPException(status_code=404, detail="unknown serial")
    return Response(content=f"Serial: {dev['serial']}\nModel: {dev['model']}\n",
                    media_type="text/plain")


@app.get("/device/{serial}/firmware.bin")
def firmware(serial: str):
    """Отдаём байты (заглушка) — для команды download."""
    dev = DEVICES.get(serial)
    if dev is None:
        raise HTTPException(status_code=404, detail="unknown serial")
    payload = (f"FIRMWARE for {dev['serial']} model={dev['model']} "
               f"mac={dev['network']['mac']} ip={dev['network']['ip']}\n").encode()
    return Response(content=payload, media_type="application/octet-stream")
