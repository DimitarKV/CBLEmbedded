#include "depth_sensor.h"

void DepthSensor::init(){
    vl.begin();
}

byte DepthSensor::getLastReading(){
    return lastReading;
}

void DepthSensor::tick(){
    if(!readingInProgress){
        vl.startRange();
        readingInProgress = true;
    } else if(vl.isRangeComplete()){
        lastReading = vl.readRangeResult();
        vl.startRange();
    }
    
}