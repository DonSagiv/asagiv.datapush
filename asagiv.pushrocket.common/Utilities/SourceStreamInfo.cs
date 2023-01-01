using System.IO;
using System.Threading.Tasks;

namespace asagiv.pushrocket.ui.Utilities
{
    public class SourceStreamInfo
    {
        public string PushDataName { get; }
        public Task<Stream> PushDataStreamTask { get; }

        public SourceStreamInfo(string pushDataName, Task<Stream> pushDataStreamTask)
        {
            PushDataName = pushDataName;
            PushDataStreamTask = pushDataStreamTask;
        }
    }
}
