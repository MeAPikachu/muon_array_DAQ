import mariadb  
import os 
# This script is to initialize the mysql database 

class Maria_Initialize():
    def __init__ (self):
        #get the environmental parameters from the unix interface
        self.username=os.getenv("muon_array_sql_username")
        self.password=os.getenv("muon_array_sql_password")
        self.port=int(os.getenv("muon_array_sql_port"))
        self.host=os.getenv("muon_array_sql_host")
        self.dbname=os.getenv("muon_array_sql_dbname")
        
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
        
    def initialize_mode(self):
        # table mode is to log the run mode 
        self.cur.execute("create table if not exists mode (time datetime , data json ,primary key(time)) engine=innodb")
        print("Create table mode ")
        
    def initialize_data(self):
        # data is to log the raw data from the ASIC
        self.cur.execute("create table if not exists data (time datetime , data varchar(50) ,primary key(time) ) engine=innodb")
        print("Create table data ")
    
    def initialize_rate(self):
        # rate is to log the event rate of from the ASIC
        self.cur.execute("create table if not exists rate (time datetime , rate float , primary key(time)) engine=innodb")
        print("Create table rate ")
    
    def initialize_command(self):
        # command is to log the command from the administrator
        self.cur.execute("create table if not exists command (time datetime , command  int , primary key(time)) engine=innodb")
        print("Create table command ")
        # insert into command (time ,command) values (str(datetime.datetime.now(), 1))
        # 1 START , 0 STOP 
    
    def initialize_log(self):
        # command is to log the command from the administrator
        self.cur.execute("create table if not exists log (time datetime , log  varchar(30) , primary key(time)) engine=innodb")
        print("Create table log ")
        
    def initialize_temperature(self):
        self.cur.execute("create table if not exists temperature (time datetime , temperature float, primary key(time)) engine=innodb")
        print("Create table temperature")
    
    def initialize_pressure(self):
        self.cur.execute("create table if not exists pressure (time datetime , pressure float , primary key(time)) engine=innodb")
        print("Create table pressure")
    
         
    
if __name__  == "__main__":
    maria= Maria_Initialize()
    maria.initial_connection()
    maria.initialize_mode()
    maria.initialize_data()
    maria.initialize_rate()
    maria.initialize_command()
    maria.initialize_log()
    maria.initialize_temperature()
    maria.initialize_pressure()
    maria.close_connection()