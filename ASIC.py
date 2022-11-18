import sys
import os 
import serial
import mariadb
import threading 
import datetime 

class ASIC():
    def __init__(self):
        #get the environmental parameters from the unix interface
        self.username=os.getenv("muon_array_sql_username")
        self.password=os.getenv("muon_array_sql_password")
        self.port=int(os.getenv("muon_array_sql_port"))
        self.host=os.getenv("muon_array_sql_host")
        self.dbname=os.getenv("muon_array_sql_dbname")
        
        self.Port=os.getenv("muon_array_device_port")
        self.Baud=int(os.getenv("muon_array_device_baudrate"))
        self.DataDigi=int(os.getenv("muon_array_device_datadigi"))
        self.ParityDigi=os.getenv("muon_array_device_paritydigi")
        self.StopDigi=int(os.getenv("muon_array_device_stopdigi"))
        
    def initialize_communicate(self):
        # Set up the serial communication between the 
        self.communicate=serial.Serial(self.port,self.Baud,self.DataDigi,self.ParityDigi,self.StopDigi)

    def initial_connection(self):
        # Initialize the maria database 
        try :
            self.conn=mariadb.connect(user=self.username,password=self.password,host=self.host,port=self.port,database=self.dbname)
        except mariadb.Error as e:
            print("Error connecting to MariaDB platform {}".format(e))  
        self.conn.autocommit = True
        self.cur=self.conn.cursor()
        self.cur.execute("use local")
        # cur is short for cursor and conn is short for connection
        # Different from pymongo , mariadb is to convey string command that should be used by the command line  
    
    def close_connection(self):
        self.conn.close()
        self.communicate.close()
    
    def check_communication(self): #check the communication with the device  
        try : 
            if (self.communicate.isOpen()==False) :
                self.initialize_communicate()
        except :
            self.initialize_communicate()
        # if we reconnect and it still fails , there is something wrong with the system
        if (self.communicate.isOpen()==False) :
            print("Still failed to open the port ???")     
    
    def process_data(self,serial_info): # This function is to process the data and write it into the mysql 
        if ord(serial_info[0])==0: # The first byte indicates that whether or not it is a muon signal 
            self.cur.execute("insert into data values ({},{})".format(str(datetime.datetime.now()) ,serial_info))
        else: # if it is not a muon signal , and then if digi 2 is zero , it means positive 
            if ord(serial_info[1])==0:
                temperature= ord(serial_info[2]) + ord(serial_info[3])*0.1  # the third and fourth byte resembles the temperature 
            else :
                temperature= ord(serial_info[2]) + ord(serial_info[3])*0.1 
            self.cur.execute("insert into temperature values ({},{})".format(str(datetime.datetime.now()) ,temperature))
            #the fifth , sixth , seventh byte means the preesure 
            pressure= ord(serial_info[4]) * pow(2,8) + ord(serial_info[5]) + ord(serial_info[6]) *0.1
            self.cur.execute("insert into pressure values ({},{})".format(str(datetime.datetime.now()) ,pressure))
            
    
    def main_loop(self):
        self.check_communication()
        
        while True:
            digi_count= self.communicate.inWaiting() 
            if digi_count!=0 : #if we have got some data
                #Store the data and flush the input buffer
                serial_info= self.communicate.read(digi_count)
                self.communicate.flushInput()
                
                # A new thread will process and log the data 
                x_thread=threading.Thread(target=self.process_data,args=(serial_info,))
                x_thread.start()
            

if __name__ ==  "__main__" :
    if sys.platform !='linux':
        print("Warning: This progrma was defined for Linux, it may not work well on Windows or MacOSx")
    else :
        print("======ASIC python driver======")

    asic=ASIC()
    asic.initial_connection()
    asic.main_loop()
    asic.close_connection()
    
