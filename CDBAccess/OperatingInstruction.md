### SQL使用说明（本次实现了单例化）
1、程序启动时候请先实例化 CDBManager.Instance.Init(数据库类型，连接名称，ip，端口，库名，用户名，密码)。

有几个用户请调用几次

2、后续有sql执行时，请调用
 
+ Open(连接名)
+ Close(连接名)
+ DoQuery(连接名,SQL语句)
+ DoNonQuery(连接名，SQL语句)
+ DoNonQuery(连接名，list<SQL语句>)
+ DOQueryPro (连接名，存储过程名，存储过程参数)
  - 请传入对应数据库类型的存储过程参数