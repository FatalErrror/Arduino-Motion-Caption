#include <Wire.h>  // подключим стандартную библиотеку I2C
#define addr 0x0D // I2C адрес цифрового компаса HMC5883L
  
void setup() {
  Serial.begin(115200); // инициализация последовательного порта
  Wire.begin(); // инициализация I2C
} 
  
void loop() {

  Wire.beginTransmission(addr); // начинаем связь с устройством по адресу 0x1E
  Wire.write(0x00); // регистр, с которого мы начнём запрашивать данные   
  Wire.endTransmission();  
 
  Wire.requestFrom(addr, 48, true); // запрашиваем 3 байта у ведомого
  while( Wire.available() ) 
  {
    for (int i = 0; i < 48;i++) {
      Serial.print(i);
      Serial.print(" = ");
      Serial.println(Wire.read(), HEX); 
    }
    /*
    char a = Wire.read(); // считываем байт из регистра 0xA; устройство само переходит к следующему регистру
    char b = Wire.read(); // считываем байт из регистра 0xB
    char c = Wire.read(); // считываем байт из регистра 0xС

   // Выводим считанное в последовательный порт:
    Serial.print("A = ");   
    Serial.println(a, HEX); 
    Serial.print("B = ");  
    Serial.println(b, HEX); 
    Serial.print("C = ");  
    Serial.println(c, HEX); 
    Serial.println();   */ 
  }    
  delay(100);
}
