namespace DemoExam.Interfaces
{
    public interface IExecutable
    {
        void Execute(object parameter);
        bool CanExecute(object parameter);
    }
}
