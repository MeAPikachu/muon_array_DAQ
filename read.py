import sys
import pymysql
import os
filepath="./log"
#this is the document for journal files.
global input_time
global input_amplitude
class ASIC():
    def __init__(self):
        #get the environmental parameters from the unix interface
        self.username=os.getenv("muon_array_sql_username")
        self.password=os.getenv("muon_array_sql_password")
        self.port=int(os.getenv("muon_array_sql_port"))
        self.host=os.getenv("muon_array_sql_host")
        self.dbname=os.getenv("muon_array_sql_dbname")
    def initial_connection(self):
        # Initialize the maria database 
        try :
            self.conn=pymysql.connect(user=self.username,password=self.password,host=self.host,port=self.port,database=self.dbname)
            print("mariadb connected successfully!")
        except pymysql.Error as e:
            print("Error connecting to MariaDB platform {}".format(e))  
        self.conn.autocommit = True
        self.cur=self.conn.cursor()
        self.cur.execute("use local")
        self.cur.execute("delete from raw_data;")
        # cur is short for cursor and conn is short for connection
        # Different from pymongo , mariadb is to convey string command that should be used by the command line
    def close_connection(self):
        self.conn.close()    
    
    def upload(self,time,amplitude):
        sql="insert into raw_data (time,amplitude) values (%s,%s)"
        self.cur.execute(sql,(time,amplitude))
        # We only record the time and amplitude , others can be added later.
        # table 'raw_data' was created in my own database, you can create this table in the initialization of the class ASIC. 
    def main_loop(self):
        readnum=1
        judge=1
        input_time="0"
        input_amplitude="0"
        while True:
            totalnum=len(os.listdir(filepath))
            if readnum<=totalnum:
                with open(filepath+'/%s.txt'%(readnum),'r',encoding='utf-8') as f:
                    for sentence in f:
                        if (judge%2)==1:
                            input_time=sentence
                            #print(input_time+'\n'),this is for test run
                        else:
                            input_amplitude=sentence
                            #print(input_amplitude+'\n'), this is for test run
                            self.upload(input_time,input_amplitude)
                            self.conn.commit()
                        judge+=1
            else:
                print("data uploaded successfully!")
                break
            readnum+=1
            judge=1
        return totalnum

if __name__ ==  "__main__" :
    if sys.platform !='linux':
        print("Warning: This progrma was defined for Linux, it may not work well on Windows or MacOSx")
    else :
        print("======ASIC python driver======")

    asic=ASIC()
    asic.initial_connection()
    totalnum=asic.main_loop()
    asic.close_connection()
    for i in range(1,totalnum+1):
        os.remove(filepath+'/%s.txt'%(i))
        # the log will be removed as soon as the data was uploaded successfully    
