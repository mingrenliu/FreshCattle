namespace ExcelUtils.ExcelCore
{
    internal class ExcelReader
    {
        private readonly IWorkbook _workbook;
        private ISheet _currentSheet;
        private int NumberOfSheets => _workbook.NumberOfSheets;
        public ExcelReader(IWorkbook workbook)
        {
            _workbook = workbook;
        }
    }
}