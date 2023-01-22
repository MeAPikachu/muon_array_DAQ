import datetime
import sys
import libdaq
from device import *
from ctypes import *
import math
def adc_single_sample_example(device):
    #get adc data
    #only satisfies one channel
    channel_list=[0]
    #if more than one channel, this can be altered into "channel_list=[0,1,2,3...]"
    (errorcode,result)=device.adc1.singleSample(channel_list)
    print("ADC1 single sample:")
    print("result:")
    #these "print" can be removed
    for data in result :
        sys.stdout.write("%2.4f " % (data))
        #only satisfies one channel
        #if more than one channel then we get a group of number
    print("")
    return data
def checking_daq():
    index=0
    libdaq.libdaq_init()
    device_count=libdaq.libdaq_device_get_count()
    if device_count<0:
        print("no device detected!\n")
        return
    else:
        print("connected successfully!\n")
    (errorcode,device_name) =  libdaq.libdaq_device_get_name(index)
    print("device: %s deteced"%(device_name))
    device=libdaq.DAQUSB3213A(device_name)
    # DAQUSB3213A is the model number of DAQ I am using
    return device       
def mainloop():    
    device=checking_daq()
    textnum=1
    while True:
        with open('./log/%s.txt'%(textnum),'w',encoding='utf-8') as f:
            #a log document is needed here, you have to create one first.
            for i in range(1,101):
                #the buffer is 100, and it can be altered here. 
                amplitude=adc_single_sample_example(device)
                time.sleep(0.01)
                #this is the sampling rate, here we use 100Hz
                f.write(str(datetime.datetime.fromtimestamp(time.time()))+'\n'+'%f'%(amplitude)+'\n')        
        textnum+=1
        
    libdaq.libdaq_exit()
if __name__ == '__main__':
    print("------checking process------")
    checking_daq()
    print("enter \" start \" to start the mainloop")
    IN=input()
    if IN=="start":
        mainloop()
