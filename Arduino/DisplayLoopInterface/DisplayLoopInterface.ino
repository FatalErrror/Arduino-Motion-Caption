//=======================================================
//=============this is code for interface================
#define PAUSE_BTN_PIN 12

int Mode = 1;

void setup() {
  pinMode(PAUSE_BTN_PIN, INPUT_PULLUP);
  Serial.begin(9600);
  Serial.println("---------------------------");
  Serial.println("Arduino has started");
  Serial.println("Started mode: 1 - blink");
  Serial.print("For chouse mode conect GND to the pin ");Serial.println(PAUSE_BTN_PIN);
  Serial.println("---------------------------");
}

void loop() {
  if (Mode != 0 && digitalRead(PAUSE_BTN_PIN) == LOW) {
    ActiveMode0();
  }
  LoopCouse();
}

void ActiveMode0() {
  Serial.println("---------------------------");
  Serial.print("Now active mode: ");Serial.println(Mode);
  Serial.println("End your chouse ';' -> '12;' or '1;'");
  Serial.println("Chouse mode:");
  PrintModes();
  Serial.println("---------------------------");
  Mode = 0;
}

void Mode0() {
  if (Serial.available() > 0) {
    // read the incoming byte:
    String incomingString = Serial.readStringUntil(';');
    Serial.print("You chousen mode: ");
    Serial.println(incomingString);
    Mode = incomingString.toInt();
    SetupChouse();
  }
}

void Mode1Setup() {
  Serial.println("Mode 1 started, Blink L led;");
  pinMode(13, OUTPUT);
}

void Mode1() {
  digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(13, LOW);    // turn the LED off by making the voltage LOW
  delay(500);                       // wait for a second
}

//====================================================
//==this is switch...cases fore chose mode functions==

void PrintModes() {
  //list of modes 
  Serial.println("1: Blink");
  Serial.println("2: Empty");
  Serial.println("3: I2C Scanner");
}

void SetupChouse() {
  //cases of setup functions
  Serial.println("---------------------------");
  switch (Mode) {
      case 1:
        Mode1Setup();
        break;
      case 3:
        Mode3Setup();
    }
}

void LoopCouse() {
  //cases of loop functions
  switch (Mode) {
      case 0:
        Mode0();
        break;
      case 1:
        Mode1();
        break;
      case 2:
        Mode2();
        break;
      case 3:
        Mode3();
        break;
      default:
        Serial.println("This mode doesn exist, chouse another;");
        Mode = 0;
    }
}

//=================================================================
//=================Write your code later===========================

void Mode2() {
   //Serial.println("Started empty mode;");
}

#include <Wire.h>
void Mode3Setup() {
   Serial.println("Started I2C Scanner;");
}

void Mode3() {
  byte error, address;
  int nDevices;

  Serial.println("Scanning...");

  nDevices = 0;
  for(address = 1; address < 127; address++ ) 
  {
    // The i2c_scanner uses the return value of
    // the Write.endTransmisstion to see if
    // a device did acknowledge to the address.
    Wire.beginTransmission(address);
    error = Wire.endTransmission();

    if (error == 0)
    {
      Serial.print("I2C device found at address 0x");
      if (address<16) 
        Serial.print("0");
      Serial.print(address,HEX);
      Serial.println("  !");

      nDevices++;
    }
    else if (error==4) 
    {
      Serial.print("Unknown error at address 0x");
      if (address<16) 
        Serial.print("0");
      Serial.println(address,HEX);
    }    
  }
  if (nDevices == 0)
    Serial.println("No I2C devices found\n");
  else
    Serial.println("done\n");

  delay(5000);           // wait 5 seconds for next scan
}
