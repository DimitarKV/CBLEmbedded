#ifndef ERRORPRONEDEVICE_H
#define ERRORPRONEDEVICE_H

enum DeviceStatus {
    STATUS_OK,
    READINGS_INVALID,
    NOT_RESPONDING
};

class ErrorProneDevice {
protected:
    DeviceStatus _status;
    int _identifier;
    bool _locked = false;
public:
/**
 * Defines the basic functionality of the devices.
 * Makes sure that the peripheral devices work properly.
 */
    virtual bool init();
    virtual bool status_check();
    virtual void tick();
    void lock(bool lock = true) {
        _locked = lock;
    }
    DeviceStatus getStatus() {
        return _status;
    }
};

#endif // !ERRORPRONEDEVICE_H