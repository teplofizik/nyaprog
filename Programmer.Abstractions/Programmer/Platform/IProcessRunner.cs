namespace Programmer.Platform
{
    /// <summary>
    /// Запуск внешних процессов. Абстракция позволяет уйти от прямой зависимости
    /// на System.Diagnostics и подменять реализацию (тесты, другой хост).
    /// </summary>
    public interface IProcessRunner
    {
        ProcessResult Run(string Program, string Arguments, string WorkingDir);
    }

    /// <summary>
    /// Результат запуска процесса: код возврата и объединённый вывод.
    /// </summary>
    public sealed class ProcessResult
    {
        public int ExitCode { get; }

        public string Output { get; }

        public ProcessResult(int ExitCode, string Output)
        {
            this.ExitCode = ExitCode;
            this.Output = Output ?? "";
        }
    }
}
