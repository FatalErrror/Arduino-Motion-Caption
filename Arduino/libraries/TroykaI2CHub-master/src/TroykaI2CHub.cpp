/****************************************************************************/
//  Function:       Cpp file for TroykaI2CHub
//  Hardware:       PCA9547
//  Arduino IDE:    Arduino-1.8.5
//  Author:         Igor Dementiev
//  Date:           Jul 25,2018
//  Version:        v1.0.0
//  by www.amperka.ru
/****************************************************************************/

#include "TroykaI2CHub.h"

TroykaI2CHub::TroykaI2CHub(uint8_t i2cHubAddr) {
    _i2cHubAddr = i2cHubAddr;
}

TroykaI2CHub::~TroykaI2CHub() {
}

void TroykaI2CHub::begin() {
    Wire.begin();
    setBusChannel(DEFAULT_CHANNEL);
}

void TroykaI2CHub::setBusChannel(uint8_t channel) {
    if (channel >= COUNT_CHANNEL) {
        return false;
    }

    Wire.beginTransmission(_i2cHubAddr);
    Wire.write(channel | ENABLE_MASK);
    Wire.endTransmission();
}
