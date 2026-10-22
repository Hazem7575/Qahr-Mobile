using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Qahr.Mobile.Database
{
   public static class QuizShow
    {
       public class Question
       {
           public int Index = 0;
           public string question = "";
           public string[] Answers = new string[4];
           public int RightAnswer;

           public bool CheckAnswer(int Answer)
           {
               return Answer == RightAnswer;
           }
           public string[] GetStr()
           {
               string[] str = new string[5];
               str[0] = question;
               for (int x = 0; x < Answers.Length; x++)
                   str[x + 1] = Answers[x];
               return str;
           }
       }

       public static List<Question> Questions = new List<Question>();
       public static void Load()
       {
           int Index = 0;
           var lines = File.ReadAllLines(Program.ServerConfig.DbLocation + "//quizquestins.ini");
           foreach (var line in lines)
           {
               DBActions.ReadLine reader = new DBActions.ReadLine(line, '#');
               Question obj = new Question();
               obj.question = reader.Read("");
               for(int x = 0; x < 4; x++)
                   obj.Answers[x] = reader.Read("");
               obj.RightAnswer = reader.Read(0);
               obj.Index = ++Index;
               Questions.Add(obj);
          
           }
       }

    }
}
