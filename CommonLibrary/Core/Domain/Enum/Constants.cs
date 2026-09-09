using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Enum
{
    public static class UserRoleId
    {
        public const int Administrator = 1;
        public const int CityExpert = 6;
        public const int ProvinceExpert = 7;
        /// <summary>
        /// مدیر استان
        /// </summary>
        public const int ProvinceManager = 8;
        public const int MonitoringWroker = 10;
        public const int MonitoringWageWorker = 13;
        public const int FinancialWorker = 14;
        public const int CityManagerWorker = 15;
        public const int SoldierWorker = 16;
        public const int SubsidiaryWorker = 17;
        public const int BudgetStaffWorker = 18;
        public const int ProtectWorker = 19;
        public const int ProtectProvinceWorker = 21;
        public const int LegalWorker = 22;
        public const int MonitoringWageProvinceWorker = 23;
        public const int ControlReportWorker = 24;
        public const int CityAssistantManagerWorker = 25;
        public const int CenterControllerStaffWorker = 26;
        public const int WageWorker = 27;
        public const int legalExpert = 28;
        public const int AdminReport = 31;
        public const int SupervisorReport = 32;
        public const int BranchReport = 33;
        public const int ReagentReport = 34;
        public const int HourlyLabor = 35;
        public const int SuperAdministrator = 46;
        /// <summary>
        /// معاون استان
        /// </summary>
        public const int Deputy_of_the_province = 47;
    }
    public static class LoanActionLogTypeEnum
    {
        public const string Read = " نمایش";
        public const string TakeBack = " تحویل پرونده";
        public const string Modify = " ویرایش";
        public const string ActorModify = " تغییر وضعیت حق السعی";
        public const string Insert = " افزودن";
        public const string ReadLoan = "جزئیات پرونده نمایش داده شد.";
        public const string RemoveActor = "حذف اقدام کننده";
        public const string NewActor = "اقدام کننده جدید برای پرونده ثبت شد.";
        public const string EditActor = "اقدام کننده پرونده ویرایش شد";
        public const string NewAction = "اقدام جدید برای پرونده ثبت شد.";
        public const string EditAction = "اطلاعات اقدام کننده ویرایش شد";
        public const string ProvinceExpertConfirmation = "تایید کارشناس استان";
        public const string ProvinceExpertCancelConfirmation = "بازگرداندن تایید کارشناس استان";
        public const string CityManagerWorkerConfirmation = "تایید توسط مدیر شهرستان";
        public const string CityManagerWorkerCancelConfirmation = "بازگرداندن تایید توسط مدیر شهرستان";
        public const string ActorConfirmation = "تایید توسط اقدام کننده ";
        public const string AddRelateLoan = "اضافه شدن پرونده مرتبط ";
        public const string MonitoringWageProvinceWorkerConfirmation = "تایید توسط کارشناس نظارت و وصول مطالبات ستاد استان ";
        public const string MonitoringWageProvinceWorkerCancelConfirmation = "بازگرداندن تایید توسط کارشناس نظارت و وصول مطالبات ستاد استان ";
        public const string SoldierWorker = "امریه (سرباز)";
        public const string ProvinceManagerConfirmation = "تایید توسط مدیر استان";
        public const string ProvinceManagerCancelConfirmation = "بازگرداندن تایید توسط مدیر استان ";
        public const string PrintRequest = "ثبت درخواست چاپ ";
        public const string NewSurveyUser = "افزودن پرونده به کارتابل ناظر ";
        public const string NewLoanPlan = "ثبت طرح نظارت برای پرونده ";
        public const string EditLoanPlan = "ویرایش طرح نظارت برای پرونده ";
        public const string NewSurvey = "ثبت نظارت";
        public const string ComputeSurvey = "محاسبه حق السعی  نظارت";
        public const string SetChangeLoanPlanStateRequest = "درخواست تغییر محل طرح";
        public const string VerifyChangeLoanPlanStateRequest = "تایید درخواست تغییر محل طرح";
        public const string NotVerifyChangeLoanPlanStateRequest = "عدم تایید درخواست تغییر محل طرح";
        public const string SetLoanPlanLocationStateAsync = "تغییر محل طرح";
        public const string ReturnToUser = "بازگرداندن پرونده به اقدام کننده.";
        public const string RemoveCartable = "حذف نظارت.";
        public const string Error = "بروز خطا هنگام ثبت نظارت ";
        public const string ReadyToLegal = "لغو حقوقی قدیم";
    }
    /// <summary>
    /// فعالیت های دسترسی
    /// </summary>
    public static class ConstActivities
    {
        public const int dashboard = 1;
        public const int SecurityMenuRoot = 2;
        public const int BasicInfoMenuRoot = 3;
        public const int userManagement = 4;
        public const int branchesManagement = 5;
        public const int changePassword = 6;
        public const int roleManagement = 7;
        public const int forms = 8;
        public const int GetBranchesAsync = 14;
        public const int PaymentConfirmation = 1202;
        public const int GetLoanById = 1210;
        public const int CanCalculateEffectiveDate = 1211;
        public const int GetLoansQuery = 95;
        public const int getLoanByIdQuery = 1182;
        public const int FilterCustomer = 1183;
        public const int FilterSupervisor = 1185;
        public const int GetAllBranches = 1186;
        public const int getLoanStatusType = 1187;
        public const int getLoanCollatTypesQuery = 1188;
        public const int GetReagent = 1189;
        public const int filterLoanMinorType = 1190;
        public const int getLoanInstallmentType = 1191;
        public const int getUser = 1192;
        public const int getAll = 1193;
        public const int GetAllActionTypes = 1195;
        public const int GetActionTypes = 1196;
        public const int GetLoanActionListQuery = 1197;
        public const int setLoanAction = 1198;
        public const int removeLoanAction = 1199;
        public const int getLoansActorsQuery = 1200;
        public const int deleteActor = 1201;
        public const int ComputeWageAmount = 1202;
        public const int RollBackActor = 1203;
        public const int TakeBackActor = 2275;
        public const int RollBackActorGroup = 2635;
        public const int SetActor = 1204;
        public const int refreshLoanDaily = 1206;
        public const int ComputeRespiteActor = 1208;
        public const int AddRelatedLoan = 1209;
        public const int uploadLoanImage = 1210;
        public const int RemoveUploadedFiles = 1211;
        public const int uploadCustomerImage = 1212;
        public const int uploadSurveyImage = 1213;
        public const int UploadFiles = 1214;
        public const int uploadUserImage = 1215;
        public const int hasDeathCustomer = 1216;
        public const int refreshLoan = 1217;
        public const int setLegal = 1218;
        public const int setDeath = 1219;
        public const int getUserListQuery = 1220;
        public const int setUser = 1221;
        public const int SetUserBranch = 1222;
        public const int deleteUserBranche = 1223;
        public const int SetUserRole = 1224;
        public const int deleteUserRole = 1225;
        public const int AddActorToLoans = 1226;
        public const int RemoveLoansActor = 1227;
        public const int FacilitiesInformation = 1228;
        public const int SetChangeLoanPlanStateRequest = 1229;
        public const int VerifyChangeLoanPlanStateRequest = 1230;
        public const int updateFacilitiesLoans = 1231;
        public const int PrintMontlyRequest = 1232;
        public const int AcceptAndCreatePayFile = 2639;
        public const int SaveActorMontlyOperationPdf = 1233;
        public const int UpdateActor = 1234;
        public const int RemovePrintMontlyRequest = 1235;
        public const int CityManagerConfirmationStepTow = 2642;
        public const int CityManagerConfirmationStepOne = 2641;
        public const int CityManagerCancelConfirmation = 1237;
        public const int ProvinceManagerConfirmation = 1238;
        public const int ProvinceManagerCancelConfirmation = 1239;
        public const int GetSurveyDocument = 1240;
        public const int ReturnToUser = 1241;
        public const int SetActivity = 1242;
        public const int SetRole = 1243;
        public const int AddAllActivitiesHasAccess = 1244;
        public const int getAllActivitiesQuery = 1245;
        public const int GetPlanNoInfoList = 1252;
        public const int SetOtherPlanNo = 1253;
        public const int ConfirmUsersMontlyPayment = 1254;
        public const int SetActionDate = 1255;
        public const int SetCartable = 1256;
        public const int RemoveSurveyImage = 1257;
        public const int RemoveCartable = 1258;
        public const int ConfirmChangeLocation = 1259;
        public const int SetLoanPlanNo = 1261;
        public const int RemoveAttachment = 1260;
        public const int RemovePlanNo = 1263;
        public const int ChangheLoansPlanNo = 1262;
        public const int ShowConfirmedActor = 1266;
        public const int ActorAggregatedWage = 1268;
        public const int SupervisorAggregatedWage = 1267;
        public const int BranchAggregatedWage = 1240;
        public const int LoanRemainList = 1273;
        public const int ReferenceToLegal = 1329;
        public const int ConfirmLegalReference = 1275;
        public const int NotConfirmLegalReference = 1276;
        public const int LegalReference = 2534;
        /// <summary>
        /// 189  -> 	SetAdminLoanPlanNo  ->  افزودن طرح وام توسط مدیر
        /// </summary>
        public const int setSupplementaryInsuranceTypes = 2350;
        public const int RemoveNonExecutionPlanPenalty = 190;
        public const int SetNonExecutionPlanPenalty = 191;
        public const int getSupplementaryInsuranceTypes = 192;
        /// <summary>
        /// لیست حق السعی - چاپ لیست پرداختی ماهیانه
        /// </summary>
        public const int GetWagePaymentList = 2355;
        /// <summary>
        /// فهرست حق السعی تجمعی ماهیانه
        /// </summary>
        public const int WageListMonthlyFee = 2356;
        /// <summary>
        /// ریز حق السعی
        /// </summary>
        public const int DetatilWagePaymentList = 2358;
        public const int TransferToPayBudgetUnit = 2638;
        public const int WaitingForEffortReviewLevelone = 2636;
        public const int WaitingForEffortAcceptLevelTwo = 2637;
        /// <summary>
        /// ثبت نظارت کمیته بیش از 300 میلیون - ثبت نظارت کمیته بالا تر از سقف
        /// </summary>
        public const int SetOutsideSurveyMoreThanLimit = 2360;
        public const int NotReferenceActionRegister = 2361;
        public const int PaidPersonalType = 2656;
        public const int LoanReinsert = 2657;

        public const int SurveyReports = 3539;
        public const int LastSurveyReports = 3548;
        public const int CollectionReports = 3550;
        public const int RefernedLoansReport = 3551;
        public const int ApprovedInfoReport = 3552;
        public const int LoansDetailsReport = 3553;
        public const int LoansSummeryReport = 3555;
        public const int CentralBankReport = 3558;
        public const int SurveyDetailsReports = 3559;
        public const int LastSurveyExcelReports = 3561;
        public const int SurveyDetailsExcelReports = 3565;
        public const int RefernedLoansExcelReport = 3566;
        public const int ReferralDetailsReport = 4547;
        public const int ExcelReferralDetailsReport = 4549;
        public const int LastReferralReport = 4545;
        public const int ExcelLastReferralReport = 4546;
        public const int ApprovedInfoExcelReport = 3567;
        public const int LoansDetailsExcelReport = 3568;
        public const int LoansSummeryExcelReport = 3569;

        /// <summary>
        /// گزارش فهرست ریز حق السعی
        /// </summary>
        public const int EffortDetailListReport = 7070;
        /// <summary>
        /// دانلود گزارش فهرست ریز حق السعی
        /// </summary>
        public const int ExcelEffortDetailListReport = 7071;
        /// <summary>
        /// گزارش فهرست حق السعی تجمعی ماهیانه 
        /// </summary>
        public const int EffortMonthlyListReport = 7072;
        /// <summary>
        /// دانلود اکسل فهرست حق السعی تجمعی ماهیانه 
        /// </summary>
        public const int ExcelEffortMonthlyListReport = 7073;
    }

    public static class SurveyListType                   //  فیلتر نمایش پروند های نظارت
    {
        public const int Referral = 1;                   // پرونده های جهت ارجاع
        public const int ReferredAll = 2;                // پرونده های ارجاع شده
        public const int ReferralWaiting = 3;            // پرونده های جهت ارجاع در انتظار   
        public const int Supervised = 4;                 // پرونده های نظارت شده   
        public const int Referalnew = 5;                 // پرونده های نظارت شده   
        public const int RequiredToSurvey = 6;           // پرونده های ملزم به نظارت   
        public const int SupervisedAllSurvey = 7;           // پرونده های نظارت شده برای همه نظارت ها   
    }

    public static class SurveyStatusTypeEnum
    {
        public const int Supervised = 1;
    } 
    //نوع نظارت
    public static class SurveyType
    {
        public const int Referral = 1;
        public const int Interstage = 2;
    }

    public static class UserRoleName
    {
        public const string Admin = "Administrator";
        public const string WageWorker = "WageWorker";
        public const string BankAssistant = "BankAssistant";
        public const string Supervisor = "Supervisor";
        // public const string CityManager = "CityManager";
        public const string CityMaster = "CityMaster";//رئیس شهرستان    15   15	
        public const string CityExpert = "CityExpert";
        public const string ProvinceExpert = "ProvinceExpert";
        public const string SoldierWorker = "SoldierWorker";
        public const string MonitoringWageWorker = "MonitoringWageWorker";
        public const string MonitoringWroker = "MonitoringWroker";
        /// <summary>
        /// مدیر استان
        /// </summary>
        public const string ProvinceManager = "ProvinceManager";
        public const string CityManagerWorker = "CityManagerWorker";
        public const string MonitoringWageProvinceWorker = "MonitoringWageProvinceWorker";
        public const string ProtectProvinceWorker = "ProtectProvinceWorker";
        public const string LegalWorker = "LegalWorker";
        public const string BudgetStaffWorker = "BudgetStaffWorker";
        public const string AdminReport = "AdminReport";
        public const string SupervisorReport = "SupervisorReport";
        public const string LegalExpert = "legalExpert";
        /// <summary>
        /// معاون استان
        /// </summary>
        public const string Deputy_of_the_province = "Deputy of the province";
    }

    public static class ActorStatusTypeIdEnum
    {
        [Description("تحویل توسط اقدام کننده - در انتظار تایید")]
        public const int ActorDeliver = 20;
        [Description("تحویل سیستمی - در انتظار تایید")]
        public const int SystemDeliver = 21;
        [Description("تایید شده - در انتظار محاسبه")]
        public const int AcceptedWaiting = 22;
        [Description("محاسبه شده")]
        public const int Calculated = 23;
        [Description("تعلق نمی گیرد")]
        public const int NotBelong = 24;
        [Description("محاسبه شده دستی")]
        public const int ManuallyCalculated = 25;
        [Description("تعلق نمی گیرد (تایید شده)")]
        public const int NotBelongCalculated = 26;
        [Description("ثبت")]
        public const int Register = 27;
        [Description("تایید توسط مدیر شهرستان")]
        public const int CityManagerWorkerConfirmation = 28;
        [Description("تایید توسط کارشناس استان")]
        public const int ProvinceExpertConfirmation = 29;
        [Description("ثبت درخواست چاپ")]
        public const int PrintRequest = 30;
        [Description("بازگرداندن تایید توسط مدیر شهرستان")]
        public const int CityManagerWorkerCancelConfirmation = 31;
        [Description("بازگرداندن تایید توسط کارشناس استان")]
        public const int ProvinceExpertCancelConfirmation = 32;
        [Description("تایید توسط سایر کارشناسان ستاد استان")]
        public const int OtherProvinceExpertsConfirmation = 33;
        [Description("بازگرداندن تایید توسط  سایر کارشناسان ستاد استان")]
        public const int OtherProvinceExpertsCancelConfirmation = 34;
        [Description("تایید توسط مدیر استان")]
        public const int ProvinceManagerConfirmation = 35;
        [Description("بازگرداندن  تایید توسط مدیر استان ")]
        public const int ProvinceManagerCancelConfirmation = 36;
        [Description(" تایید توسط کارشناس نظارت و وصول مطالبات ستاد استان ")]
        public static int MonitoringWageProvinceWorkerConfirmation = 37;
        [Description("بازگرداندن  تایید توسط کارشناس نظارت و وصول مطالبات ستاد استان ")]
        public static int MonitoringWageProvinceWorkerCancelConfirmation = 38;
        [Description("حذف درخواست چاپ")]
        public static int RemovePrintRequest = 39;
    }
    public static class UserSurveyWageStatusTypeEnum
    {
        public const int Computed = 1;
        public const int PaymentRequest = 2;
    }

    public static class AttachmentTypes
    {
        public const int Survey = 41;
      
    }

    public static class LoanRelationTypeEnum
    {
        public const int Guarantor = 0;
        public const int Obligated = 1;
        public const int Reagent = 3;
    }

}
