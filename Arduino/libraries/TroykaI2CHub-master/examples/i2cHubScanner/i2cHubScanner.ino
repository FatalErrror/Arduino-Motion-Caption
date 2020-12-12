// библиотека для работы с I²C хабом
#include <TroykaI2CHub.h>

// создаём объект для работы с хабом
// адрес по умолчанию 0x70
TroykaI2CHub splitter;

// можно создавать несколько объектов с разными адресами
// подробнее читайте на:
// http://wiki.amperka.ru/продукты:troyka-i2c-hub

void setup() {
  // открываем последовательный порт
  Serial.begin(115200);
  // ждём открытия порта
  while(!Serial) {
  }
  // печатаем сообщение об успешной инициализации Serial-порта
  Serial.println("Serial init OK");
  // начало работы с I²C хабом
  splitter.begin();
  Serial.println("Splitter init OK");
  // ждём одну секунду
  delay(1000);
}

void loop() {
  // счётчик цикла
  for (int i = 0; i < 8; i++) {
    // переключаем по очереди каналы
    splitter.setBusChannel(i);
    // выводим номер канала
    Serial.print("Set channel ");
    Serial.print(i);
    Serial.println(":");
    // запускаем I²C сканер
    startScanerI2C();
    // ждём одну секунду
    delay(1000);
  }
}

void startScanerI2C()
{
  // переменная состояние ответа
  byte state;
  // переменная хранения текущего адреса
  byte address;
  // переменная для хранения количества найденых I²C устройств
  int countDevices = 0;
  // печатем о начале поиска
  Serial.println("Scanning...");
  // перебираем по очереди все адреса от 0 до 127
  for (address = 1; address < 127; address++ ) {
    // начинаем передачу данных по текущем адресу
    Wire.beginTransmission(address);
    // завершаем передачу данных
    state = Wire.endTransmission();
    // если пришедший байт равен нулю
    if (state == 0) {
      // на адресе есть устройство
      // печатаем об этом
      Serial.print("I2C device found at address 0x");
      // если адрес меньше 16, печатем ноль
      if (address < 16) {
        Serial.print("0");
      }
      // печатаем текущий адрес в 16 разрядной системе исчесления
      Serial.print(address, HEX);
      Serial.println("  !");
      // инкрементируем кол-во найденых устройств
      countDevices++;
    }     
  }
  // если не найдено ни одного I²C устройства
  // печатаем об этом
  if (countDevices == 0) {
    Serial.println("No I²C devices found");
  } else {
    // печатаем о завершении процесса
    Serial.println("Done");
  }
}
