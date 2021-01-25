#include <Wire.h>
#include <MPU6050.h>
#include <TroykaI2CHub.h>

#define SEN_COUNT 8

int CalibratingCount;

class Sensor
{
  private:
    MPU6050 mpu;
    unsigned long tm0;
    int16_t ax, ay, az;
    int16_t gx, gy, gz;
    int n;

  public:
    void Init(int num) {
      n = num;
      // initialize device
      mpu.initialize();
      // состояние соединения
      Serial.print("MPU6050 number " + String(n));
      Serial.println(mpu.testConnection() ? " connection successful" : " connection failed");
      mpu.setFullScaleAccelRange(MPU6050_ACCEL_FS_2);
      mpu.setFullScaleGyroRange(MPU6050_GYRO_FS_2000);
      Serial.println("DONT MOVE OR TOUCH SENSORS");
      
      Serial.println("Start:");
      Serial.println("Calibrateing " + String(n) +" Accel ");
      mpu.CalibrateAccel(CalibratingCount);
      Serial.println("Calibrateing " + String(n) +" Gyro ");
      mpu.CalibrateGyro(CalibratingCount);
    }

    String GetData() {
      mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
      unsigned long tm = millis();
      unsigned long dtm = tm - tm0;
      tm0 = tm;
      return String(n) + "=" + String(dtm) + "_" + String(gx) + "_" + String(gy) + "_" + String(gz) + "_" + String(ax) + "_" + String(ay) + "_" + String(az);
    }
};

// адрес по умолчанию 0x70
TroykaI2CHub splitter;

Sensor sensors[SEN_COUNT];

void setup() {
  Serial.begin(115200);
  Serial.println("Arduino motion caption connected");
  
  Wire.begin();
  Wire.setClock(400000);
  
  pinMode(12, INPUT_PULLUP);
  
  if (digitalRead(12) == LOW) {
    CalibratingCount = 2;
  }
  else {
    CalibratingCount = 20;
  }
  
  
  splitter.begin();

  for (int i = 0; i < SEN_COUNT; i++){
    splitter.setBusChannel(i);
    sensors[i] = Sensor();
    sensors[i].Init(i);
  }
  
  
  Serial.println("Start");
}

void loop() {
  String data = "";
  for (int i = 0; i < SEN_COUNT; i++){
    splitter.setBusChannel(i);
    data += "|";
    data += sensors[i].GetData();
  }
  Serial.println(data);
}
