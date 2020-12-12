#include "Wire.h"
#include "I2Cdev.h"
#include "MPU6050.h"
#include "MadgwickAHRS.h"
#include <TroykaI2CHub.h>

#define TO_RAD 0.01745329252f
#define TO_DEG_PER_SEC 16.384f
#define SEN_COUNT 8

int CalibratingCount;

class Sensor  // имя класса принято писать с Большой Буквы
{
  private:
    MPU6050 mpu;
    
    MadgwickAHRS dmp;
    unsigned long tm0;

    int16_t ax, ay, az;
    int16_t gx_raw, gy_raw, gz_raw;
    int n;

  public:
    // список методов доступных другим функциям и объектам программы
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
      /*for (int i = 0; i < 3;i++) {
        Serial.println(String(3-i) + "...");
        delay(1000);
      }*/
      
      Serial.println("Start:");
      Serial.println("Calibrateing " + String(n) +" Accel ");
      mpu.CalibrateAccel(CalibratingCount);
      Serial.println("Calibrateing " + String(n) +" Gyro ");
      mpu.CalibrateGyro(CalibratingCount);
      /*for (int i = 0; i < 3;i++){
        Serial.println("Calibrateing " + String(n) +" Accel ");
        mpu.CalibrateAccel(2);
        Serial.println("Calibrateing " + String(n) +" Gyro ");
        mpu.CalibrateGyro(2);
      }*/
      
      //Serial.println("\n");
      
    }

    /*void Update(){
      mpu.getMotion6(&ax, &ay, &az, &gx_raw, &gy_raw, &gz_raw);
      float gx = gx_raw * TO_RAD / TO_DEG_PER_SEC;
      float gy = gy_raw * TO_RAD / TO_DEG_PER_SEC;
      float gz = gz_raw * TO_RAD / TO_DEG_PER_SEC;
      
      unsigned long tm = millis();
      float tdelta = tm - tm0;
      tm0 = tm;
      dmp.MadgwickAHRSupdateIMU(tdelta/1000.0, gx, gy, gz, (float)ax, (float)ay, (float)az);
      
    }*/

    String GetData() {
      mpu.getMotion6(&ax, &ay, &az, &gx_raw, &gy_raw, &gz_raw);
      float gx = gx_raw * TO_RAD / TO_DEG_PER_SEC;
      float gy = gy_raw * TO_RAD / TO_DEG_PER_SEC;
      float gz = gz_raw * TO_RAD / TO_DEG_PER_SEC;
      
      unsigned long tm = millis();
      dmp.MadgwickAHRSupdateIMU((tm - tm0)/1000.0, gx, gy, gz, (float)ax, (float)ay, (float)az);
      tm0 = tm;
      
      return String(dmp.q0) + "_" + String(dmp.q1) + "_" + String(dmp.q2) + "_" + String(dmp.q3);
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
  //pinMode(10, INPUT_PULLUP);
  
  if (digitalRead(12) == LOW) {
    CalibratingCount = 2;
  }
  else {
    CalibratingCount = 20;
  }
  
  
  splitter.begin();

  for (int i = 0; i < SEN_COUNT; i++){
    //if (i == 3 || i == 4) continue;
    splitter.setBusChannel(i);
    sensors[i] = Sensor();
    sensors[i].Init(i);
  }
  
  
  Serial.println("start");
}

void loop() {
  String data = "";
  
  for (int i = 0; i < SEN_COUNT; i++){
    ///if (i == 3 || i == 4) continue;
    splitter.setBusChannel(i);
    data += "|" + String(i) + "=";
    data += sensors[i].GetData();
  }
  /*
  if (digitalRead(10) == LOW) {
    data += "|HT";
  }
  else {
    data += "|HF";
  }*/
  Serial.println(data);
}
