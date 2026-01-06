using System.Threading.Tasks;

namespace Covenant.Deductions.Services
{
    public interface ICppTablesLoader
    {
        Task LoadWeeklyTablesFromExcel(string excelPath, int year);
        Task LoadBiWeeklyTablesFromExcel(string excelPath, int year);
        Task LoadSemiMonthlyTablesFromExcel(string excelPath, int year);
        Task LoadMonthlyTablesFromExcel(string path, int year);
    }
}