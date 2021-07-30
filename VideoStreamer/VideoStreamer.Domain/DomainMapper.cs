using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using VideoStreamer.Bootstrapper;

namespace VideoStreamer.Domain
{
    [Export(typeof(IMapper))]
    public class DomainMapper : IMapper
    {
        public MapContext CreateMapContext()
        {
            var sourceType = typeof(VideoStreamer.Data.Entities.User);
            var targetType = typeof(VideoStreamer.Domain.Entities.User);

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
