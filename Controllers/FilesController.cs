using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataHubFileService.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace DataHubFileService.Controllers
{
    // Access Key ID: AKIAWYU7YIH4ZUNUOI62 
    // Secret Access Key: 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

    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        // GET api/files
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "file01.pdf", "file02.pdf", "file03.pdf" };
        }

        // GET api/files/5
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<DataHubFile>> Get(string id)
        {
            List<DataHubFile> files = new List<DataHubFile>();
            id = System.Net.WebUtility.UrlDecode(id);

            files = S3Access.GetFileList(id);
            System.Console.WriteLine($"files.Count = {files.Count()}");
            return files;
        }

        // POST api/values
        [HttpPost]
        public ActionResult<string> Post()
        {
            try
            {
                var file = Request.Form.Files[0];
                
                S3Access.UploadFile($"1001/{file.Name}", file.OpenReadStream());
                System.Console.WriteLine($"File length = {file.Length}");



                /*
                string folderName = "Upload";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                */
                return "Upload Successful.";
            }
            catch (System.Exception ex)
            {
                //return Json("Upload Failed: " + ex.Message);
                return "Upload Failed: " + ex.Message;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
