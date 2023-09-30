using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Tesseract;

namespace ExtractText.Controllers
{
    public class FileController : ControllerBase
    {
        [HttpPost("extract-text")]
        public async Task<IActionResult> ExtractText([FromBody] byte[] fileData)
        {
            try
            {
                // Use the OCR library to extract text from the fileData
                string extractedText = await PerformOCR(fileData);

                // Return the extracted text as a response
                return Ok(new { Text = extractedText });
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, and return an appropriate error response
                return BadRequest(new { Error = ex.Message });
            }
        }
        public async Task<string> PerformOCR(byte[] fileData)
        {
            using (var engine = new TesseractEngine(@"wwwroot/images/1.PNG", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(fileData))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }

        public const string folderName = "wwwroot/images/";
        public const string trainedDataFolderName = "tessdata";

        [HttpPost("DoOCR")]
        public String DoOCR([FromForm] OcrModel request)
        {

            string name = request.Image.FileName;
            var image = request.Image;

            if (image.Length > 0)
            {
                using (var fileStream = new FileStream(folderName + image.FileName, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            path = Path.Combine(path, "tessdata");
            path = path.Replace("file:\\", "");
            string tessPath = Path.Combine(trainedDataFolderName, "");
            string result = "";

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(folderName + name))
                {
                    var page = engine.Process(img);
                    result = page.GetText();
                    Console.WriteLine(result);
                }
            }
            return String.IsNullOrWhiteSpace(result) ? "Ocr is finished. Return empty" : result;


        }
    }
    public class OcrModel
    {
        public IFormFile Image { get; set; }
    }
}
