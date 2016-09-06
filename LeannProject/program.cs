using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


      class Program
    {
          static void Main(string[] args)
          {
              //ConvDocはあなたが作成したDOC
              ConsoleApplication1.Dao.MuserDao dac = new ConsoleApplication1.Dao.MuserDao();
              //contable1は、DOCの中にあるメソッド
              dac.Select();
              //dac.contable1();

          }

    }




