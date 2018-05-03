using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Models.ViewModels
{
    public class ApplicantExamModel
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string UserId = HttpContext.Current.User.Identity.GetUserId();
        public async Task<int> TakeExam(Questions question, Choices choice)
        {
            question = await unitOfWork.QuestionRepo.GetByIDAsync(question.QuestionId);
            choice = await unitOfWork.ChoiceRepo.GetByIDAsync(choice.ChoiceId);
            if (question.Exams.ExamType != null && (ExamTypes)question.Exams.ExamType == ExamTypes.MBTI)
            {
                await MBTI(question, choice);
            }
            else
            {
                var applicantAnswer = unitOfWork.ApplicantAnswer.Get(filter: m => m.QuestionId == question.QuestionId && m.UserId == UserId).FirstOrDefault();
                if (applicantAnswer != null)
                {
                    applicantAnswer.ChoiceId = choice.ChoiceId;
                    unitOfWork.ApplicantAnswer.Update(applicantAnswer);

                }
                else
                {
                    unitOfWork.ApplicantAnswer.Insert(new ApplicantAnswers() { ApplicantAnswerId = Guid.NewGuid(), UserId = UserId, QuestionId = question.QuestionId, ChoiceId = choice.ChoiceId });
                    await AddScore(question, choice);
                }
                await unitOfWork.SaveAsync();
                await AddScore(question, choice);

            }
            return 0;
        }
        private async Task AddScore(Questions question, Choices choice)
        {
            var applicant = unitOfWork.Applicant.Get(filter: m => m.ExamId == question.ExamId && m.UserId == UserId).FirstOrDefault();
            if (choice.IsAnswer == true)
            {
                applicant.Result = (applicant.Result ?? 0) + 1;
            }
            await unitOfWork.SaveAsync();
        }
        private async Task MBTI(Questions question, Choices choice)
        {
            var col = question.Number % 7;
            var mbti = unitOfWork.PersonalityResult.Get(filter: m => m.UserId == UserId).FirstOrDefault();
            if (mbti == null)
            {
                mbti = new PersonalityResults() { UserId = UserId, PersonalityResultId = Guid.NewGuid(), DateTaken = DateTime.Now };
                unitOfWork.PersonalityResult.Insert(mbti);
                await unitOfWork.SaveAsync();
            }
            switch (col)
            {
                case 1:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.E = (mbti.E ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.I = (mbti.I ?? 0) + 1;
                            break;
                    }
                    break;
                case 2:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.S = (mbti.S ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.N = (mbti.N ?? 0) + 1;
                            break;
                    }
                    break;
                case 3:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.S = (mbti.S ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.N = (mbti.N ?? 0) + 1;
                            break;
                    }
                    break;
                case 4:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.T = (mbti.T ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.F = (mbti.F ?? 0) + 1;
                            break;
                    }
                    break;
                case 5:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.T = (mbti.T ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.F = (mbti.F ?? 0) + 1;
                            break;
                    }
                    break;
                case 6:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.J = (mbti.J ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.P = (mbti.P ?? 0) + 1;
                            break;
                    }
                    break;
                default:
                    switch (choice.ChoiceLetter.ToLower())
                    {
                        case "a":
                            mbti.UserId = UserId;
                            mbti.J = (mbti.J ?? 0) + 1;
                            break;
                        default:
                            mbti.UserId = UserId;
                            mbti.P = (mbti.P ?? 0) + 1;
                            break;
                    }
                    break;
            }
            unitOfWork.PersonalityResult.Update(mbti);
            await unitOfWork.SaveAsync();
        }
    }
}
