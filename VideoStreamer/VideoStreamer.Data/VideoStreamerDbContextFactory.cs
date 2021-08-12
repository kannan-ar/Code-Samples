using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace VideoStreamer.Data
{
    public class VideoStreamerDbContextFactory : IDesignTimeDbContextFactory<VideoStreamerDbContext>
    {
        public VideoStreamerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VideoStreamerDbContext>();
            optionsBuilder.UseSqlite("Filename=../streamer.db");

            return new VideoStreamerDbContext(optionsBuilder.Options);
        }
    }
}
