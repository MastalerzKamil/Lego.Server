using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace Lego.Server.IntegrationTests.Infrastructure
{
    public class IntegrationTestsFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
    {
        public Mock<IFormFile> Video { get; set; }
        
        public string VideoPath = Path.Combine(System.IO.Path.GetFullPath(@"../../../"), @"Resources/cup-01.mp4");
        public readonly string VideoFileName = "cup-01.mp4";
        private string UploadedVideoPath = Path.Combine(System.IO.Path.GetFullPath(@"../../../../../"), @"src/Lego.Server.WebApi/wwwroot/uploads/");
        private string SplittedVideoPath = Path.Combine(System.IO.Path.GetFullPath(@"../../../../../"), @"src/Lego.Server.WebApi/wwwroot/pictures/");
        public HttpClient Client;

        public IntegrationTestsFixture()
        {
            Client = CreateClient();
            Video = new Mock<IFormFile>();
        }
        
        protected override void Dispose(bool disposing)
        {
            File.Delete($"{UploadedVideoPath}/{VideoFileName}");
            Directory.Delete(SplittedVideoPath, true);
            base.Dispose(disposing);
        }

        public MultipartFormDataContent GetRequestContent()
        {
            var videoFile = GetVideoFormFile();
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(videoFile.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = videoFile.Length,
                    ContentType = new MediaTypeHeaderValue(videoFile.ContentType)
                },
            }, "file", videoFile.FileName);
            return multipartContent;
        }

        public void FakeUploadVideo()
        {
            var destinationPath = Path.Combine(UploadedVideoPath, VideoFileName);
            File.Copy(VideoPath,destinationPath);
        }
        
        private FormFile GetVideoFormFile()
        {
            var stream = File.OpenRead(VideoPath);
            return new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(VideoPath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };
        }
    }
}