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

        // GET api/files/<encoded_file_path>
        [HttpGet("{filePath}")]
        public async Task<ActionResult<IEnumerable<DataHubFile>>> Get(string filePath)
        {
            List<DataHubFile> files = new List<DataHubFile>();
            filePath = System.Net.WebUtility.UrlDecode(filePath);
            filePath = filePath.Replace(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("/")), "/");

            files = await S3Access.GetFileList(filePath);
            System.Console.WriteLine($"files.Count = {files.Count()}");
            return files;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<string>> Post()
        {
            try
            {
                var file = Request.Form.Files[0];

                string result = await S3Access.UploadFile($"{Request.Form["path"].ToString()}", file.OpenReadStream());
                System.Console.WriteLine($"File length = {file.Length}");

                return new JsonResult(result);
            }
            catch (System.Exception ex)
            {
                //return Json("Upload Failed: " + ex.Message);
                return new JsonResult("Upload Failed: " + ex.Message);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/<encoded_file_path>
        [HttpDelete("{filePath}")]
        public async Task<ActionResult<string>> Delete(string filePath)
        {
            string result = string.Empty;
            filePath = System.Net.WebUtility.UrlDecode(filePath);
            filePath = filePath.Replace(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("/")), "/");

            result = await S3Access.DeleteFile(filePath);
            return new JsonResult(result);
        }
    }
}
