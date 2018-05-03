using NorthOps.Portal.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Portal.Models
{
    public partial class Exam
    {
        public IEnumerable<Question> MBTIExams
        {
            get
            {
                return this.Questions.Where(m => m.Choices.Count() > 0).OrderBy(m => Convert.ToInt32(m.Title.Replace("MBTI", "")));
            }
        }
        public IEnumerable<Question> RandomQuestion
        {
            get { return this.Questions.Skip(0).Take(50).Where(m => m.Choices.Count() > 0).OrderBy(m => Guid.NewGuid()); }
        }
        public IEnumerable<Question> QuestionsList
        {
            get
            {
                return this.IsRandom == true ? this.RandomQuestion : this.MBTIExams;
            }
        }
    }
    public enum ExamTypes
    {
        Multiple,
        TrueorFalse,
        MBTI,
        Listening,
        TypingSkills

    }
    public partial class ExamType
    {
        public static string value(int val)
        {
            switch (val)
            {
                case 0:
                    return "Multiple Choice";
                case 1:
                    return "True or False";
                case 2:
                    return "Personality Exam";
                case 3:
                    return "Listening Exam";
                case 4:
                    return "Typing Speed";
                default:
                    return "";
                    break;
            }
        }
    }
    public class ExportUploadSettings
    {
        public static Guid ExamId;

        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".xlsx", ".xls" },
            MaxFileSize = 4000000
        };
        static string CheckDataRowIsNull(DataRow dataRow)
        {
            string returnval = string.Empty;
            try
            {
                returnval = dataRow["correct"].ToString();
            }
            catch (Exception)
            {

            }
            return returnval;
        }

        public static void FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            if (e.UploadedFile.IsValid)
            {
                var files = HttpContext.Current.Server.MapPath($"~/filemanager/excel/{ System.IO.Path.GetRandomFileName() }");
                Task.Run(new Action(async () =>
                {

                    e.UploadedFile.SaveAs(files);
                    var dt = new Excel(files).ExecuteReader("select * from [sheet1$]");
                    foreach (DataRow dr in dt.Rows)
                    {
                        var question = new Question()
                        {
                            Question1 = dr["Question"].ToString(),
                            Title = dr["Title"].ToString(),
                            QuestionId = Guid.NewGuid(),
                            ExamId = ExamId,
                            DateCreated = DateTime.Now,
                            Number = Convert.ToInt32(dr["Number"]),
                        };
                        unitOfWork.QuestionRepo.Insert(question);
                        unitOfWork.Save();
                        var choice = new Choice()
                        {
                            ChoiceId = Guid.NewGuid(),
                            Choice1 = dr["A"].ToString(),
                            QuestionId = question.QuestionId,
                            IsAnswer = CheckDataRowIsNull(dr) == "A" ? true : false,
                            DateCreated = DateTime.Now,
                            ChoiceLetter = "A"


                        };
                        unitOfWork.ChoiceRepo.Insert(choice);
                        choice = new Choice()
                        {
                            ChoiceId = Guid.NewGuid(),
                            Choice1 = dr["B"].ToString(),
                            QuestionId = question.QuestionId,
                            IsAnswer = CheckDataRowIsNull(dr) == "B" ? true : false,
                            DateCreated = DateTime.Now,
                            ChoiceLetter = "B"
                        };
                        unitOfWork.ChoiceRepo.Insert(choice);
                        try
                        {
                            choice = new Choice()
                            {
                                ChoiceId = Guid.NewGuid(),
                                Choice1 = dr["C"].ToString(),
                                QuestionId = question.QuestionId,
                                IsAnswer = dr["Correct"].ToString() == "C" ? true : false,
                                DateCreated = DateTime.Now,
                                ChoiceLetter = "C"
                            };
                            unitOfWork.ChoiceRepo.Insert(choice);
                            choice = new Choice()
                            {
                                ChoiceId = Guid.NewGuid(),
                                Choice1 = dr["D"].ToString(),
                                QuestionId = question.QuestionId,
                                IsAnswer = dr["Correct"].ToString() == "D" ? true : false,
                                DateCreated = DateTime.Now,
                                ChoiceLetter = "D"
                            };
                            unitOfWork.ChoiceRepo.Insert(choice);
                        }
                        catch (Exception)
                        {
                        }


                        await unitOfWork.SaveAsync();
                    }
                }));
            }


        }
    }
}
