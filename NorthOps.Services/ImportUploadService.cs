using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Services
{
    public class ImportUploadService
    {
        public static Guid ExamId;

        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings =
            new DevExpress.Web.UploadControlValidationSettings()
            {
                AllowedFileExtensions = new string[] {".xlsx", ".xls"},
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
                var files = HttpContext.Current.Server.MapPath(
                    $"~/filemanager/excel/{System.IO.Path.GetRandomFileName()}");
                Task.Run(new Action(async () =>
                {

                    e.UploadedFile.SaveAs(files);
                    var dt = new Excel(files).ExecuteReader("select * from [sheet1$]");
                    foreach (DataRow dr in dt.Rows)
                    {
                        var question = new Questions()
                        {
                            Question = dr["Question"].ToString(),
                            Title = dr["Title"].ToString(),
                            QuestionId = Guid.NewGuid(),
                            ExamId = ExamId,
                            DateCreated = DateTime.Now,
                            Number = Convert.ToInt32(dr["Number"]),
                        };
                        unitOfWork.QuestionRepo.Insert(question);
                        unitOfWork.Save();
                        var choice = new Choices()
                        {
                            ChoiceId = Guid.NewGuid(),
                            Choice = dr["A"].ToString(),
                            QuestionId = question.QuestionId,
                            IsAnswer = CheckDataRowIsNull(dr) == "A" ? true : false,
                            DateCreated = DateTime.Now,
                            ChoiceLetter = "A"


                        };
                        unitOfWork.ChoiceRepo.Insert(choice);
                        choice = new Choices()
                        {
                            ChoiceId = Guid.NewGuid(),
                            Choice = dr["B"].ToString(),
                            QuestionId = question.QuestionId,
                            IsAnswer = CheckDataRowIsNull(dr) == "B" ? true : false,
                            DateCreated = DateTime.Now,
                            ChoiceLetter = "B"
                        };
                        unitOfWork.ChoiceRepo.Insert(choice);
                        try
                        {
                            choice = new Choices()
                            {
                                ChoiceId = Guid.NewGuid(),
                                Choice = dr["C"].ToString(),
                                QuestionId = question.QuestionId,
                                IsAnswer = dr["Correct"].ToString() == "C" ? true : false,
                                DateCreated = DateTime.Now,
                                ChoiceLetter = "C"
                            };
                            unitOfWork.ChoiceRepo.Insert(choice);
                            choice = new Choices()
                            {
                                ChoiceId = Guid.NewGuid(),
                                Choice = dr["D"].ToString(),
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
