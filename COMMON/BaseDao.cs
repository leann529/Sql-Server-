using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Itis.ApplicationBlocks.Data;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;


namespace BtcConvertPro.Common
{
    /// <summary>
    /// Daoの基底クラス
    /// </summary>
    public abstract class BaseDao
    {
        #region MakeParam
        /// <summary>
        /// 数値型のSqlDbTypeリスト
        /// </summary>
        private static List<SqlDbType> TypeNumbers = new List<SqlDbType>(
            new SqlDbType[] { SqlDbType.BigInt, SqlDbType.Bit, SqlDbType.Decimal, SqlDbType.Float, 
                            SqlDbType.Int, SqlDbType.Money, SqlDbType.Real, SqlDbType.SmallInt,
                            SqlDbType.SmallMoney, SqlDbType.TinyInt}
        );

        /// <summary>
        /// 日付型のSqlDbTypeリスト
        /// </summary>
        private static List<SqlDbType> TypeDates = new List<SqlDbType>(
            new SqlDbType[] { SqlDbType.Date, SqlDbType.DateTime, SqlDbType.DateTime2, 
                            SqlDbType.DateTimeOffset,SqlDbType.SmallDateTime, 
                            SqlDbType.Time, SqlDbType.Timestamp}
        );

        /// <summary>
        /// 文字型のSqlDbTypeリスト
        /// </summary>
        private static List<SqlDbType> TypeStrings = new List<SqlDbType>(
            new SqlDbType[] { SqlDbType.Char, SqlDbType.NChar, SqlDbType.NText, SqlDbType.NVarChar,
                            SqlDbType.Text, SqlDbType.VarChar, SqlDbType.Xml}
        );

        /// <summary>
        /// バイナリ型のSqlDbTypeリスト
        /// </summary>
        private static List<SqlDbType> TypeBinarys = new List<SqlDbType>(
            new SqlDbType[] { SqlDbType.Binary, SqlDbType.VarBinary }
        );

        /// <summary>
        /// パラメタライズドクエリ引数を生成する
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static SqlParameter MakeParam(string parameterName, SqlDbType dbType, int size, object value)
        {
            if (dbType == SqlDbType.VarChar)
            {
                if (value != null)
                {
                    return SQLHelper.MakeParam(parameterName, dbType, size, SubStringByByte(value, size));
                }
            }

            return SQLHelper.MakeParam(parameterName, dbType, size, value);
        }

        /// <summary>
        /// パラメタライズドクエリ引数を生成する（数値型）
        /// </summary>
        /// <param name="parameterName">パラメータ名</param>
        /// <param name="dbType">SqlDbType</param>
        /// <param name="value">値</param>
        /// <returns>パラメタライズドクエリ引数</returns>
        protected static SqlParameter MakeParamNumber(string parameterName, SqlDbType dbType, ValueType value)
        {
            if (TypeNumbers.Contains(dbType))
            {
                return SQLHelper.MakeParam(parameterName.ToLower(), dbType, 0, value);
            }
            else
            {
                throw new ArgumentException(dbType.ToString());
            }
        }
        /// <summary>
        /// バイトで切取
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SubStringByByte(object value, int length)
        {
            string temp = value.ToString();
            int valueByte = System.Text.Encoding.Default.GetByteCount(temp);

            while (valueByte > length)
            {
                if (valueByte - length > 2)
                {
                    temp = temp.Substring(0, temp.Length - (valueByte - length) / 2);
                }
                else
                {
                    temp = temp.Substring(0, temp.Length - 1);
                }
                valueByte = Encoding.Default.GetByteCount(temp);
            }

            return temp;
        }
        #endregion


        /// <summary>
        /// 移行先SEQ保存
        /// </summary>
        /// <param name="DB">DB接続</param>
        /// <param name="oldSeq">移行元</param>
        /// <param name="newSeq">移行先</param>
        /// <param name="companyId">会社ID</param>
        /// <param name="kbn">区分</param>
        public void InsertWorkTable(DBCommon DB,
            object oldSeq, object newSeq, object companyId, int kbn)
        {
            // 組織2情報を登録する
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("INSERT INTO  TABLE1 ");
            sql.AppendLine("           ( company_id ");
            sql.AppendLine("           , old_seq ");
            sql.AppendLine("           , new_seq ");
            sql.AppendLine("           , kbn) ");
            sql.AppendLine("     VALUES ");
            sql.AppendLine("           (@company_id ");
            sql.AppendLine("           ,@old_seq ");
            sql.AppendLine("           ,@new_seq ");
            sql.AppendLine("           ,@kbn) ");

            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = MakeParam("@company_id", SqlDbType.Int, 4, companyId);
            parameters[1] = MakeParam("@old_seq", SqlDbType.BigInt, 8, oldSeq);
            parameters[2] = MakeParam("@new_seq", SqlDbType.BigInt, 8, newSeq);
            parameters[3] = MakeParam("@kbn", SqlDbType.Int, 4, kbn);
            DB.ExecuteNonQuery(sql.ToString(), parameters);
        }

        /// <summary>
        /// 本部区分取得
        /// </summary>
        /// <param name="kbnCd"></param>
        /// <param name="kbnSbtCd"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public object GetHonbuKbn(object kbnCd, object kbnSbtCd, DataSet ds)
        {
            if (kbnCd != null && kbnCd != DBNull.Value)
            {
                DataRow[] dr = ds.Tables[0].Select("kbn_cd_old = " + Convert.ToInt32(kbnCd) + " AND kbn_sbt_cd = " + Convert.ToInt32(kbnSbtCd));
                if (dr.Length > 0)
                {
                    return dr[0]["kbn_cd_new"];
                }
            }
            
            return DBNull.Value;
        }
    }
}
