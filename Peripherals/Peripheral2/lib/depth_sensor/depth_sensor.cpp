#include "depth_sensor.h"

void DepthSensor::init(){
    vl.begin();
}

byte DepthSensor::getLastReading(){
    return lastReading;
}

void DepthSensor::tick(){
    uint64_t now = esp_timer_get_time();
    if(!readingInProgress){
        vl.startRange();
        readingInProgress = true;
    } else if(now - lastReadingTime >= readingPeriodUs && vl.isRangeComplete()){
        lastReadingTime = now;
        lastReading = vl.readRangeResult();
        vl.startRange();
    }
    
}