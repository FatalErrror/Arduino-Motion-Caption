/****************************************************************************/
//  Function:       Header file for TroykaI2CHub
//  Hardware:       PCA9547
//  Arduino IDE:    Arduino-1.8.5
//  Author:         Igor Dementiev
//  Date:           Jul 25,2018
//  Version:        v1.0.0
//  by www.amperka.ru
/****************************************************************************/

#ifndef TROYKA_I2C_HUB_H_
#define TROYKA_I2C_HUB_H_

#include <Arduino.h>
#include <Wire.h>

#define DEFAULT_I2C_HUB_ADDRESS 0x70
#define ENABLE_MASK             0x08
#define DEFAULT_CHANNEL         0
#define COUNT_CHANNEL           8

class TroykaI2CHub
{
public:
    TroykaI2CHub(uint8_t i2cHubAddr = DEFAULT_I2C_HUB_ADDRESS);
    ~TroykaI2CHub();
    void begin();
    void setBusChannel(uint8_t channel);
private:
    uint8_t _i2cHubAddr;
};

#endif  // TROYKA_I2C_HUB_H_
