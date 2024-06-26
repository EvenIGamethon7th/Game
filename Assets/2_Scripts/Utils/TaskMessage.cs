namespace _2_Scripts.Utils
{
    public enum ETaskList
    {
        ResourceLoad
    }
    
    public class TaskMessage
    {
        public ETaskList Task { get; private set; }

        public TaskMessage(ETaskList task) {
            Task = task;
        }
    }
}