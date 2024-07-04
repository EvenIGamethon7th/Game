namespace _2_Scripts.Utils
{
    public enum ETaskList
    {
        DefaultResourceLoad,
        DemoResourceLoad,
        MaterialResourceLoad,
        SkeletonDatasResourceLoad,
        CharacterDataResourceLoad,
        BossDeath,
        GameOver,
    }
    
    public class TaskMessage
    {
        public ETaskList Task { get; private set; }

        public TaskMessage(ETaskList task) {
            Task = task;
        }
    }
}