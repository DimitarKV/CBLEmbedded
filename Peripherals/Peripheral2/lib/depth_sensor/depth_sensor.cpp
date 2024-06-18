#include "depth_sensor.h"

bool DepthSensor::init(){
    if(!vl.begin()) {
        _status = NOT_RESPONDING;
        return false;
    }
    _status = STATUS_OK;
    return true;
}

bool DepthSensor::status_check() {
    byte data[8];
    data[0] = 0;
    vl.getID(data);
    if (data[0] != 0xB4) {
        _status = NOT_RESPONDING;
        return false;
    }
    return true;
}

void DepthSensor::tick(){
    status_check();
    if(_status == NOT_RESPONDING) {
        init();
    } else if(!_locked) {
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
    if(_locked) {
        readingInProgress = false;
    }
}

byte DepthSensor::getLastReading(){
    return lastReading;
}
