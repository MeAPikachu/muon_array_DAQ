import sys
import pymysql
import os
import matplotlib.pyplot as plt
import numpy as np
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
        self.cur=self.conn.cursor()
        self.cur.execute("use local")
        # cur is short for cursor and conn is short for connection
        # Different from pymongo , mariadb is to convey string command that should be used by the command line
    def close_connection(self):
        self.conn.close()
    def get_record(self):
        record=self.cur.execute("select * from raw_data;")
        result=self.cur.fetchall()
        return result
        # "result" stores two columns from mariadb ,which are "time" and "amplitude". 
    def listlength(self):
        self.cur.execute("use information_schema")
        receive=self.cur.execute("select table_name,table_rows from tables where TABLE_SCHEMA = 'local' AND table_name='raw_data';")
        result=self.cur.fetchone()
        self.cur.execute("use local")
        return result[1]
        # this function counts the number of records that satisfies the search criteria.
if __name__=="__main__":
    asic=ASIC()
    asic.initial_connection()
    out=np.array(asic.get_record())
    y=out[:,1]
    # We only focus on the "amplitude" as the shape of the data is what really matters.
    length=asic.listlength()
    x=np.array([i for i in range(1,(length+1))])
    plt.figure(figsize=(100,50),dpi=100)
    plt.plot(x,y)  
    # Details of the figure still needs to be added
    plt.show()
    plt.savefig('test.png')
    # We can choose to build a 'log' document for this part, in order to store all the figures temporarily.
