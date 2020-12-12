/*
 * Madgwick AHRS Arduino library
 * Math author: Sebastian Madgwick
 * Written by Oleg Evsegneev for RobotClass.  
 */

class MadgwickAHRS
{
public:
    float q0 = 1.0f, q1 = 0.0f, q2 = 0.0f, q3 = 0.0f;

    void MadgwickAHRSupdateIMU(float tdelta, float gx, float gy, float gz, float ax, float ay, float az);
    float invSqrt(float x);
    void quat2Euler( float q[4], float e[3] );
};

