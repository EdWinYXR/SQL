# Sql˵�������������CDBAccess.dll���ã�
��װ�ļ���Ҫ�������������£�
## һ��DBBaseClass
### 1��CDBAccess
 ������ҪΪ���࣬�������ݿ����ӵ����з���������
### 2��DBTybe
 ������ҪΪSQL���ͣ�ö��ֵ����������ʲô���ݿ⣬Ҳ�������ݿ�����ʱ�����ж�
## ����DBAccess
�������ռ�����ҪΪ�������ݿ�����ӡ���ѯ�ȡ�
### 1��MySQLAccess
#### �����MySql.Data.dll����
+ ��ʼ��������

  - MySQLAccess oracle = new MySQLAccess(DBBaseClass.DBType.Oracle,"������");

  - oracle.Init("IP","�˿ں�", "���ݿ�����","�û���","����");
  
+ ��ѯ��DoQuery(string ��ѯ�ַ���)
+ ���� 
  - DoNonQuery(string SQL���)��������Ӱ�����������з���-1Ϊ���򼯴��������ϻ��ȷ�ϣ���0Ϊδ�������ݿ�
  - DoNonQuery(List\<string> SQL���)������0Ϊ�ɹ���-1Ϊʧ��

 + �洢����
   - DOQueryPro(string �洢������,object OracleParameter[])//object ΪOracleParameter[]���ͣ�����ᱨ��
 ### 2��OracleAccess
#### �����Oracle.ManagedDataAccess.dll������
   + ��ʼ��������

  - OracleAccess oracle = new OracleAccess(DBBaseClass.DBType.Oracle,"������");

  - oracle.Init("IP","�˿ں�", "sid ����������","�û���","����");
  
+ ��ѯ��DoQuery(string ��ѯ�ַ���)
+ ���� 
  - DoNonQuery(string SQL���)������Ӱ�����������з���-1Ϊ���򼯴��������ϻ��ȷ�ϣ���0Ϊδ�������ݿ�
  - DoNonQuery(List\<string> SQL���)������0Ϊ�ɹ���-1Ϊʧ��

 + �洢����
   - DOQueryPro(string �洢������,object MySqlParameter[])//object ΪMySqlParameter[]���ͣ�����ᱨ��
 ### 3��SQLServerAccess
  + ��ʼ��������

  - SQLServerAccess oracle = new SQLServerAccess(DBBaseClass.DBType.SQLServer,"������");

  - oracle.Init("IP","�˿ں�", "����","�û���","����");
  
+ ��ѯ��DoQuery(string ��ѯ�ַ���)
+ ���� 
  - DoNonQuery(string SQL���) ��������Ӱ�����������з���-1Ϊ���򼯴��������ϻ��ȷ�ϣ���0Ϊδ�������ݿ�
  - DoNonQuery(List\<string> SQL���)������0Ϊ�ɹ���-1Ϊʧ��

 + �洢����
   - DOQueryPro(string �洢������,object SqlParameter[])//object ΪSqlParameter[]���ͣ�����ᱨ��
## ����NET
������ҪΪ��ϻ�ӡ������������ݿ��Ѹ��ļ������¼���ڳ�������Ŀ¼�µ�Log/DB�ļ����¡�

 + ��ϻ�����Ϊ����


   - TRACE-----������٣��������
   - DEBUG------������Ϣ���������ã�
   - INFO   -----������Ϣ��
   - WARN-----������Ϣ
   - ERROR -----������



