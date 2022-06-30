# Sql说明（请首先添加CDBAccess.dll引用）
封装文件主要包含三个类如下：
## 一、DBBaseClass
### 1、CDBAccess
 此类主要为基类，包含数据库连接的所有方法、参数
### 2、DBTybe
 此类主要为SQL类型，枚举值用于区分是什么数据库，也方便数据库杂用时方便判断
## 二、DBAccess
此命名空间下主要为各个数据库的连接、查询等。
### 1、MySQLAccess
#### 需添加MySql.Data.dll引用
+ 初始化方法：

  - MySQLAccess oracle = new MySQLAccess(DBBaseClass.DBType.Oracle,"连接名");

  - oracle.Init("IP","端口号", "数据库名称","用户名","密码");
  
+ 查询：DoQuery(string 查询字符串)
+ 更改 
  - DoNonQuery(string SQL语句)，返回受影响行数：其中返回-1为程序集错误（请检查黑匣子确认），0为未更改数据库
  - DoNonQuery(List\<string> SQL语句)，返回0为成功，-1为失败

 + 存储过程
   - DOQueryPro(string 存储过程名,object OracleParameter[])//object 为OracleParameter[]类型，否则会报错。
 ### 2、OracleAccess
#### 需添加Oracle.ManagedDataAccess.dll的引用
   + 初始化方法：

  - OracleAccess oracle = new OracleAccess(DBBaseClass.DBType.Oracle,"连接名");

  - oracle.Init("IP","端口号", "sid 服务器名称","用户名","密码");
  
+ 查询：DoQuery(string 查询字符串)
+ 更改 
  - DoNonQuery(string SQL语句)返回受影响行数：其中返回-1为程序集错误（请检查黑匣子确认），0为未更改数据库
  - DoNonQuery(List\<string> SQL语句)，返回0为成功，-1为失败

 + 存储过程
   - DOQueryPro(string 存储过程名,object MySqlParameter[])//object 为MySqlParameter[]类型，否则会报错。
 ### 3、SQLServerAccess
  + 初始化方法：

  - SQLServerAccess oracle = new SQLServerAccess(DBBaseClass.DBType.SQLServer,"连接名");

  - oracle.Init("IP","端口号", "库名","用户名","密码");
  
+ 查询：DoQuery(string 查询字符串)
+ 更改 
  - DoNonQuery(string SQL语句) ，返回受影响行数：其中返回-1为程序集错误（请检查黑匣子确认），0为未更改数据库
  - DoNonQuery(List\<string> SQL语句)，返回0为成功，-1为失败

 + 存储过程
   - DOQueryPro(string 存储过程名,object SqlParameter[])//object 为SqlParameter[]类型，否则会报错。
## 三、NET
此类主要为黑匣子。调用任意数据库会把更改及报错记录放在程序启动目录下的Log/DB文件夹下。

 + 黑匣子设计为四类


   - TRACE-----错误跟踪，方便调试
   - DEBUG------调试信息（基本不用）
   - INFO   -----基础信息类
   - WARN-----警告信息
   - ERROR -----错误类



