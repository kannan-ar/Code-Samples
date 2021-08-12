using VideoStreamer.Bootstrapper;
using System.ComponentModel.Composition;

namespace VideoStreamer.Application
{
    [Export(typeof(IMapper))]
    public class ApplicationMapper : IMapper
    {
        public MapContext CreateMapContext()
        {
            var sourceType = typeof(Data.Entities.User);
            var targetType = typeof(Domain.Entities.User);

            return new MapContext
            {
                Source = new MapContextInfo
                {
                    Assembly = sourceType.Assembly,
                    Namespace = sourceType.Namespace,
                },
                Target = new MapContextInfo
                {
                    Assembly = targetType.Assembly,
                    Namespace = targetType.Namespace,
                }
            };
        }
    }
}
