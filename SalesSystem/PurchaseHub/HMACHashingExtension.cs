namespace PurchaseHub
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using System.Security.Cryptography;
    using System.IO.Pipelines;
    using System.Buffers;

    public static class HMACHashingExtension
    {
        public static IApplicationBuilder UseHMACHashing(this IApplicationBuilder app)
        {
            return app.UseWhen(context => context.Request.Method == HttpMethods.Post, appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var reader = context.Request.BodyReader;
                    var bytes = new byte[context.Request.ContentLength.Value];
                    long destinationIndex = 0;

                    while (true)
                    {
                        ReadResult readResult = await reader.ReadAsync();
                        var buffer = readResult.Buffer;

                        SequencePosition? position = null;

                        do
                        {
                            position = buffer.PositionOf((byte)'\n');

                            if (position != null)
                            {
                                var readOnlySequence = buffer.Slice(0, position.Value);
                                var tempBytes = readOnlySequence.IsSingleSegment ? readOnlySequence.First.Span.ToArray() : readOnlySequence.ToArray();
                                Array.Copy(tempBytes, 0, bytes, destinationIndex, tempBytes.Length);
                                destinationIndex = destinationIndex + tempBytes.Length;
                                buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                            }
                        }
                        while (position != null);

                        if (readResult.IsCompleted && buffer.Length > 0)
                        {
                            var tempBytes = buffer.ToArray();
                            Array.Copy(tempBytes, 0, bytes, destinationIndex, tempBytes.Length);
                            destinationIndex = destinationIndex + tempBytes.Length;
                        }

                        reader.AdvanceTo(buffer.Start, buffer.End);

                        if (readResult.IsCompleted)
                        {
                            break;
                        }
                    }

                    var txt = System.Text.Encoding.UTF8.GetString(bytes);

                    using (MD5 md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(bytes);
                        context.Items["RequestHash"] = Convert.ToBase64String(hash);
                    }

                    await next();
                });
            });
        }
    }
}